# 🎋 Tidz Framework

*A lightweight and scalable framework for Unity development.*

\---

## 🚀 Overview

**BaXoai Framework** is a clean, modular, and developer-friendly Unity toolkit designed to accelerate development and maintain a consistent project structure.

It includes architecture foundations, UI utilities, data helpers, extension methods, and optional integrations with popular Unity tools.

> ⚠️ This is an early version — more modules and documentation will be added soon.

\---

# 📦 Required Dependencies

Before installing BaXoai Framework, you **must** install the following Unity packages:

|Package|Source|Required|
|-|-|-|
|**Unity Addressables**|Unity Package Manager|✅|
|**Unity ResourceManager**|Installed with Addressables|✅|
|**Unity Mathematics**|Unity Package Manager|✅|
|**Easy Save 3**|Asset Store|✅|
|**UniTask**|https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask|✅|
|**Vertx Debugging Tools**|https://github.com/vertxxyz/Vertx.Debugging.git|Optional|

> Missing any required dependency may cause compilation errors.

\---

# 📥 Installation

## 1️⃣ Install via Git URL

Unity → *Package Manager* → *Add package from git URL*:

```
https://github.com/Dat0943/Tidz\_Framework.git
```

# ✨ Core Features

## 🧩 Architecture

* Singleton
* State Machine
* Object Pooling
* Event System
* (More incoming…)

## 🎨 UI Utilities

* Button animations (scale / fade / pulse)
* Image effects
* Scroll helpers
* UI extensions

## 📦 Data \& Helpers

* Scriptable data model
* Save/Load utilities
* Runtime configuration helpers
* Extension methods

\---

# 🔌 Optional Integrations

BaXoai Framework supports optional integrations using **Scripting Define Symbols**.

These integrations are disabled unless the related plugin is installed.

\---

# ⚙️ Enabling Integrations

Unity → *Project Settings* → *Player* → *Scripting Define Symbols*

\---

## 🧩 Odin Integration

1. Install Odin Inspector
2. Add define:

```
ODIN\_INSPECTOR
```

3. Features unlocked:
* Enhanced inspectors
* Serialized ScriptableObjects
* Editor tooling
* Additional helpers under `Integrations/Odin/`

\---

## 🎮 DOTween Integration

1. Install DOTween
2. Add define:

```
DOTWEEN
```

3. Features unlocked:
* UI tween helpers
* Animation presets
* Extension methods

\---

# 📁 Recommended Folder Structure

```
BaXoai\_Framework/
│
├── Runtime/
│   ├── Core/
│   ├── UI/
│   ├── Data/
│   └── Extensions/
│
├── Editor/
│
└── Integrations/
    ├── Odin/
    └── DOTween/
```

\---

# 📄 License

BaXoai Framework is released under the **MIT License**.

\---

# 📌 Notes

* This is an early-stage framework.
* More modules, examples, and documentation will be added soon.

