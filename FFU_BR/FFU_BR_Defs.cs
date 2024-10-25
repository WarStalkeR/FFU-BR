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
using System.IO;
using System.Linq;

namespace FFU_Beyond_Reach {
    public class FFU_BR_Defs {
        public static readonly string ModName = "BepInEx: Beyond Reach";
        public static readonly string ModVersion = "0.2.2.0";

        private static ConfigFile ModDefs = null;
        public static SyncLogs SyncLogging = SyncLogs.None;
        public static bool DynamicRandomRange = true;
        public static string[] IgnoredKeys = new string[] {
            "pbaseEyesMissing", "pbaseGlassesMissing", "pbaseHeadMissing", 
            "pbaseLipsMissing", "pbaseNeckMissing", "pbaseNoseMissing", 
            "pbasePupilsMissing", "pbaseScarMissing", "pbaseTeethMissing", 
            "pbaseHairMissing", "pbaseBeardMissing"
        };
        public static int MaxLogTextSize = 16382;
        public static bool ModifyUpperLimit = false;
        public static float BonusUpperLimit = 1000f;
        public static bool AltTempEnabled = true;
        public static string AltTempSymbol = "C";
        public static float AltTempMult = 1.0f;
        public static float AltTempShift = -273.15f;
        public static bool TowBraceAllowsKeep = true;
        public static bool NoSkillTraitCost = false;
        public static bool AllowSuperChars = false;
        public static float SuperCharMultiplier = 10f;
        public static string[] SuperCharacters = new string[] {
            "von neuman", "warstalker"
        };

        public static void InitConfig() {
            ModDefs = new ConfigFile(Path.Combine(Paths.ConfigPath, "FFU_Beyond_Reach.cfg"), true);

            // Logging Start
            ModLog.Info($"Loading Mod Configuration...");

            // Load Configuration Settings
            SyncLogging = ModDefs.Bind("ConfigSettings", "SyncLogging", SyncLogging,
                "Defines what changes will be shown in log during sync loading.").Value;
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

            // Load Gameplay Settings
            ModifyUpperLimit = ModDefs.Bind("GameplaySettings", "ModifyUpperLimit", ModifyUpperLimit,
                "Allows to change skill and trait modifier upper limit value.").Value;
            BonusUpperLimit = ModDefs.Bind("GameplaySettings", "BonusUpperLimit", BonusUpperLimit,
                "Defines the upper limit for skill and trait modifier. Original value is 10.").Value;
            ModLog.Info($"GameplaySettings => ModifyUpperLimit: {ModifyUpperLimit}");
            ModLog.Info($"GameplaySettings => BonusUpperLimit: {BonusUpperLimit}");

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
            ModLog.Info($"QualitySettings => AltTempEnabled: {AltTempEnabled}");
            ModLog.Info($"QualitySettings => AltTempSymbol: {AltTempEnabled}");
            ModLog.Info($"QualitySettings => AltTempMult: {AltTempMult}");
            ModLog.Info($"QualitySettings => AltTempShift: {AltTempShift}");
            ModLog.Info($"QualitySettings => TowBraceAllowsKeep: {TowBraceAllowsKeep}");

            // Load Superiority Settings
            NoSkillTraitCost = ModDefs.Bind("SuperSettings", "NoSkillTraitCost", NoSkillTraitCost,
                "Makes all trait and/or skill changes free, regardless of their cost.").Value;
            AllowSuperChars = ModDefs.Bind("SuperSettings", "AllowSuperChars", AllowSuperChars,
                "Allows existence of super characters with extreme performance bonuses.").Value;
            SuperCharMultiplier = ModDefs.Bind("SuperSettings", "SuperCharMultiplier", SuperCharMultiplier,
                "Defines the bonus multiplier for super characters performance.").Value;
            ModLog.Info($"SuperSettings => NoSkillTraitCost: {NoSkillTraitCost}");
            ModLog.Info($"SuperSettings => AllowSuperChars: {AllowSuperChars}");
            ModLog.Info($"SuperSettings => SuperCharMultiplier: {SuperCharMultiplier}");

            // Load List of Super Chars
            string refCharString = ModDefs.Bind("SuperSettings", "SuperCharacters", string.Join("|", SuperCharacters),
                "Lower-case list of super characters that will receive boost on name basis.").Value;
            if (!string.IsNullOrEmpty(refCharString)) SuperCharacters = refCharString.Split('|');
            ModLog.Info($"SuperSettings => SuperCharacters: {string.Join(", ", SuperCharacters)}");
        }

        public enum SyncLogs {
            None,
            ModChanges,
            DeepCopy,
            ModdedDump,
            ContentDump,
            SourceDump,
        }
    }
}