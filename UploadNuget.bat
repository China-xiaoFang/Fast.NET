@echo off
setlocal EnableExtensions DisableDelayedExpansion

REM Use the native Simplified Chinese code page so cmd.exe parses Chinese lines reliably.
for /f "tokens=2 delims=:" %%C in ('chcp') do set "ORIGINAL_CODE_PAGE=%%C"
set "ORIGINAL_CODE_PAGE=%ORIGINAL_CODE_PAGE: =%"
chcp 936 >nul

pushd "%~dp0" >nul
if errorlevel 1 (
    echo 无法进入脚本所在目录：%~dp0
    if defined ORIGINAL_CODE_PAGE chcp %ORIGINAL_CODE_PAGE% >nul
    endlocal & exit /b 1
)

set "SOLUTION_FILE=%CD%\Fast.NET.sln"
set "PACKAGE_DIR=%CD%\nupkgs"
set "BUILD_CONFIGURATION="
set "PACKAGE_COUNT=0"
set "NEXT_PACKAGE_INDEX=11"
set "MAX_PACKAGE_INDEX=10"
set "UPLOAD_SELECTION="
set "SELECTED_PACKAGE="
set "SUCCESS_COUNT=0"
set "ERROR_COUNT=0"
set "ERROR_FILES="
set "EXIT_CODE=0"

REM 可通过同名环境变量覆盖推送源，便于使用私有 NuGet 服务。
if not defined NUGET_SOURCE set "NUGET_SOURCE=https://api.nuget.org/v3/index.json"

echo 欢迎使用 Fast.NET 打包发布工具
echo.

where dotnet >nul 2>&1
if errorlevel 1 (
    echo [错误] 未找到 dotnet 命令，请先安装与 global.json 匹配的 .NET SDK。
    set "EXIT_CODE=1"
    goto :Finish
)

if not exist "%SOLUTION_FILE%" (
    echo [错误] 未找到解决方案：%SOLUTION_FILE%
    set "EXIT_CODE=1"
    goto :Finish
)

echo 正在清理旧的 NuGet 包......
if exist "%PACKAGE_DIR%" rd /s /q "%PACKAGE_DIR%"
if exist "%PACKAGE_DIR%" (
    echo [错误] 无法删除目录：%PACKAGE_DIR%
    echo 请确认其中的文件没有被其他程序占用。
    set "EXIT_CODE=1"
    goto :Finish
)
echo 清理完成。
echo.

echo 请选择生成模式：
echo [1] Debug
echo [2] Release
echo.
choice /c 12 /n /m "请输入选项："
if errorlevel 2 set "BUILD_CONFIGURATION=Release"
if not errorlevel 2 set "BUILD_CONFIGURATION=Debug"

echo.
echo 正在使用 %BUILD_CONFIGURATION% 模式编译并生成 NuGet 包......
echo dotnet build "%SOLUTION_FILE%" --configuration %BUILD_CONFIGURATION% --no-incremental
echo.

dotnet build "%SOLUTION_FILE%" --configuration "%BUILD_CONFIGURATION%" --no-incremental
if errorlevel 1 (
    echo.
    echo [错误] 编译或打包失败，已终止上传。
    set "EXIT_CODE=1"
    goto :Finish
)

echo.
echo 请选择发布方式：
echo [0] 仅完成编译和打包，不上传
echo [1] 上传全部 NuGet 包
if exist "%PACKAGE_DIR%" for /r "%PACKAGE_DIR%" %%F in (*) do if /i "%%~xF"==".nupkg" call :RegisterPackage "%%~fF"

if "%PACKAGE_COUNT%"=="0" (
    echo [错误] 没有在以下目录中找到 .nupkg 文件：%PACKAGE_DIR%
    set "EXIT_CODE=1"
    goto :Finish
)

set /a MAX_PACKAGE_INDEX=NEXT_PACKAGE_INDEX-1

echo.
echo 共找到 %PACKAGE_COUNT% 个待发布包，单独上传编号为 11 至 %MAX_PACKAGE_INDEX%。
echo 符号包 .snupkg 将由 dotnet nuget push 自动关联上传。
echo.

:SelectUploadMode
call :ReadUploadSelection
if errorlevel 1 (
    echo [错误] 请输入菜单中有效的数字编号。
    goto :SelectUploadMode
)

if "%UPLOAD_SELECTION%"=="0" goto :SkipUpload
if "%UPLOAD_SELECTION%"=="1" goto :PrepareUpload

