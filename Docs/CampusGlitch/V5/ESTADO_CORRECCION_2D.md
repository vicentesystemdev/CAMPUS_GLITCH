# Reparación de escenas 2D de Campus Glitch

Se corrigió el error de audio incluyendo el módulo built-in `com.unity.modules.audio` en `Packages/manifest.json`.

Las once escenas ahora tienen una cámara **Main Camera ortográfica**, `AudioListener`, y una vista previa lateral con sprites modulares de `Resources/CG`. Los objetos de previsualización se destruyen en tiempo de ejecución y `CGWorld.Setup` genera el terreno, las colisiones, enemigos, NPCs y la física real.

Se reemplazó el generador de escenas que antes no hacía nada cuando ya existían. En Unity ejecutar **Campus Glitch > Reparar escenas y mostrar en 2D**.

Estos archivos corrigen **el problema visual y el error de módulo de audio** mostrados en las capturas. No constituyen una certificación de campaña completa ni de compilación ya ejecutada: deben probarse con Unity 6000.3.16f1 en Windows.
