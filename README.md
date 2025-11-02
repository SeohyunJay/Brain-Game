# Brain-Game
Mizzou CS 4630 Game Development

A fast-paced brain-training game inspired by Cognifit’s subtraction game. Aim the cannon with your mouse and fire 1/2/3-value bullets (S/D/F) to reduce falling crates to **exactly zero**. Points are awarded for exact hits, deducted for overshoots and misses. The game adapts: **level up** on success, **level down** when the level score drops below zero or time runs out.

> **Author:** _Seohyun Kim_  
> **Engine:** Unity 3D Core (Unity **2022.3.62f1**)  

---

## How to Play
- Move your **mouse** to rotate the cannon.
- Press **S** to shoot a **1**-value bullet, **D** for **2**, **F** for **3**.
- Crates spawn at the top and **fall** with slight horizontal drift.
- A crate’s number decreases by the bullet’s value on hit.
- **Goal:** make the crate number **exactly 0** to destroy it and earn points.
- **Overshoot (< 0):** crate is destroyed and **penalty** applied.
- **Miss (crate hits ground):** **penalty** applied.

---

## Key Features
- **Mouse Aiming:** cannon rotation follows cursor direction.
- **S/D/F Bullets:** 1/2/3-value projectiles fired in cannon direction.
- **Randomized Spawns:** random crate values, random X positions, timed intervals.
- **Subtraction Rules:** exact zero awards, underflow (< 0) penalty, ground-hit penalty.
- **Timer:** per-level countdown (default: 3 minutes).
- **Adaptive Levels:** at least **3 levels** with escalating difficulty.
- **Level Movement:** _Up_ on target success / all crates cleared; _Down_ when **level score < 0** or **time out**.
- **UI HUD:** level, score, timer, bullets (1/2/3), crates alive/total, value labels on bullets/crates, **Score Banner** progress bar.
- **FX & Audio:** green success and red fail flashes, SFX for shoot/success/fail/miss/level up/level down, music with mute and pause.
- **Pause Menu:** pause everything except the resume button.
- Audio + graphics on **level up/down**  
- Audio + graphics on crate **exact zero**  
- Audio + graphics on **below zero** fail  
- **Audio On/Off** toggle  
- **Pause** button  
- **Horizontal drift** variation

---

## Levels
Each level is defined via a **`LevelConfig`** ScriptableObject, e.g.:
- **Level 1:** gentle fall speed, modest spawn rate, lower target score.
- **Level 2:** faster fall, more crates, higher target.
- **Level 3+:** harder; tune drift, spawn intervals, and targets.

Players start each level with **50 bullets** of each value (1/2/3), unless you disable per-level reset.

---

## Scenes
- **StartScene** — Title, how-to-play, Start, developer info
- **GameScene** — Main gameplay, HUD, Score Banner, LevelFX, etc.
- **EndScene** — Final Score, High Score, Restart → StartScene, Credits
- **CreditsScene** — Acknowledgements for external assets

> Make sure these scenes are added to **Build Settings** in the order above.

---

## Scoring & Level Movement
- **Exact 0** → `+pointsExactZero` (crate destroyed)
- **Below 0** → `pointsBelowZero` (negative, crate destroyed)
- **Ground hit** → `pointsMissed` (negative, crate destroyed)
- **Level Score** = per-level running total (can go **negative**)
- **Advance** when: `levelScore ≥ targetScoreToAdvance`  
- **Level Down** when: `levelScore < 0` **or** timer reaches **0**

---

## Audio & Effects
- **AudioManager** (persistent): background music (loop, fade), SFX (shoot/success/fail/miss/level up/down/no ammo), **mute** toggle, volumes.
- **LevelFX**: full-screen **green** flash (up) and **red** flash (down), optional confetti/burst particles.
- **Crate FX**: success/fail flashes; ground-touch red blink.

---

## Setup & Run
1. **Clone** the repo:
   ```bash
   git clone https://github.com/SeohyunJay/Brain-Game.git
   ```
2. **Open** the project in **Unity** (matching the version above).
3. Open **StartScene** and click **Play** in the editor.

> Build: **File → Build Settings…** add all scenes in order → **Build** (Windows/Mac).

---

## Troubleshooting
- **Cannon aim off by 90°:** ensure barrel forward vector matches your rotation axis; verify angle clamp and pivot.
- **Crates pushed by bullets:** set crate Rigidbody to **Kinematic** or set **mass** high, and apply movement via script.
