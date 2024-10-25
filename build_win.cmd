@echo off
powershell -executionpolicy remotesigned -File %~dp0\build_win.ps1 -SourceFile "minimp3.c" -OutFile "minimp3_64.dll" -Arch "64"
powershell -executionpolicy remotesigned -File %~dp0\build_win.ps1 -SourceFile "minimp3.c" -OutFile "minimp3.dll" -Arch "32"
