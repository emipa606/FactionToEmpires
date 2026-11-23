# GitHub Copilot Instructions for Faction to Empires (Continued)

Welcome to the Faction to Empires (Continued) mod for RimWorld! This mod allows you to convert factions and races into empires, providing unique quests and interactions. Below are the detailed instructions for using GitHub Copilot in this modding project.

## Mod Overview and Purpose

The Faction to Empires (Continued) mod is an update and continuation of YAYOs mod. It allows players to convert factions and races into empires, giving them unique quests, titles, and interactions. The mod provides a new level of complexity and depth to faction management in RimWorld.

## Key Features and Systems

### Conversion to Empires
- Players can convert factions into empires, gaining access to unique quests and titles.
- Royal favor quests are available, allowing players to interact with the royal family of the empire.
- Empires can provide special techprints and other rewards.

### Royal Interactions
- Pawns of empires can act as guards and guests, naturally appearing in the game with royal titles.
- Royal needs can be managed, including the requirement for royal apparel.

### Quests
- Each faction has a unique royal ascent quest, ending with a final quest.
- Tribute quests are available for each faction, providing different rewards.

### Mod Integration
- The mod should be placed after all faction mods to avoid conflicts.
- Save files are not compatible with changes in mod settings, requiring a game restart.

## Coding Patterns and Conventions

### C# Coding Standards
- Follow the standard RimWorld C# coding conventions.
- Use meaningful variable and method names.
- Include comments and documentation for complex logic.

### XML Integration
- XML files are used to define mod data and configurations.
- Ensure that XML files are well-structured and validated against the RimWorld XML schema.

### Harmony Patching
- Use Harmony patches to modify RimWorld behavior without altering the original codebase.
- Ensure that patches are specific and do not introduce unintended side effects.

## Suggestions for Copilot

### C# Files
- `generateWorld.cs`: Use Copilot to help with initializing and setting up factions as empires.
- `QuestNode_GetPawnKind.cs`: Use Copilot to assist with creating and managing quest nodes.
- `ThingRequiringRoyalPermissionUtility_IsViolatingRulesOf.cs`: Use Copilot to help with permission checks and rules enforcement.
- `EmpireMaker.cs`: Use Copilot to assist with copying and patching pawn and trader definitions.

### XML Files
- Use Copilot to generate and validate XML definitions for new factions and empires.
- Ensure that XML files are correctly integrated into the mod's configuration system.

### Harmony Patches
- Use Copilot to create and test Harmony patches for modifying RimWorld behavior.
- Ensure that patches are thoroughly tested to avoid conflicts with other mods.

### General Tips
- Use Copilot to generate and validate XML definitions for new factions and empires.
- Ensure that XML files are correctly integrated into the mod's configuration system.
- Use Copilot to generate and test Harmony patches for modifying RimWorld behavior.
- Ensure that patches are thoroughly tested to avoid conflicts with other mods.
- Use Copilot to generate and validate XML definitions for new factions and empires.
- Ensure that XML files are correctly integrated into the mod's configuration system.

## Contributing Guidelines

- Ensure that your code follows the mod's coding standards and conventions.
- Validate XML files against the RimWorld XML schema.
- Thoroughly test Harmony patches to avoid unintended side effects.
- Report any issues or bugs using the provided Discord channel.
- Use RimSort to sort your mods for optimal compatibility.

By following these guidelines, you can effectively use GitHub Copilot to enhance your work on the Faction to Empires (Continued) mod for RimWorld. Happy modding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Temporary files

- Do not create temporary files, build output, test artifacts, or Copilot session
  folders inside the mod root.
- Store temporary artifacts under `%USERPROFILE%\CopilotTemp\[modname]`.
- Direct build and test output there as well, without changing project settings.
- Clean up task-specific temporary artifacts when finished.

## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

