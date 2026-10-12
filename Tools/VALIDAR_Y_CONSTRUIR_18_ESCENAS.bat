@echo off
setlocal EnableExtensions
cd /d "%~dp0.."
if not exist "Assets\CampusGlitch\Scenes\SC_Bootstrap.unity" (echo Falta SC_Bootstrap & exit /b 3)
if not exist "Assets\CampusGlitch\Scenes\SC_Credits.unity" (echo Faltan pantallas UI & exit /b 4)
if not exist "Assets\CampusGlitch\Resources\CG\Themes\Level09_Panel0.png" (echo Falta la nueva biblioteca visual & exit /b 5)
if not exist "ProjectSettings\ProjectVersion.txt" (echo Falta ProjectVersion & exit /b 6)
findstr /c:"6000.3.16f1" "ProjectSettings\ProjectVersion.txt" >nul || (echo Version Unity incorrecta & exit /b 7)
set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
if not exist "%UNITY%" (echo Unity no se encontro en la ruta habitual. Configura UNITY en este BAT. & exit /b 8)
echo Ejecutando importacion en el Editor de Unity, con proyecto cerrado...
"%UNITY%" -batchmode -nographics -quit -projectPath "%CD%" -executeMethod CampusGlitch.Editor.CGBuildScenes.GenerateAll -logFile "%CD%\validacion_campus_v5.log"
if errorlevel 1 (echo Unity notifico errores. Revisa validacion_campus_v5.log & exit /b 9)
echo Importacion finalizada. Verifica la consola, el arte y el recorrido antes de publicar Git.
endlocal
