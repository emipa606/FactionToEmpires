# GitHub Copilot Instructions for Faction to Empires (Continued)

## Mod Overview and Purpose

Faction to Empires (Continued) is an update to the original Faction to Empires mod. This mod allows you to convert any faction or race into an empire within the RimWorld game. Players can choose which factions they want to become empires and can even add races from other mods as empires. Once a faction is an empire, it will offer quests that grant royal favor, and pawns from the empire can visit your settlement as guests or guards. The mod also introduces royal titles and unique royal apparel, as well as the ability to trade with empires and call for an escort. 

## Key Features and Systems

### Features:
- Convert any faction or race into an empire.
- Choose specific factions to become empires.
- Add races from other mods as empires.
- Receive quests from empires that grant royal favor.
- Pawns from empires can visit your settlement as guests or guards.
- Royal titles and unique royal apparel.
- Trade with empires.
- Call for an escort from empires.
- Automatic generation of appropriate pawns for guards and guests.
- Special techprints from each empire.
- Option to remove vanilla empires.
- Starting and desertion events.
- Royal Ascent Quest for each faction.

### Systems:
- Quest system: Empires offer quests that grant royal favor.
- Royal title system: Pawns from empires can receive royal titles.
- Royal apparel: Unique royal apparel for pawns from empires.
- Trade system: Trade with empires.
- Escort system: Call for an escort from empires.
- Pawn generation: Automatic generation of appropriate pawns for guards and guests.
- Event system: Starting and desertion events.
- Royal Ascent Quest: Ending quest for each faction.

## Coding Patterns and Conventions

### Coding Patterns:
- Use of Harmony for patching existing RimWorld methods.
- XML integration for mod settings and definitions.
- C# for implementing mod logic and systems.
- Use of HugsLib for mod options and settings management.

### Conventions:
- Follow RimWorld's coding standards and conventions.
- Use meaningful and descriptive variable and method names.
- Comment code where necessary to explain complex logic.
- Use HugsLib's settings system for mod options and settings management.
- Use XML for mod definitions and settings where appropriate.

## XML Integration

The mod uses XML for defining mod settings and other configurations. The XML files are located in the `Defs` folder of the mod. The following XML files are used in the mod:

- `Defs/FactionToEmpiresDefs.xml`: Contains definitions for the mod, such as factions, pawns, and events.
- `Defs/FactionToEmpiresSettings.xml`: Contains settings for the mod, such as options and configurations.

## Harmony Patching

Faction to Empires (Continued) uses Harmony for patching existing RimWorld methods. The following Harmony patches are used in the mod:

- `ThingRequiringRoyalPermissionUtility_IsViolatingRulesOf.cs`: Patches the `IsViolatingRulesOf` method to check for royal permissions.
- `EmpireMaker.cs`: Patches the `PatchDef` and `PatchRelation` methods to patch definitions and relations for empires.

## Suggestions for Copilot

### General Suggestions:
- Use Copilot to generate code snippets for new features and systems.
- Use Copilot to generate XML definitions for new factions and pawns.
- Use Copilot to generate Harmony patches for existing RimWorld methods.
- Use Copilot to generate code for new mod options and settings.

### Specific Suggestions:
- Use Copilot to generate code for the `generateWorld.cs` file, which is responsible for generating the world and empires.
- Use Copilot to generate code for the `QuestNode_GetPawnKind.cs` file, which is responsible for generating quests and pawns for empires.
- Use Copilot to generate code for the `EmpireMaker.cs` file, which is responsible for creating empires and their associated definitions and relations.

By following these guidelines and suggestions, developers can use Copilot effectively to generate code and XML for the Faction to Empires (Continued) mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

