# Inventario de niveles y fuentes oficiales

Los recursos no provienen de fotografías pegadas como nivel: todos los props son PNG separados bajo `Assets/CampusGlitch/Resources/CG` y sus fuentes Aseprite están bajo `Assets/CampusGlitch/Art/SourceAseprite`.

| Escena | Arte nuevo | Tema | Progresión |
|---|---|---|---|
| `SC_Level01.unity` | `01_Nuevos_Modulares/01_Exterior/` | Exterior, Guardia U, acceso | Desbloquea Planta Baja |
| `SC_Level02.unity` | `01_Nuevos_Modulares/02_PlantaBaja/` | Recepción, hall | Desbloquea skin morada y Mezanine |
| `SC_Level03.unity` | `01_Nuevos_Modulares/03_Mezanine/` | ARCA, Cajas, Dirección | Desbloquea piso 1 |
| `SC_Level04.unity` | `01_Nuevos_Modulares/04_Piso1_Salud/` | Salud y laboratorios | Desbloquea Sistemas |
| `SC_Level05.unity` | `01_Nuevos_Modulares/05_Piso2_Sistemas/` | Computación y redes | Habilita subsuelo extra |
| `SC_Level06.unity` | `01_Nuevos_Modulares/06_Piso4/` | Aulas, secuencias y jefe | Desbloquea Biblioteca |
| `SC_Level07.unity` | `01_Nuevos_Modulares/07_Piso5_Biblioteca/` | Biblioteca e Innova | Desbloquea Terraza |
| `SC_Level08.unity` | `01_Nuevos_Modulares/08_Terraza_Lounge/` | Lounge y Solarium | Desbloquea Odontología |
| `SC_Level09.unity` | `01_Nuevos_Modulares/09_Odontologia/` | Clínica y Core Glitch | Final y créditos |
| `SC_Extra_Subsuelo.unity` | `01_Nuevos_Modulares/EXTRA_Subsuelo/` | Osteoteca / Anatomía | Ruta opcional |

Todos los niveles requieren 3 datos, un puzzle sencillo en terminal, 1 checkpoint y la salida final. Los niveles 6, 8 y 9 tienen jefe. Es un **diseño jugable inicial implementado en código**, no aún recorrido de 5-7 min medido en Editor ni réplica arquitectónica exacta de Unifranz 360.

## Árbol técnico

```text
Assets/CampusGlitch/
  Art/SourceAseprite/     .aseprite originales y arte editable
  Resources/CG/
    00_Biblioteca_Previa_625/
    01_Nuevos_Modulares/
    02_Animados/
    03_Interactivos_Animados/
    Playable/              frames de avatar rojo/morado/asistente
    Backgrounds/           diez fondos decorativos independientes
    Sound/                 audio original básico
  Scripts/
    CGData.cs              especificación de niveles, input y guardado
    CGEntities.cs          lobo, enemies, chips y animaciones
    CGWorld.cs             creación de mundos, plataformas y props
    CGGame.cs              interfaz, progresión, combate y campañas
    CGAudio.cs             música y sonidos
  Editor/
    CGBuildScenes.cs       creador y registro de escenas
  Scenes/                  bootstrap + 10 escenas de juego
```

**No utilizar imágenes de infografía como sprites de gameplay.**
