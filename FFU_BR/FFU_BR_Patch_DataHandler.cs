﻿#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using FFU_Beyond_Reach;
using Ostranauts.Core;
using Ostranauts.Ships.Rooms;
using Ostranauts.Trading;
using System.Collections.Generic;
using System.IO;
using System;
using MonoMod;
using UnityEngine;
using LitJson;
using Ostranauts.Tools;
using System.Reflection;
using System.Text;
using Ostranauts.Ships.Commands;

public static class patch_DataHandler {
    public static string strModsPath = string.Empty;
    [MonoModReplace] public static void Init() {
        // Early Access Build Info
        try {
            Debug.Log("#Info# Getting build info.");
            TextAsset textAsset = (TextAsset)Resources.Load("version", typeof(TextAsset));
            DataHandler.strBuild = "Early Access Build: " + textAsset.text;
            Debug.Log(DataHandler.strBuild);
        } catch (Exception ex) {
            Debug.Log("" + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
        }

        // Custom Mod Configuration
        FFU_BR_Defs.InitConfig();
        List<string[]> modQueuedPaths = new List<string[]>();

        // Initializing Data Variables
        DataHandler.strAssetPath = Application.streamingAssetsPath + "/";
        DataHandler.dictImages = new Dictionary<string, Texture2D>();
        DataHandler.dictColors = new Dictionary<string, Color>();
        DataHandler.dictJsonColors = new Dictionary<string, JsonColor>();
        DataHandler.dictLights = new Dictionary<string, JsonLight>();
        DataHandler.dictShips = new Dictionary<string, JsonShip>();
        DataHandler.dictShipImages = new Dictionary<string, Dictionary<string, Texture2D>>();
        DataHandler.dictConds = new Dictionary<string, JsonCond>();
        DataHandler.dictItemDefs = new Dictionary<string, JsonItemDef>();
        DataHandler.dictCTs = new Dictionary<string, CondTrigger>();
        DataHandler.dictCOs = new Dictionary<string, JsonCondOwner>();
        DataHandler.dictDataCoCollections = new Dictionary<string, DataCoCollection>();
        DataHandler.dictCOSaves = new Dictionary<string, JsonCondOwnerSave>();
        DataHandler.dictInteractions = new Dictionary<string, JsonInteraction>();
        DataHandler.dictLoot = new Dictionary<string, Loot>();
        DataHandler.dictProductionMaps = new Dictionary<string, JsonProductionMap>();
        DataHandler.dictMarketConfigs = new Dictionary<string, JsonMarketActorConfig>();
        DataHandler.dictCargoSpecs = new Dictionary<string, JsonCargoSpec>();
        DataHandler.dictGasRespires = new Dictionary<string, JsonGasRespire>();
        DataHandler.dictPowerInfo = new Dictionary<string, JsonPowerInfo>();
        DataHandler.dictGUIPropMaps = new Dictionary<string, Dictionary<string, string>>();
        DataHandler.dictNamesFirst = new Dictionary<string, string>();
        DataHandler.dictNamesLast = new Dictionary<string, string>();
        DataHandler.dictNamesRobots = new Dictionary<string, string>();
        DataHandler.dictNamesFull = new Dictionary<string, string>();
        DataHandler.dictNamesShip = new Dictionary<string, string>();
        DataHandler.dictNamesShipAdjectives = new Dictionary<string, string>();
        DataHandler.dictNamesShipNouns = new Dictionary<string, string>();
        DataHandler.dictManPages = new Dictionary<string, string[]>();
        DataHandler.dictHomeworlds = new Dictionary<string, JsonHomeworld>();
        DataHandler.dictCareers = new Dictionary<string, JsonCareer>();
        DataHandler.dictLifeEvents = new Dictionary<string, JsonLifeEvent>();
        DataHandler.dictPersonSpecs = new Dictionary<string, JsonPersonSpec>();
        DataHandler.dictShipSpecs = new Dictionary<string, JsonShipSpec>();
        DataHandler.dictTraitScores = new Dictionary<string, int[]>();
        DataHandler.dictRoomSpec = new Dictionary<string, RoomSpec>();
        DataHandler.dictStrings = new Dictionary<string, string>();
        DataHandler.dictSlotEffects = new Dictionary<string, JsonSlotEffects>();
        DataHandler.dictSlots = new Dictionary<string, JsonSlot>();
        DataHandler.dictTickers = new Dictionary<string, JsonTicker>();
        DataHandler.dictCondRules = new Dictionary<string, CondRule>();
        DataHandler.dictMaterials = new Dictionary<string, Material>();
        DataHandler.dictAudioEmitters = new Dictionary<string, JsonAudioEmitter>();
        DataHandler.dictCrewSkins = new Dictionary<string, string>();
        DataHandler.dictAds = new Dictionary<string, JsonAd>();
        DataHandler.dictHeadlines = new Dictionary<string, JsonHeadline>();
        DataHandler.dictMusicTags = new Dictionary<string, List<string>>();
        DataHandler.dictMusic = new Dictionary<string, JsonMusic>();
        DataHandler.dictComputerEntries = new Dictionary<string, JsonComputerEntry>();
        DataHandler.dictCOOverlays = new Dictionary<string, JsonCOOverlay>();
        DataHandler.dictDataCOs = new Dictionary<string, DataCO>();
        DataHandler.dictLedgerDefs = new Dictionary<string, JsonLedgerDef>();
        DataHandler.dictPledges = new Dictionary<string, JsonPledge>();
        DataHandler.dictJobitems = new Dictionary<string, JsonJobItems>();
        DataHandler.dictJobs = new Dictionary<string, JsonJob>();
        DataHandler.dictSettings = new Dictionary<string, JsonUserSettings>();
        DataHandler.dictModList = new Dictionary<string, JsonModList>();
        DataHandler.dictModInfos = new Dictionary<string, JsonModInfo>();
        DataHandler.aModPaths = new List<string>();
        DataHandler.dictInstallables2 = new Dictionary<string, JsonInstallable>();
        DataHandler.dictAIPersonalities = new Dictionary<string, JsonAIPersonality>();
        DataHandler.dictTransit = new Dictionary<string, JsonTransit>();
        DataHandler.dictPlotManager = new Dictionary<string, JsonPlotManagerSettings>();
        DataHandler.dictStarSystems = new Dictionary<string, JsonStarSystemSave>();
        DataHandler.dictParallax = new Dictionary<string, JsonParallax>();
        DataHandler.dictContext = new Dictionary<string, JsonContext>();
        DataHandler.dictChargeProfiles = new Dictionary<string, JsonChargeProfile>();
        DataHandler.dictWounds = new Dictionary<string, JsonWound>();
        DataHandler.dictAModes = new Dictionary<string, JsonAttackMode>();
        DataHandler.dictPDAAppIcons = new Dictionary<string, JsonPDAAppIcon>();
        DataHandler.dictZoneTriggers = new Dictionary<string, JsonZoneTrigger>();
        DataHandler.dictTips = new Dictionary<string, JsonTip>();
        DataHandler.dictCrimes = new Dictionary<string, JsonCrime>();
        DataHandler.dictPlots = new Dictionary<string, JsonPlot>();
        DataHandler.dictPlotBeats = new Dictionary<string, JsonPlotBeat>();
        DataHandler.dictRaceTracks = new Dictionary<string, JsonRaceTrack>();
        DataHandler.dictRacingLeagues = new Dictionary<string, JsonRacingLeague>();
        DataHandler.dictSimple = new Dictionary<string, JsonSimple>();
        DataHandler.dictGUIPropMapUnparsed = new Dictionary<string, JsonGUIPropMap>();
        DataHandler.mapCOs = new Dictionary<string, CondOwner>();

        // Initializing Object Reader
        ObjReader.use.scaleFactor = new Vector3(0.0625f, 0.0625f, 0.0625f);
        ObjReader.use.objRotation = new Vector3(90f, 0f, 180f);
        DataHandler._interactionObjectTracker = new InteractionObjectTracker();

        // Loading User Settings
        DataHandler.dictSettings["DefaultUserSettings"] = new JsonUserSettings();
        DataHandler.dictSettings["DefaultUserSettings"].Init();
        if (File.Exists(Application.persistentDataPath + "/settings.json")) {
            DataHandler.JsonToData(Application.persistentDataPath + "/settings.json", DataHandler.dictSettings);
        } else {
            Debug.LogWarning("WARNING: settings.json not found. Resorting to default values.");
            DataHandler.dictSettings["UserSettings"] = new JsonUserSettings();
        }
        if (!DataHandler.dictSettings.ContainsKey("UserSettings") || DataHandler.dictSettings["UserSettings"] == null) {
            Debug.LogError("ERROR: Malformed settings.json. Resorting to default values.");
            DataHandler.dictSettings["UserSettings"] = new JsonUserSettings();
        }
        DataHandler.dictSettings["DefaultUserSettings"].CopyTo(DataHandler.GetUserSettings());
        DataHandler.dictSettings.Remove("DefaultUserSettings");
        DataHandler.SaveUserSettings();

        // Mod List Initialization
        bool isModded = false;
        DataHandler.strModFolder = DataHandler.dictSettings["UserSettings"].strPathMods;
        if (DataHandler.strModFolder == null || DataHandler.strModFolder == string.Empty) {
            DataHandler.strModFolder = Path.Combine(Application.dataPath, "Mods/");
        }
        strModsPath = DataHandler.strModFolder.Replace("loading_order.json", string.Empty);
        string directoryName = Path.GetDirectoryName(DataHandler.strModFolder);
        directoryName = Path.Combine(directoryName, "loading_order.json");

        // Creating Mod Placeholder
        JsonModInfo coreModInfo = new JsonModInfo();
        coreModInfo.strName = "Core";
        DataHandler.dictModInfos["core"] = coreModInfo;

        // Mod List Loading Routine
        bool isConsoleExists = ConsoleToGUI.instance != null;
        if (isConsoleExists) {
            ConsoleToGUI.instance.LogInfo("Attempting to load " + directoryName + "...");
        }

        // Proceed With Mod List Loading
        if (File.Exists(directoryName)) {
            if (isConsoleExists) {
                ConsoleToGUI.instance.LogInfo("loading_order.json found. Beginning mod load.");
            }
            DataHandler.JsonToData(directoryName, DataHandler.dictModList);
            JsonModList newModList = null;
            if (DataHandler.dictModList.TryGetValue("Mod Loading Order", out newModList)) {
                if (newModList.aIgnorePatterns != null) {
                    for (int i = 0; i < newModList.aIgnorePatterns.Length; i++) {
                        newModList.aIgnorePatterns[i] = DataHandler.PathSanitize(newModList.aIgnorePatterns[i]);
                    }
                }
                string[] aLoadOrder = newModList.aLoadOrder;

                // Go Through Each Mod Entry
                foreach (string aLoadEntry in aLoadOrder) {
                    isModded = true;

                    // Handle Dedicated/Invalid Settings
                    if (aLoadEntry == "core") {
                        modQueuedPaths.Add(new string[] { DataHandler.strAssetPath, aLoadEntry });
                        continue;
                    }
                    if (aLoadEntry == null || aLoadEntry == string.Empty) {
                        Debug.LogError("ERROR: Invalid mod folder specified: " + aLoadEntry + "; Skipping...");
                        continue;
                    }

                    // Prepare Mod Information
                    string aLoadPath = aLoadEntry.TrimStart(Path.DirectorySeparatorChar);
                    aLoadPath = aLoadEntry.TrimStart(Path.AltDirectorySeparatorChar);
                    aLoadPath += "/";
                    string modFolderPath = Path.GetDirectoryName(DataHandler.strModFolder);
                    modFolderPath = Path.Combine(modFolderPath, aLoadPath);
                    Dictionary<string, JsonModInfo> modInfoJson = new Dictionary<string, JsonModInfo>();
                    string modInfoPath = Path.Combine(modFolderPath, "mod_info.json");

                    // Start Mod Loading Routine
                    if (File.Exists(modInfoPath)) {
                        DataHandler.JsonToData(modInfoPath, modInfoJson);
                    }
                    if (modInfoJson.Count < 1) {
                        JsonModInfo altModInfo = new JsonModInfo();
                        altModInfo.strName = aLoadEntry;
                        modInfoJson[altModInfo.strName] = altModInfo;
                        Debug.LogWarning("WARNING: Missing mod_info.json in folder: " + aLoadEntry + "; Using default name: " + altModInfo.strName);
                    }
                    using (Dictionary<string, JsonModInfo>.ValueCollection.Enumerator modEnum = modInfoJson.Values.GetEnumerator()) {
                        if (modEnum.MoveNext()) {
                            JsonModInfo modCurrent = modEnum.Current;
                            DataHandler.dictModInfos[aLoadEntry] = modCurrent;
                            if (isConsoleExists) {
                                ConsoleToGUI.instance.LogInfo("Loading mod: " + DataHandler.dictModInfos[aLoadEntry].strName + " from directory: " + aLoadEntry);
                            }
                        }
                    }

                    // Queue Mod's Path For Loading
                    modQueuedPaths.Add(new string[] { modFolderPath, aLoadEntry });
                }

                // Sync Load All Mod Data
                SyncLoadMods(modQueuedPaths, newModList.aIgnorePatterns);
            }
        }

        // Default Non-Modded Loading
        if (!isModded) {
            if (isConsoleExists) {
                ConsoleToGUI.instance.LogInfo("No loading_order.json found. Beginning default game data load from " + DataHandler.strAssetPath);
            }
            JsonModList cleanModList = new JsonModList();
            cleanModList.strName = "Default";
            cleanModList.aLoadOrder = new string[1] { "core" };
            cleanModList.aIgnorePatterns = new string[0];
            DataHandler.dictModList["Mod Loading Order"] = cleanModList;
            DataHandler.LoadMod(DataHandler.strAssetPath, cleanModList.aIgnorePatterns, coreModInfo);
        }

        // Load Various Interactions
        DataHandler.dictSocialStats = new Dictionary<string, SocialStats>();
        foreach (JsonInteraction refInteraction in DataHandler.dictInteractions.Values) {
            if (refInteraction.bSocial) {
                DataHandler.dictSocialStats[refInteraction.strName] = new SocialStats(refInteraction.strName);
            }
        }

        // Load Other Data/Settings
        JsonInteraction skillInteraction = null;
        JsonInteraction careerInteraction = null;
        CondTrigger skillTrigger = null;
        DataHandler.dictInteractions.TryGetValue("SOCAskCareer", out careerInteraction);
        DataHandler.dictInteractions.TryGetValue("SOCTellSkill_TEMP", out skillInteraction);
        DataHandler.dictCTs.TryGetValue("TIsSOCTalkSkillTEMPUs", out skillTrigger);
        if (skillInteraction != null && skillTrigger != null && careerInteraction != null) {
            List<string> careerList = new List<string>();
            List<string> lootNames = DataHandler.GetLoot("CONDSocialGUIFilterSkills").GetLootNames();
            foreach (string lootName in lootNames) {
                JsonInteraction refSkillInteraction = skillInteraction.Clone();
                CondTrigger refSkillTrigger = skillTrigger.Clone();
                Condition refLootCondition = DataHandler.GetCond(lootName);
                refSkillTrigger.strName = "TIsSOCTalk" + lootName + "Us";
                refSkillTrigger.aReqs = new string[1] { lootName };
                refSkillInteraction.strName = "SOCTell" + lootName;
                refSkillInteraction.strTitle = refLootCondition.strNameFriendly;
                refSkillInteraction.strDesc = refLootCondition.strDesc;
                refSkillInteraction.CTTestUs = refSkillTrigger.strName;
                DataHandler.dictInteractions[refSkillInteraction.strName] = refSkillInteraction;
                DataHandler.dictCTs[refSkillTrigger.strName] = refSkillTrigger;
                careerList.Add(refSkillInteraction.strName);
            }
            string[] aInverse = careerInteraction.aInverse;
            foreach (string item in aInverse) {
                careerList.Add(item);
            }
            careerInteraction.aInverse = careerList.ToArray();
        }

        // Finish Loading Process
        DataHandler.bLoaded = true;
    }

    private static void SyncLoadMods(List<string[]> refQueuedPaths, string[] aIgnorePatterns) {
        List<string> validModInfos = new List<string>();
        List<string> validModPaths = new List<string>();
        List<string> validDataPaths = new List<string>();

        // Parse Validate Data Paths
        foreach (string[] queuedPath in refQueuedPaths) {
            if (queuedPath.Length == 2) {
                if (Directory.Exists(queuedPath[0] + "data/")) {
                    validModInfos.Add(queuedPath[1]);
                    validModPaths.Add(queuedPath[0]);
                    validDataPaths.Add(queuedPath[0] + "data/");
                    Debug.Log($"Data Mod Queued: {queuedPath[1]} => {queuedPath[0]}");
                } else if (Directory.Exists(queuedPath[0] + "images/") ||
                      Directory.Exists(queuedPath[0] + "audio/") ||
                      Directory.Exists(queuedPath[0] + "mesh/")) {
                    validModInfos.Add(queuedPath[1]);
                    validModPaths.Add(queuedPath[0]);
                    Debug.Log($"Asset Mod Queued: {queuedPath[1]} => {queuedPath[0]}");
                } else Debug.LogWarning($"WARNING! Mod folder '{queuedPath[0]}' has no data or assets! Ignoring.");
            } else Debug.LogError($"ERROR! Received 'refQueuedPaths' data set is invalid! Ignoring.");
        }

        // List All Valid Data Paths
        bool isConsoleExists = ConsoleToGUI.instance != null;
        int numConsoleErrors = 0;
        if (isConsoleExists) {
            numConsoleErrors = ConsoleToGUI.instance.ErrorCount;
            ConsoleToGUI.instance.LogInfo("Begin loading data from these paths:");
            foreach (string dataPath in validDataPaths) ConsoleToGUI.instance.LogInfo(dataPath);
        }

        // Sync Load Mods Data
        foreach (string modPath in validModPaths) DataHandler.aModPaths.Insert(0, modPath);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "colors/", DataHandler.dictJsonColors, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "lights/", DataHandler.dictLights, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "gasrespires/", DataHandler.dictGasRespires, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "powerinfos/", DataHandler.dictPowerInfo, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "guipropmaps/", DataHandler.dictGUIPropMapUnparsed, aIgnorePatterns);
        DataHandler.ParseGUIPropMaps();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "conditions/", DataHandler.dictConds, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "conditions_simple/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseConditionsSimple();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "items/", DataHandler.dictItemDefs, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "condtrigs/", DataHandler.dictCTs, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "interactions/", DataHandler.dictInteractions, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "condowners/", DataHandler.dictCOs, aIgnorePatterns);
        Dictionary<string, JsonRoomSpec> listRooms = new Dictionary<string, JsonRoomSpec>();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "rooms/", listRooms, aIgnorePatterns);
        DataHandler.ParseRoomSpecs(listRooms);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "ships/", DataHandler.dictShips, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "loot/", DataHandler.dictLoot, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "market/Production/", DataHandler.dictProductionMaps, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "market/", DataHandler.dictMarketConfigs, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "market/CargoSpecs/", DataHandler.dictCargoSpecs, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_last/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesLast);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_robots/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesRobots);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_first/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesFirst);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_full/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesFull);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "manpages/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseManPages();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "homeworlds/", DataHandler.dictHomeworlds, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "careers/", DataHandler.dictCareers, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "lifeevents/", DataHandler.dictLifeEvents, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "personspecs/", DataHandler.dictPersonSpecs, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "shipspecs/", DataHandler.dictShipSpecs, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "traitscores/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseTraitScores();
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "strings/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictStrings);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "slot_effects/", DataHandler.dictSlotEffects, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "slots/", DataHandler.dictSlots, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "tickers/", DataHandler.dictTickers, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "condrules/", DataHandler.dictCondRules, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "audioemitters/", DataHandler.dictAudioEmitters, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "crewskins/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictCrewSkins);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "ads/", DataHandler.dictAds, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "headlines/", DataHandler.dictHeadlines, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "music/", DataHandler.dictMusic, aIgnorePatterns);
        DataHandler.ParseMusic();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "cooverlays/", DataHandler.dictCOOverlays, aIgnorePatterns);
        Dictionary<string, JsonDCOCollection> listCollections = new Dictionary<string, JsonDCOCollection>();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "market/CoCollections/", listCollections, aIgnorePatterns);
        DataHandler.BuildMarketDCOCollection(listCollections);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "ledgerdefs/", DataHandler.dictLedgerDefs, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "pledges/", DataHandler.dictPledges, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "jobitems/", DataHandler.dictJobitems, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "jobs/", DataHandler.dictJobs, aIgnorePatterns);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_ship/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesShip);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_ship_adjectives/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesShipAdjectives);
        DataHandler.dictSimple.Clear();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "names_ship_nouns/", DataHandler.dictSimple, aIgnorePatterns);
        DataHandler.ParseSimpleIntoStringDict(DataHandler.dictSimple, DataHandler.dictNamesShipNouns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "ai_training/", DataHandler.dictAIPersonalities, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "transit/", DataHandler.dictTransit, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "plot_manager/", DataHandler.dictPlotManager, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "star_systems/", DataHandler.dictStarSystems, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "parallax/", DataHandler.dictParallax, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "context/", DataHandler.dictContext, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "chargeprofiles/", DataHandler.dictChargeProfiles, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "wounds/", DataHandler.dictWounds, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "attackmodes/", DataHandler.dictAModes, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "pda_apps/", DataHandler.dictPDAAppIcons, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "zone_triggers/", DataHandler.dictZoneTriggers, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "tips/", DataHandler.dictTips, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "crime/", DataHandler.dictCrimes, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "plots/", DataHandler.dictPlots, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "plot_beats/", DataHandler.dictPlotBeats, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "racing/tracks/", DataHandler.dictRaceTracks, aIgnorePatterns);
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "racing/leagues/", DataHandler.dictRacingLeagues, aIgnorePatterns);
        Dictionary<string, JsonInstallable> listInstallables = new Dictionary<string, JsonInstallable>();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "installables/", listInstallables, aIgnorePatterns);
        foreach (KeyValuePair<string, JsonInstallable> refInstallable in listInstallables) Installables.Create(refInstallable.Value);
        listInstallables.Clear();
        listInstallables = null;
        Dictionary<string, JsonInteractionOverride> listInteractions = new Dictionary<string, JsonInteractionOverride>();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "interaction_overrides/", listInteractions, aIgnorePatterns);
        foreach (KeyValuePair<string, JsonInteractionOverride> refInteraction in listInteractions) refInteraction.Value.Generate();
        listInteractions.Clear();
        listInteractions = null;
        Dictionary<string, JsonPlotBeatOverride> listPlotBeats = new Dictionary<string, JsonPlotBeatOverride>();
        foreach (string dataPath in validDataPaths) SyncLoadJSONs(dataPath, "plot_beat_overrides/", listPlotBeats, aIgnorePatterns);
        foreach (KeyValuePair<string, JsonPlotBeatOverride> refPlotBeat in listPlotBeats) refPlotBeat.Value.Generate();
        listPlotBeats.Clear();
        listPlotBeats = null;
        foreach (CondTrigger refTrigger in DataHandler.dictCTs.Values) refTrigger.PostInit();

        // Finalize Mod Load Status
        foreach (string modName in validModInfos) {
            JsonModInfo refModInfo = DataHandler.dictModInfos[modName];
            if (refModInfo.Status == GUIModRow.ModStatus.Missing) {
                refModInfo.Status = GUIModRow.ModStatus.Missing;
            } else if (numConsoleErrors < ConsoleToGUI.instance.ErrorCount) {
                refModInfo.Status = GUIModRow.ModStatus.Error;
            } else {
                refModInfo.Status = GUIModRow.ModStatus.Loaded;
            }
        }
    }

    private static void SyncLoadJSONs<TJson>(string strFolderPath, string subFolder, Dictionary<string, TJson> dataDict, string[] aIgnorePatterns) {
        // Prepare Reference Data
        string modName = strFolderPath.Contains("StreamingAssets") ? null :
            strFolderPath.Remove(strFolderPath.Length - 6).Replace("\\", "").Replace("/", "")
            .Replace(strModsPath.Replace("\\", "").Replace("/", ""), "");
        string fileType = subFolder.Remove(subFolder.Length - 1);

        // Per Mod Data Removal
        patch_JsonModInfo refModInfo = modName != null ? DataHandler.dictModInfos[modName] as patch_JsonModInfo : null;
        if (refModInfo != null && refModInfo.removeIds != null && refModInfo.removeIds.ContainsKey(fileType)) {
            foreach (string removeId in refModInfo.removeIds[fileType]) {
                bool wasRemoved = dataDict.Remove(removeId);
                if (wasRemoved) Debug.Log($"Removed existing '{fileType}' entry: {removeId}");
            }
        }

        // Ignore Missing Folder
        string strSubFolderPath = strFolderPath + subFolder;
        if (!Directory.Exists(strSubFolderPath)) return;

        // Parse Folder Contents
        string[] subFiles = Directory.GetFiles(strSubFolderPath, "*.json", SearchOption.AllDirectories);
        foreach (string subFile in subFiles) {
            string filePath = DataHandler.PathSanitize(subFile);

            // Check Ignored Patterns
            bool isIgnoredPath = false;
            if (aIgnorePatterns != null) {
                foreach (string ignorePattern in aIgnorePatterns) {
                    if (filePath.IndexOf(ignorePattern) >= 0) {
                        isIgnoredPath = true;
                        break;
                    }
                }
            }

            // Data Loading Subroutine
            if (isIgnoredPath) {
                Debug.LogWarning("Ignore Pattern match: " + filePath + "; Skipping...");
            } else {
                SyncToData(filePath, modName != null, dataDict);
            }
        }
    }

    public static void SyncToData<TJson>(string strFile, bool isMod, Dictionary<string, TJson> dataDict) {
        Debug.Log("#Info# Loading JSON: " + strFile);
        string rawDump = string.Empty;
        try {
            // Raw JSON to Data Array
            string dataFile = File.ReadAllText(strFile, Encoding.UTF8);
            rawDump += "Converting JSON into Array...\n";
            string[] rawData = isMod ? dataFile.Replace("\n", "").Replace("\r", "").Replace("\t", "")
                .Replace(" ", "").Split(new string[] { "},{" }, StringSplitOptions.None) : null;
            TJson[] fileData = JsonMapper.ToObject<TJson[]>(dataFile);

            // Parsing Each Data Block
            for (int i = 0; i < fileData.Length; i++) {
                TJson dataBlock = fileData[i];
                string rawBlock = isMod ? rawData[i] : null;
                rawDump += "Getting key: ";
                string dataKey = null;

                // Validating Data Block
                PropertyInfo dataProperty = dataBlock.GetType()?.GetProperty("strName");
                if (dataProperty == null) {
                    JsonLogger.ReportProblem("strName is missing", ReportTypes.FailingString);
                    continue;
                }

                // Data Allocation Subroutine
                object dataValue = dataProperty.GetValue(dataBlock, null);
                dataKey = dataValue.ToString();
                rawDump = rawDump + dataKey + "\n";
                if (dataDict.ContainsKey(dataKey)) {
                    // Modify Existing Data
                    Type newDataType = dataBlock.GetType();
                    Type currDataType = dataDict[dataKey].GetType();

                    // Iterate Over Properties
                    foreach (PropertyInfo currProperty in currDataType.GetProperties()) {
                        // Ignore Identifier Property
                        if (currProperty.Name == "strName") continue;

                        // New Data Property Validation
                        PropertyInfo newProperty = newDataType.GetProperty(currProperty.Name);
                        if (newProperty != null) {
                            object newValue = newProperty.GetValue(dataBlock, null);
                            object currValue = currProperty.GetValue(dataDict[dataKey], null);
                            if (isMod && rawBlock.IndexOf(currProperty.Name) >= 0) {
                                Debug.Log($"#Info# #Block# {dataKey}, #Property# {currProperty.Name}: {currValue} => {newValue}");
                                currProperty.SetValue(dataDict[dataKey], newValue, null);
                            }
                        }
                    }

                    // Iterate Over Fields
                    BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.Instance;
                    foreach (FieldInfo currField in currDataType.GetFields(fieldFlags)) {
                        // Ignore Identifier Field
                        if (currField.Name == "strName") continue;

                        // New Data Field Validation
                        FieldInfo newField = newDataType.GetField(currField.Name, fieldFlags);
                        if (newField != null) {
                            object newValue = newField.GetValue(dataBlock);
                            object currValue = currField.GetValue(dataDict[dataKey]);
                            if (isMod && rawBlock.IndexOf(currField.Name) >= 0) {
                                Debug.Log($"#Info# #Block# {dataKey}, #Field# {currField.Name}: {currValue} => {newValue}");
                                currField.SetValue(dataDict[dataKey], newValue);
                            }
                        }
                    }
                } else {
                    // Add New Data Entry
                    dataDict.Add(dataKey, dataBlock);
                }
            }

            // Resetting Data Variables
            fileData = null;
            dataFile = null;
        } catch (Exception ex) {
            JsonLogger.ReportProblem(strFile, ReportTypes.SourceInfo);
            if (rawDump.Length > 1000) {
                rawDump = rawDump.Substring(rawDump.Length - 1000);
            }
            Debug.LogError(rawDump + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
        }

        // Specific File Dump
        if (strFile.IndexOf("osSGv1") >= 0) {
            Debug.Log(rawDump);
        }
    }
}

