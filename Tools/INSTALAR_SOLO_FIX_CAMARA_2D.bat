@echo off
setlocal EnableExtensions
chcp 65001 >nul
title CAMPUS GLITCH - INSTALAR PARCHE DE CAMARA 2D
set "SOURCE=%~dp0..\Assets\CampusGlitch"
echo.
echo CAMPUS GLITCH - PARCHE DE CAMARA Y LUZ 2D
set /p "DEST=Ruta RAIZ del proyecto Unity Universal 2D en tu PC: "
set "DEST=%DEST:"=%"
if not exist "%DEST%\ProjectSettings\ProjectVersion.txt" (
 echo ERROR: carpeta Unity invalida.
 pause
 exit /b 2
)
findstr /C:"6000.3.16f1" "%DEST%\ProjectSettings\ProjectVersion.txt" >nul
if errorlevel 1 (
 echo ERROR: version Unity diferente de 6000.3.16f1.
 pause
 exit /b 3
)
if not exist "%DEST%\Assets\CampusGlitch\Scripts\CGGame.cs" (
 echo ERROR: proyecto CampusGlitch no instalado en esa carpeta.
 pause
 exit /b 4
)
echo Cierra Unity ahora. Se reemplazaran SOLO cinco scripts y sus meta.
if not exist "%DEST%\Docs\Respaldo_Camara_V5" mkdir "%DEST%\Docs\Respaldo_Camara_V5"
choice /C SN /M "Continuar"
if errorlevel 2 exit /b 0
for %%F in (CGGame.cs CGCamera2DRuntime.cs) do (
 if exist "%DEST%\Assets\CampusGlitch\Scripts\%%F" copy /Y "%DEST%\Assets\CampusGlitch\Scripts\%%F" "%DEST%\Docs\Respaldo_Camara_V5\%%F" >nul
 copy /Y "%SOURCE%\Scripts\%%F" "%DEST%\Assets\CampusGlitch\Scripts\%%F" >nul
 if errorlevel 1 goto :FAIL
)
for %%F in (CGCamera2DRuntime.cs.meta) do (
 copy /Y "%SOURCE%\Scripts\%%F" "%DEST%\Assets\CampusGlitch\Scripts\%%F" >nul
 if errorlevel 1 goto :FAIL
)
for %%F in (CGBuildScenes.cs CGProjectValidator.cs CGCameraRepairTool.cs) do (
 if exist "%DEST%\Assets\CampusGlitch\Editor\%%F" copy /Y "%DEST%\Assets\CampusGlitch\Editor\%%F" "%DEST%\Docs\Respaldo_Camara_V5\%%F" >nul
 copy /Y "%SOURCE%\Editor\%%F" "%DEST%\Assets\CampusGlitch\Editor\%%F" >nul
 if errorlevel 1 goto :FAIL
)
copy /Y "%SOURCE%\Editor\CGCameraRepairTool.cs.meta" "%DEST%\Assets\CampusGlitch\Editor\CGCameraRepairTool.cs.meta" >nul
if errorlevel 1 goto :FAIL
echo.
echo PARCHE INSTALADO. Abre Unity, espera compilar y selecciona:
echo Campus Glitch ^> CAMARA 2D ^> 01 - Reparar TODAS las escenas y luces
echo No borres los scripts anteriores ni cambies Packages/ProjectSettings.
pause
exit /b 0
:FAIL
echo ERROR copiando archivos. Revisa permisos y cierra Unity.
pause
exit /b 9
