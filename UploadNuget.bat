@echo off
setlocal EnableExtensions DisableDelayedExpansion

for /f "tokens=2 delims=:" %%C in ('chcp') do set "ORIGINAL_CODE_PAGE=%%C"
set "ORIGINAL_CODE_PAGE=%ORIGINAL_CODE_PAGE: =%"

chcp 936 >nul
REM 本脚本使用 Windows 简体中文代码页（CP936/GBK），避免 CMD 在 UTF-8 代码页下解析中文批处理时出现异常。

REM .NET CLI 强制使用中文界面；不在全局强制 UTF-8，避免与 CMD 的 CP936 输出发生冲突。

REM CMD 对连续中文行尾存在解析兼容问题，中文注释和中文输出后保留空行，请勿批量删除这些空行. 


set "DOTNET_CLI_UI_LANGUAGE=zh-CN"

REM 颜色输出使用系统 PowerShell；不可用时保留纯文本提示。

set "COLOR_OUTPUT_AVAILABLE="
where powershell.exe >nul 2>&1
if not errorlevel 1 set "COLOR_OUTPUT_AVAILABLE=1"

REM 固定在脚本所在的仓库根目录执行，避免从其他目录启动时找不到解决方案。

pushd "%~dp0" >nul
if errorlevel 1 (
    call :WriteStatus Red "[错误] 无法进入脚本所在目录：%~dp0"
    if defined ORIGINAL_CODE_PAGE chcp %ORIGINAL_CODE_PAGE% >nul
    endlocal & exit /b 1
)

REM 仓库路径和运行参数。

set "SOLUTION_FILE=%CD%\Fast.NET.sln"
set "SOURCE_DIR=%CD%\src"
set "PACKAGE_DIR=%CD%\nupkgs"
set "RUN_MODE=%~1"
set "REQUESTED_PACKAGE_ID=%~2"
set "BUILD_CONFIGURATION=Release"

REM 当前打包结果和发布状态。

set "PACKAGE_COUNT=0"
set "NEXT_PACKAGE_INDEX=11"
set "MAX_PACKAGE_INDEX=10"
set "UPLOAD_SELECTION="
set "SELECTED_PACKAGE="
set "SUCCESS_COUNT=0"
set "SUCCESS_FILES="
set "SKIPPED_COUNT=0"
set "SKIPPED_FILES="
set "WARNING_COUNT=0"
set "WARNING_FILES="
set "ERROR_COUNT=0"
set "ERROR_FILES="
set "MISSING_PACKAGE_COUNT=0"
set "EXIT_CODE=0"

REM 可通过 NUGET_SOURCE 覆盖发布源，未设置时默认发布到 NuGet.org。

if not defined NUGET_SOURCE set "NUGET_SOURCE=https://api.nuget.org/v3/index.json"

REM 开始前检查 .NET SDK 和解决方案文件。

where dotnet >nul 2>&1
if errorlevel 1 (
    call :WriteStatus Red "[错误] 未找到 dotnet 命令，请安装 global.json 指定的 .NET SDK。"
    set "EXIT_CODE=1"
    goto :Finish
)

if not exist "%SOLUTION_FILE%" (
    call :WriteStatus Red "[错误] 未找到解决方案：%SOLUTION_FILE%"
    set "EXIT_CODE=1"
    goto :Finish
)

REM 支持以下命令行模式：

REM   pack                         只执行还原、构建和打包。

REM   publish-all                  构建并打包后发布当前全部包。

REM   publish-one Fast.Cache       构建并打包后只发布指定 PackageId。

REM 不带参数运行时，进入构建配置和发布范围的交互选择。

if not defined RUN_MODE goto :InteractiveConfiguration

if /i "%RUN_MODE%"=="pack" (
    set "UPLOAD_SELECTION=0"
    goto :BuildAndPack
)

if /i "%RUN_MODE%"=="publish-all" (
    set "UPLOAD_SELECTION=1"
    goto :BuildAndPack
)

