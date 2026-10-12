@echo off
setlocal EnableExtensions
chcp 65001 >nul
set "TOOLDIR=%~dp0"
:MENU
cls
echo ================================================
echo     CAMPUS GLITCH - GIT Y GITHUB (V5)
echo ================================================
echo 1 - Revisar Git, rama y archivos
echo 2 - Clonar rama desde GitHub
echo 3 - Instalar assets y scripts V5 en el repo
echo 4 - Crear commits por partes de cambios reales
echo 5 - Publicar rama tras pruebas de Unity
echo 0 - Salir
echo.
choice /C 123450 /N /M "Selecciona una opcion: "
set "SEL=%ERRORLEVEL%"
if "%SEL%"=="6" exit /b 0
if "%SEL%"=="1" call "%TOOLDIR%00_DIAGNOSTICO_GIT.bat"
if "%SEL%"=="2" call "%TOOLDIR%01_CLONAR_RAMA.bat"
if "%SEL%"=="3" call "%TOOLDIR%02_INSTALAR_EN_REPO.bat"
if "%SEL%"=="4" call "%TOOLDIR%03_COMMITS_POR_FASES.bat"
if "%SEL%"=="5" call "%TOOLDIR%04_PUBLICAR_RAMA.bat"
goto MENU
