# Cave Sweeper

A first-person atmospheric survival game set in a pitch-black desert cave network. Your torch is your only light source, your only weapon, and a potential death trigger. Some pockets of the cave hold invisible flammable gas — strike a spark in the wrong place and you're done.

Audio is the puzzle. A faint directional hiss tells you what's safe and what's not. Stand too long in gas and your oxygen runs out. Light the torch in gas and you don't get a second chance.

Inspired by the cave systems around Moab, Utah.

---

## Status

In active development. Not playable end-to-end yet.

**Shipped systems:**
- First-person controller (walk, look, crouch)
- Torch (toggle, point light, pitch-black scene without it)
- Cave geometry (ProBuilder enclosed room, inverted normals)
- Gas pocket system (trigger zones, 3D spatial hiss with crossfade loop, instant death on torch ignition)
- Oxygen meter (drains in gas zones, refills outside, death at zero)

**Not yet built:** torch melee + viewmodel, zone grid for chambers, creature AI (scorpions, rattlesnakes), rock throwing, chalk marking, health system, HUD, checkpoints, save system, settings, controller support, cave level layout, exit vista.

See [`PROJECT_CONTRACT.md`](PROJECT_CONTRACT.md) for the full design.

---

## Stack

- **Engine:** Unity 6 (6000.4.5f1, Update track)
- **Render pipeline:** Universal Render Pipeline (URP)
- **Language:** C#
- **Target platform:** PC (Steam), keyboard + mouse
- **Repo layout:** standard Unity project (`Assets/`, `Packages/`, `ProjectSettings/`); `Library/` and IDE files gitignored and regenerated on first open

---

## Running locally

1. Install Unity Hub and Unity Editor `6000.4.5f1` (URP template).
2. Clone:
   ```
   git clone https://github.com/Wyabro/cave-sweeper.git
   ```
3. Open the project in Unity Hub. First open will rebuild the `Library/` cache (a few minutes).
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press Play.

Controls: WASD to move, mouse to look, Left Ctrl to crouch, F to toggle torch.

---

## Repo

- [`PROJECT_CONTRACT.md`](PROJECT_CONTRACT.md) — game design, stack decisions, V1 scope, known risks
- [`CLAUDE.md`](CLAUDE.md) — current build state and rules for AI coding agents working in the repo
- `Assets/Scripts/` — gameplay scripts
- `Assets/Art/Reference/` — art direction reference

---

## Credits

Game concept by Peep. Built by Y-Dawg (Red Shift Studios) using AI-assisted development tooling.
