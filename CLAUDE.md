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

## Current State (Session 1 complete)
- FPS controller: WASD, mouse look, Left Ctrl crouch
- Torch toggle: F key, point light on player camera
- Cave room: ProBuilder 20x20x5m enclosed box, inverted normals, Cave_Rock material
- Lighting: pitch black with no ambient, skybox reflection bug fixed

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
