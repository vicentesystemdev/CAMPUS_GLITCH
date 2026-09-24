# CAMPUS_GLITCH

Videojuego 2D desarrollado en **Unity 6000.3.16f1**, ambientado en un campus universitario. El proyecto se trabaja de forma colaborativa utilizando **Git + Git Flow**.

## Flujo de ramas

```text
main
│
├── hotfix/*
│
└── develop
    │
    ├── feature/*
    └── release/*
```

- **main**: contiene únicamente versiones estables.
- **develop**: integra el trabajo del equipo.
- **feature/***: cada funcionalidad se desarrolla en una rama independiente creada desde `develop`.
- **release/***: preparación y estabilización de una versión.
- **hotfix/***: correcciones urgentes sobre una versión estable.

> No se desarrolla directamente en `main`.

## Roles del equipo

### Vicente — Interaction, Scene Flow & UI Lead
Responsabilidades principales: interacciones, puertas, tecla E, triggers, SceneManager, transiciones, SpawnPoints, checkpoints, respawn, flujo general, menús, HUD e integración general.

Ramas de ejemplo:
```text
feature/interactions
feature/scene-flow
feature/checkpoint-respawn
feature/ui-hud
```

### Carla — Player, Animation & Audio Lead
Responsabilidades principales: Player, movimiento, Rigidbody2D, Collider2D, animaciones, Animator, cámara y audio.

Ramas de ejemplo:
```text
feature/player-movement
feature/player-animation
feature/player-camera
feature/audio
```

### Víctor — Environment, Assets & Level Design
Responsabilidades principales: entorno, assets, Tilemap, terreno, diseño de niveles y colliders del entorno.

Ramas de ejemplo:
```text
feature/environment-assets
feature/tilemap-level-design
feature/environment-collisions
```

### Marcelo — Enemy & Game Mechanics Lead
Responsabilidades principales: enemigo, prefab, Animator, patrullaje, detección, persecución, ataque, IA, daño, vida, Game Over, balance y pruebas.

Ramas de ejemplo:
```text
feature/enemy-prefab
feature/enemy-animation
feature/enemy-ai
feature/enemy-combat
feature/game-mechanics
```

> Las ramas se nombran por funcionalidad, no por integrante.

## Configuración inicial

Clonar el repositorio:

```bash
git clone https://github.com/vicentesystemdev/CAMPUS_GLITCH.git
cd CAMPUS_GLITCH
```

Actualizar referencias remotas y cambiar a `develop`:

```bash
git fetch origin
git checkout develop
git pull origin develop
```

Inicializar Git Flow una sola vez por copia local:

```bash
git flow init
```

Configuración recomendada:

```text
Production branch: main
Development branch: develop
Feature branches: feature/
Bugfix branches: bugfix/
Release branches: release/
Hotfix branches: hotfix/
Support branches: support/
Version tag prefix: v
```

## Trabajo con features

Antes de comenzar:

```bash
git checkout develop
git pull origin develop
```

Crear una funcionalidad:

```bash
git flow feature start nombre-feature
```

Ejemplo:

```bash
git flow feature start player-movement
```

Guardar avances:

```bash
git status
git add .
git commit -m "Implementa movimiento básico del jugador"
```

Publicar la feature:

```bash
git push -u origin feature/player-movement
```

Después del primer push:

```bash
git push
```

## Integración de una feature

Antes de integrar una feature se debe verificar que:

1. Unity abre el proyecto correctamente.
2. No existen errores de compilación.
3. La funcionalidad asignada funciona.
4. No se dañaron escenas, prefabs o scripts de otros integrantes.
5. Todos los cambios tienen commit.

Para finalizar según Git Flow:

```bash
git flow feature finish nombre-feature
git push origin develop
```

Cuando la feature haya sido publicada en GitHub, eliminar la rama remota después de confirmar la integración:

```bash
git push origin --delete feature/nombre-feature
```

En trabajo colaborativo se recomienda abrir un **Pull Request hacia `develop`** antes de integrar.

## Historial de ramas

```bash
git log --oneline --graph --all --decorate
```

## Releases

Crear una release desde `develop`:

```bash
git checkout develop
git pull origin develop
git flow release start 0.1.0
```

Finalizar:

```bash
git flow release finish 0.1.0
git push origin main
git push origin develop
git push origin --tags
```

## Hotfix

Crear desde `main`:

```bash
git checkout main
git pull origin main
git flow hotfix start 0.1.1
```

Después de corregir el error:

```bash
git add .
git commit -m "Corrige error crítico"
git flow hotfix finish 0.1.1
git push origin main
git push origin develop
git push origin --tags
```

## Reglas importantes para Unity

- No subir `Library`, `Temp`, `Logs`, `obj` ni archivos generados localmente.
- Conservar siempre los archivos `.meta`.
- Evitar que dos personas modifiquen simultáneamente la misma escena `.unity`.
- Evitar que dos personas modifiquen simultáneamente el mismo prefab.
- Avisar al equipo antes de modificar una escena compartida.
- Hacer commits pequeños y relacionados con una sola tarea.
- Actualizar `develop` antes de iniciar una nueva feature.
- No mezclar trabajos de roles distintos en el mismo commit sin coordinación.

## Resumen rápido

```text
main       = versión estable
develop    = integración
feature/*  = trabajo nuevo
release/*  = preparación de versión
hotfix/*   = corrección urgente de producción
```

**Ningún integrante desarrolla directamente sobre `main`.**
