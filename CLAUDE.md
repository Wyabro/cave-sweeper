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

## Current State (Session 7 complete)
- FPS controller: WASD, mouse look, Left Ctrl crouch
- Torch toggle: F key, point light on player camera
- Cave room: ProBuilder 20x20x5m enclosed box, inverted normals, Cave_Rock material
- Lighting: pitch black with no ambient, skybox reflection bug fixed
- Player tag set to "Player"; PlayerHealth + PlayerOxygen components on Player
- PlayerHealth: _maxHealth=100, _currentHealth starts at _maxHealth in Awake; public CurrentHealth, MaxHealth, HealthNormalized (0–1 float); TakeDamage(int) clamps to 0 and calls Die(); Die() reloads scene, guarded by _dead flag
- GasPocket_01 at (3, 1.5, 3): BoxCollider trigger (3x3x3), GasPocket script (torch-on-inside → PlayerHealth.Die())
- 3D spatial hiss: gas_hiss_loop.wav, spatialBlend=1, min=1, max=12, Logarithmic rolloff
- GasCrossfadeLoop: two AudioSources crossfade over overlapDuration (default 4.5s) — no hard loop cut
- Debug: red point light (intensity 0.3, range 8) on GasPocket_01 as DebugLight child — remove before ship
- PlayerOxygen: normalized float 0–1, drain rate 0.05 (= 1/20s), symmetric refill, death at 0 via PlayerHealth.Die()
- HUD: HUDController on "HUD" GameObject; builds entire Canvas hierarchy at runtime in Awake (no saved scene UI)
  - Canvas: ScreenSpaceOverlay, CanvasScaler 1920×1080 match 0.5, EventSystem uses InputSystemUIInputModule
  - Top-left: "CURRENT TASK" bold + italic "Find the way out." — static
  - Bottom-left: warm square icon + "UNLIT" + "[F] torch" — static (torch binding not wired yet)
  - Bottom-right: HEALTH bar (10 blocks, driven by PlayerHealth.HealthNormalized) + OXYGEN bar (10 blocks, driven by PlayerOxygen.Oxygen)
  - Block color = warm on / dim off; SetBar uses Ceil(normalized × N)
  - All elements raycastTarget=false; PlayerHealth/PlayerOxygen auto-found via FindFirstObjectByType if not assigned
- TMP Essential Resources imported (Assets/TextMesh Pro/)
- Input system: project uses New Input System (com.unity.inputsystem 1.19.0); StandaloneInputModule must NOT be used; use FindAnyObjectByType (FindFirstObjectByType is deprecated)
- TorchController: left-click swings TorchLight (localPosition lunge: +0.4z/-0.15y and back over 0.3s); _swinging guard prevents re-entry; TorchMeleeHit() stub present for future damage; point light rotation has no visual effect — translation used instead
- HUD torch binding: TorchStatus label updates each frame to "LIT"/"UNLIT" via TorchController.IsOn
- HUD styling: no panel backgrounds, atmospheric opacity, character spacing on labels, torch icon = head+shaft primitives, "[F] TORCH" caps, 2.5x scale pass applied; note HUD rebuilds in Awake() so scale changes only visible after full play mode restart
- Zone system: Zone.cs (trigger-based zones, ZoneType enum Tunnel/ChamberCell, hasGas bool, torch-on-in-gas death, oxygen drain hookup, ZoneManager reporting, editor gizmos green=safe red=gas)
- ZoneManager.cs: singleton, HashSet<Zone> tracking player occupancy, IsInGasZone() query API
- Chamber.cs: grid generator, rows×cols BoxCollider trigger children with Zone components, bool[] gasMask for gas assignment, [ContextMenu("Regenerate Cells")]
- Test geometry: Tunnel (3×4×12m) connecting +Z wall of original room, CaveRoom2 (10×5×10m) at tunnel end, WallFill_Left/Right/Top closing wall around 3×4m tunnel opening
- GasPocket_01 migrated: GasPocket component replaced with Zone (type=Tunnel, hasGas=true); single AudioSource loop=True, playOnAwake=True (no GasCrossfadeLoop — simple loop)
- Chamber_Cave: 3×3 grid on original room, one corner cell marked gas
- TunnelGasZone: GasCrossfadeLoop + two AudioSources added (spatialBlend=1, min=1, max=12, Logarithmic, gas_hiss_loop.wav); hiss is spatial and seamless

## Planned Systems (not yet built)
- Torch melee: viewmodel with player arms (TorchMeleeHit stub ready for damage hookup)
- Zone structure: tunnels = single binary zone; chambers = hidden grid of cells (3x3 / 4x4)
- Creatures — scorpions: light-reactive (flee torch, creep back in dark), attack on proximity, killable with torch swing
- Creatures — variety: rattlesnakes near entrance, deeper creatures TBD (bats, centipedes, spiders)
- Rock throwing: pickup from ground, throw to distract/spark gas, audio masking of hiss
- Chalk marking: limited supply, X stamp on wall surface, survival resource
- Health damage sources (TakeDamage exists but nothing calls it yet — needs creatures)
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
