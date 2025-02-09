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

if defined havemsbuild (
    echo 1. MSBuild debug
    echo 2. MSBuild release
)
if defined havedotnet (
    echo 3. dotnet build debug
    echo 4. dotnet build release
)

choice /c %havemsbuild%%havedotnet% /m "Select build option: "

REM does not work unless in descending order
if errorlevel 4 (
    echo dotnet release
    dotnet build -c Release
)
if errorlevel 3 (
    echo dotnet debug
    dotnet build -c Debug
)
if errorlevel 2 (
    echo MSBuild Release
    %MSBUILD_PATH% /p:Configuration=Release
)
if errorlevel 1 ( 
    echo MSBuild Debug
    %MSBUILD_PATH% /p:Configuration=Debug
)

pause