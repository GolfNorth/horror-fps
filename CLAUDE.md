# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

It describes the **architectural vision, design principles, and long-term goals** of the project.
Not all systems described here are implemented yet. This document defines intent, not current state.

---

## Project Overview

A **first-person horror shooter** set in space, built with Unity 6 and modern frameworks.

The project focuses on building a **flexible, modular, and extensible core architecture** that supports:
- Complex player movement
- Diverse weapons and combat mechanics
- AI-driven enemies with varied behaviors
- Environmental interaction
- Long-term scalability and experimentation

This project deliberately avoids monolithic solutions and tightly coupled samples in favor of **explicit composition and replaceable systems**.

---

## Core Design Philosophy

### Key Principles

- **Modularity first**
- **Replaceability over convenience**
- **Explicit dependencies**
- **Clear separation of responsibilities**
- **Data-driven behavior where possible**
- **Long-term maintainability over short-term speed**

Systems are expected to evolve, be replaced, or removed entirely without destabilizing the whole project.

---

## Technology Stack

### Engine & Rendering
- Unity 6
- HDRP (High Definition Render Pipeline)
- HDRP Volumes for atmospheric and horror effects

### Architecture & Patterns
- GameObject-based architecture with modular components
- VContainer as the primary dependency injection and composition framework
- MessagePipe for decoupled event-based communication
- UniTask for async workflows
- LitMotion for tweening and procedural animation
- R3 for reactive streams (input handling, events)

> Focus on clean architecture, modularity, and performance optimization.
> Plain C# services composed via VContainer.

---

## Assembly & Modularity

- Systems are split into independent modules
- `asmdef` files are encouraged for:
  - Core gameplay
  - Player systems
  - AI
  - Combat
  - UI
  - Infrastructure / services
- Cross-module communication must happen via interfaces or events

Avoid circular dependencies at all costs.

---

## Configuration & Game Parameters

### ScriptableObject Configuration

- Game parameters are stored in **ScriptableObjects**
- Configuration is **not global**
- Each module owns its own configuration assets

Examples:
- Player movement parameters
- Weapon and projectile tuning
- Enemy behavior parameters
- Interaction and environment settings

Rules:
- No single “God Config”
- No implicit static access
- Configuration is injected via VContainer

### Remote Configuration (Future)

- Architecture must allow future integration of **Remote Config**
- Runtime overrides of ScriptableObject-backed values must be possible
- Systems should tolerate live parameter changes without restart

Remote config is treated as a **data source**, not a system dependency.

---

## Player Character System

The player character is **not a monolithic controller**, but a composition of independent capabilities.

### Movement Capabilities (Configurable)

- Walking
- Running / sprinting
- Jumping
- Optional double jump
- Strafing
- Sliding (crouch-slide)
- Crouching
- Ladder climbing

Each capability should be:
- Enableable / disableable
- Replaceable
- Independent from input and camera logic

### Movement & Physics

- No hard dependency on Unity sample **Character Controller**
- Custom or alternative movement implementations are expected
- Movement logic must be:
  - Decoupled from input
  - Decoupled from rendering
  - Configurable via data

### Gravity & Space Environments

The game supports:
- Variable gravity
- Zero-G environments
- Modified gravity zones
- Exterior traversal outside stations

Gravity is treated as a **modifiable gameplay parameter**, not a constant.

---

## Weapons & Combat

Combat systems are **data-driven, extensible, and decoupled** from player movement.

### Weapons

- The player may own and switch between multiple weapons
- Weapons can be:
  - Ranged
  - Melee

Each weapon defines:
- Damage model
- Fire mode
- Cooldowns and reload logic
- Recoil and spread behavior
- Projectile configuration

Weapons must be configurable without code duplication.

### Projectiles

- Projectiles are independent entities
- Supported types:
  - Hitscan
  - Physical projectiles
  - Energy or simulated projectiles

Projectile behavior may define:
- Movement model
- Collision and hit response
- Environmental interaction
- Damage and secondary effects

Projectile logic must not be hardcoded into weapon logic.

---

## Inventory System

