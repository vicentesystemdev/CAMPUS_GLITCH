DOCUMENTO DE DISEÑO DE JUEGO (GDD): IDEA PRINCIPAL

1. Ficha Técnica y Concepto Core
• Nombre Provisional: Proyecto Jarvis (CAMPUS_GLITCH)
• Género: Plataformas 2D / Acción Arcade / Puzzle Lógico (Side-scrolling)
• Público Objetivo: Estudiantes de primer semestre de la Universidad Franz Tamayo (UNIFRANZ), específicamente de la carrera de Ingeniería de Sistemas.
• Premisa: Un estudiante, representado como un lobo cyberpunk, debe abrirse paso a través de una versión corrompida digitalmente de su propia universidad, enfrentando la carga académica materializada en monstruos de código y hardware.

───

2. Narrativa y Lore
2.1 La Anomalía Digital
Un error crítico de origen desconocido (posiblemente un desbordamiento de búfer en el servidor central) ha provocado que la red interna de la UNIFRANZ colapse y se superponga con la realidad física. Las señales de orientación se han vuelto encriptadas, las puertas exigen validación de tokens inexistentes, y el estrés académico ha cobrado vida formando criaturas llamadas "Bugs" .

2.2 Objetivos y Coleccionables
• Misión Principal: Escalar desde la Planta Baja hasta la Terraza para reiniciar el servidor central (Enrutador Maestro).
• Coleccionables ("Fragmentos de Información"): Dispersos por el mapa habrá disquetes o memorias USB que contienen fragmentos del plan de estudios, consejos de orientación universitaria o easter eggs sobre programación y bases de datos. Recolectarlos todos en un nivel otorga una calificación de "Excelencia" al finalizarlo.

───

3. Personaje Principal: El Lobo Cyberpunk
El protagonista es el avatar del jugador. Su diseño mezcla la mascota de la universidad con implantes biomecánicos y de neón.

3.1 Controles y Capacidades (Máquina de Estados - FSM)
El Character Controller se rige por los siguientes estados y mecánicas, usando el Unity Input System:
• Idle (Reposo): Animación base. Si se deja inactivo mucho tiempo, el lobo revisa un holograma de su horario de clases.
• Run (Correr): Movimiento horizontal con físicas de aceleración y fricción para un control preciso (Arcade)
• Jump (Salto): Movimiento vertical con Coyote Time (margen de salto al caer de una plataforma) y Jump Buffering (registro anticipado del botón de salto).
• Attack (Mordida): Ataque cuerpo a cuerpo rápido. Genera un Hitbox (Trigger 2D) frente al personaje.
• Interact (Interactuar): Usado para leer señalética, activar terminales o iniciar los puzzles de combate.

───

4. Diseño de Niveles (Level Design) y Progresión
El juego mapea el edificio real de la UNIFRANZ. La progresión es vertical y se divide en 8 pisos. Se utilizarán Tilemaps para construir los escenarios de forma modular.

4.1 Zonas y Bosses Temáticos (Ingeniería de Sistemas)
• Nivel 1 (Planta Baja): El lobby principal. Cables sueltos, torniquetes de seguridad descontrolados. Nivel de tutorial de movimiento.
• Nivel 2 (Piso 1 - Cajas/Arca): Plataformas móviles y ventanillas bloqueadas. 
◦ 👑 BOSS 1: "El Compilador" (Programación). Dispara errores de sintaxis (proyectiles). Puzzle: Activar nodos en orden lógico secuencial para aturdirlo.
• Nivel 3 (Piso 2 - Labs de Medicina): Charcos de químicos (daño continuo). Plataformeo de precisión.
• Nivel 4 (Piso 3 - Labs de Computación): Servidores sobrecalentados, ventiladores que alteran el salto. 
◦ 👑 BOSS 2: "El Arquitecto de Monolitos" (Ingeniería de Software).Puzzle: Mover bloques (UML) para formar un diagrama funcional que desactive su escudo.
• Niveles 5 y 6 (Pisos 4 y 5 - Aulas Teóricas): Densidad alta de enemigos comunes (Bugs cuerpo a cuerpo y Bugs a distancia). Pupitres flotantes. 
◦ 👑 BOSS 3: "El Micro-tirano" (Microcontroladores).Puzzle: Sincronizar ataques con un temporizador (referencia al circuito NE555) para causar un cortocircuito.
• Nivel 7 (Piso 6 - Aulas): Plataformas invisibles que solo se ven al recibir daño.
• Nivel 8 (Terraza/Biblioteca): Zonas de gravedad alterada. 
◦ 👑 BOSS FINAL: "El Cuello de Botella" (Redes).Puzzle: Redirigir la topología de un láser (Estrella a Malla) para impactar en su puerto principal.

───

5. Game Loop y Sistemas de Interfaz
5.1 Ciclo de Jugabilidad (Game Loop)
1. Exploración y Plataformeo: Navegar el nivel evitando trampas ambientales.
2. Combate Menor: Eliminar Bugs básicos para abrir caminos.
3. Checkpoints: Interactuar con terminales Wi-Fi seguras que guardan el progreso del nivel.
4. Combate de Boss: Fase de esquiva -> Fase de Puzzle Lógico -> Fase de Daño Crítico (Stun).

5.2 Interfaz de Usuario (HUD)
• Barra de Salud (HP): Representada por 3 a 5 "baterías" o "corazones digitales".
• Contador de Fragmentos: Muestra los coleccionables recogidos (ej. 2/5 USBs).
• Menús: Pantalla de Título (Jugar, Opciones, Salir) y Menú de Pausa integrado.

───

6. Arquitectura Técnica y Organización (Fase Unity y Git)
6.1 Componentes del Motor
• Cámara: Se utilizará Cinemachine para un seguimiento fluido del personaje, con Dead Zones para que la cámara no tiemble con saltos pequeños.
• Físicas: Componentes Rigidbody2D y BoxCollider2D/CapsuleCollider2D.
• Patrones de Diseño (C#):
◦ Singleton: Para el GameManager (control del estado general del juego) y AudioManager.
◦ Observer (Eventos delegados): Para actualizar el HUD solo cuando el jugador recibe daño o recoge un ítem (optimización de recursos).