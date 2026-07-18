# All Classes Rebalanced - Unofficial Patch

Adjusts the balance of certain traits in the [AllClasses](https://mods.vintagestory.at/allclasses) mod to make them less extreme and improve overall balance. It also adds numerous new class-exclusive recipes to make some classes more immersive and useful.
If you are looking for a mod that enriches gameplay and encourages cooperation with other players without forcing them into narrow roles through powerful buffs and debuffs that make other activities inefficient, this mod is for you. My goal is to tone down those modifiers while preserving each class’s identity.

## Reasoning
Why does this mod exist?

> I wanted a more personalized version of AllClasses for my private server. The changes in this patch better reflect how we intended to play Vintage Story. I posted this mod here to make distribution easier for myself and to provide an option for players who feel similarly about the balance of the base AllClasses mod.

## Installation
Download this mod through [official Vintage Story modding site](https://mods.vintagestory.at/allrebalanced) or check out [Releases](https://github.com/Eques72/VS-Classes-Rebalanced/releases) seciton.
Drop the zip file into `VintageStory/config/VintagestoryData/Mods/` and play.

### FAQ
- Can I hack locusts with a Tuning Spear as any class?
> No. Only the Clockmaker can do that, although Tinker can craft the spear.

- Can this mod be safely disabled in an existing world?
> Yes. You can disable it, and AllClasses will return to its base state.

- Can this mod be added to an existing world?
> Yes, but AllClasses must also be installed.

- How do I change my or others class mid-game?
> Enter the command `/player NICK_OF_A_PLAYER_THAT_CHANGES_CLASS allowcharselonce`, and then, player declared in previous command must use command `.charsel`. You need administrator rights to do this on a server.

- Can I play with this mod enabled in language different than english?
> Yes, but some stuff that this mod changes will be shown in english. 

## Compatibility
This mod has the same compatibility and incompatibility as [AllClasses](https://mods.vintagestory.at/allclasses), except for mods that also modify the traits of the vanila **Blackguard** or **Tailor** classes.

## Dependencies and Acknowledgements
This mod is a custom and unofficial patch for [AllClasses](https://mods.vintagestory.at/allclasses) by [DejFidOFF](https://linktr.ee/DejFidOFF).
Thank you for creating and supporting such a robust and elaborate mod!
[DejFidOFF](https://linktr.ee/DejFidOFF) is in no way responsible for any issues or effects that this mod may have on your worlds.

## Mod Descriptopn
Changes, additions and balancing.
### Traits
#### Base Game Classes
- Tailor: Removes the Kind debuff *(-10% animal loot, -25% harvesting speed)* and expands the range of recipes available to Tailors.
- Blackguard: Removes the Heavyhanded debuff *(-10% loot from cracked vessels, -15% loot from foraging, -20% wild crop drop rate)*.

#### AllClasses Classes
> TL;DR: Each class has four positive traits and two negative traits. One of the four positive traits usually unlocks crafting recipes.

- **Alchemyst:**
  - **Transmutation:** *Unlocks unique recipes. See [Recipes](#recipes) for Alchemyst.*
  - **Fleetfooted:** *+15% walk speed.*
  - \+ **Forager:** *+10% loot from foraging, +20% wild crop drop rate.*
  - **Fasting:** *~~-20% hunger rate.~~ → -15% hunger rate.*
  - \+ **Farsighted:** *-15% melee damage.*
  - \+ **Weak:** *-2 health points, -10% mining speed.*
  - ~~**Forager:** *+25% wild crop drops, +50% forage drops.~~*
  - ~~**Sluggish:** *-25% mining speed.~~*

- **Artisan:**
  - **Artifice:** *Allows the exclusive crafting of decorative paintings and cozy small carpets.*
  - \+ **Clothier:** *Unlocks unique recipes. See [Recipes](#recipes) for Tailor.*
  - **Pilferer:** *~~+35% vessel-content drops, +200% rusty gear drops, +25% whole-vessel loot chance, -20% hunger rate.~~ → +25% vessel-content drops, +150% rusty gear drops, +25% whole-vessel loot chance.*
  - **Rugged:** *~~+5 maximum health.~~ → +2.5 maximum health.*
  - \+ **Civil:** *-25% animal loot drops, +50% animal harvesting time, -10% forage drops, -10% wild crop drops.*
  - **Nervous:** *~~+50% armor durability loss, -20% melee weapon damage.~~ → +25% armor durability loss, -10% melee weapon damage.*

- **Chef:**
  - **Culinary:** *Allows the crafting of various metal cooking pots and high-nutrition dried aged beef.*
  - \+ **Forager:** *+10% loot from foraging, +20% wild crop drop rate.*
  - \+ **Cultivator:** *+10% produce drops.*
  - **Butcher:** *~~+25% animal loot drops, -50% animal harvesting time.~~ → +20% animal loot drops, +25% animal harvesting time.*
  - **Clumsy:** *~~+25% armor durability loss.~~ → +15% armor durability loss.*
  - \+ **Inaccurate:** *-20% ranged weapon accuracy, -15% ranged weapon speed.*
  - ~~**Exhausted:** *-35% ranged weapon damage, -35% bow drawing strength, -20% ranged weapon accuracy, -30% ranged weapon speed.~~*
  - ~~**Fasting:** *-20% hunger rate.~~*

- **Homesteader:**
  - **Pioneer:** *Allows seed extraction and the crafting of compost, soil, and universal fertilizers.*
  - \+ **Sneaky:** *-40% animal detection range, +5% walk speed.*
  - **Farmer:** *+30% produce drops.*
  - **Rugged:** *~~+5 maximum health.~~ → +2.5 maximum health.*
  - ~~**Butcher:** *+25% animal loot drops, -50% animal harvesting time.~~*
  - \+ **Nervous:** *-15% melee damage.*
  - **Petraphobia:** *-20% ore drops, -25% mining speed.*

- **Lumberjack:**
  - **Carpenter:** *Allows the crafting of elegant doors, wagon wheels, wallpaper, and steel lumberjack axes.*
  - \+ **Burner:** *+35% charcoal-pile drops.*
  - **Lumberjack:** *~~+10% wood drops, +50% fruit-tree drops, +100% tree-seed drops, +50% stick drops.~~ → +10% wood drops, +50% fruit-tree drops, +100% tree-seed drops, +25% stick drops.*
  - **Rugged:** *~~+5 maximum health.~~ → +2.5 maximum health.*
  - ~~**Scavenger:** *+20% wild crop drops, +30% forage drops.~~*
  - \+ **Kind:** *-10% animal loot, -25% harvesting speed.*
  - **Inaccurate:** *~~-25% ranged weapon damage, -20% ranged weapon accuracy, -25% bow drawing strength, -15% ranged weapon speed.~~ → -20% ranged weapon accuracy, -15% ranged weapon speed.*

- **Mason:**
  - **Mason:** *Allows cheaper crafting of mortar, plaster, refractory bricks, and raw bricks, as well as the crafting of stone bricks directly from rocks and brown clay bricks.*
  - \+ **Gearhead:** *+125% rusty gear drops, +150% temporal gear drops.*
  - **Defender:** *~~+10% melee weapon damage, -20% armor durability loss, -20% armor movement-speed penalty, -20% hunger rate.~~ → +10% melee weapon damage, -10% armor movement-speed penalty.*
  - **Rockhound:** *~~+15% ore drops, +50% mining speed.~~ → +15% ore drops, +25% mining speed.*
  - ~~**Rugged:** *+5 maximum health.~~*
  - \+ **Fragile:** *-3 maximum health.*
  - **Heavyfooted:** *~~+20% animal detection range.~~ → +40% animal detection range, -5% walk speed.*

- **Merchant:**
  - **Mercantile:** *Allows the crafting of the 15-slot LV Handbag, pirate gold coins, and rusty gears.*
  - \+ **Scavenger:** *+20% wild crop drops, +30% forage drops.*
  - **Sneaky:** *~~-60% animal detection range, +15% walk speed.~~ → -40% animal detection range, +5% walk speed.*
  - \+ **Clumsy:** *+15% armor durability loss.*
  - **Squeamish:** *-25% animal loot drops, +10% animal harvesting time.*

- **Miner:**
  - **Prospector:** *Allows cheaper crafting of rock bombs, blasting powder, and waterproof lanterns.*
  - \+ **Pilferer:** *+15% drop rate from cracked vessels, +10% rusty gear drop rate, 12% chance to collect cracked vessels.*
  - \+ **Fasting:** *-15% hunger rate.*
  - **Miner:** *~~+40% ore drops, +100% mining speed.~~ → +25% ore drops, +50% mining speed.*
  - \+ **Unwell:** *-25% healing effectiveness, +10% armor movement-speed penalty.*
  - **Exhausted:** *~~-35% ranged weapon damage, -35% bow drawing strength, -20% ranged weapon accuracy, -30% ranged weapon speed.~~ → -25% ranged weapon damage, -25% bow drawing strength, -10% ranged weapon accuracy, -15% ranged weapon speed.*

- **Mystic:**
  - **Transcription:** *Allows the crafting of carpets and parchment and the summoning of butterflies.*
  - **Scavenger:** *+20% wild crop drops, +30% forage drops.*
  - **Pilferer:** *~~+35% vessel-content drops, +200% rusty gear drops, +25% whole-vessel loot chance, -20% hunger rate.~~ → +25% vessel-content drops, +150% rusty gear drops, +25% whole-vessel loot chance.*
  - **Sneaky:** *~~-60% animal detection range, +15% walk speed.~~ → -40% animal detection range, +5% walk speed.*
  - \+ **Hungry:** *+10% hunger rate.*
  - **Clumsy:** *~~+25% armor durability loss.~~ → +15% armor durability loss.*
  - ~~**Careless:** *-20% ore drops.~~*

- **Ranger:**
  - **Sentry:** *Allows the crafting of simple bows, reflex bows, and crude arrows.*
  - \+ **Soldier:** *+10% melee weapon damage.*
  - **Sharpeye:** *~~+100% ranged weapon damage, +50% bow drawing strength, +25% ranged weapon speed, +100% ranged weapon accuracy.~~ → +10% ranged weapon damage, +10% bow drawing strength, +10% ranged weapon speed, +10% ranged weapon accuracy.*
  - **Furtive:** *~~-25% animal detection range.~~ → -20% animal detection range.*
  - ~~**Defender:** *+10% melee weapon damage, -20% armor durability loss, -20% armor movement-speed penalty, -20% hunger rate.~~*
  - \+ **Ravenous:** *+20% hunger rate.*
  - **Squeamish:** *-25% animal loot drops, +10% animal harvesting time.*
  - ~~**Petraphobia:** *-20% ore drops, -25% mining speed.~~*

- **Smith:**
  - **Smith:** *Allows armor and shields to be repaired with toolkits and enables the crafting of frying pans, cheaper ore bombs, and blasting powder.*
  - \+ **Mender:** *-25% armor durability loss.*
  - **Defender:** *~~+10% melee weapon damage, -20% armor durability loss, -20% armor movement-speed penalty, -20% hunger rate.~~ → +10% melee weapon damage, -10% armor movement-speed penalty.*
  - **Burner:** *+35% charcoal-pile drops.*
  - \+ **Exhausted:** *-25% ranged weapon damage, -25% bow drawing strength, -10% ranged weapon accuracy, -15% ranged weapon speed.*
  - **Heavyhanded:** *-20% vessel-content drops, -20% forage drops, -20% wild crop drops.*

- **Medic:**
  - **Stitch Doctor:** *Allows the crafting of sewing kits and protective quilted armor.*
  - **Surgeon:** *Unlocks unique recipes. See [Recipes](#recipes) for Medic.*
  - \+ **Fleetfooted:** *+10% walk speed.*
  - \+ **Cultivator:** *+10% produce drops.*
  - \+ **Squeamish:** *-25% animal loot drops, +10% animal harvesting time.*
  - \+ **Farsighted:** *-15% melee damage.*
  - ~~**Civil:** *-25% animal loot drops, +15% animal harvesting time, -15% forage drops, -15% wild crop drops.~~*
  - ~~**Mender:** *-25% armor durability loss.~~*
  - ~~**Rugged:** *+5 maximum health.~~*

- **Tinker:**
  - **Tinkerer:** *Unlocks unique recipes. See [Recipes](#recipes) for Tinker.*
  - **Technical:** *~~-100% temporal gear repair cost, +50% mechanical damage.~~ → -100% temporal gear repair cost, +30% mechanical damage.*
  - \+ **Gearhead:** *+125% rusty gear drops, +150% temporal gear drops.*
  - \+ **Sneaky:** *-40% animal detection range, +5% walk speed.*
  - ~~**Crafty:** *Allows the efficient crafting of metal parts, scrap weapon kits, junction, straight, T-shaped, elbow, and directional pipes, angled gears, and shafts.~~*
  - ~~**Fleetfooted:** *+15% walk speed.~~*
  - ~~**Rockhound:** *+15% ore drops, +50% mining speed.~~*
  - \+ **Unwell:** *-25% healing effectiveness, +10% armor movement-speed penalty.*
  - **Civil:** *~~-25% animal loot drops, +15% animal harvesting time, -15% forage drops, -15% wild crop drops.~~ → -25% animal loot drops, +50% animal harvesting time, -10% forage* drops, -10% wild crop drops.

### Recipes
#### Common Recipes
- New recipe: Boil seawater in a pot to produce salt. Four liters of boiled seawater produce one unit of salt.
- Repairing armor and shields consumes less tool durability.

#### Base Game
- Clockmaker has an expanded list of custom recipes and can craft cheaper mechanical parts for windmills and other mechanisms.
- Tailor can make cheaper thread and craft sails and large sails more efficiently.

#### AllClasses
- \+ Artisan and Tailor can now craft flax twine at a 25% lower cost, as well as more cost-effective sails and large sails.
- \+ Tinker has an expanded list of mechanical parts that can be crafted more cheaply, including transmissions, wooden toggles, and spur gears. This also applies to Clockmaker, as both classes now share the same unique recipes.
- \+ Alchemyst can create saltpeter, produce rot from bones, and craft cheaper blasting powder and scrap bombs. Alchemyst can also transmute silver into gold or electrum, and vice versa.
- \+ Medic can make bandages and poultices much more efficiently.
- Smith can also repair Forlorn Hope armor and Blackguard armor. The Smith's blasting powder recipe has been changed to resemble the base recipe while remaining more efficient.
- The Miner's recipes for blasting powder and stone bombs have been changed to resemble the base recipes while remaining more efficient.
- Tinker now uses rendered fat instead of regular fat and can no longer craft sails or metal parts.

## Issues and Bug reports:
Use Issue tracker to report bugs. Please provide description, logs and steps to reproduce the issue. This will allow me to act quicker on the matter.
**DO NOT SEND BUG FIX REQUESTS to creator of original AllClasses if you have this mod enabled!**

## Suggestions:
Place your suggestions and feedback in the [comment section](https://mods.vintagestory.at/allrebalanced). 

## License
[MIT Standard License](https://github.com/Eques72/)

## Author
[**@Eques**](https://github.com/Eques72), 2026
