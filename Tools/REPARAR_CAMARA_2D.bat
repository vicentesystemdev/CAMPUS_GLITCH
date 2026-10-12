@echo off
setlocal EnableExtensions
chcp 65001 >nul
title CAMPUS GLITCH - REPARACION CAMARA 2D - UNITY 6000.3.16f1
echo.
echo ==============================================
echo     CAMPUS GLITCH - REPARAR CAMARA Y LUZ 2D
echo ==============================================
echo.
echo ATENCION: Cierra Unity completamente antes de ejecutar este archivo.
echo.
set "PROJECT="
set /p "PROJECT=Ruta COMPLETA del proyecto abierto desde Unity Hub: "
set "PROJECT=%PROJECT:"=%"
if not exist "%PROJECT%\Assets\CampusGlitch\Editor\CGCameraRepairTool.cs" (
 echo ERROR: Instala primero el parche en la carpeta Assets del proyecto.
 pause
 exit /b 3
)
if not exist "%PROJECT%\ProjectSettings\ProjectVersion.txt" (
 echo ERROR: Esta ruta no es la raiz de un proyecto Unity.
 pause
 exit /b 4
)
findstr /C:"6000.3.16f1" "%PROJECT%\ProjectSettings\ProjectVersion.txt" >nul
if errorlevel 1 (
 echo ERROR: Version de Unity incorrecta. Se requiere 6000.3.16f1.
 pause
 exit /b 5
)
set "UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
if not exist "%UNITY%" (
 set /p "UNITY=No encuentro Unity. Ruta COMPLETA de Unity.exe: "
 set "UNITY=%UNITY:"=%"
)
if not exist "%UNITY%" (
 echo ERROR: Unity.exe no existe en la ruta indicada.
 pause
 exit /b 6
)
if not exist "%PROJECT%\Docs" mkdir "%PROJECT%\Docs"
echo.
echo Se van a reparar las 18 escenas, su camara y Global Light 2D.
echo NO se enviara nada a GitHub.
choice /C SN /M "Ejecutar ahora"
if errorlevel 2 exit /b 0
"%UNITY%" -batchmode -nographics -quit -projectPath "%PROJECT%" -executeMethod CampusGlitch.Editor.CGCameraRepairTool.RepairAll -logFile "%PROJECT%\Docs\CAMARA_2D_BATCH.log"
set "ERR=%ERRORLEVEL%"
echo.
if not "%ERR%"=="0" (
 echo ERROR durante la reparacion. Codigo: %ERR%
 echo Revisa "%PROJECT%\Docs\CAMARA_2D_BATCH.log"
 pause
 exit /b %ERR%
)
if exist "%PROJECT%\Docs\CAMARA_2D_DIAGNOSTICO.txt" (
 type "%PROJECT%\Docs\CAMARA_2D_DIAGNOSTICO.txt"
)
echo.
echo Reparacion solicitada. Abre Unity, revisa Console y Game con Play.
echo Si la camara sigue negra, usa la prueba magenta-amarillo del menu CAMARA 2D.
pause
