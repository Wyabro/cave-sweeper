# Cave Sweeper — Claude Code Rules

## Startup Check (run every session)
1. Verify MCP tools are available: list any tool containing "UnityMCP"
2. If zero tools found: stop and tell Wyatt "MCP not connected — run: claude mcp add --transport http UnityMCP http://127.0.0.1:8080/mcp and relaunch"
3. Do not proceed with any work until MCP tools are confirmed

## Read First
- Read PROJECT_CONTRACT.md before any work session
- One feature at a time. Plan before edits. Targeted edits only.
- No full file rewrites — surgical changes
- No success claims without verification in the Unity editor
- Diff-before-apply

## Project
- Unity 6 (6000.4.5f1), URP, C#
- Singleplayer first-person atmospheric puzzle game
- Torch is only light source — pitch black without it

## Current State (Session 2 complete)
- FPS controller: WASD, mouse look, Left Ctrl crouch
- Torch toggle: F key, point light on player camera
- Cave room: ProBuilder 20x20x5m enclosed box, inverted normals, Cave_Rock material
- Lighting: pitch black with no ambient, skybox reflection bug fixed
- Player tag set to "Player"; PlayerHealth component on Player
- GasPocket_01 at (3, 1.5, 3): BoxCollider trigger (3x3x3), GasPocket script (torch-on-inside → PlayerHealth.Die())
- 3D spatial hiss: gas_hiss_loop.wav, spatialBlend=1, min=1, max=12, Logarithmic rolloff
- GasCrossfadeLoop: two AudioSources crossfade over overlapDuration (default 4.5s) — no hard loop cut
- Debug: red point light (intensity 0.3, range 8) on GasPocket_01 as DebugLight child — remove before ship

## Planned Systems (not yet built)
- Torch melee: click to swing, viewmodel with player arms visible at all times
- Zone structure: tunnels = single binary zone; chambers = hidden grid of cells (3x3 / 4x4)
- Creatures — scorpions: light-reactive (flee torch, creep back in dark), attack on proximity, killable with torch swing
- Creatures — variety: rattlesnakes near entrance, deeper creatures TBD (bats, centipedes, spiders)
- Rock throwing: pickup from ground, throw to distract/spark gas, audio masking of hiss
- Chalk marking: limited supply, X stamp on wall surface, survival resource
- Oxygen meter: drains inside gas zones, refills outside, death at zero
- Health system: drains from creature attacks, no regen (or slow TBD), death at zero
- HUD: top-left task label, bottom-left torch status, bottom-right health + oxygen bars
- Checkpoints: save progress and refill resources throughout cave
- Death/restart flow with UI
- Level exit + ending vista: Moab canyon at golden hour, implies world continues
- Full cave level: hand-built tunnels + chambers, one complete playable layout
- Save system: checkpoint-based
- Settings menu: volume, sensitivity, keybinds, display
- Controller support

## Architecture
- player_controller or PlayerController script handles movement + mouse look
- Torch is a child light on the camera, toggled via script
- Cave geometry is ProBuilder with mesh colliders

## Session Workflow
- Phases: Research → Plan → Build → Review → Verify → Handoff
- End every session with Changed/Broken/Next
- Commit tags: [claude] [v4-pro] [v4-flash] [local] [wyatt]
- Verify in Unity editor before claiming done

## Do NOT
- Add multiplayer, save system, inventory, procedural generation, or settings menu
- Change movement feel without explicit approval
- Install new packages without explicit approval
- Touch unrelated files
- Combine phases (don't plan and build in the same step)