// Reference ILSpy Output
/*
public static void Init()
{
    string empty = string.Empty;
    try
    {
        Debug.Log("#Info# Getting build info.");
        TextAsset textAsset = (TextAsset)Resources.Load("version", typeof(TextAsset));
        strBuild = "Early Access Build: " + textAsset.text;
        Debug.Log(strBuild);
    }
    catch (Exception ex)
    {
        Debug.Log(empty + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
    }
    strAssetPath = Application.streamingAssetsPath + "/";
    dictImages = new Dictionary<string, Texture2D>();
    dictColors = new Dictionary<string, Color>();
    dictJsonColors = new Dictionary<string, JsonColor>();
    dictLights = new Dictionary<string, JsonLight>();
    dictShips = new Dictionary<string, JsonShip>();
    dictShipImages = new Dictionary<string, Dictionary<string, Texture2D>>();
    dictConds = new Dictionary<string, JsonCond>();
    dictItemDefs = new Dictionary<string, JsonItemDef>();
    dictCTs = new Dictionary<string, CondTrigger>();
    dictCOs = new Dictionary<string, JsonCondOwner>();
    dictDataCoCollections = new Dictionary<string, DataCoCollection>();
    dictCOSaves = new Dictionary<string, JsonCondOwnerSave>();
    dictInteractions = new Dictionary<string, JsonInteraction>();
    dictLoot = new Dictionary<string, Loot>();
    dictProductionMaps = new Dictionary<string, JsonProductionMap>();
    dictMarketConfigs = new Dictionary<string, JsonMarketActorConfig>();
    dictCargoSpecs = new Dictionary<string, JsonCargoSpec>();
    dictGasRespires = new Dictionary<string, JsonGasRespire>();
    dictPowerInfo = new Dictionary<string, JsonPowerInfo>();
    dictGUIPropMaps = new Dictionary<string, Dictionary<string, string>>();
    dictNamesFirst = new Dictionary<string, string>();
    dictNamesLast = new Dictionary<string, string>();
    dictNamesRobots = new Dictionary<string, string>();
    dictNamesFull = new Dictionary<string, string>();
    dictNamesShip = new Dictionary<string, string>();
    dictNamesShipAdjectives = new Dictionary<string, string>();
    dictNamesShipNouns = new Dictionary<string, string>();
    dictManPages = new Dictionary<string, string[]>();
    dictHomeworlds = new Dictionary<string, JsonHomeworld>();
    dictCareers = new Dictionary<string, JsonCareer>();
    dictLifeEvents = new Dictionary<string, JsonLifeEvent>();
    dictPersonSpecs = new Dictionary<string, JsonPersonSpec>();
    dictShipSpecs = new Dictionary<string, JsonShipSpec>();
    dictTraitScores = new Dictionary<string, int[]>();
    dictRoomSpec = new Dictionary<string, RoomSpec>();
    dictStrings = new Dictionary<string, string>();
    dictSlotEffects = new Dictionary<string, JsonSlotEffects>();
    dictSlots = new Dictionary<string, JsonSlot>();
    dictTickers = new Dictionary<string, JsonTicker>();
    dictCondRules = new Dictionary<string, CondRule>();
    dictMaterials = new Dictionary<string, Material>();
    dictAudioEmitters = new Dictionary<string, JsonAudioEmitter>();
    dictCrewSkins = new Dictionary<string, string>();
    dictAds = new Dictionary<string, JsonAd>();
    dictHeadlines = new Dictionary<string, JsonHeadline>();
    dictMusicTags = new Dictionary<string, List<string>>();
    dictMusic = new Dictionary<string, JsonMusic>();
    dictComputerEntries = new Dictionary<string, JsonComputerEntry>();
    dictCOOverlays = new Dictionary<string, JsonCOOverlay>();
    dictDataCOs = new Dictionary<string, DataCO>();
    dictLedgerDefs = new Dictionary<string, JsonLedgerDef>();
    dictPledges = new Dictionary<string, JsonPledge>();
    dictJobitems = new Dictionary<string, JsonJobItems>();
    dictJobs = new Dictionary<string, JsonJob>();
    dictSettings = new Dictionary<string, JsonUserSettings>();
    dictModList = new Dictionary<string, JsonModList>();
    dictModInfos = new Dictionary<string, JsonModInfo>();
    aModPaths = new List<string>();
    dictInstallables2 = new Dictionary<string, JsonInstallable>();
    dictAIPersonalities = new Dictionary<string, JsonAIPersonality>();
    dictTransit = new Dictionary<string, JsonTransit>();
    dictPlotManager = new Dictionary<string, JsonPlotManagerSettings>();
    dictStarSystems = new Dictionary<string, JsonStarSystemSave>();
    dictParallax = new Dictionary<string, JsonParallax>();
    dictContext = new Dictionary<string, JsonContext>();
    dictChargeProfiles = new Dictionary<string, JsonChargeProfile>();
    dictWounds = new Dictionary<string, JsonWound>();
    dictAModes = new Dictionary<string, JsonAttackMode>();
    dictPDAAppIcons = new Dictionary<string, JsonPDAAppIcon>();
    dictZoneTriggers = new Dictionary<string, JsonZoneTrigger>();
    dictTips = new Dictionary<string, JsonTip>();
    dictCrimes = new Dictionary<string, JsonCrime>();
    dictPlots = new Dictionary<string, JsonPlot>();
    dictPlotBeats = new Dictionary<string, JsonPlotBeat>();
    dictRaceTracks = new Dictionary<string, JsonRaceTrack>();
    dictRacingLeagues = new Dictionary<string, JsonRacingLeague>();
    dictSimple = new Dictionary<string, JsonSimple>();
    dictGUIPropMapUnparsed = new Dictionary<string, JsonGUIPropMap>();
    mapCOs = new Dictionary<string, CondOwner>();
    ObjReader.use.scaleFactor = new Vector3(0.0625f, 0.0625f, 0.0625f);
    ObjReader.use.objRotation = new Vector3(90f, 0f, 180f);
    _interactionObjectTracker = new InteractionObjectTracker();
    dictSettings["DefaultUserSettings"] = new JsonUserSettings();
    dictSettings["DefaultUserSettings"].Init();
    if (File.Exists(Application.persistentDataPath + "/settings.json"))
    {
        JsonToData(Application.persistentDataPath + "/settings.json", dictSettings);
    }
    else
    {
        Debug.LogWarning("WARNING: settings.json not found. Resorting to default values.");
        dictSettings["UserSettings"] = new JsonUserSettings();
    }
    if (!dictSettings.ContainsKey("UserSettings") || dictSettings["UserSettings"] == null)
    {
        Debug.LogError("ERROR: Malformed settings.json. Resorting to default values.");
        dictSettings["UserSettings"] = new JsonUserSettings();
    }
    dictSettings["DefaultUserSettings"].CopyTo(GetUserSettings());
    dictSettings.Remove("DefaultUserSettings");
    SaveUserSettings();
    bool flag = false;
    strModFolder = dictSettings["UserSettings"].strPathMods;
    if (strModFolder == null || strModFolder == string.Empty)
    {
        strModFolder = Path.Combine(Application.dataPath, "Mods/");
    }
    string directoryName = Path.GetDirectoryName(strModFolder);
    directoryName = Path.Combine(directoryName, "loading_order.json");
    JsonModInfo jsonModInfo = new JsonModInfo();
    jsonModInfo.strName = "Core";
    dictModInfos["core"] = jsonModInfo;
    bool flag2 = ConsoleToGUI.instance != null;
    if (flag2)
    {
        ConsoleToGUI.instance.LogInfo("Attempting to load " + directoryName + "...");
    }
    if (File.Exists(directoryName))
    {
        if (flag2)
        {
            ConsoleToGUI.instance.LogInfo("loading_order.json found. Beginning mod load.");
        }
        JsonToData(directoryName, dictModList);
        JsonModList value = null;
        if (dictModList.TryGetValue("Mod Loading Order", out value))
        {
            if (value.aIgnorePatterns != null)
            {
                for (int i = 0; i < value.aIgnorePatterns.Length; i++)
                {
                    value.aIgnorePatterns[i] = PathSanitize(value.aIgnorePatterns[i]);
                }
            }
            string[] aLoadOrder = value.aLoadOrder;
            foreach (string text in aLoadOrder)
            {
                flag = true;
                if (text == "core")
                {
                    LoadMod(strAssetPath, value.aIgnorePatterns, jsonModInfo);
                    continue;
                }
                if (text == null || text == string.Empty)
                {
                    Debug.LogError("ERROR: Invalid mod folder specified: " + text + "; Skipping...");
                    continue;
                }
                string text2 = text.TrimStart(Path.DirectorySeparatorChar);
                text2 = text.TrimStart(Path.AltDirectorySeparatorChar);
                text2 += "/";
                string directoryName2 = Path.GetDirectoryName(strModFolder);
                directoryName2 = Path.Combine(directoryName2, text2);
                Dictionary<string, JsonModInfo> dictionary = new Dictionary<string, JsonModInfo>();
                string text3 = Path.Combine(directoryName2, "mod_info.json");
                if (File.Exists(text3))
                {
                    JsonToData(text3, dictionary);
                }
                if (dictionary.Count < 1)
                {
                    JsonModInfo jsonModInfo2 = new JsonModInfo();
                    jsonModInfo2.strName = text;
                    dictionary[jsonModInfo2.strName] = jsonModInfo2;
                    Debug.LogWarning("WARNING: Missing mod_info.json in folder: " + text + "; Using default name: " + jsonModInfo2.strName);
                }
                using (Dictionary<string, JsonModInfo>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        JsonModInfo current = enumerator.Current;
                        dictModInfos[text] = current;
                        if (flag2)
                        {
                            ConsoleToGUI.instance.LogInfo("Loading mod: " + dictModInfos[text].strName + " from directory: " + text);
                        }
                    }
                }
                LoadMod(directoryName2, value.aIgnorePatterns, dictModInfos[text]);
            }
        }
    }
    if (!flag)
    {
        if (flag2)
        {
            ConsoleToGUI.instance.LogInfo("No loading_order.json found. Beginning default game data load from " + strAssetPath);
        }
        JsonModList jsonModList = new JsonModList();
        jsonModList.strName = "Default";
        jsonModList.aLoadOrder = new string[1] { "core" };
        jsonModList.aIgnorePatterns = new string[0];
        dictModList["Mod Loading Order"] = jsonModList;
        LoadMod(strAssetPath, jsonModList.aIgnorePatterns, jsonModInfo);
    }
    dictSocialStats = new Dictionary<string, SocialStats>();
    foreach (JsonInteraction value5 in dictInteractions.Values)
    {
        if (value5.bSocial)
        {
            dictSocialStats[value5.strName] = new SocialStats(value5.strName);
        }
    }
    JsonInteraction value2 = null;
    JsonInteraction value3 = null;
    CondTrigger value4 = null;
    dictInteractions.TryGetValue("SOCAskCareer", out value3);
    dictInteractions.TryGetValue("SOCTellSkill_TEMP", out value2);
    dictCTs.TryGetValue("TIsSOCTalkSkillTEMPUs", out value4);
    if (value2 != null && value4 != null && value3 != null)
    {
        List<string> list = new List<string>();
        List<string> lootNames = GetLoot("CONDSocialGUIFilterSkills").GetLootNames();
        foreach (string item2 in lootNames)
        {
            JsonInteraction jsonInteraction = value2.Clone();
            CondTrigger condTrigger = value4.Clone();
            Condition cond = GetCond(item2);
            condTrigger.strName = "TIsSOCTalk" + item2 + "Us";
            condTrigger.aReqs = new string[1] { item2 };
            jsonInteraction.strName = "SOCTell" + item2;
            jsonInteraction.strTitle = cond.strNameFriendly;
            jsonInteraction.strDesc = cond.strDesc;
            jsonInteraction.CTTestUs = condTrigger.strName;
            dictInteractions[jsonInteraction.strName] = jsonInteraction;
            dictCTs[condTrigger.strName] = condTrigger;
            list.Add(jsonInteraction.strName);
        }
        string[] aInverse = value3.aInverse;
        foreach (string item in aInverse)
        {
            list.Add(item);
        }
        value3.aInverse = list.ToArray();
    }
    bLoaded = true;
}

private static void LoadMod(string strFolderPath, string[] aIgnorePatterns, JsonModInfo jmi)
{
    if (!Directory.Exists(strFolderPath + "data/"))
    {
        Debug.LogError("ERROR: Mod folder not found: " + strFolderPath + "data/");
        jmi.Status = GUIModRow.ModStatus.Missing;
        return;
    }
    bool flag = ConsoleToGUI.instance != null;
    int num = 0;
    if (flag)
    {
        num = ConsoleToGUI.instance.ErrorCount;
        ConsoleToGUI.instance.LogInfo("Begin loading data from: " + strFolderPath);
    }
    aModPaths.Insert(0, strFolderPath);
    strFolderPath += "data/";
    LoadModJsons(strFolderPath + "colors/", dictJsonColors, aIgnorePatterns);
    LoadModJsons(strFolderPath + "lights/", dictLights, aIgnorePatterns);
    LoadModJsons(strFolderPath + "gasrespires/", dictGasRespires, aIgnorePatterns);
    LoadModJsons(strFolderPath + "powerinfos/", dictPowerInfo, aIgnorePatterns);
    LoadModJsons(strFolderPath + "guipropmaps/", dictGUIPropMapUnparsed, aIgnorePatterns);
    ParseGUIPropMaps();
    LoadModJsons(strFolderPath + "conditions/", dictConds, aIgnorePatterns);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "conditions_simple/", dictSimple, aIgnorePatterns);
    ParseConditionsSimple();
    LoadModJsons(strFolderPath + "items/", dictItemDefs, aIgnorePatterns);
    LoadModJsons(strFolderPath + "condtrigs/", dictCTs, aIgnorePatterns);
    LoadModJsons(strFolderPath + "interactions/", dictInteractions, aIgnorePatterns);
    LoadModJsons(strFolderPath + "condowners/", dictCOs, aIgnorePatterns);
    Dictionary<string, JsonRoomSpec> dictionary = new Dictionary<string, JsonRoomSpec>();
    LoadModJsons(strFolderPath + "rooms/", dictionary, aIgnorePatterns);
    ParseRoomSpecs(dictionary);
    LoadModJsons(strFolderPath + "ships/", dictShips, aIgnorePatterns);
    LoadModJsons(strFolderPath + "loot/", dictLoot, aIgnorePatterns);
    LoadModJsons(strFolderPath + "market/Production/", dictProductionMaps, aIgnorePatterns);
    LoadModJsons(strFolderPath + "market/", dictMarketConfigs, aIgnorePatterns);
    LoadModJsons(strFolderPath + "market/CargoSpecs/", dictCargoSpecs, aIgnorePatterns);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_last/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesLast);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_robots/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesRobots);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_first/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesFirst);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_full/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesFull);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "manpages/", dictSimple, aIgnorePatterns);
    ParseManPages();
    LoadModJsons(strFolderPath + "homeworlds/", dictHomeworlds, aIgnorePatterns);
    LoadModJsons(strFolderPath + "careers/", dictCareers, aIgnorePatterns);
    LoadModJsons(strFolderPath + "lifeevents/", dictLifeEvents, aIgnorePatterns);
    LoadModJsons(strFolderPath + "personspecs/", dictPersonSpecs, aIgnorePatterns);
    LoadModJsons(strFolderPath + "shipspecs/", dictShipSpecs, aIgnorePatterns);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "traitscores/", dictSimple, aIgnorePatterns);
    ParseTraitScores();
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "strings/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictStrings);
    LoadModJsons(strFolderPath + "slot_effects/", dictSlotEffects, aIgnorePatterns);
    LoadModJsons(strFolderPath + "slots/", dictSlots, aIgnorePatterns);
    LoadModJsons(strFolderPath + "tickers/", dictTickers, aIgnorePatterns);
    LoadModJsons(strFolderPath + "condrules/", dictCondRules, aIgnorePatterns);
    LoadModJsons(strFolderPath + "audioemitters/", dictAudioEmitters, aIgnorePatterns);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "crewskins/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictCrewSkins);
    LoadModJsons(strFolderPath + "ads/", dictAds, aIgnorePatterns);
    LoadModJsons(strFolderPath + "headlines/", dictHeadlines, aIgnorePatterns);
    LoadModJsons(strFolderPath + "music/", dictMusic, aIgnorePatterns);
    ParseMusic();
    LoadModJsons(strFolderPath + "cooverlays/", dictCOOverlays, aIgnorePatterns);
    Dictionary<string, JsonDCOCollection> dictionary2 = new Dictionary<string, JsonDCOCollection>();
    LoadModJsons(strFolderPath + "market/CoCollections/", dictionary2, aIgnorePatterns);
    BuildMarketDCOCollection(dictionary2);
    LoadModJsons(strFolderPath + "ledgerdefs/", dictLedgerDefs, aIgnorePatterns);
    LoadModJsons(strFolderPath + "pledges/", dictPledges, aIgnorePatterns);
    LoadModJsons(strFolderPath + "jobitems/", dictJobitems, aIgnorePatterns);
    LoadModJsons(strFolderPath + "jobs/", dictJobs, aIgnorePatterns);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_ship/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesShip);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_ship_adjectives/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesShipAdjectives);
    dictSimple.Clear();
    LoadModJsons(strFolderPath + "names_ship_nouns/", dictSimple, aIgnorePatterns);
    ParseSimpleIntoStringDict(dictSimple, dictNamesShipNouns);
    LoadModJsons(strFolderPath + "ai_training/", dictAIPersonalities, aIgnorePatterns);
    LoadModJsons(strFolderPath + "transit/", dictTransit, aIgnorePatterns);
    LoadModJsons(strFolderPath + "plot_manager/", dictPlotManager, aIgnorePatterns);
    LoadModJsons(strFolderPath + "star_systems/", dictStarSystems, aIgnorePatterns);
    LoadModJsons(strFolderPath + "parallax/", dictParallax, aIgnorePatterns);
    LoadModJsons(strFolderPath + "context/", dictContext, aIgnorePatterns);
    LoadModJsons(strFolderPath + "chargeprofiles/", dictChargeProfiles, aIgnorePatterns);
    LoadModJsons(strFolderPath + "wounds/", dictWounds, aIgnorePatterns);
    LoadModJsons(strFolderPath + "attackmodes/", dictAModes, aIgnorePatterns);
    LoadModJsons(strFolderPath + "pda_apps/", dictPDAAppIcons, aIgnorePatterns);
    LoadModJsons(strFolderPath + "zone_triggers/", dictZoneTriggers, aIgnorePatterns);
    LoadModJsons(strFolderPath + "tips/", dictTips, aIgnorePatterns);
    LoadModJsons(strFolderPath + "crime/", dictCrimes, aIgnorePatterns);
    LoadModJsons(strFolderPath + "plots/", dictPlots, aIgnorePatterns);
    LoadModJsons(strFolderPath + "plot_beats/", dictPlotBeats, aIgnorePatterns);
    LoadModJsons(strFolderPath + "racing/tracks/", dictRaceTracks, aIgnorePatterns);
    LoadModJsons(strFolderPath + "racing/leagues/", dictRacingLeagues, aIgnorePatterns);
    Dictionary<string, JsonInstallable> dictionary3 = new Dictionary<string, JsonInstallable>();
    LoadModJsons(strFolderPath + "installables/", dictionary3, aIgnorePatterns);
    foreach (KeyValuePair<string, JsonInstallable> item in dictionary3)
    {
        Installables.Create(item.Value);
    }
    dictionary3.Clear();
    dictionary3 = null;
    Dictionary<string, JsonInteractionOverride> dictionary4 = new Dictionary<string, JsonInteractionOverride>();
    LoadModJsons(strFolderPath + "interaction_overrides/", dictionary4, aIgnorePatterns);
    foreach (KeyValuePair<string, JsonInteractionOverride> item2 in dictionary4)
    {
        item2.Value.Generate();
    }
    dictionary4.Clear();
    dictionary4 = null;
    Dictionary<string, JsonPlotBeatOverride> dictionary5 = new Dictionary<string, JsonPlotBeatOverride>();
    LoadModJsons(strFolderPath + "plot_beat_overrides/", dictionary5, aIgnorePatterns);
    foreach (KeyValuePair<string, JsonPlotBeatOverride> item3 in dictionary5)
    {
        item3.Value.Generate();
    }
    dictionary5.Clear();
    dictionary5 = null;
    foreach (CondTrigger value in dictCTs.Values)
    {
        value.PostInit();
    }
    if (jmi.Status == GUIModRow.ModStatus.Missing)
    {
        jmi.Status = GUIModRow.ModStatus.Missing;
    }
    else if (num < ConsoleToGUI.instance.ErrorCount)
    {
        jmi.Status = GUIModRow.ModStatus.Error;
    }
    else
    {
        jmi.Status = GUIModRow.ModStatus.Loaded;
    }
}

private static void LoadModJsons<TJson>(string strFolderPath, Dictionary<string, TJson> dict, string[] aIgnorePatterns)
{
	if (!Directory.Exists(strFolderPath))
	{
		return;
	}
	string[] files = Directory.GetFiles(strFolderPath, "*.json", SearchOption.AllDirectories);
	string[] array = files;
	foreach (string strIn in array)
	{
		string text = PathSanitize(strIn);
		bool flag = false;
		if (aIgnorePatterns != null)
		{
			foreach (string value in aIgnorePatterns)
			{
				if (text.IndexOf(value) >= 0)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Debug.LogWarning("Ignore Pattern match: " + text + "; Skipping...");
		}
		else
		{
			JsonToData(text, dict);
		}
	}
}

public static void JsonToData<TJson>(string strFile, Dictionary<string, TJson> dict)
{
    Debug.Log("#Info# Loading json: " + strFile);
    string text = string.Empty;
    try
    {
        string json = File.ReadAllText(strFile, Encoding.UTF8);
        text += "Converting json into Array...\n";
        TJson[] array = JsonMapper.ToObject<TJson[]>(json);
        TJson[] array2 = array;
        for (int i = 0; i < array2.Length; i++)
        {
            TJson val = array2[i];
            text += "Getting key: ";
            string text2 = null;
            Type type = val.GetType();
            PropertyInfo property = type.GetProperty("strName");
            if (property == null)
            {
                JsonLogger.ReportProblem("strName is missing", ReportTypes.FailingString);
            }
            object value = property.GetValue(val, null);
            text2 = value.ToString();
            text = text + text2 + "\n";
            if (dict.ContainsKey(text2))
            {
                Debug.Log("Warning: Trying to add " + text2 + " twice.");
                dict[text2] = val;
            }
            else
            {
                dict.Add(text2, val);
            }
        }
        array = null;
        json = null;
    }
    catch (Exception ex)
    {
        JsonLogger.ReportProblem(strFile, ReportTypes.SourceInfo);
        if (text.Length > 1000)
        {
            text = text.Substring(text.Length - 1000);
        }
        Debug.LogError(text + "\n" + ex.Message + "\n" + ex.StackTrace.ToString());
    }
    if (strFile.IndexOf("osSGv1") >= 0)
    {
        Debug.Log(text);
    }
}
*/