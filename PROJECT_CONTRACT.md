# Project Contract

## Project
- Name: Cave Sweeper
- One-sentence concept: A first-person atmospheric survival-puzzle game set in a pitch-black desert cave network where your torch is your only light source, your weapon, and a potential death trigger.
- Target player: Fans of immersive sims, puzzle games, and atmospheric horror — people who liked Amnesia's tension, Minesweeper's logic, and Outer Wilds' observation-based gameplay.
- Target platform: PC (Steam)
- Target hardware: Mid-range PC, keyboard+mouse
- Origin: Game concept from Peep — cave escape where gas pockets ignite if torch is used in wrong areas.
- Art direction: Stylized semi-realistic, slightly desaturated warm palette, subtle painterly quality. Between AI-slop and AAA. Reference: Firewatch, Outer Wilds tone. See Assets/Art/Reference/ for concept art.

## Stack
- Engine: Unity 6 Update (6000.4.5f1) — not LTS, Update track
- Render pipeline: URP (Universal Render Pipeline) — needed for real-time point light torch, shadow casting, pitch-black environments
- Language: C#
- IDE: VS Code
- AI coding tools: Claude Code (primary), Claude Pro chat (planning/review)
- AI backup: DeepSeek V4-Pro (research/planning/long-context), V4-Flash (boilerplate/scripts/commits)
- MCP: "MCP for Unity" by CoplayDev (free, open-source, MIT licensed, v9.6.8)
  - Git URL: https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
  - Transport: HTTP Local (http://127.0.0.1:8080/mcp)
  - Config location: C:\Users\wyatt\.claude.json (project-scoped to cave-sweeper)
  - If MCP tools missing in Claude Code: `claude mcp add --transport http UnityMCP http://127.0.0.1:8080/mcp`, relaunch CC
  - Unity Editor must be open with MCP server green for tools to work
- Local AI: Ollama on RTX 4090 (unchanged from previous setup)
- Backend: None (singleplayer)
- Hosting/deployment: Steam (Steamworks)
- Asset pipeline: Placeholder art first, final assets later

## Model Rules
- Commit tags: [claude] [v4-pro] [v4-flash] [local] [wyatt]
- Add Model line to Changed/Broken/Next handoffs
- One model per task
- Claude only for core gameplay architecture
- Sonnet for planning/contracts/review
- Opus for gameplay arch/hard debugging
- Haiku for simple/trivial tasks
- DeepSeek V4 NOT for core gameplay architecture

## Game Design

### Setting
A claustrophobic, realistic cave system grounded in deep-red sandstone and shadowed crevices of a desert landscape. Moab, Utah inspired. One large interconnected cave with checkpoints. Final exit opens to a Moab-inspired canyon vista at golden hour — the payoff after hours of darkness.

### Zone Structure
Hybrid system:
- **Tunnels** are single zones — either safe or gas-filled, binary. Hiss tells you if the whole tunnel is dangerous.
- **Chambers** are multi-zone — large rooms subdivide into a hidden grid of cells (e.g. 3x3 or 4x4). Gas may occupy some cells but not others. This is where the puzzle logic lives.
- The grid is invisible to the player. No visual grid lines. Audio tells are the only feedback.
- Creatures position relative to zones for adjacency warnings.

### Core Mechanics

**Torch (Risk vs. Reward)**
- Heavy iron pipe torch with fuel-soaked cloth wrap (utilitarian, not fantasy)
- Toggle on/off (F key) — lighting strikes flint, creating a spark
- Spark in a gas zone = instant death (explosion). This is the core tension. No health buffer.
- Torch is also a melee weapon (click to swing) — works lit or unlit
- Lit: see surroundings, fight creatures, but risk gas ignition
- Unlit: safe from gas, but blind and swinging blind at creatures
- First-person viewmodel: player arms and torch visible on screen at all times

**Gas Pockets (The "Mines")**
- Invisible zones filled with flammable gas
- Emit a faint directional hiss (3D spatial audio, louder as you approach)
- Torch spark or rock impact in gas = instant death explosion
- Also displace oxygen — being inside a gas zone drains the player's oxygen meter
- Stepping into a gas zone without a spark is safe but costs oxygen

**Creatures**
- Vary by cave depth:
  - Near entrance: rattlesnakes (shelter in cave crevices, plausible for desert setting)
  - Mid-cave: scorpions (light-reactive — flee torch into adjacent dark zones, creep back in darkness)
  - Deep cave: TBD (bats, centipedes, spiders — decide later)
- All creatures attack on close proximity regardless of torch state
- Creature contact deals health damage (not instant death)
- Scorpions are killable with torch swing — killed permanently, thins the board
- Scorpion light-reactivity effectively moves "mines" around while you play

**Audio Tells (The "Numbers")**
- Gas hiss: proximity to gas pockets (always audible, torch state irrelevant)
- Creature sounds: rattling, skittering, etc. as proximity warnings
- Audio masking: throwing a rock creates a loud clatter that temporarily masks the subtle gas hiss — you trade intel for distraction

**Rock Throwing**
- Rocks picked up off the ground (limited by level design placement)
- Throw to trigger creature movement or create audio distraction
- Rock impact can spark and ignite gas (same as torch) — risk/reward
- Noise attracts creatures toward the sound
- Loud clatter temporarily deafens player to gas hiss

**Chalk Marking**
- Player carries limited chalk supply
- Aim at wall + press key = place an X mark
- X means "I've been here" — simple navigation aid
- Limited supply makes it a survival resource — choose which areas to mark
- Chalk is a survival resource, not unlimited

**Oxygen**
- Meter drains while inside gas-heavy zones (even without igniting)
- Refills when outside gas zones
- Forces the player to move through dangerous areas instead of standing still and listening forever
- Reaching zero = death

**Health**
- Drains from creature attacks (scorpion stings, snake bites, etc.)
- Death at zero
- No regeneration (or very slow — TBD)
- Separate from oxygen — two survival pressures simultaneously

### Environmental Design
- Environmental traps: gas-filled pits (instinct to light torch to see depth = death), dead-end tunnels, bait scenarios
- Checkpoints throughout the cave (save progress, refill resources TBD)
- Final exit: narrow trail down a cliff face into Moab canyon at golden hour, cave openings visible in distant canyon walls, path leads somewhere — implies the world continues

### HUD
- Top left: "CURRENT TASK" label + objective text (italic)
- Bottom left: torch icon + status ("LIT"/"UNLIT") + keybind hint
- Bottom right: HEALTH bar (segmented blocks) + OXYGEN bar (segmented blocks)
- Minimal, atmospheric, does not break immersion

### Controls
- WASD: move
- Mouse: look
- Left Ctrl: crouch
- F: toggle torch
- Click: swing torch (melee)
- Key TBD: place chalk mark
- Key TBD: throw rock
- Key TBD: pick up rock

## The First Shippable Version
Core loop: Navigate dark caves using audio tells. Light torch to see but risk explosion in gas zones. Manage oxygen in gas-heavy areas. Fight or avoid creatures. Mark your path with chalk. Reach the exit alive.
- Win condition: Player reaches the final cave exit.
- Lose condition: Gas explosion (instant), health depletion (creatures), oxygen depletion (gas zones).
- Minimum systems needed:
  1. First-person controller (walk, look, crouch) ✅
  2. Torch system (toggle on/off, melee swing, viewmodel with arms) — toggle done, swing + viewmodel TODO
  3. Cave environment (enclosed geometry, pitch black without torch) — basic room done, full cave layout TODO
  4. Gas pocket system (invisible zones, hiss audio, torch spark = death, oxygen drain) — trigger + hiss done, oxygen drain TODO
  5. Zone structure (tunnels as single zones, chambers as hidden grid)
  6. Creature system (scorpions: light-reactive movement, attack on proximity, killable)
  7. Creature variety (rattlesnakes near entrance, deeper creatures TBD)
  8. Rock throwing (pickup, throw, spark risk, audio masking)
  9. Chalk marking (limited supply, X stamp on walls)
  10. Oxygen meter (drains in gas zones, refills outside)
  11. Health system (creature damage, death at zero)
  12. HUD (torch status, health bar, oxygen bar, current task)
  13. Audio tells (gas hiss, creature proximity sounds, rock clatter masking)
  14. Checkpoints
  15. Death/restart flow with UI
  16. Level exit + ending vista
  17. One complete playable cave (tunnels + chambers, hand-built)
  18. Save system (checkpoint-based)
  19. Settings menu (volume, sensitivity, keybinds, display)
  20. Controller support

## What Is NOT In V1
- Multiplayer
- Procedural cave generation (hand-built levels first)
- Inventory beyond torch/chalk/rocks
- Deep narrative or cutscenes
- Multiple cave biomes
- Leaderboards
- Freeform chalk drawing (preset X only)

## Session Workflow (from Playbook)
- Phases: Research → Plan → Build → Review → Verify → Handoff
- Project contract before building (this document)
- Changed/Broken/Next session handoffs
- No full file rewrites — targeted edits only
- Diff-before-apply
- Verify changes work before moving on
- Scope creep kills — skeleton means walks+looks+doesn't crash, nothing else

## Known Risks
1. Unity scenes/prefabs are opaque to AI agents — C# scripts are fine, but inspector-configured components need manual verification
2. MCP for Unity registration can disappear between sessions — CLAUDE.md has startup check to catch this immediately
3. Pitch-black scene means broken torch = can't see to debug — debug light on gas pockets during dev
4. Mouse capture may need click-into-window on first launch
5. Wyatt directs agents, doesn't write code — Claude must verify generated C# is correct before running
6. Viewmodel (first-person arms + torch mesh) requires 3D art assets — placeholder or asset store needed
7. Scorpion AI (light-reactive repositioning) is the most complex system — scope carefully
8. Cave level design will take longer than expected — save for dedicated sessions after all systems work
9. Unity 6000.4.x is Update track, not LTS — supported but may receive fewer backports than 6.3 LTS. Acceptable for a new project.

## Repo
- GitHub: github.com/Wyabro/cave-sweeper
- Branch strategy: main only until complexity demands otherwise