if /i "%RUN_MODE%"=="publish-one" (
    if not defined REQUESTED_PACKAGE_ID (
        call :WriteStatus Red "[错误] publish-one 模式必须指定 PackageId。"
        echo 示例：UploadNuget.bat publish-one Fast.Cache
        set "EXIT_CODE=1"
        goto :Finish
    )
    goto :BuildAndPack
)

call :WriteStatus Red "[错误] 未知运行模式：%RUN_MODE%"
call :ShowUsage
set "EXIT_CODE=1"
goto :Finish

:InteractiveConfiguration
echo Fast.NET 打包与发布工具

echo.
echo 请选择构建配置：

echo [1] Debug
echo [2] Release
choice /c 12 /n /m "请输入选项："
if errorlevel 2 set "BUILD_CONFIGURATION=Release"
if not errorlevel 2 set "BUILD_CONFIGURATION=Debug"

:BuildAndPack
REM 打包流程固定为 Restore、Build、Pack，Build 本身不会隐式生成 NuGet 包。

echo.
echo [1/3] 正在还原解决方案...
dotnet restore "%SOLUTION_FILE%"
if errorlevel 1 (
    call :WriteStatus Red "[错误] 解决方案还原失败。"
    set "EXIT_CODE=1"
    goto :Finish
)

echo.
echo [2/3] 正在使用 %BUILD_CONFIGURATION% 配置构建解决方案...
dotnet build "%SOLUTION_FILE%" --configuration "%BUILD_CONFIGURATION%" --no-restore
if errorlevel 1 (
    call :WriteStatus Red "[错误] 构建失败，未执行打包和发布。"
    set "EXIT_CODE=1"
    goto :Finish
)

echo.
echo [3/3] 正在将 NuGet 包输出到 %PACKAGE_DIR%...
dotnet pack "%SOLUTION_FILE%" --configuration "%BUILD_CONFIGURATION%" --no-build --no-restore --property:WarnOnPackingNonPackableProject=false
if errorlevel 1 (
    call :WriteStatus Red "[错误] 打包失败，未执行发布。"
    set "EXIT_CODE=1"
    goto :Finish
)

REM 根据每个项目当前的 PackageId 和 PackageVersion 精确定位包，避免误选 nupkgs 中的旧版本。

echo.
echo 当前项目版本对应的 NuGet 包：

for /r "%SOURCE_DIR%" %%P in (*.csproj) do call :RegisterProjectPackage "%%~fP"

if not "%MISSING_PACKAGE_COUNT%"=="0" (
    call :WriteStatus Red "[错误] 有 %MISSING_PACKAGE_COUNT% 个预期包文件不存在。"
    set "EXIT_CODE=1"
    goto :Finish
)

if "%PACKAGE_COUNT%"=="0" (
    call :WriteStatus Red "[错误] 没有找到可发布的 NuGet 包。"
    set "EXIT_CODE=1"
    goto :Finish
)

set /a MAX_PACKAGE_INDEX=NEXT_PACKAGE_INDEX-1

REM 命令行模式直接进入对应分支，交互模式则继续选择发布范围。

if /i "%RUN_MODE%"=="pack" goto :SkipPublish
if /i "%RUN_MODE%"=="publish-all" goto :PreparePublish
if /i "%RUN_MODE%"=="publish-one" (
    if not defined SELECTED_PACKAGE (
        call :WriteStatus Red "[错误] 未找到指定的 PackageId：%REQUESTED_PACKAGE_ID%"
        set "EXIT_CODE=1"
        goto :Finish
    )
    goto :PreparePublish
)

:SelectPublishMode
echo.
echo 请选择发布方式：

echo [0] 结束发布（尚未发布时仅完成构建和打包）

echo [1] 发布当前全部 NuGet 包

echo [11-%MAX_PACKAGE_INDEX%] 发布上方列表中的单个包

call :ReadNumericSelection
if errorlevel 1 (
    call :WriteStatus Red "[错误] 请输入有效的数字选项。"
    goto :SelectPublishMode
)

