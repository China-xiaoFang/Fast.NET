@echo off

REM 使用 chcp 65001 命令将命令行窗口的代码页设置为 UTF-8
chcp 65001 > nul

echo 欢迎使用 Fast.NET 打包编译工具

REM 换行
echo.

echo 正在删除 NuGet 包缓存

REM 换行
echo.

REM 删除包缓存
rd /s /q "%~dp0nupkgs"

REM 判断删除包缓存是否成功
if %errorlevel% equ 0 (
	REM 换行
	echo.

	echo 删除包缓存成功...... 
) else (
	REM 换行
	echo.

	echo 删除包缓存失败...... 
)

REM 换行
echo.

REM 开启变量延迟
setlocal enabledelayedexpansion

echo 请选择您 Visual Studio 的安装目录

REM 换行
echo.

REM 设置要编译和生成的 .sln 项目文件路径。使用 %~dp0 变量来获取 当前批处理文件所在的目录
set solution_file=%~dp0backend\Fast.NET.sln

REM 设置多个可选择的 devenv.com 文件路径
set "devenv_path[1]=C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.com"
set "devenv_path[2]=D:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.com"

REM 定义一个标签
:DevenvExceptionInput

echo 请选择您 Visual Studio 的安装目录（包含 devenv.com 文件）：
echo [1] !devenv_path[1]!
echo [2] !devenv_path[2]!

REM 换行
echo.

REM 数据数字
choice /c 12 /n /m "输入当前计算机所存在的 devenv.com 文件路径的编号："

REM 获取输入的值，得到 devenv.com 文件路径
set "devenv_path=!devenv_path[%errorlevel%]!"

REM 判断是否输入正确
if not defined devenv_path (
	echo 输入有误，请重新输入。
	goto DevenvExceptionInput
)

REM 换行
echo.

REM 设置多个生成模式
set build_mode[1]=Debug
set build_mode[2]=Release

REM 定义一个标签
:BuildMode

echo 选择生成模式： 
echo [1] !build_mode[1]!
echo [2] !build_mode[2]!

REM 换行
echo.

REM 数据数字
choice /c 12 /n /m "输入当前需要生成的模式："

REM 获取输入的值，得到生成模式
set "build_mode=!build_mode[%errorlevel%]!"

REM 判断是否输入正确
if not defined build_mode (
	echo 输入有误，请重新输入。
	goto BuildMode
)

REM 换行
echo.

REM 输出执行命令
echo "%devenv_path%" "%solution_file%" /Rebuild "%build_mode%|Any CPU"

REM 换行
echo.

echo 等待重新生成中......

REM 换行
echo.

REM 编译生成项目，指定生成模式，先清理，再生成
"%devenv_path%" "%solution_file%" /Rebuild "%build_mode%|Any CPU"

REM 换行
echo.

REM 输入 APIKey
set /p api_key=请输入 NuGet Api 密钥：

REM 换行
echo.

echo 当前上传的所有 NuGet 包信息：

REM 获取 nupkgs 文件夹中所有的包文件
set "nuget_file_list="
for /f "delims=" %%a in ('dir /b /s "%~dp0nupkgs"') do (
	REM 这里可以自动上传 .snupkg 符号包，所以手动排除
	if not "%%~xa"==".snupkg" (
		set "nuget_file_list=!nuget_file_list! "%%a""
	)
)

REM 初始化计数器和记录变量
set "success_count=0"
set "error_count=0"
set "error_file="

REM 循环遍历文件列表
for %%f in (%nuget_file_list%) do (
	REM 换行
	echo.
	
	echo 正在上传：%%f
	
	REM 这里因为 dotnet 命令的错误码不会直接传递给批处理脚本的 %errorlevel% 变量。所以这里使用 && 和 || 运算符进行判断
	
	REM 上传 NuGet 服务器
	dotnet nuget push --api-key %api_key% --skip-duplicate --source https://api.nuget.org/v3/index.json %%f && (
		REM 记录成功次数
		set /a success_count+=1
	
		REM 换行
		echo.

		echo 上传：%%f 成功...... 
	) || (
		REM 记录失败次数
		set /a error_count+=1
		
		REM 记录失败文件
		set "error_file=!error_file! %%f"
	
		REM 换行
		echo.

		echo 上传：%%f 失败......
	)
)

REM 换行
echo.

REM 输出上传结果
echo 上传成功 %success_count% 个，失败 %error_count% 个，失败列表：

REM 循环遍历失败文件列表
for %%f in (%error_file%) do (
	echo %%f
)

REM 结束关闭变量延迟
endlocal

REM 等待用户按下任意键继续。
REM pause

set /p="按任意键继续 . . ." <nul
pause >nul
