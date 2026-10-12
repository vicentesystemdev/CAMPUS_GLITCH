@echo off
setlocal EnableExtensions
cd /d "%~dp0.."
echo.
echo [CAMPUS GLITCH 2D] REPARAR ESCENAS - Unity 6000.3.16f1
echo Si el Editor esta abierto, USA el menu: Campus Glitch ^> Reparar escenas y mostrar en 2D
echo Este BAT requiere Unity CERRADO para evitar que dos instancias usen el mismo proyecto.
echo.
set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
if not exist "%UNITY%" (
 echo ERROR: no se encontro Unity. Edita la ruta UNITY dentro de este archivo.
 pause
 exit /b 1
)
set "PROYECTO=%CD%"
set "LOG=%PROYECTO%\campusglitch_escenas.log"
"%UNITY%" -batchmode -nographics -quit -projectPath "%PROYECTO%" -executeMethod CampusGlitch.Editor.CGBuildScenes.GenerateAll -logFile "%LOG%"
if errorlevel 1 (
 echo.
 echo ERROR al reparar las escenas. Revisa "%LOG%" y la consola del Editor.
 pause
 exit /b 2
)
echo.
echo Comando finalizado. Comprueba el log y abre SC_Level01 en Unity.
pause