if "%UPLOAD_SELECTION%"=="0" (
    if not "%SUCCESS_COUNT%"=="0" goto :PublishSummary
    if not "%SKIPPED_COUNT%"=="0" goto :PublishSummary
    if not "%WARNING_COUNT%"=="0" goto :PublishSummary
    if not "%ERROR_COUNT%"=="0" goto :PublishSummary
    goto :SkipPublish
)

if "%UPLOAD_SELECTION%"=="1" goto :PreparePublish

call set "SELECTED_PACKAGE=%%PACKAGE_%UPLOAD_SELECTION%%%"
if not defined SELECTED_PACKAGE (
    call :WriteStatus Red "[错误] 包序号不存在：%UPLOAD_SELECTION%"
    goto :SelectPublishMode
)

:PreparePublish
REM 优先读取当前环境中的 API Key；未设置时再使用 PowerShell 隐藏输入。

if not defined NUGET_API_KEY call :ReadNuGetApiKey
if not defined NUGET_API_KEY (
    call :WriteStatus Red "[错误] NuGet API Key 不能为空，已取消发布。"
    set "EXIT_CODE=1"
    goto :Finish
)

echo.
if "%UPLOAD_SELECTION%"=="1" (
    echo 即将向以下源发布当前全部 %PACKAGE_COUNT% 个包：

) else (
    echo 即将发布单个包：

    echo   %SELECTED_PACKAGE%
    echo 发布源：

)
echo   %NUGET_SOURCE%
echo.

if "%UPLOAD_SELECTION%"=="1" goto :PublishAllPackages

call :PushPackage "%SELECTED_PACKAGE%"
if defined RUN_MODE goto :PublishSummary

REM 交互模式发布单包后保留本次打包结果和 API Key，返回菜单继续选择其他包。

echo.
echo 单包发布流程已完成，可继续选择或重试其他包。

set "UPLOAD_SELECTION="
set "SELECTED_PACKAGE="
goto :SelectPublishMode

:PublishAllPackages
REM 全量模式仅遍历本次识别出的当前版本包。

for /l %%I in (11,1,%MAX_PACKAGE_INDEX%) do call :PushPackageByIndex %%I

:PublishSummary
echo.
echo 发布完成：

call :WriteStatus Green "[成功] %SUCCESS_COUNT% 个" "%SUCCESS_FILES%"
call :WriteStatus DarkGray "[跳过] 已存在 %SKIPPED_COUNT% 个" "%SKIPPED_FILES%"
call :WriteStatus Yellow "[警告] %WARNING_COUNT% 个" "%WARNING_FILES%"
call :WriteStatus Red "[失败] %ERROR_COUNT% 个" "%ERROR_FILES%"

if not "%WARNING_COUNT%"=="0" set "EXIT_CODE=2"
if not "%ERROR_COUNT%"=="0" set "EXIT_CODE=1"
goto :Finish

:SkipPublish
echo.
echo 构建和打包已完成，本次未执行发布。

echo NuGet 包目录：%PACKAGE_DIR%
goto :Finish

:RegisterProjectPackage
REM 读取项目最终生效的包名和版本，再检查对应 nupkg 是否存在。

set "CURRENT_PACKAGE_ID="
set "CURRENT_PACKAGE_VERSION="
for /f "usebackq delims=" %%I in (`dotnet msbuild "%~1" -nologo -getProperty:PackageId`) do if not defined CURRENT_PACKAGE_ID set "CURRENT_PACKAGE_ID=%%I"
for /f "usebackq delims=" %%V in (`dotnet msbuild "%~1" -nologo -getProperty:PackageVersion`) do if not defined CURRENT_PACKAGE_VERSION set "CURRENT_PACKAGE_VERSION=%%V"

if not defined CURRENT_PACKAGE_ID (
    call :WriteStatus Red "[错误] 无法读取项目的 PackageId：%~1"
    set /a MISSING_PACKAGE_COUNT+=1
    exit /b 0
)

if not defined CURRENT_PACKAGE_VERSION (
    call :WriteStatus Red "[错误] 无法读取项目的 PackageVersion：%~1"
    set /a MISSING_PACKAGE_COUNT+=1
    exit /b 0
)

