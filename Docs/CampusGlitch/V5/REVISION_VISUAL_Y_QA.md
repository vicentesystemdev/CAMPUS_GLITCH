# Problemas vistos en las capturas y soluciones realizadas

- Menús planos de Unity IMGUI: estilo de tarjetas oscuro/naranja, fondo artístico pixelado, retrato animado del lobo, mejor jerarquía.
- Overlay del HUD cuando se pausaba: evitar dibujar HUD bajo ventana modal y separar información superior.
- Niveles vacíos: ambientación modular específica por nivel, separación visual de arquitectura y colisiones, cambios de altura y luz.
- Lobo provisional de 32×48: revisión a 64×80, 21 poses/animaciones por skin, fuente `.aseprite` editable, posibilidad de generar Animator nativo.
- Objetos sin animación: 68 frames de efectos de sprites para checkpoint/portal/terminal y otros elementos.
- Mapas desincronizados: selección del teclado actualiza `mapSelection`.
- Transición inexistente: pantalla breve de carga al cambiar nivel.

## Qué debe probar el equipo en Unity

[ ] Abre SC_Level01 en Scene 2D, sin cámara de perspectiva activa.
[ ] SC_Bootstrap muestra portada pixel art y permite seleccionar partida.
[ ] El lobo camina, corre, salta, cae, muerde, recibe daño; sus frames se actualizan.
[ ] El lobo morado se bloquea hasta completar N2, sin alterar hitbox.
[ ] Los NPC, checkpoints, portales y terminales funcionan.
[ ] Un Gamepad físico navega menú y controla el lobo; reasignación persiste.
[ ] Los diez sectores pueden recorrer hasta final sin quedarse bloqueado.
[ ] Los jefes tienen telegráficos, vulnerabilidad y victoria verdadera (requiere pulido adicional).
[ ] Los créditos aparecen luego de vencer Core Glitch.
[ ] La compilación Windows x64 ejecuta sin errores críticos.
[ ] No hay referencias a assets descartados de GitHub.

**Si falla uno de estos puntos no declarar juego listo para publicar.**
