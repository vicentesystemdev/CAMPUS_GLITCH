#requires -Version 5.1
<#
  Campus Glitch V5 - Git de Windows (PowerShell 5.1+).
  Nunca cambia main, nunca hace force-push y no publica sin confirmacion.
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [ValidateSet('Diagnostico','Clonar','Instalar','Commits','Publicar')]
  [string]$Accion,
  [string]$RutaRepo = '',
  [string]$RutaFuente = ''
)

$ErrorActionPreference = 'Stop'
$Rama = 'feature/develop_victor_asturizaga'
$UrlRepo = 'https://github.com/vicentesystemdev/CAMPUS_GLITCH.git'
$VersionUnity = '6000.3.16f1'

function GitSalida([string[]]$Parametros) {
    $salida = @(& git @Parametros)
    if ($LASTEXITCODE -ne 0) { throw ('Git fallo (' + $LASTEXITCODE + '): git ' + ($Parametros -join ' ') + "`n" + ($salida -join "`n")) }
    return (($salida | Out-String).Trim())
}
function GitEjecutar([string[]]$Parametros) {
    & git @Parametros
    if ($LASTEXITCODE -ne 0) { throw ('Git fallo (' + $LASTEXITCODE + '): git ' + ($Parametros -join ' ')) }
}
function Preguntar([string]$Pregunta, [string]$Esperado) {
    $r = (Read-Host $Pregunta).Trim()
    return [string]::Equals($r, $Esperado, [StringComparison]::OrdinalIgnoreCase)
}
function ResolverCarpeta([string]$Ruta, [string]$Pregunta) {
    if ([string]::IsNullOrWhiteSpace($Ruta)) { $Ruta = Read-Host $Pregunta }
    $Ruta = $Ruta.Trim().Trim('"')
    if ([string]::IsNullOrWhiteSpace($Ruta)) { throw 'No indicaste una ruta.' }
    return [System.IO.Path]::GetFullPath($Ruta)
}
function ElegirRepo([string]$Ruta) {
    if ([string]::IsNullOrWhiteSpace($Ruta)) {
        $candidato = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
        if (Test-Path -LiteralPath (Join-Path $candidato '.git')) { $Ruta = $candidato }
    }
    $Ruta = ResolverCarpeta $Ruta 'Ruta RAIZ de tu repositorio Git local (carpeta que contiene .git)'
    if (-not (Test-Path -LiteralPath $Ruta -PathType Container)) { throw "Carpeta inexistente: $Ruta" }
    if (-not (Test-Path -LiteralPath (Join-Path $Ruta '.git'))) { throw "Falta .git en $Ruta. Ejecuta CLONAR primero; un proyecto de Unity Hub no es por si solo un repositorio Git." }
    Push-Location -LiteralPath $Ruta
    try {
        $raiz = GitSalida @('rev-parse','--show-toplevel')
        if ([IO.Path]::GetFullPath($raiz).TrimEnd('\','/') -ine [IO.Path]::GetFullPath($Ruta).TrimEnd('\','/')) {
            throw "Selecciona la raiz del repositorio y no una subcarpeta. Raiz: $raiz"
        }
        $ramaActual = GitSalida @('branch','--show-current')
        if ($ramaActual -cne $Rama) { throw "Rama incorrecta: '$ramaActual'. Se requiere '$Rama'. Cambia de rama manualmente con el arbol limpio." }
        $remoto = GitSalida @('remote','get-url','origin')
        if ($remoto -notmatch '(?i)^(https://github\.com/|git@github\.com:|ssh://git@github\.com/)(vicentesystemdev/CAMPUS_GLITCH)(\.git)?/?$') {
            throw "El origin NO corresponde a CAMPUS_GLITCH: $remoto"
        }
        if (-not (Test-Path -LiteralPath 'ProjectSettings/ProjectVersion.txt')) { throw 'No se encontro ProjectSettings/ProjectVersion.txt.' }
        $v = Get-Content -LiteralPath 'ProjectSettings/ProjectVersion.txt' -Raw
        if ($v -notmatch [regex]::Escape($VersionUnity)) { throw "Proyecto con version de Unity diferente a $VersionUnity." }
        if (-not (Test-Path -LiteralPath 'Assets' -PathType Container)) { throw 'No existe Assets.' }
        if (-not (Test-Path -LiteralPath 'Packages/manifest.json')) { throw 'No existe Packages/manifest.json.' }
    } finally { Pop-Location }
    return $Ruta
}
function VerificarEstadoLimpio() {
    $cambios = GitSalida @('status','--porcelain','--untracked-files=normal')
    if (-not [string]::IsNullOrWhiteSpace($cambios)) {
        Write-Host $cambios
        throw 'Hay cambios locales sin confirmar. Guardalos en una rama/commit o respaldata antes de instalar/publicar. No se borrara nada.'
    }
}
function CopiarArbol([string]$Origen, [string]$Destino) {
    if (-not (Test-Path -LiteralPath $Origen -PathType Container)) { throw "Carpeta de origen inexistente: $Origen" }
    if (-not (Test-Path -LiteralPath $Destino)) { New-Item -ItemType Directory -Path $Destino -Force | Out-Null }
    & robocopy.exe $Origen $Destino /E /R:2 /W:1 /NFL /NDL /NJH /NJS /NP
    $rc = $LASTEXITCODE
    if ($rc -ge 8) { throw "Error de robocopy codigo $rc. Origen: $Origen" }
}

try {
    if (-not (Get-Command git.exe -ErrorAction SilentlyContinue)) {
        throw 'Git para Windows no esta instalado o no esta en PATH. Instala Git for Windows y vuelve a abrir la terminal.'
    }
    if ($Accion -eq 'Clonar') {
        $destino = ResolverCarpeta $RutaRepo 'Ruta NUEVA donde clonar el repo (por ejemplo C:\Proyectos\CAMPUS_GLITCH)'
        if (Test-Path -LiteralPath $destino) {
            if ((Get-ChildItem -LiteralPath $destino -Force | Measure-Object).Count -gt 0) { throw 'La carpeta para clonar ya contiene archivos. Elige una carpeta NUEVA o vacia.' }
        }
        $parent = Split-Path -Parent $destino
        if (-not (Test-Path -LiteralPath $parent)) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
        Write-Host "Clonando unicamente $Rama en $destino" -ForegroundColor Cyan
        GitEjecutar @('clone','--branch',$Rama,'--single-branch',$UrlRepo,$destino)
        Write-Host 'Clon completado. Ahora usa INSTALAR con esa ruta.' -ForegroundColor Green
        exit 0
    }

    $repo = ElegirRepo $RutaRepo
    Push-Location -LiteralPath $repo
    try {
        $head = GitSalida @('rev-parse','--short','HEAD')
        $remote = GitSalida @('remote','get-url','origin')
        Write-Host "Repositorio: $repo" -ForegroundColor Cyan
        Write-Host "Rama: $Rama | HEAD: $head | Origin: $remote"
        if ($Accion -eq 'Diagnostico') {
            Write-Host '--- Estado Git ---'
            GitEjecutar @('status','--short','--branch')
            Write-Host '--- Ultimos commits ---'
            GitEjecutar @('log','-5','--pretty=format:%h %s')
            Write-Host "`nDiagnostico terminado (sin modificar archivos)." -ForegroundColor Green
        }
        elseif ($Accion -eq 'Instalar') {
            VerificarEstadoLimpio
            if ([string]::IsNullOrWhiteSpace($RutaFuente)) { $RutaFuente = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..')) }
            $fuente = [IO.Path]::GetFullPath($RutaFuente)
            $fuenteAssets = Join-Path $fuente 'Assets/CampusGlitch'
            if ($fuente.TrimEnd('\','/') -ieq $repo.TrimEnd('\','/')) {
                Write-Host 'El origen y el repositorio son la misma carpeta. Los archivos ya estan dentro: usa COMMITS para registrar cambios existentes.' -ForegroundColor Yellow
                exit 0
            }
            if (-not (Test-Path -LiteralPath $fuenteAssets)) { throw "No existe $fuenteAssets. Ejecuta este BAT desde Tools dentro del ZIP V5 descomprimido." }
            Write-Host "Origen V5: $fuente"
            Write-Host 'Se copiaran Assets/CampusGlitch, Tools y Docs. NO se sobreescribe ProjectSettings ni Packages. NO se hace push.'
            if (-not (Preguntar 'Escribe INSTALAR para comenzar' 'INSTALAR')) { Write-Host 'Cancelado.'; exit 0 }
            $previo = Join-Path $repo 'Assets/CampusGlitch'
            if (Test-Path -LiteralPath $previo) {
                $stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
                $respaldo = Join-Path (Split-Path -Parent $repo) ("CAMPUS_GLITCH_BACKUP_" + $stamp)
                New-Item -ItemType Directory -Path $respaldo -Force | Out-Null
                CopiarArbol $previo (Join-Path $respaldo 'CampusGlitch')
                Write-Host "Respaldo fuera del repositorio: $respaldo"
            }
            CopiarArbol $fuenteAssets $previo
            if (Test-Path -LiteralPath (Join-Path $fuente 'Docs')) { CopiarArbol (Join-Path $fuente 'Docs') (Join-Path $repo 'Docs/CampusGlitch/V5') }
            if (Test-Path -LiteralPath (Join-Path $fuente 'Tools')) { CopiarArbol (Join-Path $fuente 'Tools') (Join-Path $repo 'Tools') }
            Write-Host 'Archivos copiados. No se creo commit ni se hizo push.' -ForegroundColor Green
            GitEjecutar @('status','--short')
            Write-Host 'Abre el repo con Unity Universal 2D y prueba antes de publicar.'
        }
        elseif ($Accion -eq 'Commits') {
            $stagedPre = GitSalida @('diff','--cached','--name-only')
            if (-not [string]::IsNullOrWhiteSpace($stagedPre)) { throw 'Ya hay archivos en staging. Haz un commit manual o desmarca esos archivos antes; no mezclaremos cambios ajenos.' }
            $nombre = ((& git config user.name) | Out-String).Trim()
            $correo = ((& git config user.email) | Out-String).Trim()
            if (-not $nombre -or -not $correo) { throw 'Falta identidad Git. Ejecuta: git config --global user.name "Tu Nombre" y git config --global user.email "tu@email.com"' }
            $grupos = @(
                @{ Mensaje='Incorporar fuentes y sprites oficiales del campus'; Rutas=@('Assets/CampusGlitch/Art','Assets/CampusGlitch/Art.meta','Assets/CampusGlitch/ArtSource','Assets/CampusGlitch/ArtSource.meta','Assets/CampusGlitch.meta') },
                @{ Mensaje='Integrar escenarios, efectos y recursos del juego'; Rutas=@('Assets/CampusGlitch/Resources','Assets/CampusGlitch/Resources.meta') },
                @{ Mensaje='Actualizar la jugabilidad, las animaciones y los controles'; Rutas=@('Assets/CampusGlitch/Scripts','Assets/CampusGlitch/Scripts.meta') },
                @{ Mensaje='Configurar las escenas y herramientas del editor'; Rutas=@('Assets/CampusGlitch/Scenes','Assets/CampusGlitch/Scenes.meta','Assets/CampusGlitch/Editor','Assets/CampusGlitch/Editor.meta') },
                @{ Mensaje='Documentar la integracion y los controles de Git'; Rutas=@('Tools','Docs/CampusGlitch','README_V5_CORRECCIONES_2D.md') }
            )
            Write-Host 'Solo se versionan rutas especificas de CampusGlitch. No se usan git add . ni push automatico.'
            GitEjecutar @('status','--short')
            if (-not (Preguntar 'Escribe COMMITS si ya revisaste los cambios y quieres registrar los avances' 'COMMITS')) { Write-Host 'Cancelado.'; exit 0 }
            $total = 0
            foreach ($g in $grupos) {
                $permitidas = @()
                foreach ($ruta in $g.Rutas) {
                    $registrados = GitSalida @('ls-files','--',$ruta)
                    if ((Test-Path -LiteralPath $ruta) -or $registrados) { $permitidas += $ruta }
                }
                if ($permitidas.Count -eq 0) { continue }
                $argumentos = @('status','--porcelain','--') + $permitidas
                $cambiosGrupo = GitSalida $argumentos
                if (-not $cambiosGrupo) { continue }
                Write-Host "`n--- $($g.Mensaje) ---" -ForegroundColor Cyan
                GitEjecutar (@('add','-A','--') + $permitidas)
                $preparados = GitSalida @('diff','--cached','--name-only')
                if (-not $preparados) { continue }
                GitEjecutar @('diff','--cached','--stat')
                GitEjecutar @('commit','-m',$g.Mensaje)
                $total++
            }
            Write-Host "Commits reales creados: $total" -ForegroundColor Green
            Write-Host 'Los archivos de otras carpetas NO fueron incluidos. NO se realizo push.'
            GitEjecutar @('status','--short','--branch')
        }
        elseif ($Accion -eq 'Publicar') {
            VerificarEstadoLimpio
            Write-Host 'Buscando cambios en remoto...'
            GitEjecutar @('fetch','origin',$Rama)
            & git merge-base --is-ancestor ("origin/" + $Rama) 'HEAD' *> $null
            if ($LASTEXITCODE -ne 0) { throw 'El remoto contiene cambios que no estan en esta rama local, o hay historial divergente. NO se publicara. Revisar git log y hacer integracion manual.' }
            Write-Host 'Ultimos commits locales:'
            GitEjecutar @('log','-8','--oneline')
            GitEjecutar @('push','--dry-run','origin',('HEAD:refs/heads/' + $Rama))
            Write-Host 'Asegurate de haber probado escenas, animations y build en Unity antes de publicar.' -ForegroundColor Yellow
            if (-not (Preguntar 'Para publicar, escribe PUBLICAR (esto enviara los commits a GitHub)' 'PUBLICAR')) { Write-Host 'Cancelado sin push.'; exit 0 }
            GitEjecutar @('push','origin',('HEAD:refs/heads/' + $Rama))
            Write-Host 'Push enviado a la rama indicada. Verifica GitHub.' -ForegroundColor Green
        }
    } finally { Pop-Location }
}
catch {
    Write-Host "`nERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host 'Ningun reset --hard, clean, rebase, force push ni cambios de rama automaticos fueron ejecutados.' -ForegroundColor Yellow
    exit 1
}
