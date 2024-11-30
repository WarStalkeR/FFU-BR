#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using BepInEx;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;

namespace FFU_Beyond_Reach {
    public partial class FFU_BR_Defs {
        public static readonly string ModName = "Fight For Universe: Beyond Reach";
        public static readonly string ModVersion = "0.4.1.5";

        private static ConfigFile ModDefs = null;
        public static SyncLogs SyncLogging = SyncLogs.None;
        public static ActLogs ActLogging = ActLogs.None;
        public static bool DynamicRandomRange = true;
        public static string[] IgnoredKeys = new string[] {
            "pbaseEyesMissing", "pbaseGlassesMissing", "pbaseHeadMissing", 
            "pbaseLipsMissing", "pbaseNeckMissing", "pbaseNoseMissing", 
            "pbasePupilsMissing", "pbaseScarMissing", "pbaseTeethMissing", 
            "pbaseHairMissing", "pbaseBeardMissing"
        };
        public static int MaxLogTextSize = 16382;
        public static bool ModSyncLoading = true;
        public static bool ModifyUpperLimit = false;
        public static float BonusUpperLimit = 1000f;
        public static float SuitOxygenNotify = 10.0f;
        public static float SuitPowerNotify = 15.0f;
        public static bool ShowEachO2Battery = true;
        public static bool StrictInvSorting = true;
        public static bool AltTempEnabled = true;
        public static string AltTempSymbol = "C";
        public static float AltTempMult = 1.0f;
        public static float AltTempShift = -273.15f;
        public static bool TowBraceAllowsKeep = true;
        public static bool OrgInventoryMode = true;
        public static float[] OrgInventoryTweaks = new float[] { -60, -65, -30, -50, 16 };
        public static bool NoSkillTraitCost = false;
        public static bool AllowSuperChars = false;
        public static float SuperCharMultiplier = 10f;
        public static string[] SuperCharacters = new string[] {
            "von neuman", "warstalker"
        };