- The player owns an **inventory abstraction**
- Inventory responsibilities:
  - Weapon storage and selection
  - Item usage
  - Tool and consumable handling

Rules:
- Inventory does not know about UI
- Inventory does not process input
- Inventory acts as a **state holder**, not a behavior monolith

Inventory must be reusable by AI or other entities.

---

## Interaction System

The player can interact with:
- Items
- World objects
- Environmental elements
- Interactive systems

Interaction system requirements:
- Context-aware
- Data-driven
- Decoupled from input and camera logic
- Easily extendable

MonoBehaviours may be used as adapters only.

---

## Enemies & AI

Enemies are a core gameplay pillar.

### Enemy Capabilities

- Multiple enemy types
- Diverse behaviors
- Multiple attack styles:
  - Melee
  - Ranged
  - Area-based or environmental

### AI Architecture

Possible approaches:
- Node-based behavior systems
- Data-driven state machines
- Hierarchical state machines

AI systems must be:
- Extensible
- Testable
- Decoupled from animation and rendering

Shared abstractions with player combat are encouraged but not mandatory.

---

## Input System

- Unity Input System is used
- Input actions represent **player intent**
- Gameplay systems never directly query input

---

## User Interface (UI)

### UI Framework

- **Unity UI Toolkit** is used as the primary UI framework
- UI Toolkit is considered **production-ready**

### UI Principles

- UI is a presentation layer only
- UI does not own game state
- UI reacts to data and events

### Responsibilities

UI Toolkit handles:
- HUD
- Inventory visualization
- Weapon selection
- Interaction prompts
- Menus and overlays

UI Toolkit does NOT handle:
- Game logic
- State ownership
- Input interpretation

### Architecture

- UI lives in a dedicated module / asmdef
- UI Documents act as views
- Controllers / Presenters bind UI to state
- Communication via MessagePipe or service interfaces

---

## Localization

- Localization is implemented via **Unity Localization package**
- All user-facing text must be localizable
- No hardcoded strings in UI or gameplay logic
- Localization is treated as data, not logic

---

## Asset Management

- **Addressables** are used for asset loading
- Systems must not assume synchronous asset availability
- Runtime loading and unloading must be supported
- Addressables are preferred for:
  - Weapons
  - Enemies
  - UI assets
  - Environment content

---

## Performance & Multithreading

- Jobs System and **Burst** are used wherever reasonable
- CPU-heavy logic should be jobified
- Burst compatibility is preferred for core systems
- Main thread usage should be minimized

Performance considerations are part of system design, not a later optimization step.

---

## Architectural Expectations

When implementing systems:
- Prefer plain C# services composed via **VContainer**
- Avoid static access and hidden dependencies
- Use MessagePipe for decoupled communication
- Use UniTask for async operations
- MonoBehaviours should be thin: initialization, Unity callbacks, delegation to services
- Separate concerns: physics, logic, presentation

Clarity is preferred over clever abstractions.

---

## Long-Term Vision

This project is a **sandbox for modern Unity architecture**.

- Systems may be rewritten or replaced
- Framework choices may evolve
- Technical clarity outweighs premature optimization

Claude Code should prioritize:
- Clean abstractions
- Explicit lifetimes
- Minimal coupling
- Predictable data flow

---

## Code Conventions

### Using Directives

- Place `using` directives **outside** the namespace declaration
- Order: System namespaces first, then Unity, then third-party, then project namespaces

```csharp
using System;
using UnityEngine;
using VContainer;
using Game.Core.Logging;

namespace Game.MyModule
{
    // ...
}
```

### Naming

- Private fields: `_camelCase` with underscore prefix
- Public properties: `PascalCase`
- Local variables: `camelCase`
- Constants: `PascalCase`

### Logging

Inject `ILogService` into services. Each service defines a `LogTag` constant for identification.
Tags are automatically colorized in the Unity Console based on their hash.