set "CURRENT_PACKAGE_PATH=%PACKAGE_DIR%\%CURRENT_PACKAGE_ID%.%CURRENT_PACKAGE_VERSION%.nupkg"
if not exist "%CURRENT_PACKAGE_PATH%" (
    call :WriteStatus Red "[错误] 缺少包文件：%CURRENT_PACKAGE_PATH%"
    set /a MISSING_PACKAGE_COUNT+=1
    exit /b 0
)

REM 从 11 开始编号，保留 0 和 1 作为不发布与全量发布选项。

set "CURRENT_PACKAGE_INDEX=%NEXT_PACKAGE_INDEX%"
set "PACKAGE_%CURRENT_PACKAGE_INDEX%=%CURRENT_PACKAGE_PATH%"
set /a PACKAGE_COUNT+=1
set /a NEXT_PACKAGE_INDEX+=1
echo [%CURRENT_PACKAGE_INDEX%] %CURRENT_PACKAGE_ID% %CURRENT_PACKAGE_VERSION%

REM publish-one 模式通过 PackageId 匹配需要发布的单个包。

if /i "%RUN_MODE%"=="publish-one" if /i "%CURRENT_PACKAGE_ID%"=="%REQUESTED_PACKAGE_ID%" set "SELECTED_PACKAGE=%CURRENT_PACKAGE_PATH%"
exit /b 0

:ReadNumericSelection
REM 仅接受纯数字，避免无效序号进入动态变量查询。

setlocal EnableDelayedExpansion
set "INPUT_VALUE="
set /p "INPUT_VALUE=请输入选项："
if not defined INPUT_VALUE (
    endlocal
    exit /b 1
)

for /f "delims=0123456789" %%A in ("!INPUT_VALUE!") do (
    endlocal
    exit /b 1
)

endlocal & set "UPLOAD_SELECTION=%INPUT_VALUE%"
exit /b 0

:ReadNuGetApiKey
REM 中文提示由批处理输出，PowerShell 命令仅负责安全读取，避免 API Key 显示在控制台中。

where powershell.exe >nul 2>&1
if errorlevel 1 goto :ReadNuGetApiKeyPlainText

echo 请输入 NuGet API Key（输入内容不会显示）：

for /f "delims=" %%K in ('powershell.exe -NoLogo -NoProfile -Command "$secureValue = $Host.UI.ReadLineAsSecureString(); $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureValue); try { [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer) } finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }"') do if not defined NUGET_API_KEY set "NUGET_API_KEY=%%K"
exit /b 0

:ReadNuGetApiKeyPlainText
REM 极少数没有 PowerShell 的环境只能使用明文输入，并明确给出安全提示。

call :WriteStatus Yellow "[警告] 未找到 PowerShell，输入的 API Key 将显示在控制台中。"
set /p "NUGET_API_KEY=请输入 NuGet API Key："
exit /b 0

:PushPackageByIndex
REM 根据交互菜单中的序号取出包路径。

set "PACKAGE_PATH="
call set "PACKAGE_PATH=%%PACKAGE_%~1%%"
if defined PACKAGE_PATH call :PushPackage "%PACKAGE_PATH%"
exit /b 0

:PushPackage
REM 保留 NuGet 的重复版本跳过行为。发布日志临时强制为 UTF-8，识别后再转换为 CP936 输出到控制台。

echo.
echo 正在发布 %~nx1...
set "PUSH_RESULT=error"
:CreatePushLog
set "PUSH_LOG_FILE=%TEMP%\Fast.NET-nuget-push-%RANDOM%-%RANDOM%.log"
if exist "%PUSH_LOG_FILE%" goto :CreatePushLog

REM PowerShell 可用时使用中文 NuGet 输出，并通过 UTF-8 日志可靠识别中文/英文结果。

if defined COLOR_OUTPUT_AVAILABLE goto :PushPackageLocalized

REM 极少数没有 PowerShell 的环境保持中文发布输出，只根据退出码判断；零退出码保守记为警告。

