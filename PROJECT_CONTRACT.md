# Project Contract

## Project
- Name: Cave Sweeper
- One-sentence concept: A first-person atmospheric puzzle game that translates Minesweeper logic into a pitch-black desert cave network where your torch is both your only light source and a potential death trigger.
- Target player: Fans of immersive sims, puzzle games, and atmospheric horror — people who liked Amnesia's tension, Minesweeper's logic, and Outer Wilds' observation-based gameplay.
- Target platform: PC (Steam)
- Target hardware: Mid-range PC, keyboard+mouse
- Origin: Game concept from Peep — cave escape where gas pockets ignite if torch is used in wrong areas.

## Stack
- Engine: Unity 6 Update (6000.4.5f1) — not LTS, Update track
- Render pipeline: URP (Universal Render Pipeline) — needed for real-time point light torch, shadow casting, pitch-black environments
- Language: C#
- IDE: VS Code
- AI coding tools: Claude Code (primary), Claude Pro chat (planning/review)
- AI backup: DeepSeek V4-Pro (research/planning/long-context), V4-Flash (boilerplate/scripts/commits)
- MCP: "MCP for Unity" by CoplayDev (free, open-source, MIT licensed, v9.6.3, 9.1k stars)
  - Git URL: https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
  - NOT the paid "Coplay" product or "Coplay MCP" — those are separate products with different tech stacks
  - Requires: Python 3.10+, `uv` package manager, Claude Code CLI
  - Transport: stdio for Claude Code (single-agent)
  - Auto-setup: Window > MCP for Unity > Auto-Setup
- Local AI: Ollama on RTX 4090 (unchanged from previous setup)
- Backend: None (singleplayer first)
- Hosting/deployment: Steam (Steamworks)
- Asset pipeline: Placeholder art first, final assets later

## Model Rules
- Commit tags: [claude] [v4-pro] [v4-flash] [local] [wyatt]
- Add Model line to Changed/Broken/Next handoffs
- One model per task
- Claude only for core gameplay and multiplayer architecture
- Sonnet for planning/contracts/review
- Opus for gameplay arch/multiplayer/hard debugging
- Haiku for simple/trivial tasks
- DeepSeek V4 NOT for core gameplay or multiplayer architecture

## The First Shippable Version
- Core loop: Navigate dark caves using audio and visual tells (gas hiss, subtle shimmer). Light torch to see surroundings, but risk explosion if inside a gas pocket. Mark safe/dangerous zones. Reach the exit alive.
- Win condition: Player reaches the cave exit without dying.
- Lose condition: Player ignites a gas pocket (explosion + death) or falls into a hazard.
- Minimum systems needed:
  1. First-person controller (walk, look, crouch)
  2. Torch system (toggle on/off, point light with realistic falloff)
  3. Cave environment (enclosed geometry, pitch black without torch)
  4. Gas pocket system (invisible zones that explode when torch is lit inside them)
  5. Detection mechanic (audio cues — hissing near gas, visual cues — subtle particle shimmer)
  6. Marking system (player can place chalk marks or flags on walls)
  7. Level exit trigger
  8. Death/restart flow
  9. Basic UI (health or lives, restart prompt)
  10. One complete playable level

## What Is NOT In V1
- Multiplayer (future — Unity NGO)
- Procedural cave generation (hand-built levels first)
- Inventory beyond torch
- Story/narrative
- Save system
- Settings menu
- Multiple cave biomes
- Leaderboards
- Sound design polish (placeholder SFX acceptable)
- Controller support

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
2. MCP for Unity is unproven in this workflow — test early, have fallback plan (manual Unity editor work)
3. Pitch-black scene means broken torch = can't see to debug — add debug position logging as fallback
4. Mouse capture may need click-into-window on first launch
5. Don't tune feel in skeleton session — defer to session 2
6. Wyatt directs agents, doesn't write code — Claude must verify generated C# is correct before running
7. If Unity AI Assistant package is installed, it can conflict with MCP for Unity (System.Collections.Immutable version clash) — remove Unity AI Assistant if issues arise
8. On Windows, MCP for Unity writes an absolute uv.exe path (prefers WinGet Links shim) — if uv not found, use "Choose uv Install Location" button in MCP for Unity window
9. Unity 6000.4.x is Update track, not LTS — supported but may receive fewer backports than 6.3 LTS. Acceptable for a new project.

## Repo
- GitHub: github.com/Wyabro/cave-sweeper
- Branch strategy: main only until complexity demands otherwise


