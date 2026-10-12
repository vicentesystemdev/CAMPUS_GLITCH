@echo off
setlocal EnableExtensions
cd /d "%~dp0.."
set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
if not exist "%UNITY%" (echo Unity no encontrado. Cambia la ruta UNITY. & pause & exit /b 1)
"%UNITY%" -batchmode -nographics -quit -projectPath "%CD%" -executeMethod CampusGlitch.Editor.CG2DRenderSetup.Configure -logFile "%CD%\campusglitch_urp2d.log"
if errorlevel 1 (echo ERROR: ver campusglitch_urp2d.log & pause & exit /b 2)
echo URP 2D configurado o se ha dejado una advertencia en el log.
pause
