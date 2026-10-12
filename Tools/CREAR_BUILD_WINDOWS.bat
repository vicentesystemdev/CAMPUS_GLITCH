@echo off
setlocal EnableExtensions
cd /d "%~dp0.."
set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
if not exist "%UNITY%" (
 echo ERROR: Unity 6000.3.16f1 no encontrado. Ajusta UNITY en el BAT.
 pause
 exit /b 1
)
echo [CAMPUS GLITCH 2D] Cierra Unity antes del build automatico.
"%UNITY%" -batchmode -nographics -quit -projectPath "%CD%" -executeMethod CampusGlitch.Editor.CGBuildScenes.BuildWindows -logFile "%CD%\campusglitch_build.log"
if errorlevel 1 (
 echo ERROR: build no completado. Revisa campusglitch_build.log
 pause
 exit /b 2
)
if not exist "%CD%\Build_Windows\CampusGlitch.exe" (
 echo ERROR: Unity no produjo CampusGlitch.exe. Revisa campusglitch_build.log
 pause
 exit /b 3
)
echo Build disponible en Build_Windows\CampusGlitch.exe
pause
