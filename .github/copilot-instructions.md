# GitHub Copilot Instructions for Faction to Empires (Continued)

Welcome to the **Faction to Empires (Continued)** GitHub repository. This document provides information and guidelines on how to work with GitHub Copilot for this RimWorld modding project developed in C#.

## Mod Overview and Purpose

**Faction to Empires (Continued)** is a mod that allows players to elevate any faction or race in RimWorld to empire status. This transformation provides new in-game interactions, such as quests for royal favor and opportunities for players to join the empire's royal family. The mod aims to enhance the player's RimWorld experience by adding depth to the faction mechanics.

## Key Features and Systems

- **Empire Elevation**: Transform any faction or race into an empire, with the ability to select specific factions for this change.
- **Royal Interactions**: Empires offer quests with royal favor and allow trading and escort calls once the player gains a title.
- **Natural Integration**: Empire pawns are selected and developed to blend naturally into the game world.
- **Automatic Customization**: Automation handles the assignment of appropriate pawns for various empire roles.
- **Dynamic Options**: Custom options allow for the management of features like royal apparel needs, empire-creation toggles, and error resolutions for bug-prone factions.

## Coding Patterns and Conventions

- The project leverages C#'s object-oriented principles.
- Classes and methods are named using PascalCase.
- The use of **Harmony** for patching requires following the Harmony patching guidelines for method patching.
- XML is used in conjunction with C# to configure mod settings and options.

## XML Integration

XML is integral in this mod for defining mod options and integration with RimWorld's game settings:

- Options settings for toggling features like becoming an empire and royal apparel needs are managed through XML.
- XML files should adhere to RimWorld's format and structure for compatibility.

## Harmony Patching

The mod uses Harmony for patching methods to achieve desired game mechanics changes:

- Patches are implemented using Harmony's patching tools.
- Each patch class should follow the naming convention of `Patch_ClassName` for clarity.
- Ensure patches are specific and well-documented to avoid unnecessary overrides and conflicts.

## Suggestions for Copilot

To maximize the effectiveness of GitHub Copilot in this project, consider the following:

1. **Focus on Automation**: Given the mod's emphasis on automation, Copilot can assist with generating code that handles repetitive tasks like pawn selection and development.

2. **Error Checking**: Use Copilot to help detect common pitfalls specific to C# and Harmony patching, such as method signature mismatches or incorrect access levels.

3. **XML Integration**: When working with XML, leverage Copilot to auto-generate XML schema based on predefined settings patterns.

4. **Code Patterns**: Encourage Copilot to recognize and suggest consistent coding patterns, especially for Harmony patches and class implementations like `EmpireMaker`.

5. **Class Generation**: Use Copilot to generate boilerplate code for new classes and methods that fit within the existing architecture, such as new WorldComponents or QuestNodes.

## Mod Compatibility and Installation Tips

- Mod is **incompatible** with mods like Hospitality, Ferian Faction, and Lapelli Faction.
- Ensure this mod is loaded **after all faction mods**.
- Changing options requires a game restart and does not apply to existing save games.

## Known Issues and Solutions

- Some factions may cause a quest content bug; use the 'Empire : Bug Resolution' option or disable the faction.
- Disabling Rimsenal's 'make empire' option is necessary to prevent errors.

By following these instructions and guidelines, you can effectively use GitHub Copilot to develop and maintain the **Faction to Empires (Continued)** mod.
