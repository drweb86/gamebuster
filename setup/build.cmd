@echo off

set msbuild=C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe
set logFile=build.log
set innoCompiler=C:\Program Files (x86)\Inno Setup 5\ISCC.exe

if not exist "%PROGRAMFILES(x86)%" (
	echo 64-bit windows is required.
	exit 1
)

if not exist "%msbuild%" (
	echo Microsoft Build Tools 2015 are not installed. Please install it and rerun script.
	exit 1
)

if not exist "%innoCompiler%" (
	echo Inno Setup 5.5.8 Unicode is not installed. Please install it and rerun script.
	exit 1
)

del %logFile% /F /Q

"%msbuild%" ../src/GameBuster.sln ^
	/t:Rebuild ^
	/p:Configuration=Release ^
	/fl /flp:logfile=%logFile%;verbosity=normal

if %ERRORLEVEL% NEQ 0 (
	echo Solution build failed. See the build log.
	pause
	exit 1
)

"%innoCompiler%" Setup.iss

if %ERRORLEVEL% NEQ 0 (
	echo Setup build failed
	pause
	exit 1
)
