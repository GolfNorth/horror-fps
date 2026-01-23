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
- DOTS / ECS for performance-critical and scalable systems
- Hybrid ECS + classic C# architecture where appropriate
- VContainer as the primary dependency injection and composition framework
- MessagePipe for decoupled event-based communication
- UniTask for async workflows
- LitMotion for tweening and procedural animation
- R3 for reactive streams (optional, system-dependent)

> ECS and DOTS are used deliberately, not dogmatically.
> Plain C# services remain first-class citizens.

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
- ECS-based AI

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
- MonoBehaviours are adapters or views only
- ECS systems should remain pure and data-oriented

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
