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
6\. Completely new parameters and properties that expand existing different gameplay mechanics.  
7\. Option to unlock max random range value that allows unrestricted random/loot lists.  
8\. Other various options and settings to alter gameplay and/or make it easier/harder.  

**Note:** coding API examples can be viewed below, in relevant paragraph.  

# Configuration Options
Settings file can be found at `\Ostranauts\BepInEx\config\FFU_Beyond_Reach.cfg` (it is
created after running Ostranauts for the first time with this mod installed).

## Configuration Settings
**SyncLogging** - defines what logging type is used for when overwriting data and/or
copy-referencing existing items into new items with various parameters overwriting.  
**ActLogging** - defines what activity will be shown in the log during gameplay/runtime. Applies
only to very specific action that related to modified game code (such as inventory sorting).  
**DynamicRandomRange** - By default loot random range is limited to `1.0f`, thus preventing use of 
loot tables, if total sum of their chances goes beyond `1.0f`. This feature allows to increase max 
possible random range beyond `1.0f`, to the total sum of all chances in the loot table.  
**IgnoredKeys** - Case-sensitive list of entries for Dynamic Random Range feature to ignore (for 
avoiding errors). In the vanilla behavior, some items were expected to never appear due to random 
range being limited to `1f`, but it isn't the case, if Dynamic Random Range is enabled.  
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

## Superiority Settings
**NoSkillTraitCost** - Option to allow learn/unlearn any trait or skill for free.  
**AllowSuperChars** - Option to allow character to be superior or utterly miserable.  
**SuperCharMultiplier** - Set above `1.0` to reach the stars, or below `1.0` to descent into abyss.  
**SuperCharacters** - List of character names in lower case that you want to apply multiplier to.  

# New Parameters & Properties
**StatEmittedTemp** - a `simple condition` value that allow to override `StatSolidTemp` without changing it. 
When set, temperature emitted from object via `Heater.Heat()` will be based on it instead of `StatSolidTemp` 
parameter.

**invSlotEffect** - a `condowners` parameter that applies `slot effect` to every inventory item and/or the
inventory owner. Works pretty much same way as other `slot effects`, except parameters `mapMeshTextures`,
`strSlotImage` and `strSlotImageUnder` are ignored. Thread carefully, as it applies effect to **every** item,
including whole stacks.

**sOrder** - a `slots` parameter that only used, if `StrictInvSorting` gameplay setting is enabled. Requires 
an integer value, but can be nulled. If nulled, defaults to `nDepth` to avoid potential issues. Slots with 
lesser numbers are rendered first in open inventory UI. Was implemented to avoid `nDepth` collision/issues.

**maxDepth** - a `condtrigs` parameter that used to check how deeply nested `condowners` object. If object
is nested at depth greater than `maxDepth` parameter, then condition trigger automatically returns `false`. 
Use console command `getcond [them] *coParents` on selected object and count number of **in**'s to identify 
its current depth.

## Improved Console Commands
`getcond [them] *` - now `getcond` command supports wildcard `*` that lists all stats, regardless of their name.  
`getcond [them]-NUM` - tells `getcond` to fetch data recursively from parent that is `NUM` above targeted object.  
`getcond [them] *coParents` - lists all `condowners` parent object recursively and how they are nested.  
`getcond [them] *coRules` - lists all `condrules` (with stat-related information) attached the targeted object.  
`getcond [them] *coTickers` - lists all `tickers` (plus related timer information) attached the targeted object.  

# Modding API Examples
In addition to implementation of synchronized loading, this mod improves quality of modding itself and 
releases from burden of copying entire code blocks just to overwrite a couple of parameters.  

## Partial Data Overwrite Example

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

## Removal of Existing Entries Example
```json
[
	{
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
]
```
Prior to processing/loading data, mod loader will remove every item listed in `removeIds` from the `mod_info.json`. 
It allows removal of existing core and mod items, given correct loading order is used. It identifies entry type
based on folder name, where data is stored: `cooverlays`, `conditions`, `loot` & etc. In example above I removed
all `cooverlays` for Bingham-12C EVA Suit/Helmet, to give space to proper `condowners` in the mod.  

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

**Note**: make sure that precisely modified arrays start from any command. Otherwise, they will be copied as is
and you're pretty much asking for a trouble. Same applies to sub-arrays, except first goes number of modifier row
in the array - loader can't do anything with row number, but without command and will tell you about it.

### Precision Array Modification Example
```json
{
    //...code...//
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
    //...code...//
}
```
In example above existing array `aNormalArray` was modified in the following way: the `StatBasePrice` was 
changed to `1.0x9000.0`, `IsBingham` was removed from the list disregarding the value, the parameter
`StatDismantleProgressMax=1.0x180` was added as is to the list and the parameter `StatSpecial=1.0x60` was
inserted at the place of 4th entry in the array and whatever was there initially shifted forward.

### Precision Sub-Array Modification Example
```json
{
    //...code...//
    "aNestedSubArray": [
        "5|--ADD--|IsClumsy=0.0675x1.0|IsStupid=9000x1.0|--MOD--|IsBrave=0.5675x1.0|--DEL--|IsCraven",
		"13|--ADD--|IsMagic=9000x1.0|IsFriendship=9000x1.0|IsHeresy=9000x1.0|--INS--4|IsGood=1.0x60"
    ]
    //...code...//
}
```
Sub-array is essentially array within array, but with different separator (`|` in that case). First you write 
index of the row you intend to override (1st row is `1`, 8th is `8` & etc), then modification command and then
entries you want to add/remove/modify (depending on type of the sub-array). In example above existing array
`aNestedSubArray` received modification for 5th and 13th existing rows.

### Mixed Array Modification Example
```json
{
    //...code...//
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
    //...code...//
}
```
In example above, array is modified both, as array and as sub-array. Be extremely careful when using 
`--MOD--` command, as you can accidentally modify whole sub-array as array entry, if you'll forget to put 
a dedicated command into the sub-array entry itself.  

### Loot Table Array Edge Cases
```json
{
    //...code...//
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
    //...code...//
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