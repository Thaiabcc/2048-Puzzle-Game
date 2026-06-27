# 2048 Optimized Puzzle Game

An optimized, production-ready 2048 puzzle game built with Unity and C#. This project is designed with a strong focus on mobile performance, featuring zero memory allocation in the game loop and clean UI/UX state management.

## 🚀 Playable Demo
*   **WebGL Playable Build:** [Insert your itch.io or web link here]
*   **Gameplay Video:** [Insert your Youtube or Drive video link here]

---

## 🛠️ Technical Highlights (What makes this project special?)

### 1. Zero GC Alloc in Core Game Loop
*   **The Problem:** Continuous memory allocations (instantiating lists, arrays, or coroutine yields) trigger Unity's Garbage Collector (GC), causing micro-stutters and frame drops on mobile devices.
*   **The Solution:** Implemented strict memory management techniques:
    *   **Cached Dynamic Collections:** Reused `List<int>` and `List<Vector2Int>` with `.Clear()` instead of allocating new instances every turn.
    *   **Instruction Caching:** Cached `WaitForSeconds` instances inside `Awake()` to avoid heap allocation within Coroutines.
    *   **Matrix Manipulation:** Utilized `System.Array.Copy` on pre-allocated matrix buffers during grid rotation, eliminating runtime heap allocations during input handling.

### 2. Robust Tween & Animation Lifecycle
*   **Safe Tween Killing:** Managed animation states explicitly by caching DOTween `Tweener` references. All active tweens are safely checked and killed via `.Kill()` prior to restarting animations or destroying objects, preventing memory leaks and visual overlapping.
*   **State-Driven Animations:** Tiles intelligently determine whether to trigger `Spawn` or `Merge` scaling profiles based on discrete runtime value differentials, achieving clean separation between gameplay logic and visual presentation.

### 3. Scalable Matrix Logic
*   **Single-Direction Consolidation:** Instead of writing four separate complex algorithms for Up, Down, Left, and Right movements, the grid is programmatically rotated (`RotateGrid`) to face a single direction prior to execution, and then rotated back. This reduced logic redundancy by **75%** and minimized structural code complexity.

### 4. Fully Responsive UI Architecture
*   **Canvas Scaler:** Configured with `Scale With Screen Size (1080x1920)` and an even width/height match profile to guarantee flawless visual layouts across varied mobile aspect ratios.
*   **Text Mesh Pro Auto-Sizing:** Dynamically scales large numerical values (e.g., 1024, 2048) to fit grid boundaries natively without breaking layout constraints.

---

## 🎮 Features
*   **Classic 2048 Gameplay:** Smooth sliding mechanics with responsive arrow key controls.
*   **Persistent Score System:** Automatically tracks current session scores and saves the absolute High Score persistently using `PlayerPrefs`.
*   **Modular Architecture:** Complete Game Loop state controls (`Start Menu` -> `Gameplay` -> `Settings Popup` -> `Game Over Overlays` -> `Auto Reset`).

---

## 💻 Tech Stack & Packages
*   **Engine:** Unity 2022.3 LTS
*   **Language:** C# (Object-Oriented Programming)
*   **Animation Package:** DOTween (Demigiant)
*   **Text Component:** TextMeshPro (Unity)

---

## 📖 How to Run
1. Clone this repository: `https://github.com/Thaiabcc/2048-Puzzle-Game`
2. Open the project directory inside **Unity Hub** (Version 2022.3.x recommended).
3. Open the main scene located in `Assets/Scenes/`.
4. Press **Play** inside the Unity Editor. Use `Arrow Keys` to control.
