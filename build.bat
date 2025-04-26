@echo off

set msbuildcfg=msbuild.cfg
set MSBUILD_PATH=
set havedotnet=
set havemsbuild=
set anyfound=

if exist %msbuildcfg% (   
    for /f "usebackq delims=" %%x in (%msbuildcfg%) do (set "%%x")
)

if defined MSBUILD_PATH (
  if exist %MSBUILD_PATH% (
    set havemsbuild=12
    set anyfound=1
  )
)

where /Q dotnet.exe && (
  set havedotnet=34
  set anyfound=1
)

if not "%anyfound%"=="1" (
    echo Neither MSBuild nor dotnet build found.
    echo=
    pause
    exit /b 1
)

echo 1. Debug Build
echo 2. Release Build
choice /c 12 /m "Select build config: "

if errorlevel 2 (
    goto release    
)
if errorlevel 1 (
    goto debug
)

:release
set CONFIG=Release
goto config_end

:debug
set CONFIG=Debug
goto config_end

:config_end

echo Building Config %CONFIG%
timeout /t 1

if defined havemsbuild (
    if not defined havedotnet goto build_msbuild
)

if defined havedotnet (
    if not defined havemsbuild goto build_dotnet
)

echo 1. dotnet
echo 2. MSBuild
choice /c 12 /m "Select build system: "

if errorlevel 2 (
  goto build_msbuild
)
if errorlevel 1 (
  goto build_dotnet
)

:build_msbuild
%MSBUILD_PATH% /p:Configuration=%CONFIG%
goto end

:build_dotnet
dotnet build -c %CONFIG%
goto end

:end
pause