```csharp
using Game.Core.Logging;

public class MyService
{
    private const string LogTag = "MyService";
    private readonly ILogService _log;

    [Inject]
    public MyService(ILogService log)
    {
        _log = log;
    }

    public void DoWork()
    {
        _log.Info(LogTag, "Working...");
        _log.Verbose(LogTag, "Debug details");
        _log.Warning(LogTag, "Something suspicious");
        _log.Error(LogTag, "Something failed");
    }
}
```

**In static methods/bootstrappers** (where DI unavailable):
Use `UnityEngine.Debug` directly with manual color formatting.

**Log configuration** (`LogConfig` ScriptableObject):
- `MinLevel` - minimum log level (Verbose, Info, Warning, Error, None)
- `ColorizeTag` - enable/disable tag colorization
- Create asset: Right-click → Create → Game → Core → Log Config
- Add to `_configs` array in `ApplicationScope` prefab

---

## Current Implementation

This section describes what is **already implemented** in the codebase.

### Code Structure

```
Assets/
├── Code/
│   ├── Core/                    # Game.Core.asmdef (namespace: Game.Core)
│   │   ├── Bootstrap/           # GameBootstrapper, ApplicationScope
│   │   ├── Configuration/       # GameConfig base class
│   │   ├── Coroutines/          # ICoroutineRunner, CoroutineRunner
│   │   ├── Interfaces/          # IGameService, IInitializableAsync
│   │   ├── Logging/             # ILogService, UnityLogService, LogConfig
│   │   └── Time/                # IGameTime, GameTimeService
│   ├── Core.Events/             # Game.Core.Events.asmdef (namespace: Game.Core.Events)
│   ├── Infrastructure/          # Game.Infrastructure.asmdef (namespace: Game.Infrastructure)
│   │   ├── Assets/              # IAssetLoader, AddressableAssetLoader
│   │   └── SceneManagement/     # ISceneLoader, SceneLoader (supports Addressables)
│   └── Gameplay/                # Game.Gameplay.asmdef (namespace: Game.Gameplay)
│       ├── Character/           # Универсальная система движения (namespace: Game.Gameplay.Character)
│       │   ├── Motor/           # CharacterMotor (KCC wrapper)
│       │   └── Abilities/       # MovementAbility base + конкретные abilities
│       ├── Player/
│       │   ├── Drivers/         # Input → Abilities связь (namespace: Game.Gameplay.Player.Drivers)
│       │   └── Input/           # IPlayerInput, PlayerInputService (namespace: Game.Gameplay.Player.Input)
│       └── GameplayScope.cs
└── Settings/
    └── Core/
        └── InputSystem_Actions.inputactions  # Input actions definition
```

### Assembly Dependencies

```
Game.Core.Events  ← (no game dependencies, only Unity.Mathematics)
       ↑
   Game.Core      ← VContainer, UniTask, MessagePipe, MessagePipe.VContainer
       ↑
Game.Infrastructure ← Game.Core, Unity.Addressables
       ↑
  Game.Gameplay   ← Game.Core, Game.Core.Events, Game.Infrastructure,
                    VContainer, UniTask, MessagePipe, R3.Unity,
                    Unity.InputSystem, Unity.Cinemachine, KinematicCharacterController
```

### VContainer Setup

**ApplicationScope** (`Assets/Code/Core/Bootstrap/ApplicationScope.cs`)
- Root LifetimeScope, lives in DontDestroyOnLoad
- `_configs` array for global GameConfig assets (auto-registered by type)
- Registers MessagePipe and all event brokers
- Registers singleton services: `ILogService`, `IGameTime`, `ICoroutineRunner`
- Create prefab and reference in `VContainerSettings.asset` (Resources folder)

**GameplayScope** (`Assets/Code/Gameplay/GameplayScope.cs`)
- Scene-level LifetimeScope, child of ApplicationScope
- `_configs` array for scene-specific GameConfig assets (PlayerMovementConfig, PlayerLookConfig)
- Registers scoped services: `IAssetLoader`, `ISceneLoader`, `IPlayerInput`
- Add to a GameObject in each gameplay scene

### Character Movement System

Universal movement system based on KinematicCharacterController, usable by players and AI.

