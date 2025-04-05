# Description
**Fight for Universe: Beyond Reach** is modification for Ostranauts, which
expands modding capabilities of the original game, makes modding API more 
flexible and resolves some issues related to having multiple mods. It relies 
on BepInEx and MonoMod Patch Loader.

Ostranauts Steam Page: https://store.steampowered.com/app/1022980/Ostranauts/  
Ostranauts Discord Server: https://discord.gg/bluebottlegames (#modding-discussion channel)

# Modding Features
1\. Custom synchronized core + mods loading execution flow that resolves data mangling issue.  
2\. Partial data overwrite of existing entries (from any JSON file, whether it is core or other mod).  
3\. Reference-based creation of new entries with applied values. No need to copy whole code block.  
4\. Removal of specific data entries based on their IDs ('strName' parameter) via `mod_info.json`.  
5\. Precision array modification via `--ADD--`, `--INS--`, `--DEL--` and `--MOD--` (data only) commands.  
6\. Dynamic changes mapping and dynamic data modification (for existing templates and saved games).  
7\. Completely new parameters and properties that expand existing different gameplay mechanics.  
8\. Option to unlock max random range value that allows unrestricted random/loot lists.  
9\. Other various options and settings to alter gameplay and/or make it easier/harder.  

**Note:** in-depth explanation and API examples can be viewed below, in relevant paragraph.  

# Configuration Options
Settings file can be found at `\Ostranauts\BepInEx\config\FFU_Beyond_Reach.cfg` (it is
created after running Ostranauts for the first time with this mod installed).

## Configuration Settings
**SyncLogging** - defines what logging type is used for when overwriting data and/or
copy-referencing existing items into new items with various parameters overwriting.  
**ActLogging** - defines what activity will be shown in the log during specific actions. Applies
only to very specific action that related to modified game code (such as inventory sorting).  
**DynamicRandomRange** - By default loot random range is limited to `1.0f`, thus preventing use of 
loot tables, if total sum of their chances goes beyond `1.0f`. This feature allows to increase max 
possible random range beyond `1.0f`, to the total sum of all chances in the loot table.  
**MaxLogTextSize** - Defines the max length of the text in the console. Needed in case if you want 
to see the whole list of entries from the console commands without missing anything (whether it is
`getcond` or any other command). May impact performance, if the value is too big.  
**ModSyncLoading** - Enables smart loading of modified COs and synchronizing of existing CO saved 
data with updated CO templates, if they are mapped in the mod info file. Frees the user from the
need of manually updating existing save file and existing ship templates, if Parent CO's got new 
(or modified existing) built-in locked COs.  

## Gameplay Settings
**ModifyUpperLimit** - Enables use of `BonusUpperLimit` to change skill and trait modifier upper limit value.  
**BonusUpperLimit** - Defines the upper limit for skill and trait modifier bonuses. Original value is **10**.  
**SuitOxygenNotify** - Specifies the oxygen level threshold (as a percentage) for the gauge of a sealed/airtight 
suit. When the oxygen level falls below this threshold, the wearer will receive a notification (via occasional 
beeps) about oxygen usage. If set to `0`, no notification will be given at any time.  
**SuitPowerNotify** - Specifies the power level threshold (as a percentage) for the gauge of a sealed/airtight 
suit. When the power level falls below this threshold, the wearer will receive a notification (via frequent 
beeps) about power usage. If set to `0`, no notification will be given at any time.  
**ShowEachO2Battery** - Defines whether to show average percentage across all O2/Batteries or calculate each 
O2/Battery independently and summarize their percentages. Affects how soon notifications will begin.  
**StrictInvSorting** - Defines if game will be using custom, order-based inventory windows sorting that enforces 
strict UI rendering order. Relevant if slots have `sOrder` parameter set. Defaults to `nDepth` otherwise.  