dotnet nuget push "%~f1" --api-key "%NUGET_API_KEY%" --source "%NUGET_SOURCE%" --skip-duplicate
set "PUSH_EXIT_CODE=%ERRORLEVEL%"
if "%PUSH_EXIT_CODE%"=="0" set "PUSH_RESULT=warning"
goto :PushPackageResult

:PushPackageLocalized
REM dotnet 重定向日志固定为 UTF-8，避免不同系统区域设置导致中文日志无法识别。

set "PREVIOUS_DOTNET_CLI_FORCE_UTF8_ENCODING=%DOTNET_CLI_FORCE_UTF8_ENCODING%"
set "DOTNET_CLI_FORCE_UTF8_ENCODING=1"
dotnet nuget push "%~f1" --api-key "%NUGET_API_KEY%" --source "%NUGET_SOURCE%" --skip-duplicate >"%PUSH_LOG_FILE%" 2>&1
set "PUSH_EXIT_CODE=%ERRORLEVEL%"
if defined PREVIOUS_DOTNET_CLI_FORCE_UTF8_ENCODING (
    set "DOTNET_CLI_FORCE_UTF8_ENCODING=%PREVIOUS_DOTNET_CLI_FORCE_UTF8_ENCODING%"
) else (
    set "DOTNET_CLI_FORCE_UTF8_ENCODING="
)
set "PREVIOUS_DOTNET_CLI_FORCE_UTF8_ENCODING="

if not exist "%PUSH_LOG_FILE%" goto :PushPackageResult

set "FAST_NUGET_PUSH_LOG=%PUSH_LOG_FILE%"
set "FAST_NUGET_PUSH_EXIT_CODE=%PUSH_EXIT_CODE%"
powershell.exe -NoLogo -NoProfile -NonInteractive -Command "$ErrorActionPreference = 'Stop'; $path = $env:FAST_NUGET_PUSH_LOG; $content = [IO.File]::ReadAllText($path, [Text.UTF8Encoding]::new($false)); $zhError = -join ([char[]](0x9519,0x8BEF)); $zhWarning = -join ([char[]](0x8B66,0x544A)); $zhPushed = -join ([char[]](0x5DF2,0x63A8,0x9001,0x5305)); $zhSuccessPushed = -join ([char[]](0x5DF2,0x6210,0x529F,0x63A8,0x9001)); $zhExistsPackage = -join ([char[]](0x5DF2,0x5B58,0x5728,0x5305)); $hasError = [regex]::IsMatch($content, '(?im)^\s*error\s*[: ]') -or [regex]::IsMatch($content, '(?m)^\s*' + [regex]::Escape($zhError) + '\s*(?::|\xFF1A)'); $hasWarning = [regex]::IsMatch($content, '(?im)^\s*(?:warn|warning)\s*[: ]') -or [regex]::IsMatch($content, '(?m)^\s*' + [regex]::Escape($zhWarning) + '\s*(?::|\xFF1A)'); $hasSuccess = $content.Contains('Your package was pushed.') -or $content.Contains($zhPushed) -or $content.Contains($zhSuccessPushed) -or [regex]::IsMatch($content, '(?im)^\s*Created\s+https?://'); $hasDuplicate = $content.Contains('already exists') -or $content.Contains($zhExistsPackage) -or [regex]::IsMatch($content, '(?im)^\s*Conflict\s+https?://'); [IO.File]::WriteAllText($path, $content, [Text.Encoding]::GetEncoding(936)); if ([int]$env:FAST_NUGET_PUSH_EXIT_CODE -ne 0 -or $hasError) { exit 11 }; if ($hasSuccess -and $hasDuplicate) { exit 12 }; if ($hasDuplicate) { exit 13 }; if ($hasWarning) { exit 12 }; if ($hasSuccess) { exit 10 }; exit 12" >nul 2>&1
set "PUSH_CLASSIFY_CODE=%ERRORLEVEL%"

type "%PUSH_LOG_FILE%"

if "%PUSH_CLASSIFY_CODE%"=="10" set "PUSH_RESULT=success"
if "%PUSH_CLASSIFY_CODE%"=="11" set "PUSH_RESULT=error"
if "%PUSH_CLASSIFY_CODE%"=="12" set "PUSH_RESULT=warning"
if "%PUSH_CLASSIFY_CODE%"=="13" set "PUSH_RESULT=skipped"