**Architecture:**
- `CharacterMotor` - wrapper for KCC, dispatches to abilities by priority
- `MovementAbility` - base class for movement mechanics
- Abilities return `true` from Update methods to take exclusive control

**Built-in Abilities:**

| Ability | Priority | Description |
|---------|----------|-------------|
| `GravityAbility` | 0 | Applies gravity when airborne |
| `GroundMoveAbility` | 10 | Walking, sprinting, air control |
| `BodyRotationAbility` | 10 | Body yaw rotation |
| `JumpAbility` | 30 | Jump with coyote time and buffer |

**Adding new abilities:**
```csharp
public class WallRunAbility : MovementAbility
{
    public override int Priority => 50; // Higher = checked first

    public override bool UpdateVelocity(
        KinematicCharacterMotor motor,
        ref Vector3 velocity,
        float deltaTime)
    {
        if (!_isWallRunning) return false; // Let others handle

        velocity = CalculateWallRunVelocity();
        return true; // Exclusive control
    }
}
```

### Player Setup

1. Create player GameObject with:
   - `KinematicCharacterMotor` (from KCC plugin)
   - `CharacterMotor` (wrapper, references abilities)
   - `GroundMoveAbility`, `JumpAbility`, `GravityAbility`, `BodyRotationAbility`
   - `PlayerMoveDriver`, `PlayerJumpDriver`, `PlayerLookDriver` (input → abilities)
   - `CapsuleCollider`
   - Child with `CinemachineCamera` + `CinemachinePanTilt`

2. Scene requires:
   - Main Camera with `CinemachineBrain`
   - `GameplayScope` with player GameObject reference

3. Driver pattern:
   - Drivers read input and call ability methods
   - Abilities don't know about input
   - Same abilities work for AI (with AI drivers instead)

### Implemented Services

| Interface | Implementation | Scope | Module |
|-----------|---------------|-------|--------|
| `ILogService` | `UnityLogService` | Singleton | Core |
| `IGameTime` | `GameTimeService` | Singleton | Core |
| `ICoroutineRunner` | `CoroutineRunner` | Singleton | Core |
| `IAssetLoader` | `AddressableAssetLoader` | Scoped | Infrastructure |
| `ISceneLoader` | `SceneLoader` | Scoped | Infrastructure |
| `IPlayerInput` | `PlayerInputService` | Scoped | Gameplay/Player/Input |

### MessagePipe Events

Registered in `ApplicationScope`:
- `GameStateChangedEvent`, `PauseStateChangedEvent`
- `PlayerSpawnedEvent`, `PlayerDeathEvent`, `PlayerHealthChangedEvent`
- `DamageDealtEvent`, `WeaponFiredEvent`, `WeaponReloadedEvent`
- `UINavigationEvent`, `UIVisibilityChangedEvent`

### Input System

- Input actions defined in `Assets/Settings/Core/InputSystem_Actions.inputactions`
- Actions: Move, Look, Attack, Interact, Crouch, Jump, Previous, Next, Sprint
- Wrapped by `PlayerInputService` (in `Game.Gameplay.Player.Input`) exposing R3 observables
- Generated C# class `InputSystem_Actions` uses `Game.Input` namespace (auto-generated)

### Configuration Base

- `GameConfig` (`Assets/Code/Core/Configuration/GameConfig.cs`) - base ScriptableObject
- Inherit from this for module-specific configs (e.g., `MovementConfig`, `WeaponConfig`)
- Store assets in `Assets/Configuration/[Module]/` folder:

```
Assets/Configuration/
├── Core/
│   └── LogConfig.asset
├── Player/
│   ├── MovementConfig.asset
│   └── CameraConfig.asset
├── Combat/
│   └── WeaponConfigs/
└── AI/
    └── EnemyConfigs/
```

### Adding New Systems

1. **New service**: Create interface + implementation, register in appropriate Scope
2. **New event**: Add struct to `Core.Events`, register broker in `ApplicationScope`
3. **New module**: Create folder + `.asmdef`, reference only needed assemblies
4. **New config**: Inherit from `GameConfig`, create asset, add to `_configs` array in Scope