## Quality Settings
**AltTempEnabled** - Allows to show temperature in alternative measure beside Kelvin value (in **top right** 
info window), similar to how gas shows mass and pressure. Uses formula: `AltTemp = K x Mult + Shift`.  
**AltTempSymbol** - Defines what symbol will represent alternative temperature measure.  
**AltTempMult** - Defines alternative temperature multiplier for conversion from Kelvin.  
**AltTempShift** - Defines alternative temperature value shift for conversion from Kelvin.  
**TowBraceAllowsKeep** - Defines if it is allowed to use station keeping command, while tow braced to 
another vessel. As you remember, even when tow braced to another ship, you can only control ship manually, 
but 'Station Keeping' command is getting turned off. This option resolves this issue.  
**OrgInventoryMode** - Enables inventory UI tweaking mode that allows to adjust inventory window offsets,
where they open, what range between them and what is the size of padding between sub-windows.  
**OrgInventoryTweaks** - Defines various offsets that adjust inventory UI. Allows float values. Required
values: Base, Top, Bottom, Padding, Grid, Safety. **Base** - horizontal offset between inventory equipment 
window and leftmost side of the screen. **Top** - horizontal offset from topmost inventory window to top of 
the screen. **Bottom** - horizontal cutout offset, below which inventory window will be shown in next column,
required to balance the 'top' offset. **Padding** - padding between inventory window columns. **Grid** -
inventory windows column auto-adjustment width for each additional horizontal inventory grid tile, only
relevant when inventory width is above 4 grid tiles. **Safety** - is safety multiplier for inventory
window height. If its too big inventory window might overflow to the next column, despite having enough
space in the current. If its too small, inventory window might overflow below intended height limit in the
same column. If you see that inventory windows overflow one way or another, enable `ActLogging` and try
various `Safety` and `Bottom` values. If there are less than 6 values, parameter is ignored and default 
values loaded instead.  
**BetterInvTransfer** - Changes behavior of **shift-click** item transferring in inventory. Items will be 
auto-transferred to the last inventory window, where player has placed the item manually. Last inventory 
window is forgotten, when inventory is closed - thus the exploit of long-distance transferring is avoided.  
**QuickBarPinning** - Allows to permanently lock the interactions quick bar, where you desire on the screen.  
**QuickBarTweaks** - Defines various offsets that adjust interactions quick bar position and mode. Required
values: Horizontal, Vertical, Expanded. **Horizontal** - defines horizontal position offset for the 
interactions quick bar. **Vertical** - defines vertical position offset for the interactions quick bar. 
**Expanded** - defines if interactions quick bar is always expanded or not (only `1` is treated as `true`,
any other number is treated as `false`).

## Superiority Settings
**NoSkillTraitCost** - Option to allow learn/unlearn any trait or skill for free.  
**AllowSuperChars** - Option to allow character to be superior or utterly miserable.  
**SuperCharMultiplier** - Set above `1.0` to reach the stars, or below `1.0` to descent into abyss.  
**SuperCharacters** - List of character names in lower case that you want to apply multiplier to.  

# New Parameters & Properties
**strReference** - a `condowners` parameter that allows to create reference-based copy of the existing CO, 
whilst overriding only specific parameters. **Note:** will inherit all properties of the original CO.

**strInvSlotEffect** - a `condowners` parameter that applies `slot effect` to every inventory item and/or the
inventory owner. Works pretty much same way as other `slot effects`, except parameters `mapMeshTextures`,
`strSlotImage` and `strSlotImageUnder` are ignored. Thread carefully, as it applies effect to **every** item,
including whole stacks.

**nSlotOrder** - a `slots` parameter that only used, if `StrictInvSorting` gameplay setting is enabled. 
Requires an integer value, but can be nulled. If nulled, defaults to `nDepth` to avoid potential issues. Slots 
with lesser numbers are rendered first in open inventory UI. Was implemented to avoid `nDepth` collision/issues.

**nMaxDepth** - a `condtrigs` parameter that used to check how deeply nested `condowners` object. If object
is nested at depth greater than `maxDepth` parameter, then condition trigger automatically returns `false`. 
Use console command `getcond [them] *coParents` on selected object and count number of **in**'s to identify 
its current depth.

**nIsSameShipCO** - a `shipspecs` parameter that allows to find a ship, where object itself is placed. Should
be a quite optimized option for `interactions` that allows to access all other COs within confines of same ship 
via `ShipTest3rd` and `CTTest3rd` parameters without much hassle.

**bForceVerbose** - a `interactions` parameter that forces interaction to be verbose even during failed attempts.
Exists only so other mod developers can debug where interactions fail and where they successfully execute. If
`ActLogging` in options is set to `Interactions`, everything also will be written into log files. Do note, that
if `nLogging` is set to `0`, no information will be written to logs, except interaction failures.

