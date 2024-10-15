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
using System.IO;

namespace FFU_Beyond_Reach {
    public class FFU_BR_Defs {
        public static readonly string ModName = "BepInEx: Beyond Reach";
        public static readonly string ModVersion = "0.0.1.0";

        private static IniFile ModDefs = new IniFile();
        public static bool InfoCelsiusKelvin = true;
        public static bool TowBraceAllowsKeep = true;
        public static bool ModifyUpperLimit = false;
        public static float BonusUpperLimit = 1000f;
        public static bool NoSkillTraitCost = false;
        public static bool AllowSuperChars = false;
        public static float SuperCharMultiplier = 10f;
        public static string[] SuperCharacters = new string[] { };

        public static void InitCfg() {
            bool invalidVersion = false;
            string modConfigPath = Path.Combine(Paths.ConfigPath, "FFU_Beyond_Reach.ini");

            // Check if Config Exists
            if (!File.Exists(modConfigPath)) {
                ModLog.Warning($"Configuration file is missing! Creating new configuration file...");
                SetupCfg(modConfigPath);
            }

            // Load Mod Configuration
            ModDefs.Load(modConfigPath);

            // Validate Config Version
            string refModVersion = ModDefs["ConfigSettings"]["ModVersion"].ToString();
            if (string.IsNullOrEmpty(refModVersion) || refModVersion != ModVersion) {
                ModLog.Warning($"Configuration file setting mismatch! Performing configuration file sync...");
                SetupCfg(modConfigPath, ModDefs);
                ModDefs.Clear();
                ModDefs.Load(modConfigPath);
            }

            // Load Quality Settings
            InfoCelsiusKelvin = ModDefs["QualitySettings"]["InfoCelsiusKelvin"].ToBool(InfoCelsiusKelvin);
            TowBraceAllowsKeep = ModDefs["QualitySettings"]["TowBraceAllowsKeep"].ToBool(TowBraceAllowsKeep);

            // Load Gameplay Settings
            TowBraceAllowsKeep = ModDefs["GameplaySettings"]["ModifyUpperLimit"].ToBool(ModifyUpperLimit);
            BonusUpperLimit = ModDefs["GameplaySettings"]["BonusUpperLimit"].ToFloat(BonusUpperLimit);

            // Load Cheat Settings
            NoSkillTraitCost = ModDefs["CheatSettings"]["NoSkillTraitCost"].ToBool(NoSkillTraitCost);
            AllowSuperChars = ModDefs["CheatSettings"]["AllowSuperChars"].ToBool(AllowSuperChars);
            SuperCharMultiplier = ModDefs["CheatSettings"]["SuperCharMultiplier"].ToFloat(SuperCharMultiplier);

            // Load List of Super Chars
            string refCharString = ModDefs["CheatSettings"]["SuperCharacters"].ToString();
            if (!string.IsNullOrEmpty(refCharString)) {
                SuperCharacters = refCharString.Split('|');
            }
        }

        private static void SetupCfg(string configPath, IniFile refModDefs = null) {
            IniFile newModDefs = new IniFile();
            newModDefs["ConfigSettings"]["ModVersion"] = ModVersion;
            if (refModDefs == null) {
                // Quality Settings
                newModDefs["QualitySettings"]["InfoCelsiusKelvin"] = true;
                newModDefs["QualitySettings"]["TowBraceAllowsKeep"] = true;

                // Gameplay Settings
                newModDefs["GameplaySettings"]["ModifyUpperLimit"] = false;
                newModDefs["GameplaySettings"]["BonusUpperLimit"] = 1000f;

                // Cheat Settings
                newModDefs["CheatSettings"]["NoSkillTraitCost"] = false;
                newModDefs["CheatSettings"]["AllowSuperChars"] = false;
                newModDefs["CheatSettings"]["SuperCharMultiplier"] = 10f;
                newModDefs["CheatSettings"]["SuperCharacters"] = "von neuman|warstalker";
            } else {
                // Quality Settings
                newModDefs["QualitySettings"]["InfoCelsiusKelvin"] = 
                    refModDefs["QualitySettings"]["InfoCelsiusKelvin"].Value != null ?
                    refModDefs["QualitySettings"]["InfoCelsiusKelvin"].ToBool() : true;
                newModDefs["QualitySettings"]["TowBraceAllowsKeep"] =
                    refModDefs["QualitySettings"]["TowBraceAllowsKeep"].Value != null ?
                    refModDefs["QualitySettings"]["TowBraceAllowsKeep"].ToBool() : true;

                // Gameplay Settings
                newModDefs["GameplaySettings"]["ModifyUpperLimit"] =
                    refModDefs["GameplaySettings"]["ModifyUpperLimit"].Value != null ?
                    refModDefs["GameplaySettings"]["ModifyUpperLimit"].ToBool() : false;
                newModDefs["GameplaySettings"]["BonusUpperLimit"] =
                    refModDefs["GameplaySettings"]["BonusUpperLimit"].Value != null ?
                    refModDefs["GameplaySettings"]["BonusUpperLimit"].ToFloat() : 1000f;

                // Cheat Settings
                newModDefs["CheatSettings"]["NoSkillTraitCost"] =
                    refModDefs["CheatSettings"]["NoSkillTraitCost"].Value != null ?
                    refModDefs["CheatSettings"]["NoSkillTraitCost"].ToBool() : false;
                newModDefs["CheatSettings"]["AllowSuperChars"] =
                    refModDefs["CheatSettings"]["AllowSuperChars"].Value != null ?
                    refModDefs["CheatSettings"]["AllowSuperChars"].ToBool() : false;
                newModDefs["CheatSettings"]["SuperCharMultiplier"] =
                    refModDefs["CheatSettings"]["SuperCharMultiplier"].Value != null ?
                    refModDefs["CheatSettings"]["SuperCharMultiplier"].ToFloat() : 10f;
                newModDefs["CheatSettings"]["SuperCharacters"] =
                    refModDefs["CheatSettings"]["SuperCharacters"].Value != null ?
                    refModDefs["CheatSettings"]["SuperCharacters"].ToString() : "von neuman|warstalker";
            }
            newModDefs.Save(configPath);
        }
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