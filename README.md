[![.NET](https://github.com/xoxfaby/BetterAPI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/xoxfaby/BetterAPI/actions/workflows/dotnet.yml)

# BetterAPI

A simple library that adds thin wrappers around the new RoR2 Content Pack system with some helper functionality.
This is far from complete, if you want me to add something to it, I might do it, see how to contact me below.
It should be enough to implement simple items, buffs, etc. 

For an example, check out https://github.com/xoxfaby/MoreItems


## Support Me

If you like what I'm doing, consider supporting me through GitHub Sponsors so I can keep doing it:

https://github.com/sponsors/xoxfaby (preferred)
https://patreon.com/xoxfaby
or directly through paypal:
https://streamelements.com/xoxfaby/tip


## Currently implemented APIs:
 - Artifacts
 - Buffs
 - CostTypes
 - Equipments
 - Interactables (BETA)
 - Items
 - Languages
 - Prefabs
 - Objectives
 - Stats

## Help & Feedback

If you need help or have suggestions, create an issue on github, join my discord or find me on the RoR2 Modding Discord 

[My Discord](https://discord.gg/Zy2HSB4) XoXFaby#1337

https://github.com/xoxfaby/BetterAPI

## Changelog

### v4.0.3
 - Bugfix: Updated for item tier changes.

### v4.0.2
 - Minor Change:
   - ContentPacks: Create a ContentPackProvider if it is requested by GetContentPackProvider and it doesn't exist.
   - Interactables: Added Stages.None flag
 - Bugfixes:
   - Interactables: Try to spawn interactables again if the node they happen to pick is occupied. 

### v4.0.1
 - Bugfixes:
   - Delayed applying of IDRS until Item and BodyCatalog is ready
   - Removed some debug messages

# v4.0.0
 - Breaking changes:
   - Changed Interactables Stages flag to Int64

# v3.0.0
 - Breaking changes:
   - Changed all the Templates to classes and made fields nullable, default values are taken from Vanilla.
 - Additions:
   - Added Critical Damage Stat. 

# v2.0.3
 - Bugfix: Hides BepInEx_Manager object so it doesn't get deleted which stops all mods from working. 

### v2.0.2
 - Bugfix: Fixes for CUM

### v2.0.1
 - Bugfix: Internal change that fixes Stat hooks

# v2.0.0
 - Breaking changes:
   - Removed some deprecated overloads. If you were using these you will have to recompile your mod but you likely have to make no other changes
   - Reworked Stats API, now has a lot more stats and collects the changes different. The system is now very robust so more stats can be added much more easily. If you need a stats, just request it. 
   - Moved BetterUnityPlugin to its own assembly. It will ship with BetterAPI so you can still depend on it.
 - Other Changes:
 - Added methods to get the content pack of a mod and the assembly a contentpack was added by.
 - BetterAPI and all my other mods are now on NuGet for easier development. 


## v1.5.0
 - BetterUnityPlugin: ContentPacks: Added Method FindAssembly to return the Assembly that added a content pack via BetterAPI. 

### v1.4.1
 - BetterUnityPlugin: Bugfix: Fixed bug that prevent hooks from being added if they were added after plugin enable. 

## v1.4.0
 - Removed MMHook Dependency
 - Added BetterUnityPlugin which includes a HookManager for adding hooks and events for Awake, OnEnable, etc. 
   HookManager only adds hooks for methods actually used so it should provide better performance than MMHook. See MoreItems for examples on how to use the HookManager.  
 - Buffs: It's now possible to provide a buff name & descriptions. These can be consumed by other mods, for example BetterUI adding better tooltips for buffs.
 - Interactables Beta: Minimum count for interactables can be provided (Spawn at least #).
 - Stats Beta: Made the Health hook IL way more robust, it should now survive most updates to RecalculateStats.
 - Stats Beta: Added BaseMultiplier which is applied before flat bonuses. The order is now (BaseStat =>) Base Multiplier => Flat Bonuses => Multiplier. 
 - 

### v1.3.3
 - Registers mods that add a contentpack to the networed mod list.
 - Only sets isModded to true if a mod has added a contentpack.
 - Utils: ItemDefsFromTier optionally only returns items that are unlocked. 
 - Changed dependency to HookgenPatcher instead of MMHOOK Standalone.

### v1.3.2
 - Bugfix: Fixed NetworkedPrefabs not actually getting registered
 - BETA: Stats: Added placeholders for other stats that should get added in the future. If you need specific support for one of these stats or any other, contact XoXFaby.
 - Bumped BepInEx dependency version.

### v1.3.1
 - Internal change: Content from different mods now loads into separate content packs. This likely won't affect anything but it would help if a mod wants to wait for a specific contentpack to load. 
   All methods that add "content" now have overloads that let you provide an identifier, if left empty, the Assembly name will be used. 
 - Bugfix: Fixed PrefabFromGameObject for networked objects.

## v1.3.0
 - Added Stats API:
   This API provides an easy way to add stats characterBody's without having to deal with IL hooks. 
   Currently the API barely has any implemented, if you need easy access to a stat for your mod it's best to suggest it to us in our Discord so we can add the appropriate functionality.
 - This version also ensures that RoR2 knows the game is modded when BetterAPI is used. I don't see any reason a clientside non-game-affecting mod would need to use BetterAPI so this is not likely to be changed. 

### v1.2.1
 - Updated for latest RoR2 Patch. 
   This is NOT a breaking change, if your mod uses BetterAPI it should continue working now. 

## v1.2.0
 - Buffs: Fixed error that stopped buffs from being added correctly;
 - CostTypes: Added CostType API, mainly used for interactables.
 - Objectives: Added Objectives API to add objectives to the tracker on the top right.
 - Added various prefab APIs:
   - BodyPrefabs
   - MasterPrefabs
   - NetworkedPrefabs
   - ProjectilePrefabs
   - Added Util method for creating prefabs from existing gameobjects.
 - BETA: Interactables: Added Interactables API for adding interactables such as chests, shrines, anything you want. 
   This API is under active development, expect breaking changes in future updates as it is worked on. 
   Join the discord if you are planning to use this API.

### v1.1.2
 - Items: Added ability to add ItemDisplays to characters that didn't originally have an ItemDisplayRuleSet (Heretic)

### v1.1.1
 - Items: Improved ItemDisplay support:
   - Set ItemDisplay by CharacterModel name or bodyPrefab name
   - Easier to use CharacterItemDisplayRuleSet helper to make adding ItemDisplays super simple
   - ItemDisplay component automatically added to followerPrefab if it's missing to avoid errors.
   - Fully backwards compatible, but I encourage using the new helper, once again see MoreItems for examples. 

## v1.1.0
 - Items: Added ItemDisplay support

## v1.0.0
 - Inital Release