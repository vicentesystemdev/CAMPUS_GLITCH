@echo off
setlocal EnableExtensions
chcp 65001 >nul
set "REPO=%~1"
echo.
echo =======================================================
echo  CAMPUS GLITCH - CLONAR REPOSITORIO
echo  Solo: feature/develop_victor_asturizaga
echo =======================================================
echo.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0CampusGlitch_Git.ps1" -Accion Clonar -RutaRepo "%REPO%"
set "RESULTADO=%ERRORLEVEL%"
echo.
if not "%RESULTADO%"=="0" (echo PROCESO DETENIDO. Codigo: %RESULTADO%) else (echo PROCESO FINALIZADO.)
pause
exit /b %RESULTADO%
