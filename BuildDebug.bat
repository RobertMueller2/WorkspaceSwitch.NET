@echo off

:: Check if the configuration file exists
if not exist msbuild.cfg (
    echo Error: msbuild.cfg does not exist. Please copy either of the configs from "msbuild.cfg examples" to msbuild.cfg and/or edit the path in it.
    pause
    exit /b 1
)

:: Include the configuration file
for /f "delims=" %%x in (msbuild.cfg) do (set "%%x")

:: Use the MSBUILD_PATH variable set in the configuration file
%MSBUILD_PATH% /t:Build /p:Configuration=Debug

pause

