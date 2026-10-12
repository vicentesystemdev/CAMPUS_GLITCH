@echo off
setlocal EnableExtensions
cd /d "%~dp0.."
set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
if not exist "%UNITY%" (
 echo ERROR: Unity 6000.3.16f1 no encontrado en la ruta predeterminada.
 echo Modifica la variable UNITY en este archivo.
 pause
 exit /b 1
)
echo [CAMPUS GLITCH 2D] Ejecutando validaciones. Cierra Unity primero.
"%UNITY%" -batchmode -nographics -quit -projectPath "%CD%" -executeMethod CampusGlitch.Editor.CGProjectValidator.Run -logFile "%CD%\campusglitch_validacion.log"
if errorlevel 1 (
 echo ERROR. Revisar campusglitch_validacion.log
 pause
 exit /b 2
)
echo Validaciones automatizadas finalizadas. Aun es obligatorio probar niveles manualmente.
pause
