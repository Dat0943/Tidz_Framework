# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BaXoai Framework is a Unity UPM package (Unity 6000.2+) providing architectural patterns, UI utilities, data management, and extension methods for 2D/3D game development. It is installed via Git URL into Unity projects.

**Single assembly**: `BaXoai.asmdef` — all runtime code compiles into one assembly under the `BaXoai` namespace.

## Build & Development

There are no standalone build/lint/test scripts — this is a Unity package. All development is done inside a Unity Editor project that references this package. There are currently no unit tests.

To test changes, add the package to a Unity project via Package Manager (Git URL) and validate in-Editor.

## Architecture

### Module Layout

All source lives under `Base Framework/Runtime/`:

- **Patterns/** — Core design patterns: `Singleton/`, `StateMachine/`, `EventBus/`, `Pooling/`
- **Mono/** — MonoBehaviour utilities: `MonoBase`, `MonoCallback` (global Unity event dispatcher), `MonoSingleton<T>`
- **View/** — UI/View system with animation support: `View.cs`, `ViewContainer.cs`, `ViewTransition/`, `Extra/`
- **Data/** — Persistence: `LDataBlock<T>` (auto-saving singleton data model), `LDataHelper.cs` (Easy Save 3 wrapper)
- **Extensions/** — Extension methods for C# collections and Unity types (Transform, Vector, Color, etc.)
- **Ultilities/** — Audio, Scene loading, Popup dialogs, Button animations, SpringMotion, CoroutineRunner
- **Diagnostics/** — `LDebug.cs` generic-type-aware logging
- `Editor/` — Editor tools and ScriptableObject factory utilities
- `Plugins/` — Bundled third-party: Easy Save 3, DOTween, Odin Inspector

### Key Dependencies

| Package | Purpose | Required |
|---|---|---|
| Easy Save 3 | Serialization / persistence | Yes |
| DOTween | Tween animations in View system | Yes |
| UniTask | async/await throughout | Yes |
| Unity Addressables | Asset loading | Yes |
| Odin Inspector | Editor attributes (optional compile symbol) | No |

Optional scripting define symbols: `ODIN_INSPECTOR`, `DOTWEEN`

### Core Patterns

**Singleton**: `MyClass.Instance` — auto-creates if missing; `MonoSingleton<T>` also handles `DontDestroyOnLoad` and destruction safety.

**Event Bus**: Type-safe pub/sub.
```csharp
EventBus.Subscribe<MyEvent>(handler);
EventBus.Post(new MyEvent());
```

**State Machine**: Generic enum-labeled FSM; call `Update()` each frame.

**Object Pooling**: Inherit `MonoPool<T, TPool>`, configure prefab, use `Get()` / `Release()`.

**Data Persistence**: Inherit `LDataBlock<T>` — automatically saves on `OnApplicationPause`, focus loss, and quit via Easy Save 3.

**View/UI**: Configure `ViewTransition` components (CanvasGroup, Scale, Move, Rotate) on a `View`; call `Open()` / `Close()`. All animations run via DOTween + UniTask. Callbacks: `onOpenStart`, `onOpenEnd`, `onCloseStart`, `onCloseEnd`.

**MonoCallback**: Global event dispatcher singleton — subscribe to `Update`, `LateUpdate`, `FixedUpdate`, `ApplicationQuit`, `ApplicationPause`, `ApplicationFocus`, `ActiveSceneChanged` from non-MonoBehaviour classes.