call set "SELECTED_PACKAGE=%%PACKAGE_%UPLOAD_SELECTION%%%"
if not defined SELECTED_PACKAGE (
    echo [错误] 编号 %UPLOAD_SELECTION% 不存在，请重新选择。
    goto :SelectUploadMode
)

:PrepareUpload

REM 优先读取环境变量；未设置时使用隐藏输入，避免密钥直接显示在窗口中。
if not defined NUGET_API_KEY call :ReadApiKey
if not defined NUGET_API_KEY (
    echo.
    echo [错误] NuGet API Key 不能为空，已终止上传。
    set "EXIT_CODE=1"
    goto :Finish
)

echo.
echo 开始上传到：%NUGET_SOURCE%
if "%UPLOAD_SELECTION%"=="1" goto :PushAllPackages

call :PushPackage "%SELECTED_PACKAGE%"
goto :UploadSummary

:PushAllPackages
for /l %%I in (11,1,%MAX_PACKAGE_INDEX%) do call :PushPackageByIndex %%I

:UploadSummary

REM 尽早清除脚本局部环境中的密钥。
set "NUGET_API_KEY="

echo.
echo 上传完成：成功 %SUCCESS_COUNT% 个，失败 %ERROR_COUNT% 个。
if not "%ERROR_COUNT%"=="0" (
    echo 失败包列表：%ERROR_FILES%
    set "EXIT_CODE=1"
)
goto :Finish

:SkipUpload
echo.
echo 已完成编译和打包，本次未执行上传。
echo NuGet 包目录：%PACKAGE_DIR%
goto :Finish

:RegisterPackage
set "CURRENT_PACKAGE_INDEX=%NEXT_PACKAGE_INDEX%"
set "PACKAGE_%CURRENT_PACKAGE_INDEX%=%~f1"
set /a PACKAGE_COUNT+=1
set /a NEXT_PACKAGE_INDEX+=1
echo [%CURRENT_PACKAGE_INDEX%] %~nx1
exit /b 0

:ReadUploadSelection
setlocal EnableDelayedExpansion
set "INPUT_VALUE="
set /p "INPUT_VALUE=请输入发布编号："
if not defined INPUT_VALUE (
    endlocal
    exit /b 1
)

REM 只允许纯数字，避免多位编号被 choice 拆成单个字符。
for /f "delims=0123456789" %%A in ("!INPUT_VALUE!") do (
    endlocal
    exit /b 1
)

endlocal & set "UPLOAD_SELECTION=%INPUT_VALUE%"
exit /b 0

:ReadApiKey
where powershell.exe >nul 2>&1
if errorlevel 1 goto :ReadApiKeyPlainText

for /f "delims=" %%K in ('powershell.exe -NoLogo -NoProfile -Command "$secure = Read-Host '请输入 NuGet API Key' -AsSecureString; $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure); try { [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer) } finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }"') do set "NUGET_API_KEY=%%K"
exit /b 0

:ReadApiKeyPlainText
echo [警告] 未找到 PowerShell，API Key 输入将会显示在窗口中。
set /p "NUGET_API_KEY=请输入 NuGet API Key："
exit /b 0

:PushPackageByIndex
set "PACKAGE_PATH="
call set "PACKAGE_PATH=%%PACKAGE_%~1%%%"
if defined PACKAGE_PATH call :PushPackage "%PACKAGE_PATH%"
set "PACKAGE_PATH="
exit /b 0

:PushPackage
echo.
echo 正在上传：%~nx1
dotnet nuget push "%~f1" --api-key "%NUGET_API_KEY%" --skip-duplicate --source "%NUGET_SOURCE%"
if errorlevel 1 goto :PushFailed

set /a SUCCESS_COUNT+=1
echo [成功] %~nx1
timeout /t 1 /nobreak >nul 2>&1
exit /b 0

:PushFailed
set /a ERROR_COUNT+=1
set "ERROR_FILES=%ERROR_FILES% %~nx1"
echo [失败] %~nx1
timeout /t 1 /nobreak >nul 2>&1
exit /b 0

:Finish
popd
if defined CI goto :Exit
if defined NO_PAUSE goto :Exit
echo.
pause

:Exit
if defined ORIGINAL_CODE_PAGE chcp %ORIGINAL_CODE_PAGE% >nul
endlocal & exit /b %EXIT_CODE%
