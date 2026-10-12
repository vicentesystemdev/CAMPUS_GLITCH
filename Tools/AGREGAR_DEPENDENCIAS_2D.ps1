param([Parameter(Mandatory=$true)][string]$Project)
$path = Join-Path $Project 'Packages/manifest.json'
if (!(Test-Path $path)) { throw 'No Packages/manifest.json en el proyecto Universal 2D' }
Copy-Item $path ($path + '.pre_campus_glitch.bak') -Force
$manifest = Get-Content -Raw -Encoding UTF8 $path | ConvertFrom-Json
$required = [ordered]@{
 'com.unity.inputsystem'='1.19.0'
 'com.unity.2d.aseprite'='3.0.2'
 'com.unity.2d.animation'='13.0.5'
 'com.unity.2d.tilemap'='1.0.0'
 'com.unity.modules.physics2d'='1.0.0'
 'com.unity.modules.audio'='1.0.0'
 'com.unity.modules.imageconversion'='1.0.0'
}
foreach($key in $required.Keys){
  if (!($manifest.dependencies.PSObject.Properties.Name -contains $key)) {
    $manifest.dependencies | Add-Member -MemberType NoteProperty -Name $key -Value $required[$key]
    Write-Host ('Se agrego dependencia requerida: ' + $key)
  }
}
# Do NOT rewrite existing com.unity.render-pipelines.universal; Hub owns that version and Renderer2D.
$manifest | ConvertTo-Json -Depth 80 | Set-Content -Encoding UTF8 $path
Write-Host 'Paquetes requeridos registrados; se preservaron URP y los ajustes originales del Hub.'