**bRoomLookup** - a `interactions` parameter that allows object to identify its `room` and send message to all 
the crewmembers in it, if interaction's `nLogging` parameter is set to `2`. Required, because by default CO don't 
contain current room information and it is only assigned via `CrewSim` (i.e. only to crewmember COs).

## New Hardcoded Conditions
**StatEmittedTemp** - a `simple condition` value that allow to override `StatSolidTemp` without changing it. 
When set, temperature emitted from object via `Heater.Heat()` will be based on it instead of `StatSolidTemp` 
parameter.

## Existing Functionality Changes
**Sensor** - an `aUpdateCommands` command. By default it can only execute interactions based on the room
conditions. Modification allows to run it condition triggers against itself, if **strPoint** is set to `null`.
In addition, if **dictGUIPropMap** that it uses, contains **dfUpdateInterval** entry - it replaces default
`1.0` (value is in **seconds**) update interval with a new value as long as its greater than `0.0`. **Note:** it
relies on Unity's internal `Update()` method, so if update interval is set to a too small value (i.e. too 
short), it just won't be able to keep up with correct timing.

## Improved Console Commands
`getcond [them] *` - now `getcond` command supports wildcard `*` that lists all stats, regardless of their name.  
`getcond [them]-NUM` - tells `getcond` to fetch data recursively from parent that is `NUM` above targeted object.  
`getcond [them] *coParents` - lists all `condowners` parent object recursively and how they are nested.  
`getcond [them] *coRules` - lists all `condrules` (with stat-related information) attached the targeted object.  
`getcond [them] *coTickers` - lists all `tickers` (plus related timer information) attached the targeted object.  
`repairship` - too lazy to repair ship or ship is too big for proper repairing? This command will do the repairs.  
`findcondcos <conditions>` - lists all CO template with corresponding condition. Supports any amount of conditions 
via `IsCondition` or/and via inverse `!IsCondition`. All conditions must be separated by space.  

## Elastic Mod Data Handling
Extension to the original modding API that allows precise modification of individual parameters in specific items
without need to overwrite JSON data block. To be precise, it allows:  
*\* Simplified Data Overwrite* - frees from need to copy entire data block just to modify single parameter.  
*\* Reference-based Creation* - allows to copy entire original data block with only specific parameters modified.  
*\* Precision Array Modification* - set of commands to precisely modify original contents of array in a safe manner.

## Existing Entities Removal
Whilst modding API already implements exclusion based on folder paths, it removes whole files with data and not 
individual items. Existing entities removal allows to remove specific items based on their type and their exact 
identifier. For example, by adding the JSON block `"removeIds": {"cooverlays": ["OutfitEVA03", "OutfitEVA03Off", 
"OutfitEVA03Dmg"]}` into `mod_info.json` will make Data Handler remove specified identifiers from `cooverlays` 
and free them for full implementation as complete condition owners for example. This routine runs before JSON 
objects are processed, thus prevents identifiers pollution.

## Dynamic Changes Mapping
Modification or removal of some items, might require lots of manual patching for existing ship templates and 
potentially starting new game (or at least pruning existing entities). In order to avoid it, features that allow 
objects mapping and removal were implemented. Configurable via `mod_info.json` file only.

### Dynamic Changes Map Commands
**Slotted Items Mapping**: `Switch_Slotted` command that allows to remap existing slotted sub-items into other
items in the list. By using code such as this `"changesMap": {"OutfitEVA01": {"Switch_Slotted" : ["PocketClipPoint01=
PocketEVAClipPoint01"]}`, on game or template load, all slotted `PocketClipPoint01` will be converted into the
`PocketEVAClipPoint01` for all `OutfitEVA01` items. The `Switch_Slotted` can contain multiple entries.

**Recover Missing Sub-Items**: `Recover_Missing` command allows to recover missing sub-items based on the assigned
loot table of the item itself. By using code `"changesMap": {"OutfitEVA01": {"Recover_Missing" : []}` game 
will attempt to recover all `bSlotLocked` sub-items for the item itself, if they are list in the assigned loot
table of the item. Specifying name of sub-items in the list will make game re-add items, even if they aren't
`bSlotLocked` as long as they are in the assigned loot table. In addition, this command also supports inverted mode
`!Recover_Missing: []` - it will attempt to recover all `bSlotLocked` sub-items based on the loot table, if they
aren't listed in the command.

