# Validación en Unity pendiente — V5

La preparación de este ZIP incluye una revisión estática de GUIDs, rutas, sprites, la estructura binaria de 2 Aseprite y llaves de scripts C#.

NO se ha ejecutado Unity Editor en este entorno. No se han realizado pruebas PlayMode ni se ha generado un build de Windows nuevo. Estas pruebas son necesarias para declarar el juego listo:

1. Importar Assets/CampusGlitch en plantilla Universal 2D de Unity Hub 6000.3.16f1 y resolver dependencias.
2. Ejecutar Campus Glitch → Reparar escenas y mostrar en 2D. Deberán aparecer 18 escenas en Build Settings.
3. Abrir cada escena y comprobar que los paneles temáticos se visualizan, que la cámara sea ortográfica y que el preview reproduzca el contenido runtime.
4. Probar el nuevo lobo bípedo en Idle, Walk, Run, Jump, Attack y Hurt sin congelaciones; comprobar que los .aseprite están reconocidos.
5. Recorrer diez sectores para validar plataformas, enemigos, checkpoints, quizzes y transiciones.
6. Probar UI de mapa, controles, personalización y créditos (teclado y mando real), guardar y cargar, volver al menú.
7. Revisar errores rojos y referencias Missing Sprite; correr build Windows y probar EXE + Data.
8. Solo entonces commits individuales con cambios reales y push a feature/develop_victor_asturizaga, sin fuerza.

No afirmar un resultado de esas pruebas sin evidencia.