        public static void InitConfig() {
            ModDefs = new ConfigFile(Path.Combine(Paths.ConfigPath, "FFU_Beyond_Reach.cfg"), true);

            // Logging Start
            UnityEngine.Debug.Log($"{ModName} v{ModVersion}");
            UnityEngine.Debug.Log($"Loading Mod Configuration...");

            // Load Configuration Settings
            SyncLogging = ModDefs.Bind("ConfigSettings", "SyncLogging", SyncLogging,
                "Defines what changes will be shown in the log during sync loading.").Value;
            ActLogging = ModDefs.Bind("ConfigSettings", "ActLogging", ActLogging,
                "Defines what activity will be shown in the log during gameplay/runtime.").Value;
            DynamicRandomRange = ModDefs.Bind("ConfigSettings", "DynamicRandomRange", DynamicRandomRange,
                "By default loot random range is limited to 1f, thus preventing use of loot tables, if " +
                "total sum of their chances goes beyond 1f. This feature allows to increase max possible " +
                "random range beyond 1f, to the total sum of all chances in the loot table.").Value;
            string refIgnoreString = ModDefs.Bind("ConfigSettings", "IgnoredKeys", string.Join("|", IgnoredKeys),
                "Case-sensitive list of entries for Dynamic Random Range feature to ignore (for avoiding " +
                "errors). In the vanilla behavior, some items were expected to never appear due to random range " +
                "being limited to 1f, but it isn't the case, if Dynamic Random Range is enabled.").Value;
            if (!string.IsNullOrEmpty(refIgnoreString)) IgnoredKeys = refIgnoreString.Split('|');
            MaxLogTextSize = ModDefs.Bind("ConfigSettings", "MaxLogTextSize", MaxLogTextSize,
                "Defines the max length of the text in the console. May impact performance.").Value;
            ModSyncLoading = ModDefs.Bind("ConfigSettings", "ModSyncLoading", ModSyncLoading,
                "Enables smart loading of modified COs and synchronizing of existing CO saved " +
                "data with updated CO templates, if they are mapped in the mod info file.").Value;

            // Load Gameplay Settings
            ModifyUpperLimit = ModDefs.Bind("GameplaySettings", "ModifyUpperLimit", ModifyUpperLimit,
                "Allows to change skill and trait modifier upper limit value.").Value;
            BonusUpperLimit = ModDefs.Bind("GameplaySettings", "BonusUpperLimit", BonusUpperLimit,
                "Defines the upper limit for skill and trait modifier. Original value is 10.").Value;
            SuitOxygenNotify = ModDefs.Bind("GameplaySettings", "SuitOxygenNotify", SuitOxygenNotify,
                "Specifies the oxygen level threshold (as a percentage) for the gauge of a sealed/airtight suit. " +
                "When the oxygen level falls below this threshold, the wearer will receive a notification (via " +
                "occasional beeps) about oxygen usage. If set to 0, no notification will be given at any time.").Value;
            SuitPowerNotify = ModDefs.Bind("GameplaySettings", "SuitPowerNotify", SuitPowerNotify,
                "Specifies the power level threshold (as a percentage) for the gauge of a sealed/airtight suit. " +
                "When the power level falls below this threshold, the wearer will receive a notification (via " +
                "frequent beeps) about power usage. If set to 0, no notification will be given at any time.").Value;
            ShowEachO2Battery = ModDefs.Bind("GameplaySettings", "ShowEachO2Battery", ShowEachO2Battery,
                "Defines whether to show average percentage across all O2/Batteries or calculate each O2/Battery " +
                "independently and summarize their percentages. Affects how soon notifications will begin.").Value;
            StrictInvSorting = ModDefs.Bind("GameplaySettings", "StrictInvSorting", StrictInvSorting,
                "Enables custom, order-based inventory windows sorting that enforces strict UI rendering order.").Value;
            UnityEngine.Debug.Log($"GameplaySettings => ModifyUpperLimit: {ModifyUpperLimit}");
            UnityEngine.Debug.Log($"GameplaySettings => BonusUpperLimit: {BonusUpperLimit}");
            UnityEngine.Debug.Log($"GameplaySettings => SuitOxygenNotify: {SuitOxygenNotify}%");
            UnityEngine.Debug.Log($"GameplaySettings => SuitPowerNotify: {SuitPowerNotify}%");
            UnityEngine.Debug.Log($"GameplaySettings => ShowEachO2Battery: {ShowEachO2Battery}%");
            UnityEngine.Debug.Log($"GameplaySettings => StrictInvSorting: {StrictInvSorting}%");

            // Load Quality Settings
            AltTempEnabled = ModDefs.Bind("QualitySettings", "AltTempEnabled", AltTempEnabled,
                "Allows to show temperature in alternative measure beside Kelvin value.").Value;
            AltTempSymbol = ModDefs.Bind("QualitySettings", "AltTempSymbol", AltTempSymbol,
                "What symbol will represent alternative temperature measure.").Value;
            AltTempMult = ModDefs.Bind("QualitySettings", "AltTempMult", AltTempMult,
                "Alternative temperature multiplier for conversion from Kelvin.").Value;
            AltTempShift = ModDefs.Bind("QualitySettings", "AltTempShift", AltTempShift,
                "Alternative temperature value shift for conversion from Kelvin.").Value;
            TowBraceAllowsKeep = ModDefs.Bind("QualitySettings", "TowBraceAllowsKeep", TowBraceAllowsKeep,
                "Allows to use station keeping command, while tow braced to another vessel.").Value;
            OrgInventoryMode = ModDefs.Bind("QualitySettings", "OrgInventoryMode", OrgInventoryMode,
                "Changes inventory layout and makes smart use of available space.").Value;
            string refTweakString = ModDefs.Bind("QualitySettings", "OrgInventoryTweaks", 
                string.Join("|", Array.ConvertAll(OrgInventoryTweaks, n => n.ToString())),
                "Organized inventory offsets for tweaking: Base, Top, Bottom, Padding, Grid.").Value;
            if (!string.IsNullOrEmpty(refTweakString)) OrgInventoryTweaks = Array.ConvertAll(
                refTweakString.Split('|'), x => float.TryParse(x, out float v) ? v : 0f);
            UnityEngine.Debug.Log($"QualitySettings => AltTempEnabled: {AltTempEnabled}");
            UnityEngine.Debug.Log($"QualitySettings => AltTempSymbol: {AltTempEnabled}");
            UnityEngine.Debug.Log($"QualitySettings => AltTempMult: {AltTempMult}");
            UnityEngine.Debug.Log($"QualitySettings => AltTempShift: {AltTempShift}");
            UnityEngine.Debug.Log($"QualitySettings => TowBraceAllowsKeep: {TowBraceAllowsKeep}");
            UnityEngine.Debug.Log($"QualitySettings => OrgInventoryMode: {OrgInventoryMode}");
            UnityEngine.Debug.Log($"QualitySettings => OrgInventoryTweaks: " + string.Join(", ", 
                Array.ConvertAll(OrgInventoryTweaks, x => x.ToString())));

            // Load Superiority Settings
            NoSkillTraitCost = ModDefs.Bind("SuperSettings", "NoSkillTraitCost", NoSkillTraitCost,
                "Makes all trait and/or skill changes free, regardless of their cost.").Value;
            AllowSuperChars = ModDefs.Bind("SuperSettings", "AllowSuperChars", AllowSuperChars,
                "Allows existence of super characters with extreme performance bonuses.").Value;
            SuperCharMultiplier = ModDefs.Bind("SuperSettings", "SuperCharMultiplier", SuperCharMultiplier,
                "Defines the bonus multiplier for super characters performance.").Value;
            string refCharString = ModDefs.Bind("SuperSettings", "SuperCharacters", string.Join("|", SuperCharacters),
                "Lower-case list of super characters that will receive boost on name basis.").Value;
            if (!string.IsNullOrEmpty(refCharString)) SuperCharacters = refCharString.Split('|');
            UnityEngine.Debug.Log($"SuperSettings => NoSkillTraitCost: {NoSkillTraitCost}");
            UnityEngine.Debug.Log($"SuperSettings => AllowSuperChars: {AllowSuperChars}");
            UnityEngine.Debug.Log($"SuperSettings => SuperCharMultiplier: {SuperCharMultiplier}");
            UnityEngine.Debug.Log($"SuperSettings => SuperCharacters: {string.Join(", ", SuperCharacters)}");
        }

        public const string SYM_DIV = "|";
        public const string SYM_EQU = "=";
        public const string SYM_IGN = "*";
        public const string SYM_INV = "!";
        public const string CMD_SWITCH_SLT = "Switch_Slotted";
        public const string CMD_REC_MISSING = "Recover_Missing";
        public const string CMD_CONDS_SYN = "Sync_Conditions";
        public const string CMD_CONDS_UPD = "Update_Conditions";
        public const string CMD_EFFECT_SLT = "Sync_Slot_Effects";
        public const string CMD_EFFECT_INV = "Sync_Inv_Effects";
        public const string FLAG_INVERSE = "*IsInverse*";
        public const string OPT_DEL = "~";
        public const string OPT_MOD = "*";
        public const string OPT_REM = "-";

        public enum SyncLogs {
            None,
            ModChanges,
            DeepCopy,
            ModdedDump,
            ExtendedDump,
            ContentDump,
            SourceDump,
        }

        public enum ActLogs {
            None,
            Runtime
        }
    }
}