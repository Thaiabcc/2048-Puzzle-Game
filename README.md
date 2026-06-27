# Unity 2048 - Simple & Optimized Puzzle Game

A lightweight 2048 puzzle game built during my journey of learning Unity and C#. The primary goal of this project is to implement a complete Game Loop and apply basic memory optimization techniques to prevent micro-stutters during gameplay.

## 🚀 Playable Demo
*   **WebGL Playable Link:** [Insert your itch.io link here]

---

## 🛠️ Technical Highlights (What I learned from this project)

### 1. Hitting Zero GC Alloc in Game Loop (Memory Management)
During development, I realized that continuously allocating dynamic collections or yield instructions in the Game Loop triggers Unity's Garbage Collector (GC), causing micro-stutters. I optimized this by:
*   **Collection Caching:** Declared `rowCache` and `emptyPositionsCache` upfront and managed them using `.Clear()` to reuse memory buffers instead of instantiating new objects every turn.
*   **Instruction Caching:** Cached the `WaitForSeconds` delay inside `Awake()` to completely eliminate heap allocation within Coroutines.
*   **Fast Array Copy:** Utilized `System.Array.Copy` to handle matrix manipulation during grid rotation, keeping the code clean and memory-efficient.

### 2. Consolidated Grid Movement Logic
*   Instead of writing four separate, complex algorithms for each direction (Up, Down, Left, Right), I programmatically rotated the grid matrix (`RotateGrid`) to face a single direction, processed the slide/merge logic, and then rotated it back. This approach reduced boilerplate code by **75%** and made debugging much easier.

### 3. Safe Tween & Animation Lifecycle
*   Integrated the **DOTween** package to create subtle, responsive animations: a scale-up pop when a new tile appears (`Spawn`) and a slight bounce when tiles combine (`Merge`).
*   Cached and explicitly stopped (`.Kill()`) active tweens on individual tiles before triggering new ones, preventing visual overlapping and memory leaks when players press keys rapidly.

### 4. Responsive UI Architecture
*   Configured the `Canvas Scaler` using a mobile portrait reference resolution (1080x1920) to ensure the layout stretches and fits perfectly across different mobile screen ratios.
*   Enabled `TextMeshPro` Auto-Sizing to dynamically scale down text sizes when values grow large (1024, 2048, etc.), keeping numbers perfectly within tile boundaries.

---

## 🎮 Completed Features
*   **Core Gameplay:** Smooth shifting, compressing, and merging mechanics controlled via Arrow Keys.
*   **Scoring System:** Increments score dynamically upon successful tile merges and persistently saves the high score (`Best Score`) locally using `PlayerPrefs`.
*   **Complete Game Loop:** Structured flow featuring a Start Menu, a Game Over screen with a fully functional "Try Again" restart mechanism.

---

## 💻 Tech Stack & Packages
*   **Engine:** Unity 2022.3 LTS
*   **Language:** C#
*   **Packages:** DOTween (For UI/Tile Animations), TextMeshPro (For Text Rendering)

---

## 📖 How to Run
1. Clone this repository: `https://github.com/Thaiabcc/2048-Puzzle-Game`
2. Open the project directory inside **Unity Hub** (Unity 2022.3.x recommended).
3. Open the main scene located in `Assets/Scenes/`.
4. Press **Play** inside the Unity Editor and use the `Arrow Keys` to play.
