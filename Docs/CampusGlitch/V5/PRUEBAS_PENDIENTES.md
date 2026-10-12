# Auditoría de aceptación — estado del paquete

**Fecha:** 2026-10-11  
**Unity objetivo:** 6000.3.16f1  
**Estado:** código y recursos entregados; pruebas en Unity Editor pendientes.

## Pruebas obligatorias en Editor

- [ ] Unity importa todos los recursos sin errores de compilación ni `Missing (Mono Script)`.
- [ ] Se puede abrir `SC_Bootstrap.unity` y pulsar Play; menús con teclado y gamepad.
- [ ] Todas las 11 escenas aparecen en Build Settings y la escena inicial es Bootstrap.
- [ ] El lobo se mueve, corre, salta y cae con físicas y colliders sin atravesar suelo.
- [ ] Animaciones rojo/morado tienen fotogramas visibles y tamaño consistente.
- [ ] Mordida elimina un bug, daño resta corazones, muerte hace respawn.
- [ ] Los chips se recogen, la terminal hace una pregunta y bloquea salida hasta resolverla.
- [ ] Checkpoint funciona y desbloqueos se conservan al cerrar/abrir el juego.
- [ ] Niveles 1 a 9 se pueden terminar y Subsuelo es opcional.
- [ ] Nivel 2 desbloquea skin morada; nivel 5 desbloquea Subsuelo.
- [ ] Secuencias, IF/ELSE y Core Glitch tienen combates superables, vida y muerte.
- [ ] Pantallas de control reasignan botones para teclado y mando.
- [ ] Pantallas de pausa y menú responden correctamente en 16:9.
- [ ] La escena final muestra créditos de los cuatro desarrolladores.
- [ ] Integración de assets no deja referencias a sprites antiguos de GitHub.
- [ ] Build Windows x64 ejecuta inicio→final sin excepciones ni bloqueo.
- [ ] El BAT publica solo en `feature/develop_victor_asturizaga`, tras confirmación y varias confirmaciones reales.

## Limitaciones detectadas en esta entrega

- Unity Editor **no está instalado** en la máquina donde se armó el ZIP; por tanto no se realizó prueba real de compilación ni un build `.exe`.
- No se pudo ejecutar `git clone` desde esta terminal por DNS; el BAT se diseñó para clonación local bajo Windows. No se realizaron commits en el repositorio del usuario.
- Los niveles se generan por composición programática de tramos y assets oficiales; todavía necesitan dibujo manual de recorridos únicos, colocación refinada de props y QA de dificultades.
- Audio chiptune básico, sin grabaciones de voces ni música final profesional.
- No hay garantía de estar libre de bugs; cualquier excepción en Unity exige corrección antes de publicar.

## Criterio de finalización

No declarar el juego «100 % finalizado» hasta que todos los checks anteriores estén marcados como probados con evidencias.
