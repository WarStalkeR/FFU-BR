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
using FFU_Beyond_Reach;
using System.IO;

namespace FFU_Beyond_Reach {
    public class FFU_BR_Defs {
        public static readonly string ModName = "BepInEx: Beyond Reach";
        public static readonly string ModVersion = "0.0.1.0";

        private static ConfigFile ModDefs = null;
        public static ConfigEntry<bool> cfgEntryExample;
        public static bool InfoCelsiusKelvin = true;
        public static bool TowBraceAllowsKeep = true;
        public static bool ModifyUpperLimit = false;
        public static float BonusUpperLimit = 1000f;
        public static bool NoSkillTraitCost = false;
        public static bool AllowSuperChars = false;
        public static float SuperCharMultiplier = 10f;
        public static string[] SuperCharacters = new string[] {
            "von neuman",
            "warstalker"
        };

        public static void InitConfig() {
            ModDefs = new ConfigFile(Path.Combine(Paths.ConfigPath, "FFU_Beyond_Reach.cfg"), true);

            // Logging Start
            ModLog.Info($"Loading Mod Configuration...");

            // Load Quality Settings
            InfoCelsiusKelvin = ModDefs.Bind("QualitySettings", "InfoCelsiusKelvin", InfoCelsiusKelvin, 
                "Allows to show temperature in Celsius beside Kelvin value.").Value;
            TowBraceAllowsKeep = ModDefs.Bind("QualitySettings", "TowBraceAllowsKeep", TowBraceAllowsKeep,
                "Allows to use station keeping command, while tow braced to another vessel.").Value; 
            ModLog.Info($"QualitySettings => InfoCelsiusKelvin: {InfoCelsiusKelvin}");
            ModLog.Info($"QualitySettings => TowBraceAllowsKeep: {TowBraceAllowsKeep}");

            // Load Gameplay Settings
            ModifyUpperLimit = ModDefs.Bind("GameplaySettings", "ModifyUpperLimit", ModifyUpperLimit,
                "Allows to change skill and trait modifier upper limit value.").Value;
            BonusUpperLimit = ModDefs.Bind("GameplaySettings", "BonusUpperLimit", BonusUpperLimit,
                "Defines the upper limit for skill and trait modifier. Original value is 10.").Value;
            ModLog.Info($"GameplaySettings => ModifyUpperLimit: {ModifyUpperLimit}");
            ModLog.Info($"GameplaySettings => BonusUpperLimit: {BonusUpperLimit}");

            // Load Cheat Settings
            NoSkillTraitCost = ModDefs.Bind("CheatSettings", "NoSkillTraitCost", NoSkillTraitCost,
                "Makes all trait and/or skill changes free, regardless of their cost.").Value;
            AllowSuperChars = ModDefs.Bind("CheatSettings", "AllowSuperChars", AllowSuperChars,
                "Allows existence of super characters with extreme performance bonuses.").Value;
            SuperCharMultiplier = ModDefs.Bind("CheatSettings", "SuperCharMultiplier", SuperCharMultiplier,
                "Defines the bonus multiplier for super characters performance.").Value;
            ModLog.Info($"CheatSettings => NoSkillTraitCost: {NoSkillTraitCost}");
            ModLog.Info($"CheatSettings => AllowSuperChars: {AllowSuperChars}");
            ModLog.Info($"CheatSettings => SuperCharMultiplier: {SuperCharMultiplier}");

            // Load List of Super Chars
            string refCharString = ModDefs.Bind("CheatSettings", "SuperCharacters", string.Join("|", SuperCharacters),
                "Lower-case list of super characters that will receive boost on name basis.").Value;
            if (!string.IsNullOrEmpty(refCharString)) {
                SuperCharacters = refCharString.Split('|');
            }
            ModLog.Info($"CheatSettings => SuperCharacters: {string.Join(", ", SuperCharacters)}");
        }
    }
}

public static class patch_DataHandler {
    public static void orig_Init() { }
    public static void Init() {
        FFU_BR_Defs.InitConfig();
        orig_Init();
    }
}

/*
namespace PatchTargetNamespace {
    public class TargetClassName {
        // ----------------------------------------------------------- //
        // Class for example purpose only. Doesn't exist in your code. //
        // Only in targeted namespace of DLL you want to modify.       //
        // If target is in global namespace (-) in ILSpy/DnSpy then,   //
        // patch_Target should be outside all namespaces in the code.  //
        // ----------------------------------------------------------- //
        public int a;
        public string b;
        public TargetClassName(int refA, string refB) {
            a = refA;
            b = refB;
        }
    }
    public class patch_TargetClassName : TargetClassName {
        // ------------------------------------------------------ //
        // Example: a way to access private variable or function. //
        // ------------------------------------------------------ //
        [MonoModIgnore] private bool privateVar;
        [MonoModIgnore] private bool privateFunc() { return true; }
        // ------------------------------------------- //
        // Example: a way to modify existing function. //
        // ------------------------------------------- //
        public extern bool orig_TargetFunctionToModify();
        public bool TargetFunctionToModify() {
            // The code you run before original function.
            var moddedResult = orig_TargetFunctionToModify();
            // The code you run after original function.
            return moddedResult;
        }
        // ---------------------------------------------- //
        // Example: a way to overwrite existing function. //
        // ---------------------------------------------- //
        [MonoModReplace]
        public bool FunctionToReplace() {
            // Careful, if it has multiple constructors, you might need to give up on MonoModReplace.
            return true;
        }
        // ---------------------------------------------- //
        // Example: a way to modify existing constructor. //
        // ---------------------------------------------- //
        [MonoModIgnore] public patch_TargetClassName(int refC, string refD) : base(refC, refD) { }
        // You can remove [MonoModIgnore] row, if compiler doesn't throw error without it.
        [MonoModOriginal] public extern void orig_TargetConstructor(int refA, string refB);
        [MonoModConstructor]
        public void TargetConstructor(int refA, string refB) {
            orig_TargetConstructor(refA, refB);
            // Here you can initialize variables you've added.
            // You also can do it before original constructor initialized.
            // Don't forget to use it in exactly same namespace as original.
        }
        // ------------------------------------------------ //
        // Example: a way to overwrite existing enumerator. //
        // ------------------------------------------------ //
        [MonoModEnumReplace]
        public enum patch_TargetEnum_Replace {
            // Don't forget to add original entires used by the code.
            // Or you might end up with whole bag of errors that make no sense.
            origEnumA,
            origEnumB,
            origEnumC,
            modEnumD
        }
        // ------------------------------------------------ //
        // Example: add new entries to existing enumerator. //
        // ------------------------------------------------ //
        public enum patch_TargetEnum_Extend {
            // Ensure that new entries use numbers that way above original.
            // In case, if original enumerator will get more entries.
            modEnumX = 1337,
            modEnumY = 1338,
            modEnumZ = 1339
        }
    }
}
*/