This core is based pretty heavily on TrinityCore(TC)'s setup.

With Unity, however, things are a bit simpler when it comes to assigning variables in scripts. Language is C#.

Unfortunately in Unity, there isn't a base setup for client/server communication as Unity is primarily a "single-player" engine.
That said, however, I'm using a package called Mirror that allows for client/server communication.

Spells are created and modified in the SpellData.json file.

***Core Systems***


Spell System: Handles the creation, casting, and management of spells, including attributes and effects.


**Unit Management:** Manages all entities in the game, including players, NPCs, and creatures. Units can have health, mana, and can interact with other units.


**Combat System:** Implements combat mechanics, including damage calculation, health management, and spell interactions.


**VFX Management:** Manages visual effects associated with spells, including projectiles, impacts, and other effects.


**UI Components:** Custom UI elements such as health bars, cast bars, and other game HUD components.


****Key Scripts****


**SpellDataHandler.cs:** Loads and manages all spell-related data, including effects, attributes, and associated VFX.


**VFXManager.cs:** Handles the instantiation and management of visual effects for spells.


**ClientNetworkManager.cs:** Manages client-side networking, including handling opcodes and synchronizing game state with the server.


**GameNetworkManager.cs:** Manages server-side networking, including handling opcodes from the client.


**Unit.cs:** Base class for all game units, including players and NPCs, handling health, mana, and interactions.


Data Files
SpellData.json: JSON file containing all spell data, including effects, attributes, and cooldowns.
ScriptableObjects: Used to store prefab references and configurations for spells and VFX.