**Saved COs Conditions Sync**: `Sync_Conditions` command allows to add missing conditions to already existing COs
if they are present in their template counterparts. By using code `"changesMap": {"OutfitEVA01": {"Sync_Conditions": 
[]}}` game will add missing conditions from the template to the existing saved `OutfitEVA01` CO. If you intend only
to add specific missing conditions that exist in template, listing them as `["IsNewCond", "StatNewCond"]` will make
game add only them, if they aren't present in saved CO. This command also supports inverted `"!Sync_Conditions": 
[]` mode, in which only non-listed conditions are added to the saved CO from the template.

**Saved COs Conditions Update**: `Update_Conditions` command allows to update existing condition in the saved CO
with the values from the template CO. By using code `"changesMap": {"OutfitEVA01": {"Update_Conditions": []}}` game
will fetch all existing condition values from CO templates and overwrite them in the saved COs.  Do note that such 
approach is extremely dangerous, as some conditions are dynamic and changing them in such way might break game logic 
(or game itself). Use precise approach `"Update_Conditions": ["StatBasePrice", "StatDamageMax"]` to update only 
conditions you know that are static. This command also supports `"!Update_Conditions": []` inverted mode that will
update all existing conditions values, except ones that are listed in the command.

**Sync Slotted Conditions**: `Sync_Slot_Effects` command allows to enforce specific conditions to the items that are
slotted in targeted COs. By using `"changesMap": {"OutfitEVA01": {"Sync_Slot_Effects": ["IsSlotted=1.0x1"]}}` all COs
that are slotted into `OutfitEVA01` will receive `IsSlotted` condition. If you intention is to apply effect only to
specific sub-items, you need to use `"Sync_Slot_Effects": ["IsSlotted=1.0x1|ItemName1|ItemName2"]` and it will apply 
the condition only specific items types, if they are slotted. In addition, this feature supports inverted mode via `!`
symbol in entries. Code `"Sync_Slot_Effects": ["!IsSlotted"]` will remove specified condition from slotted sub-items
and `"Sync_Slot_Effects": ["!IsSlotted|ItemName3"]` will remove it only from specific item types. It should be only 
used, when you're certain that these conditions are assigned and/or removed to/from the slotted sub-items naturally, 
when you slot them into target item, since it ignores all the trigger validations and checks.

**Sync Inventory Conditions**: `Sync_Inv_Effects` command allows to enforce specific conditions to the items that are
stored in targeted COs. By using `"changesMap": {"OutfitEVA01": {"Sync_Inv_Effects": ["IsStored=1.0x1"]}}` all COs
that are stored in the `OutfitEVA01` will receive `IsStored` condition. If you intention is to apply effect only to
specific items in the inventory, you need to use `"Sync_Inv_Effects": ["IsStored=1.0x1|ItemName1|ItemName2"]` and it 
will apply the condition only specific items types. In addition, this feature supports inverted mode via `!` symbol 
in entries. Code `"Sync_Inv_Effects": ["!IsStored"]` will remove specified condition from all the stored items
and `"Sync_Inv_Effects": ["!IsStored|ItemName3"]` will remove it only from specific item types. It should be only 
used, when you're certain that these conditions are assigned and/or removed to/from the stored items naturally, 
when you put them into item's attached inventory, since it ignores all the trigger validations and checks.

### Dynamic Changes Map Syntax
Since all parameters in `changesMap` are additive, option to remove various entries and sub-entries was implemented. 
To remove specific sub-entry, use code `"changesMap": {"OutfitEVA01": {"CommandName": ["~"]}}` and to remove entire 
entry use code `"changesMap": {"OutfitEVA01": {"~": []}}`. In addition, if you're using `{"~": [], "Command" : []}` 
it pretty much wipes previous mapped changes for that CO, whist filling with your mapped data only (of course - 
depending on mod order and what loads after your mod). Individual command entries can be modified as well. Using 
`"Switch_Slotted" : ["*Pocket01=NewPocket02"]` will modify existing entry to have `NewPocket02` instead whatever 
there was before. Using `"Switch_Slotted" : ["-Pocket01"]` will remove it instead. Do note that the `changesMap` is
modified in the order of loaded mods, thus you need pay utmost attention to it.

If the `SyncLogging` is set to `ModdedDump` or above, the final/compiled `changesMap` will be shown in the logs.

# Modding API Examples
A various JSON examples that demonstrate usage of extended modding API features in different ways.

## Removal of Existing Entries Example
```json
{
    "strName": "Your Mod Name",
    //...vaious mod parameters...//
    "removeIds": {
        "cooverlays": [
            "OutfitHelmet05",
            "OutfitHelmet05Dmg",
            "OutfitEVA05",
            "OutfitEVA05Off",
            "OutfitEVA05Dmg"
        ]
    }
}
```
In this example all `cooverlays` for Bingham-12C EVA Suit and Helmet will be completely removed from the game, in 
order to give space to proper `condowners` in the mod.  

## Dynamic Changes Map Example 
```json
{
    "strName": "Your Mod Name",
    //...vaious mod parameters...//
    "changesMap": {
        "OutfitEVA01": {
            "Switch_Slotted": ["PocketClipPoint01=PocketEVAClipPoint01"],
            "Recover_Missing": [],
            "Sync_Conditions": ["StatArmorCut", "StatArmorBlunt"],
            "!Update_Conditions": ["StatMass"],
            "Sync_Slot_Effects": ["IsModified=1.0x1|PocketEVABatt01"],
            "Sync_Inv_Effects": ["!IsUnsableItem"]
        },
        "OutfitEVA03": {
            "Switch_Slotted": ["~"],
            "Recover_Missing": ["PocketEVAO201"],
            "!Sync_Conditions": ["TestCondition"],
            "Sync_Slot_Effects": ["-IsSpecial"]
        },
        "OutfitEVA05": {
            "~": []
        }
    }
}
```
In this example in saved game or template, all `OutfitEVA01` slotted `PocketClipPoint01` COs with be replaced with
`PocketEVAClipPoint01` COs. All missing **locked** COs will be restored and re-attached to `OutfitEVA01`. If the CO
`OutfitEVA01` is missing `StatArmorCut` or/and `StatArmorBlunt` conditions, they will be added to it. In addition,
all condition values except `StatMass` will be fetched from original template and added to the `OutfitEVA01` CO.
Anything that is slotted in the `OutfitEVA01` slots will receive `IsModified` condition. And everything that is
stored in the inventory attached to `OutfitEVA01` will lose `IsUnsableItem` condition.

For the `OutfitEVA03` CO, the command `Switch_Slotted` will be removed. The `Recover_Missing` command will only
restore slotted `PocketEVAO201`, if it is present in the attached loot table. The `Sync_Conditions` command will
add all missing conditions (except `TestCondition`) from the template to `OutfitEVA03` CO. And the `Sync_Slot_Effects`
command no longer will add `IsSpecial` to items that are slotted in the `OutfitEVA03` CO.

The `OutfitEVA05` will be completely removed from the `changesMap`, regardless of what it had previously.

## Simplified Data Overwrite Example

```json
{
    "strName": "ItmRackUnder01",
    "strContainerCT": "TIsFitContainerSolidCargoBay",
    "nContainerHeight": 6,
    "nContainerWidth": 6
}
```
As in example above, you no longer need to copy entire `ItmRackUnder01` code block, just to alter a few 
lines of parameters. Writing valid `strName` (and using relevant folder) is enough to identify which 
entry you want to change.  

## Reference-based Creation Example
```json
{
    "strName": "OutfitEVA05",
    "strReference": "OutfitEVA01",
    "strNameFriendly": "Bingham-12 Civilian EVA Suit",
    "strItemDef": "OutfitEVA05",
    "strPortraitImg": "CrewSuit05",
    "mapSlotEffects": [
        "shirt_out", "BodyEVA05",
        "heldL", "HeldItmDefaultSoftL",
        "heldR", "HeldItmDefaultSoftR"
    ],
    "jsonPI": "EVASuit05"
}
```
In this example as well, you no longer need to copy entire block just to create one. When creating new entry,
it is enough to choose valid `strReference` (`strName` of some other entry) and loader will create exact same 
copy of the entry, but with new `strName` of your choice. Any additional parameters will be used to overwrite
existing copied parameters of the new entry.  

## Precision Dictionary Modifications
Some of the Ostranauts data is stored as dictionaries. In order to precisely modify it, a set modding API
handling mechanisms was implemented. In addition, modding API supports indefinite amount of nested dictionaries,
as long as dictionaries themselves are supported by the data architecture of the game itself.  

There are 3 methods of modifying dictionary records:  
`"KeyName": {//...DATA...//}` adds new or modifies existing record entry with the new data.  
`"*KeyName": {//...DATA...//}` removes all old data in the record and replaces it with the new data.  
`"~KeyName": {//...DATA...//}` completely removes all data along with the record from the dictionary.  

```json
{
    "strName": "Robot",
    "mapIAHist2": {
        "~StatSecurity": {},
        "*StatSelfRespect": {
            "strCondName": "StatSelfRespect",
            "mapInteractions": {
                "SeekKOAllow": {
                    "strName": "SeekKOAllow",
                    "nIterations": 41,
                    "fTotalValue": 2461.0
                }
            }
        },
        "StatSolidTemp": {
            "mapInteractions": {
                "ACTExcerciseTreadmillDo": {
                    "strName": "ACTExcerciseTreadmillDo",
                    "nIterations": 276,
                    "fTotalValue": 20.42594
                },
                "ACTExcerciseTreadmillDoHot": {
                    "strName": "ACTExcerciseTreadmillDoHot",
                    "nIterations": 242,
                    "fTotalValue": 9.623437
                },
                "~ACTExcerciseStrengthTrainerDo": {},
                "*ACTExcerciseStrengthTrainerDoHot": {
                    "strName": "ACTExcerciseStrengthTrainerDoHot",
                    "nIterations": 555
                },
                "ACTSomethingUnexpected": {
                    "strName": "ACTSomethingUnexpected",
                    "nIterations": 777,
                    "fTotalValue": 77
                }
            }
        }
    }
}
```
The best example of usage for this modding API is `ai_training` data. In example above, record `StatSecurity` is
completely removed, record `StatSelfRespect` has all its data replaced with completely new data in the example,
two records `ACTExcerciseTreadmillDo` and `ACTExcerciseTreadmillDoHot` are updated with new data, the record
`ACTExcerciseStrengthTrainerDo` is removed as well, record `ACTExcerciseStrengthTrainerDoHot` also has its data
completely discarded and replaced with new data, and completely new record `ACTSomethingUnexpected` is added 
to the dictionary. As you can see in example, it allows to operate at various sub-levels without an issue.

## Precision Array Modifications
Since Ostranauts is extremely data-drive game, lots of various parameters and flags are stored in various arrays 
and sub-arrays. Modified modding/loading API supports precise modifications of such arrays and sub-arrays. If
you found yourself in need of modifying such arrays: *here be dragons, thus, abandon all hope, ye who enter here*.

Arrays and sub-arrays can be of two types. Data arrays and simple arrays. Data array contain in each entry 
`"name=value"` and simple arrays arrays don't contain `=` symbol, but just string as whole, i.e. `"BodyEVA05"`.

There are 4 commands to perform such modifications:  
`--MOD--` modifies existing entry based on 'name' (for data arrays only), without losing existing data.   
`--DEL--` removes existing entry based on 'name' (for data arrays) or on whole string (for simple arrays).  
`--ADD--` just adds the entry to the existing data as is, doesn't care about array type.  
`--INS--` inserts the entry at designated index, shifting whatever was at this index forward.

**Note:** make sure that precisely modified arrays start from any command. Otherwise, they will be copied as is
and you're pretty much asking for a trouble. Same applies to sub-arrays, except first goes number of modifier row
in the array - loader can't do anything with row number, but without command and will tell you about it.

### Precision Array Modification Example
```json
{
    "aNormalArray": [
        "--MOD--",
        "StatBasePrice=1.0x9000.0",
        "--DEL--",
        "IsBingham=",
        "--ADD--",
        "StatDismantleProgressMax=1.0x180"
        "--INS--4",
        "StatSpecial=1.0x60"
    ]
}
```
In example above existing array `aNormalArray` was modified in the following way: the `StatBasePrice` was 
changed to `1.0x9000.0`, `IsBingham` was removed from the list disregarding the value, the parameter
`StatDismantleProgressMax=1.0x180` was added as is to the list and the parameter `StatSpecial=1.0x60` was
inserted at the place of 4th entry in the array and whatever was there initially shifted forward.

### Precision Sub-Array Modification Example
```json
{
    "aNestedSubArray": [
        "5|--ADD--|IsClumsy=0.0675x1.0|IsStupid=9000x1.0|--MOD--|IsBrave=0.5675x1.0|--DEL--|IsCraven",
        "13|--ADD--|IsMagic=9000x1.0|IsFriendship=9000x1.0|IsHeresy=9000x1.0|--INS--4|IsGood=1.0x60"
    ]
}
```
Sub-array is essentially array within array, but with different separator (`|` in that case). First you write 
index of the row you intend to override (1st row is `1`, 8th is `8` & etc), then modification command and then
entries you want to add/remove/modify (depending on type of the sub-array). In example above existing array
`aNestedSubArray` received modification for 5th and 13th existing rows.

### Mixed Array Modification Example
```json
{
    "aVeryMixedArray": [
        "5|--ADD--|IsClumsy=0.0675x1.0|IsStupid=9000x1.0|--MOD--|IsBrave=0.5675x1.0|--DEL--|IsCraven",
        "--ADD--",
        "IsGenius=0.0675x1.0|IsObtuse=0.0675x1.0",
        "IsGlutton=0.0675x1.0|IsTemperate=0.0675x1.0",
        "IsGregarious=0.0675x1.0|IsShy=0.0675x1.0",
        "IsHonest=0.0675x1.0|IsLiar=0.0675x1.0",
        "--DEL--",
        "IsBeautiful",
        "IsClumsy",
        "13|--ADD--|IsMagic=9000x1.0|IsFriendship=9000x1.0|IsHeresy=9000x1.0|--INS--4|IsGood=1.0x60"
    ]
}
```
In example above, array is modified both, as array and as sub-array. Be extremely careful when using 
`--MOD--` command, as you can accidentally modify whole sub-array as array entry, if you'll forget to put 
a dedicated command into the sub-array entry itself.  

### Loot Table Array Edge Cases
```json
{
    "aLootArrayCase": [
        "5=|--ADD--=|IsClumsy=0.0675x1.0|IsStupid=9000x1.0|--MOD--=|IsBrave=0.5675x1.0|--DEL--=|IsCraven=",
        "--ADD--=",
        "IsGenius=0.0675x1.0|IsObtuse=0.0675x1.0",
        "IsGlutton=0.0675x1.0|IsTemperate=0.0675x1.0",
        "IsGregarious=0.0675x1.0|IsShy=0.0675x1.0",
        "IsHonest=0.0675x1.0|IsLiar=0.0675x1.0",
        "--DEL--=",
        "IsBeautiful=",
        "IsClumsy=",
        "13=|--ADD--=|IsMagic=9000x1.0|IsFriendship=9000x1.0|IsHeresy=9000x1.0|--INS--4=|IsGood=1.0x60"
    ]
}
```
Loot table arrays located in `loot` folder are edge cases for arrays, because `Loot` class has its own parser,
and if you dare you forget about it for a second, it will remind you about it with big red text in main menu.
Basically, due to the nature of the `Loot` class, you need to shove `=` everywhere as in example above, even
if it isn't needed or used.

# Installation
1\. Download BepInEx v5.4.23.2 WINx64: https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2  
2\. Unzip all contents into `Ostranauts` root folder. It should contain now `BepInEx`, `winhttp.dll` & etc.  
3\. Download MonoMod Loader: https://github.com/BepInEx/BepInEx.MonoMod.Loader/releases/tag/v1.0.0.0  
4\. Unzip all contents into `Ostranauts` root folder. `Ostranauts/BepInEx/core` should now contain `MonoMod.dll`.  
5\. Download my mod's DLL file from releases (or compile it) and put it into `Ostranauts/BepInEx/monomod`.  
6\. Launch game, once in the main menu, exit, modify config files as you want. Enjoy playing/modding.  

## Potential Features
1\. Implementation of `condtrigs` parameters that allow to compare values with specific numbers, including 
possibility of minor mathematical operations (i.e. compare stat with 1% of another stat for example).  
2\. Implementation of option that prevents game from unpausing, even when you're queuing any interaction.  