if "%PUSH_CLASSIFY_CODE%"=="10" goto :PushPackageResult
if "%PUSH_CLASSIFY_CODE%"=="11" goto :PushPackageResult
if "%PUSH_CLASSIFY_CODE%"=="12" goto :PushPackageResult
if "%PUSH_CLASSIFY_CODE%"=="13" goto :PushPackageResult

set "PUSH_RESULT=warning"
call :WriteStatus Yellow "[警告] 无法可靠识别 NuGet 发布结果，请检查上方输出：%~nx1"

:PushPackageResult
if exist "%PUSH_LOG_FILE%" del /q "%PUSH_LOG_FILE%" >nul 2>&1

if "%PUSH_RESULT%"=="error" (
    set /a ERROR_COUNT+=1
    set "ERROR_FILES=%ERROR_FILES% %~nx1"
    call :WriteStatus Red "[错误] 发布失败：%~nx1"
    exit /b 0
)

if "%PUSH_RESULT%"=="warning" (
    set /a WARNING_COUNT+=1
    set "WARNING_FILES=%WARNING_FILES% %~nx1"
    call :WriteStatus Yellow "[警告] 发布存在警告、部分跳过或结果未确认，请检查上方输出：%~nx1"
    exit /b 0
)

if "%PUSH_RESULT%"=="skipped" (
    set /a SKIPPED_COUNT+=1
    set "SKIPPED_FILES=%SKIPPED_FILES% %~nx1"
    call :WriteStatus DarkGray "[跳过] 包版本已存在：%~nx1"
    exit /b 0
)

set /a SUCCESS_COUNT+=1
set "SUCCESS_FILES=%SUCCESS_FILES% %~nx1"
call :WriteStatus Green "[成功] 已发布：%~nx1"
exit /b 0

:WriteStatus
REM 文本通过环境变量传入 PowerShell，避免把消息内容当作命令解析。

setlocal DisableDelayedExpansion
set "FAST_NUGET_STATUS_COLOR=%~1"
set "FAST_NUGET_STATUS_TEXT=%~2"
set "FAST_NUGET_STATUS_FILES=%~3"

if not defined COLOR_OUTPUT_AVAILABLE goto :WriteStatusPlain

powershell.exe -NoLogo -NoProfile -NonInteractive -Command "$ErrorActionPreference = 'Stop'; [Console]::OutputEncoding = [Text.Encoding]::GetEncoding(936); $useColor = -not [Console]::IsOutputRedirected; $originalColor = [Console]::ForegroundColor; try { if ($useColor) { [Console]::ForegroundColor = [ConsoleColor]$env:FAST_NUGET_STATUS_COLOR }; [Console]::WriteLine($env:FAST_NUGET_STATUS_TEXT); foreach ($file in ($env:FAST_NUGET_STATUS_FILES -split ' ')) { if ($file) { [Console]::WriteLine('  - ' + $file) } } } finally { if ($useColor) { [Console]::ForegroundColor = $originalColor } }" 2>nul
if errorlevel 1 goto :WriteStatusPlain
endlocal & exit /b 0

:WriteStatusPlain
echo %FAST_NUGET_STATUS_TEXT%
for %%F in (%FAST_NUGET_STATUS_FILES%) do echo   - %%F
endlocal & exit /b 0

:ShowUsage
echo 使用方式：

echo   UploadNuget.bat
echo   UploadNuget.bat pack
echo   UploadNuget.bat publish-all
echo   UploadNuget.bat publish-one Fast.Cache
exit /b 0

:Finish
REM 密钥仅存在于当前进程环境，退出前清空本地副本。

set "NUGET_API_KEY="
popd

if defined CI goto :Exit
if defined NO_PAUSE goto :Exit

echo.
pause

:Exit
if defined ORIGINAL_CODE_PAGE chcp %ORIGINAL_CODE_PAGE% >nul
endlocal & exit /b %EXIT_CODE%
