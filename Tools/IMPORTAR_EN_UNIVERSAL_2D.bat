@echo off
setlocal EnableExtensions
chcp 65001 >nul
set "SOURCE=%~dp0..\Assets\CampusGlitch"
echo.
echo CAMPUS GLITCH - IMPORTACION A PROYECTO UNIVERSAL 2D REAL
set /p "TARGET=Ruta completa del proyecto CREADO EN UNITY HUB con plantilla UNIVERSAL 2D: "
set "TARGET=%TARGET:"=%"
if not exist "%TARGET%\Assets" (echo ERROR: no existe la carpeta Assets del proyecto. & pause & exit /b 1)
if not exist "%TARGET%\ProjectSettings\ProjectVersion.txt" (echo ERROR: no es raiz de un proyecto Unity. & pause & exit /b 2)
findstr /C:"6000.3.16f1" "%TARGET%\ProjectSettings\ProjectVersion.txt" >nul
if errorlevel 1 (echo ERROR: proyecto no corresponde a Unity 6000.3.16f1. & pause & exit /b 3)
if not exist "%TARGET%\Assets\Settings\Renderer2D.asset" (
 echo ATENCION: no encontro Renderer2D.asset en Assets\Settings.
 echo Confirma que elegiste UNIVERSAL 2D y no UNIVERSAL 3D.
 pause
 exit /b 4
)
if exist "%TARGET%\Assets\CampusGlitch" (
 echo YA existe CampusGlitch en el destino. Se fusionaran archivos sin eliminar los existentes.
 choice /C SN /M "Continuar"
 if errorlevel 2 exit /b 5
)
echo Copiando sprites oficiales, escenas, codigo y editores...
robocopy "%SOURCE%" "%TARGET%\Assets\CampusGlitch" /E /R:2 /W:1
set "RC=%ERRORLEVEL%"
if %RC% GEQ 8 (echo ERROR de copia: %RC% & pause & exit /b 6)
echo.
echo Ajustando solo paquetes que necesita Campus Glitch; preservando URP 2D del Hub...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0AGREGAR_DEPENDENCIAS_2D.ps1" -Project "%TARGET%"
if errorlevel 1 (echo ADVERTENCIA: fallo al agregar paquetes. Comprueba Packages/manifest.json. & pause & exit /b 7)
echo IMPORTACION COPIADA. Cierra este BAT y abre el proyecto destino en Unity Hub.
echo Despues selecciona Campus Glitch ^> 01 Validar y configurar Universal 2D.
echo Despues selecciona Campus Glitch ^> Reparar escenas y mostrar en 2D.
echo NO ejecutes build ni Git sin antes probar todos los niveles.
pause
