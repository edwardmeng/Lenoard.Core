@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
) else (
   set version=-Version 0.1.0
)
REM Determine msbuild path
set msbuildtmp="%ProgramFiles%\MSBuild\14.0\bin\msbuild"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set msbuildtmp="%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set VisualStudioVersion=14.0

REM Package restore
echo.
echo Running package restore...
call :ExecuteCmd nuget.exe restore ..\Lenoard.Core.sln -NonInteractive -ConfigFile nuget.config
IF %ERRORLEVEL% NEQ 0 goto error

echo Building solution...
call :ExecuteCmd %msbuild% "..\Lenoard.Core.sln" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
IF %ERRORLEVEL% NEQ 0 goto error

echo Packaging...
set libtmp=%cd%\lib
set packagestmp="%cd%\packages"
if not exist %libtmp% mkdir %libtmp%
if not exist %packagestmp% mkdir %packagestmp%

if not exist %libtmp%\net40 mkdir %libtmp%\net40
copy ..\src\bin\%config%\Lenoard.Core.dll %libtmp%\net40 /Y
copy ..\src\bin\%config%\Lenoard.Core.xml %libtmp%\net40 /Y

if not exist %libtmp%\netstandard1.3 mkdir %libtmp%\netstandard1.3
copy ..\netcore\Lenoard.Core\bin\%config%\netstandard1.3\Lenoard.Core.dll %libtmp%\netstandard1.3 /Y
copy ..\netcore\Lenoard.Core\bin\%config%\netstandard1.3\Lenoard.Core.xml %libtmp%\netstandard1.3 /Y
copy ..\netcore\Lenoard.Core\bin\%config%\netstandard1.3\Lenoard.Core.deps.json %libtmp%\netstandard1.3 /Y


call :ExecuteCmd nuget.exe pack "%cd%\Lenoard.Core.nuspec" -OutputDirectory %packagestmp% %version%
IF %ERRORLEVEL% NEQ 0 goto error

rmdir %libtmp% /S /Q

goto end

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
exit /b %ERRORLEVEL%

:error
endlocal
echo An error has occurred during build.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Build finished successfully.