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
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class patch_Ship : Ship {
    [MonoModIgnore] public extern patch_Ship(GameObject go);
    public extern void orig_InitShip(bool bTemplateOnly, Loaded nLoad, string strRegIDNew = null);
    public void InitShip(bool bTemplateOnly, Loaded nLoad, string strRegIDNew = null) {
        if (FFU_BR_Defs.ModSyncLoading && json != null) {
            patch_DataHandler.SwitchSlottedItems(json, bTemplateOnly);
            patch_DataHandler.RecoverMissingItems(json);
            if (!bTemplateOnly) {
                patch_DataHandler.RecoverMissingCOs(json);
                patch_DataHandler.SyncConditions(json);
                patch_DataHandler.UpdateConditions(json);
                patch_DataHandler.SyncSlotEffects(json);
                patch_DataHandler.SyncInvEffects(json);
            }
        }
        orig_InitShip(bTemplateOnly, nLoad, strRegIDNew);
    }
}

public static partial class patch_DataHandler {
    public static void SwitchSlottedItems(JsonShip aShipRef, bool isTemplate) {
        if (aShipRef == null) return;
        List<JsonItem> aItemList = aShipRef.aItems != null ? aShipRef.aItems.ToList() : null;
        List<JsonCondOwnerSave> aSavedCOs = aShipRef.aCOs != null ? aShipRef.aCOs.ToList() : null;
        if (aItemList == null) return;

        // Valid Ship Data Only
        foreach (JsonItem aItem in aItemList) {
            if (!string.IsNullOrEmpty(aItem.strSlotParentID)) {
                
                // Proceed With Parent Only
                JsonItem aParent = aItemList.Find(x => x.strID == aItem.strSlotParentID);
                if (aParent != null && dictChangesMap.ContainsKey(aParent.strName) &&
                    dictChangesMap[aParent.strName].ContainsKey(FFU_BR_Defs.CMD_SWITCH_SLT) && 
                    dictChangesMap[aParent.strName][FFU_BR_Defs.CMD_SWITCH_SLT] != null) {

                    // Create Items Map For Parent
                    Dictionary<string,string> mapSwitchSlots = 
                        dictChangesMap[aParent.strName][FFU_BR_Defs.CMD_SWITCH_SLT]
                        .Where(x => x.Split(FFU_BR_Defs.SYM_EQU[0]).Length == 2)
                        .Select(x => x.Split(FFU_BR_Defs.SYM_EQU[0]))
                        .ToDictionary(x => x[0], x => x[1]);

                    // Verify Item Mapping
                    if (mapSwitchSlots.ContainsKey(aItem.strName)) {
                        string refTarget = mapSwitchSlots[aItem.strName];
                        if (string.IsNullOrEmpty(refTarget)) continue;

                        // Valid Mapped CO Only
                        Debug.Log($"#Info# Found the mismatched CO [{aItem.strName}:{aItem.strID}] " +
                            $"for the Parent CO [{aParent.strName}:{aParent.strID}] for remapping! " +
                            $"Syncing to the CO [{refTarget}] from the template.");
                        if (DataHandler.dictCOs.TryGetValue(refTarget, out JsonCondOwner refCO)) {
                            
                            // Sync Existing Item
                            aItem.strName = refCO.strName;
                            if (!isTemplate && aSavedCOs != null &&
                                DataHandler.dictCOs.TryGetValue(aParent.strName, out JsonCondOwner prntCO)) {
                                JsonCondOwnerSave aSavedCO = aSavedCOs.Find(x => x.strID == aItem.strID);

                                // Sync Saved CO Data
                                if (aSavedCO != null) {
                                    aSavedCO.strSlotName = refCO.mapSlotEffects.Intersect(prntCO.aSlotsWeHave).First();
                                    aSavedCO.strCondID = refCO.strName + aItem.strID;
                                    aSavedCO.strFriendlyName = refCO.strNameFriendly;
                                    aSavedCO.strCODef = refCO.strName;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Update Saved Items & Saved COs
        if (aSavedCOs != null) aShipRef.aCOs = aSavedCOs.ToArray();
        aShipRef.aItems = aItemList.ToArray();
    }

    public static void RecoverMissingItems(JsonShip aShipRef) {
        if (aShipRef == null) return;
        List<JsonItem> aItemList = aShipRef.aItems != null ? aShipRef.aItems.ToList() : null;
        List<JsonItem> aMissingItems = new List<JsonItem>();
        if (aItemList == null) return;

        // Valid Ship Data Only
        foreach (JsonItem aItem in aItemList) {
            if (dictChangesMap.ContainsKey(aItem.strName) &&
                dictChangesMap[aItem.strName].ContainsKey(FFU_BR_Defs.CMD_REC_MISSING) &&
                dictChangesMap[aItem.strName][FFU_BR_Defs.CMD_REC_MISSING] != null &&
                DataHandler.dictCOs.TryGetValue(aItem.strName, out JsonCondOwner refCO)) {
                
                // Prepare Base Commands
                List<string> targetKeys = dictChangesMap[aItem.strName][FFU_BR_Defs.CMD_REC_MISSING].ToList();
                bool isInverse = targetKeys.Remove(FFU_BR_Defs.FLAG_INVERSE);
                bool doAll = targetKeys.Count == 0;

                // Only For COs With Slots And Assigned Loot Table
                if (refCO.aSlotsWeHave != null && refCO.aSlotsWeHave.Length > 0
                    && refCO.strLoot != null && DataHandler.dictLoot.ContainsKey(refCO.strLoot)) {
                    
                    // Find All Existing Locked COs
                    List<string> aSlotStr = aItemList.FindAll(x => x.strSlotParentID == aItem.strID
                        && listLockedCOs.Contains(x.strName) || targetKeys.Contains(x.strName))
                        .Select(x => x.strName).ToList();

                    // Find All Missing Locked COs
                    List<string> aOrigStr = DataHandler.dictLoot[refCO.strLoot].GetAllLootNames()
                        .Where(x => (doAll && listLockedCOs.Contains(x)) || (!isInverse && targetKeys.Contains(x))
                        || (isInverse && !targetKeys.Contains(x) && listLockedCOs.Contains(x))).ToList();
                    List<string> aItemAdd = aOrigStr.Except(aSlotStr).ToList();

                    // Create From Reference With New ID
                    foreach (string aNewItem in aItemAdd) {
                        JsonItem aMissingItem = new JsonItem();
                        aMissingItem.strName = aNewItem;
                        aMissingItem.fX = aItem.fX;
                        aMissingItem.fY = aItem.fY;
                        aMissingItem.fRotation = 0.0f;
                        aMissingItem.strID = Guid.NewGuid().ToString();
                        aMissingItem.strSlotParentID = aItem.strID;
                        aMissingItem.bForceLoad = aItem.bForceLoad;

                        // Logging And Adding New Entry
                        Debug.Log($"#Info# Found the missing locked CO [{aNewItem}] " +
                            $"for the Parent CO [{aItem.strName}:{aItem.strID}] in " +
                            $"the list! New ID [{aMissingItem.strID}], adding.");
                        aMissingItems.Add(aMissingItem);
                    }
                }
            }
        }

        // Add Missing Locked COs
        aItemList.AddRange(aMissingItems);
        aShipRef.aItems = aItemList.ToArray();
    }

    public static void RecoverMissingCOs(JsonShip aShipRef) {
        if (aShipRef == null) return;
        List<JsonItem> aItemList = aShipRef.aItems != null ? aShipRef.aItems.ToList() : null;
        List<JsonCondOwnerSave> aSavedCOs = aShipRef.aCOs != null ? aShipRef.aCOs.ToList() : null;
        if (aItemList == null || aSavedCOs == null) return;

        // Valid Ship Data Only
        foreach (JsonItem aItem in aItemList) {
            if (aSavedCOs.Find(x => x.strID == aItem.strID) == null) {
                if (!string.IsNullOrEmpty(aItem.strSlotParentID)) {
                    JsonItem aParent = aItemList.Find(x => x.strID == aItem.strSlotParentID);
                    JsonCondOwnerSave aParentCO = aSavedCOs.Find(x => x.strID == aItem.strSlotParentID);
                    if (aParent == null || aParentCO == null) continue;

                    // Proceed If Parent Has CO
                    if (DataHandler.dictCOs.ContainsKey(aItem.strName) && 
                        DataHandler.dictCOs.ContainsKey(aParent.strName) &&
                        dictChangesMap.ContainsKey(aParent.strName) &&
                        dictChangesMap[aParent.strName].ContainsKey(FFU_BR_Defs.CMD_REC_MISSING)) {

                        // Prepare Base Variables
                        Debug.Log($"#Info# Found the CO [{aItem.strName}:{aItem.strID}] " +
                            $"with missing save data! Creating data from template.");
                        JsonCondOwner refCO = DataHandler.dictCOs[aItem.strName];
                        JsonCondOwner prntCO = DataHandler.dictCOs[aParent.strName];

                        // Create From Template If Item Type
                        if (refCO.strType == "Item") {
                            JsonCondOwnerSave coSaveData = new JsonCondOwnerSave();

                            // Getting Identifiers From Item
                            coSaveData.strID = aItem.strID;
                            coSaveData.strCODef = aItem.strName;
                            coSaveData.strCondID = aItem.strName + aItem.strID;

                            // Filling Static Saved CO Data
                            coSaveData.bAlive = true;
                            coSaveData.inventoryX = 0;
                            coSaveData.inventoryY = 0;
                            coSaveData.fDGasTemp = 0;
                            coSaveData.nDestTile = 0;
                            coSaveData.strIdleAnim = "Idle";
                            coSaveData.fMSRedamageAmount = 0;

                            // Filling Dynamic Saved CO Data
                            coSaveData.fLastICOUpdate = StarSystem.fEpoch;
                            coSaveData.aConds = refCO.aStartingConds.Concat(new[]{"DEFAULT"}).ToArray();
                            coSaveData.strSlotName = refCO.mapSlotEffects.Intersect(prntCO.aSlotsWeHave).First();
                            coSaveData.strIMGPreview = DataHandler.GetItemDef(refCO.strItemDef)?.strImg;
                            if (coSaveData.strIMGPreview == null) coSaveData.strIMGPreview = "blank";
                            coSaveData.strFriendlyName = refCO.strNameFriendly;
                            coSaveData.strRegIDLast = aShipRef.strRegID;

                            // Adding Saved CO Data To List
                            aSavedCOs.Add(coSaveData);
                        } else Debug.LogWarning($"Warning! The [{aItem.strName}] isn't item CO and not supported! Ignoring.");
                    } else Debug.LogWarning($"Warning! Template CO [{aItem.strName}] for parent or item doesn't exist! Ignoring.");
                }
            }
        }

        // Add Missing Locked CO Save Data
        aShipRef.aCOs = aSavedCOs.ToArray();
    }

    public static void SyncConditions(JsonShip aShipRef) {
        if (aShipRef == null) return;
        List<JsonCondOwnerSave> aSavedCOs = aShipRef.aCOs != null ? aShipRef.aCOs.ToList() : null;
        if (aSavedCOs == null) return;

        // Valid Ship Data Only
        foreach (JsonCondOwnerSave aSavedCO in aSavedCOs) {
            if (aSavedCO != null && dictChangesMap.ContainsKey(aSavedCO.strCODef) && 
                dictChangesMap[aSavedCO.strCODef].ContainsKey(FFU_BR_Defs.CMD_CONDS_SYN) &&
                dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_CONDS_SYN] != null &&
                DataHandler.dictCOs.TryGetValue(aSavedCO.strCODef, out JsonCondOwner refCO)) {

                // Prepare Base Variables
                List<string> targetKeys = dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_CONDS_SYN].ToList();
                bool isInverse = targetKeys.Remove(FFU_BR_Defs.FLAG_INVERSE);
                bool doAll = targetKeys.Count == 0;

                // Conditions Syncing
                if (refCO.aStartingConds != null && aSavedCO.aConds != null && 
                    refCO.aStartingConds.Length > 0 && aSavedCO.aConds.Length >= 0) {

                    // Make Conditions List
                    List<string> aSavedConds = aSavedCO.aConds.ToList();
                    List<string> objCondKeys = aSavedCO.aConds.Select(x => x.Split('=')[0]).ToList();
                    List<string> refCondKeys = refCO.aStartingConds.Select(x => x.Split('=')[0]).Where(x => doAll
                        || (!isInverse && targetKeys.Contains(x)) || (isInverse && !targetKeys.Contains(x))).ToList();
                    List<string> newCondKeys = refCondKeys.Except(objCondKeys).ToList();

                    // Syncing New Conditions
                    foreach (string newCondKey in newCondKeys) {
                        string newCond = refCO.aStartingConds.ToList().Find(x => x.StartsWith(newCondKey + "="));
                        Debug.Log($"#Info# Saved CO [{aSavedCO.strCODef}:{aSavedCO.strID}] is missing " +
                            $"[{newCond}] condition! Syncing to the CO from the template.");
                        aSavedConds.Insert(0, newCond);
                    }

                    // Saving Synced Conditions
                    if (newCondKeys.Count > 0) aSavedCO.aConds = aSavedConds.ToArray();
                }
            }
        }

        // Update Saved COs
        aShipRef.aCOs = aSavedCOs.ToArray();
    }

    public static void UpdateConditions(JsonShip aShipRef) {
        if (aShipRef == null) return;
        List<JsonCondOwnerSave> aSavedCOs = aShipRef.aCOs != null ? aShipRef.aCOs.ToList() : null;
        if (aSavedCOs == null) return;

        // Valid Ship Data Only
        foreach (JsonCondOwnerSave aSavedCO in aSavedCOs) {
            if (aSavedCO != null && dictChangesMap.ContainsKey(aSavedCO.strCODef) && 
                dictChangesMap[aSavedCO.strCODef].ContainsKey(FFU_BR_Defs.CMD_CONDS_UPD) &&
                dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_CONDS_UPD] != null &&
                DataHandler.dictCOs.TryGetValue(aSavedCO.strCODef, out JsonCondOwner refCO)) {

                // Prepare Base Variables
                List<string> targetKeys = dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_CONDS_UPD].ToList();
                bool isInverse = targetKeys.Remove(FFU_BR_Defs.FLAG_INVERSE);
                bool doAll = targetKeys.Count == 0;

                // Conditions Updating
                if (refCO.aStartingConds != null && aSavedCO.aConds != null &&
                    refCO.aStartingConds.Length > 0 && aSavedCO.aConds.Length >= 0) {

                    // Make Conditions List
                    List<string> aSavedConds = aSavedCO.aConds.ToList();
                    List<string> objCondKeys = aSavedCO.aConds.Select(x => x.Split('=')[0]).ToList();
                    List<string> refCondKeys = refCO.aStartingConds.Select(x => x.Split('=')[0]).Where(x => doAll
                        || (!isInverse && targetKeys.Contains(x)) || (isInverse && !targetKeys.Contains(x))).ToList();
                    List<string> extCondKeys = refCondKeys.Intersect(objCondKeys).ToList();

                    // Syncing Condition Values
                    foreach (string extCondKey in extCondKeys) {
                        string extCond = refCO.aStartingConds.ToList().Find(x => x.StartsWith(extCondKey + "="));
                        string currCond = aSavedConds.Find(x => x.StartsWith(extCondKey + "="));
                        Debug.Log($"#Info# Saved CO [{aSavedCO.strCODef}:{aSavedCO.strID}] condition " +
                            $"[{extCondKey}] received new value from the template CO.");
                        aSavedConds[aSavedConds.IndexOf(currCond)] = extCond;
                    }

                    // Saving Synced Conditions
                    if (extCondKeys.Count > 0) aSavedCO.aConds = aSavedConds.ToArray();
                }
            }
        }

        // Update Saved COs
        aShipRef.aCOs = aSavedCOs.ToArray();
    }

    public static void SyncSlotEffects(JsonShip aShipRef) {
        if (aShipRef == null) return;
        List<JsonItem> aItemList = aShipRef.aItems != null ? aShipRef.aItems.ToList() : null;
        List<JsonCondOwnerSave> aSavedCOs = aShipRef.aCOs != null ? aShipRef.aCOs.ToList() : null;
        if (aItemList == null || aSavedCOs == null) return;

        // Valid Ship Data Only
        foreach (JsonCondOwnerSave aSavedCO in aSavedCOs) {
            if (aSavedCO != null && dictChangesMap.ContainsKey(aSavedCO.strCODef) &&
                dictChangesMap[aSavedCO.strCODef].ContainsKey(FFU_BR_Defs.CMD_EFFECT_SLT) &&
                dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_SLT] != null &&
                dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_SLT].Count > 0 &&
                DataHandler.dictCOs.TryGetValue(aSavedCO.strCODef, out JsonCondOwner refCO)) {

                // Prepare Base Variables
                List<string> addConds = dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_SLT]
                    .Where(x => x.Contains(FFU_BR_Defs.SYM_EQU)).ToList();
                List<string> remConds = dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_SLT]
                    .Where(x => x.StartsWith(FFU_BR_Defs.SYM_INV)).Select(x => x.Substring(1)).ToList();
                List<JsonCondOwnerSave> targetCOs = aSavedCOs.FindAll(x => aItemList
                    .Any(i => i.strSlotParentID == aSavedCO.strID && i.strID == x.strID));

                // Syncing Slot Effects
                foreach (JsonCondOwnerSave targetCO in targetCOs) {
                    List<string> aTargetConds = targetCO.aConds != null ?
                        targetCO.aConds.ToList() : new List<string>();
                    foreach (string addCond in addConds) {
                        string addCondEntry = addCond.Split(FFU_BR_Defs.SYM_DIV[0])[0];
                        string addCondKey = addCondEntry.Split(FFU_BR_Defs.SYM_EQU[0])[0];
                        List<string> validCOs = addCond.Split(FFU_BR_Defs.SYM_DIV[0]).Skip(1).ToList();

                        // Add Only If No Condition
                        if (!aTargetConds.Any(x => x.StartsWith(addCondKey + "=")) && 
                            (validCOs.Count == 0 || validCOs.Contains(targetCO.strCODef))) {
                            Debug.Log($"#Info# Saved CO [{targetCO.strCODef}:{targetCO.strID}] " +
                                $"got condition [{addCondEntry}] due to the Parent CO " +
                                $"[{aSavedCO.strCODef}:{aSavedCO.strID}] slot effects.");
                            aTargetConds.Insert(0, addCondEntry);
                            continue;
                        }
                    }
                    foreach (string remCond in remConds) {
                        string remCondEntry = remCond.Split(FFU_BR_Defs.SYM_DIV[0])[0];
                        string remCondsKey = remCondEntry.Split(FFU_BR_Defs.SYM_EQU[0])[0];
                        List<string> validCOs = remCond.Split(FFU_BR_Defs.SYM_DIV[0]).Skip(1).ToList();

                        // Remove Only If Condition
                        if (aTargetConds.Any(x => x.StartsWith(remCondsKey + "=")) &&
                            (validCOs.Count == 0 || validCOs.Contains(targetCO.strCODef))) {
                            Debug.Log($"#Info# Saved CO [{targetCO.strCODef}:{targetCO.strID}] " +
                                $"lost condition [{remCondEntry}] due to the Parent CO " +
                                $"[{aSavedCO.strCODef}:{aSavedCO.strID}] slot effects.");
                            aTargetConds.Remove(aTargetConds.Find(x => x.StartsWith(remCondsKey + "=")));
                            continue;
                        }
                    }

                    // Saving Synced Conditions
                    if (addConds.Count > 0 || remConds.Count > 0) targetCO.aConds = aTargetConds.ToArray();
                }
            }
        }
    }

    public static void SyncInvEffects(JsonShip aShipRef) {
        if (aShipRef == null) return;
        List<JsonItem> aItemList = aShipRef.aItems != null ? aShipRef.aItems.ToList() : null;
        List<JsonCondOwnerSave> aSavedCOs = aShipRef.aCOs != null ? aShipRef.aCOs.ToList() : null;
        if (aItemList == null || aSavedCOs == null) return;

        // Valid Ship Data Only
        foreach (JsonCondOwnerSave aSavedCO in aSavedCOs) {
            if (aSavedCO != null && dictChangesMap.ContainsKey(aSavedCO.strCODef) &&
                dictChangesMap[aSavedCO.strCODef].ContainsKey(FFU_BR_Defs.CMD_EFFECT_INV) &&
                dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_INV] != null &&
                dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_INV].Count > 0 &&
                DataHandler.dictCOs.TryGetValue(aSavedCO.strCODef, out JsonCondOwner refCO)) {

                // Prepare Base Variables
                List<string> addConds = dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_INV]
                    .Where(x => x.Contains(FFU_BR_Defs.SYM_EQU)).ToList();
                List<string> remConds = dictChangesMap[aSavedCO.strCODef][FFU_BR_Defs.CMD_EFFECT_INV]
                    .Where(x => x.StartsWith(FFU_BR_Defs.SYM_INV)).Select(x => x.Substring(1)).ToList();
                List<JsonCondOwnerSave> targetCOs = aSavedCOs.FindAll(x => aItemList
                    .Any(i => i.strParentID == aSavedCO.strID && i.strID == x.strID));

                // Syncing Inventory Effects
                foreach (JsonCondOwnerSave targetCO in targetCOs) {
                    List<string> aTargetConds = targetCO.aConds != null ?
                        targetCO.aConds.ToList() : new List<string>();
                    foreach (string addCond in addConds) {
                        string addCondEntry = addCond.Split(FFU_BR_Defs.SYM_DIV[0])[0];
                        string addCondKey = addCondEntry.Split(FFU_BR_Defs.SYM_EQU[0])[0];
                        List<string> validCOs = addCond.Split(FFU_BR_Defs.SYM_DIV[0]).Skip(1).ToList();

                        // Add Only If No Condition
                        if (!aTargetConds.Any(x => x.StartsWith(addCondKey + "=")) &&
                            (validCOs.Count == 0 || validCOs.Contains(targetCO.strCODef))) {
                            Debug.Log($"#Info# Saved CO [{targetCO.strCODef}:{targetCO.strID}] " +
                                $"got condition [{addCondEntry}] due to the Parent CO " +
                                $"[{aSavedCO.strCODef}:{aSavedCO.strID}] inventory effects.");
                            aTargetConds.Insert(0, addCondEntry);
                            continue;
                        }
                    }
                    foreach (string remCond in remConds) {
                        string remCondEntry = remCond.Split(FFU_BR_Defs.SYM_DIV[0])[0];
                        string remCondsKey = remCondEntry.Split(FFU_BR_Defs.SYM_EQU[0])[0];
                        List<string> validCOs = remCond.Split(FFU_BR_Defs.SYM_DIV[0]).Skip(1).ToList();

                        // Remove Only If Condition
                        if (aTargetConds.Any(x => x.StartsWith(remCondsKey + "=")) &&
                            (validCOs.Count == 0 || validCOs.Contains(targetCO.strCODef))) {
                            Debug.Log($"#Info# Saved CO [{targetCO.strCODef}:{targetCO.strID}] " +
                                $"lost condition [{remCondEntry}] due to the Parent CO " +
                                $"[{aSavedCO.strCODef}:{aSavedCO.strID}] inventory effects.");
                            aTargetConds.Remove(aTargetConds.Find(x => x.StartsWith(remCondsKey + "=")));
                            continue;
                        }
                    }

                    // Saving Synced Conditions
                    if (addConds.Count > 0 || remConds.Count > 0) targetCO.aConds = aTargetConds.ToArray();
                }
            }
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
Ship.InitShip
public void InitShip(bool bTemplateOnly, Loaded nLoad, string strRegIDNew = null)
{
	if (nLoad <= nLoadState || json == null)
	{
		return;
	}
	if (Comms == null)
	{
		Comms = new Comms(this, json.commData);
	}
	bNoCollisions = json.bNoCollisions;
	dLastScanTime = json.dLastScanTime;
	bLocalAuthority = json.bLocalAuthority;
	bAIShip = json.bAIShip;
	if (nLoad > Loaded.Shallow)
	{
		CrewSim.bPoolVisUpdates = true;
	}
	GameObject gameObject = null;
	CondOwner condOwner = null;
	CondOwner condOwner2 = null;
	List<CondOwner> aLootSpawners = new List<CondOwner>();
	Dictionary<string, CondOwner> dictPlaceholders = new Dictionary<string, CondOwner>();
	Dictionary<int, JsonRoom> dictionary = new Dictionary<int, JsonRoom>();
	List<JsonItem> list = new List<JsonItem>();
	if (nLoadState == Loaded.None)
	{
		if (nLoad == Loaded.Edit)
		{
			if (json.publicName != null)
			{
				publicName = json.publicName;
			}
			else
			{
				publicName = "$TEMPLATE";
			}
			if (json.origin != null)
			{
				origin = json.origin;
			}
			else
			{
				origin = "$TEMPLATE";
			}
			if (json.description != null)
			{
				description = json.description;
			}
		}
		else
		{
			if (json.origin != null)
			{
				origin = json.origin;
			}
			if (json.description != null)
			{
				description = json.description;
			}
			if (json.publicName == null || json.publicName == string.Empty || json.publicName == "$TEMPLATE")
			{
				publicName = DataHandler.GetShipName();
			}
			else
			{
				publicName = json.publicName;
			}
			if (json.origin == "$TEMPLATE")
			{
				Loot loot = DataHandler.GetLoot("TXTShipOrigin");
				if (loot != null)
				{
					List<string> lootNames = loot.GetLootNames();
					if (lootNames != null && lootNames.Count > 0)
					{
						origin = loot.GetLootNames()[0];
					}
					else
					{
						origin = DataHandler.GetString("SHIP_ORIGIN_UNKNOWN");
					}
				}
			}
		}
		if (json.make != null)
		{
			make = json.make;
		}
		if (json.model != null)
		{
			model = json.model;
		}
		if (json.year != null)
		{
			year = json.year;
		}
		if (json.designation != null)
		{
			designation = json.designation;
		}
		if (json.dimensions != null)
		{
			dimensions = json.dimensions;
		}
		if (json.aRating != null)
		{
			rating = json.aRating;
		}
		if (json.aProxCurrent != null)
		{
			aProxCurrent = json.aProxCurrent.ToList();
		}
		if (json.aProxIgnores != null)
		{
			aProxIgnores = json.aProxIgnores.ToList();
		}
		if (json.aTrackCurrent != null)
		{
			aTrackCurrent = json.aTrackCurrent.ToList();
		}
		if (json.aTrackIgnores != null)
		{
			aTrackIgnores = json.aTrackIgnores.ToList();
		}
		if (json.aFactions != null)
		{
			aFactions = CrewSim.system.GetFactions(json.aFactions);
		}
		if (json.aMarketConfigs != null)
		{
			MarketConfigs = json.aMarketConfigs.CloneShallow();
		}
		Classification = json.ShipType;
		strLaw = json.strLaw;
		strParallax = json.strParallax;
		fShallowMass = json.fShallowMass;
		fShallowRCSRemass = json.fShallowRCSRemass;
		fShallowRCSRemassMax = json.fShallowRCSRemassMax;
		fShallowFusionRemain = json.fShallowFusionRemain;
		fFusionThrustMax = json.fFusionThrustMax;
		fFusionPelletMax = json.fFusionPelletMax;
		fEpochNextGrav = json.fEpochNextGrav;
		fLastQuotedPrice = json.fLastQuotedPrice;
		fBreakInMultiplier = json.fBreakInMultiplier;
		fRCSCount = json.nRCSCount;
		fShallowRotorStrength = json.fShallowRotorStrength;
		if (CrewSim.bSaveUsesOldDockCount)
		{
			nDockCount = 1;
		}
		else
		{
			nDockCount = json.nDockCount;
		}
		nRCSDistroCount = json.nRCSDistroCount;
		fAeroCoefficient = json.fAeroCoefficient;
		bFusionTorch = json.bFusionTorch;
		strXPDR = json.strXPDR;
		bXPDRAntenna = json.bXPDRAntenna;
		bShipHidden = json.bShipHidden;
		bIsUnderConstruction = json.bIsUnderConstruction;
		if (json.nConstructionProgress > 0)
		{
			nConstructionProgress = json.nConstructionProgress;
		}
		strTemplateName = json.strTemplateName;
		nInitConstructionProgress = json.nInitConstructionProgress;
		if (nLoad >= Loaded.Shallow && json.aShallowPSpecs != null)
		{
			list.AddRange(json.aShallowPSpecs);
		}
		if (bTemplateOnly)
		{
			if (strRegIDNew == null)
			{
				strRegID = GenerateID();
			}
			else
			{
				strRegID = strRegIDNew;
			}
			bPrefill = true;
			bResetLocks = true;
			if (json != null)
			{
				bBreakInUsed = json.bBreakInUsed;
			}
		}
		else
		{
			strRegID = json.strRegID;
			bPrefill = json.bPrefill;
			bBreakInUsed = json.bBreakInUsed;
		}
		this.gameObject.name = strRegID;
		DMGStatus = json.DMGStatus;
	}
	Debug.Log(string.Concat("#Info# Loading ship ", strRegID, "; Requesting: ", nLoad, "; Currently: ", nLoadState));
	if (nLoad >= Loaded.Edit)
	{
		list.AddRange(json.aItems);
		this.gameObject.SetActive(value: true);
		nRCSDistroCount = 0;
		fRCSCount = 0f;
		nDockCount = 0;
		LiftRotorsThrustStrength = -1f;
		aActiveHeavyLiftRotors.Clear();
		if (DMGStatus != Damage.Derelict)
		{
			fLastQuotedPrice = 0.0;
		}
		if (nConstructionProgress < 100 && fFirstVisit > 0.0)
		{
			List<JsonItem> list2 = Reconstruct();
			if (list2.Count > 0)
			{
				json.aRooms = null;
				SpawnItems(list2, bTemplateOnly: true, nLoad, ref dictPlaceholders, ref aLootSpawners);
				if (aLootSpawners != null && aLootSpawners.Count > 0)
				{
					DoLootSpawners(aLootSpawners);
				}
			}
		}
	}
	if (json.aCrew != null && nLoadState == Loaded.None && (!bTemplateOnly || DMGStatus != Damage.Derelict))
	{
		list.AddRange(json.aCrew);
		JsonItem[] aItems = json.aItems;
		foreach (JsonItem jsonItem in aItems)
		{
			if (jsonItem.ForceLoad())
			{
				list.Add(jsonItem);
			}
		}
	}
	if (json.aCOs != null)
	{
		JsonCondOwnerSave[] aCOs = json.aCOs;
		foreach (JsonCondOwnerSave jsonCondOwnerSave in aCOs)
		{
			DataHandler.dictCOSaves[jsonCondOwnerSave.strID] = jsonCondOwnerSave;
		}
		json.aCOs = null;
	}
	if (ShipCO == null)
	{
		ShipCO = DataHandler.GetCondOwner("ShipCO", strRegID, null, bLoot: false, null, json.shipCO, null, this.gameObject.transform);
		ShipCO.ship = this;
		ShipCO.ClaimShip(strRegID);
	}
	SpawnItems(list, bTemplateOnly, nLoad, ref dictPlaceholders, ref aLootSpawners);
	if (!bTemplateOnly)
	{
		publicName = json.publicName;
		strRegID = json.strRegID;
		nCurrentWaypoint = json.nCurrentWaypoint;
		fTimeEngaged = json.fTimeEngaged;
		fWearManeuver = json.fWearManeuver;
		fWearAccrued = json.fWearAccrued;
		if (nLoadState != Loaded.Shallow)
		{
			objSS = new ShipSitu(json.objSS);
			if (objSS.NavData != null)
			{
				objSS.NavData.SetShip(this);
			}
		}
		fLastVisit = json.fLastVisit;
		fFirstVisit = json.fFirstVisit;
		fAIPauseTimer = json.fAIPauseTimer;
		fAIDockingExpire = json.fAIDockingExpire;
		if (json.aWPs != null)
		{
			for (int k = 0; k < json.aWPs.Length; k++)
			{
				aWPs.Add(new WaypointShip(new ShipSitu(json.aWPs[k]), json.aWPTimes[k]));
			}
		}
		if (json.aRooms != null)
		{
			for (int l = 0; l < json.aRooms.Length; l++)
			{
				if (json.aRooms[l].aTiles != null)
				{
					JsonRoom jsonRoom = json.aRooms[l];
					for (int m = 0; m < jsonRoom.aTiles.Length; m++)
					{
						dictionary[jsonRoom.aTiles[m]] = jsonRoom;
					}
				}
			}
		}
	}
	else
	{
		List<CondOwner> cOs = GetCOs(null, bSubObjects: true, bAllowDocked: false, bAllowLocked: true);
		RectifyBrokenIDs(cOs);
		List<string> list3 = MarketConfigs.Keys.ToList();
		foreach (string item in list3)
		{
			if (mapIDRemap.TryGetValue(item, out var value))
			{
				string value2 = MarketConfigs[item];
				MarketConfigs.Remove(item);
				MarketConfigs[value] = value2;
				MarketManager.TraderIDUpdated(strRegID, item, value);
			}
		}
	}
	if (nLoad > Loaded.Shallow)
	{
		if (!string.IsNullOrEmpty(strParallax))
		{
			CrewSim.system.SetParallax(strParallax);
		}
		SetZoneData(json.aZones);
		CreateRooms(dictionary);
		TileUtils.GetPoweredTiles(this);
		if (json.aBGXs != null && json.aBGYs != null && json.aBGNames != null)
		{
			for (int n = 0; n < json.aBGNames.Length && n < json.aBGYs.Length && n < json.aBGXs.Length; n++)
			{
				if (json.aBGNames[n] == null || json.aBGXs[n] == null || json.aBGYs[n] == null)
				{
					continue;
				}
				for (int num = 0; num < json.aBGXs[n].Length; num++)
				{
					float num2 = json.aBGXs[n][num];
					float num3 = json.aBGYs[n][num];
					if (BGItemFits(json.aBGNames[n], num2, num3))
					{
						Item background = DataHandler.GetBackground(json.aBGNames[n]);
						Vector3 position = new Vector3(tfBGs.position.x, tfBGs.position.y, background.TF.position.z);
						position.x += num2;
						position.y += num3;
						background.TF.position = position;
						BGItemAdd(background);
					}
				}
			}
		}
	}
	if (json.objSS == null || !json.objSS.bIsBO)
	{
		FloorPlan = SilhouetteUtility.GetFloorVectors(json.aItems);
	}
	objSS.SetSize(SilhouetteUtility.GetSilhouetteLength(FloorPlan));
	CrewSim.system.AddShip(this, CrewSim.system.GetShipOwner(strRegID));
	if (bTemplateOnly)
	{
		if (nLoad == Loaded.Edit)
		{
			foreach (CondOwner item2 in aLootSpawners)
			{
				LootSpawner component = item2.GetComponent<LootSpawner>();
				component.UpdateAppearance();
			}
		}
		else if (nLoad >= Loaded.Shallow)
		{
			DoLootSpawners(aLootSpawners);
		}
	}
	else
	{
		foreach (CondOwner item3 in aLootSpawners)
		{
			LootSpawner component2 = item3.GetComponent<LootSpawner>();
			component2.UpdateAppearance();
			item3.Visible = false;
		}
		if (json.aPlaceholders != null)
		{
			JsonPlaceholder[] aPlaceholders = json.aPlaceholders;
			foreach (JsonPlaceholder jsonPlaceholder in aPlaceholders)
			{
				if (dictPlaceholders.ContainsKey(jsonPlaceholder.strName))
				{
					CondOwner condOwner3 = dictPlaceholders[jsonPlaceholder.strName];
					string strID = condOwner3.strID;
					CondOwner condOwner4 = DataHandler.GetCondOwner(jsonPlaceholder.strActionCO);
					CondOwner condOwner5 = DataHandler.GetCondOwner(jsonPlaceholder.strInstalledCO);
					condOwner5.tf.position = condOwner3.tf.position;
					condOwner5.Item.fLastRotation = condOwner3.tf.rotation.eulerAngles.z;
					condOwner4.strPersistentCO = jsonPlaceholder.strPersistentCO;
					condOwner4.strPersistentCT = jsonPlaceholder.strPersistentCT;
					CondOwner cOPlaceholder = DataHandler.GetCOPlaceholder(condOwner5, condOwner4, jsonPlaceholder.strInstallIA);
					cOPlaceholder.jCOS = condOwner3.jCOS;
					condOwner3.jCOS = null;
					RemoveCO(condOwner3);
					condOwner3.Destroy();
					condOwner4.Destroy();
					condOwner5.Destroy();
					cOPlaceholder.strID = strID;
					AddCO(cOPlaceholder, bTiles: true);
				}
			}
		}
	}
	aLootSpawners.Clear();
	aLootSpawners = null;
	dictPlaceholders.Clear();
	dictPlaceholders = null;
	if (bPrefill && nLoad >= Loaded.Edit)
	{
		PreFillRooms();
		if (DMGStatus == Damage.Derelict || DMGStatus == Damage.Damaged || (DMGStatus == Damage.Used && bBreakInUsed))
		{
			BreakIn();
			if (fLastQuotedPrice == 0.0)
			{
				SetDerelictValue();
			}
			bBreakInUsed = false;
		}
		else if (DMGStatus == Damage.Used)
		{
			DamageAllCOs(0.33f);
			if (ShipCO.HasCond("IsVendorShip", isThreshold: false))
			{
				ShipCO.ZeroCondAmount("IsVendorShip");
				if (Reactor != null)
				{
					FusionIC component3 = Reactor.GetComponent<FusionIC>();
					if (component3 != null)
					{
						component3.SetDerelict();
					}
				}
			}
		}
		bPrefill = false;
	}
	if (nLoad == Loaded.Full)
	{
		for (int num5 = aPeople.Count - 1; num5 >= 0; num5--)
		{
			PersonSpec personSpec = aPeople[num5];
			CondOwner condOwner6 = personSpec.MakeCondOwner(PersonSpec.StartShip.OLD, strRegID);
			Pathfinder component4 = condOwner6.GetComponent<Pathfinder>();
			Vector2 vector = new Vector2(condOwner6.tf.position.x, condOwner6.tf.position.y);
			component4.tilCurrent = GetTileAtWorldCoords1(vector.x, vector.y, bAllowDocked: true);
			if (component4.tilCurrent == null)
			{
				component4.tilCurrent = GetCrewSpawnTile(condOwner6);
			}
			FaceAnim2.GetPNG(condOwner6);
			if (condOwner6.currentRoom == null && component4.tilCurrent != null)
			{
				condOwner6.tf.position = component4.tilCurrent.tf.position;
				condOwner6.AddFloatText(bAdd: true);
				condOwner6.gameObject.SetActive(value: true);
				condOwner6.Visible = true;
				if (condOwner6.HasTickers())
				{
					CrewSim.AddTicker(condOwner6);
				}
				List<CondOwner> cOs2 = condOwner6.GetCOs(bAllowLocked: true);
				if (cOs2 != null)
				{
					foreach (CondOwner item4 in cOs2)
					{
						if (item4 != null && item4.HasTickers())
						{
							CrewSim.AddTicker(item4);
						}
					}
				}
				condOwner6.currentRoom = component4.tilCurrent.room;
				if (condOwner6.currentRoom != null)
				{
					condOwner6.currentRoom.AddToRoom(condOwner6);
				}
			}
		}
		List<CondOwner> list4 = mapICOs.Values.ToList();
		foreach (Room aRoom in aRooms)
		{
			list4.Add(aRoom.CO);
		}
		PostGameLoad(list4, nLoad);
	}
	else if (nLoad >= Loaded.Shallow)
	{
		List<CondOwner> aCOsLoaded = mapICOs.Values.ToList();
		PostGameLoad(aCOsLoaded, nLoad);
	}
	nLoadState = nLoad;
	bCheckRooms = false;
	bCheckPower = false;
	bCheckTargets = true;
	CheckAccruedWear();
	UpdatePower();
	SilhouettePoints = SilhouetteUtility.GenerateVectorPoints(FloorPlan);
	string[] array = strRegID.Split('|');
	if (array.Length > 1)
	{
		HideFromSystem = true;
		_subStation = true;
	}
	if (nLoad >= Loaded.Edit)
	{
		CrewSim.AddLoadedShip(this);
	}
	if (json.aDocked != null || (aDocked != null && aDocked.Count > 0))
	{
		List<Ship> list5 = new List<Ship>();
		if (json.aDocked != null)
		{
			string[] array2 = json.aDocked;
			foreach (string text in array2)
			{
				Ship shipByRegID = CrewSim.system.GetShipByRegID(text);
				if (shipByRegID != null && list5.IndexOf(shipByRegID) < 0)
				{
					if (shipByRegID.LoadState >= Loaded.Edit)
					{
						list5.Insert(0, shipByRegID);
					}
					else
					{
						list5.Add(shipByRegID);
					}
				}
			}
		}
		if (aDocked != null)
		{
			foreach (Ship item5 in aDocked)
			{
				if (item5 != null && !list5.Contains(item5))
				{
					list5.Insert(0, item5);
				}
			}
			aDocked.Clear();
		}
		if (DockCount < list5.Count)
		{
			list5.RemoveRange(DockCount, list5.Count - DockCount);
		}
		foreach (Ship item6 in list5)
		{
			item6.objSS.UpdateTime(StarSystem.fEpoch);
			if (nLoad == Loaded.Full)
			{
				CrewSim.DockShip(this, item6.strRegID);
				continue;
			}
			Dock(item6, bSyncOnly: true);
			item6.Dock(this, bSyncOnly: true);
		}
	}
	if (nLoad == Loaded.Full)
	{
		if (bTemplateOnly)
		{
			SetFactions(aFactions, bRemoveOld: false);
		}
		CrewSim.objInstance.workManager.ShowShipTasks(strRegID);
		UpdateRating();
		Debug.Log("#Info# " + strRegID + GetRatingString());
		VisualizeWear();
	}
}
*/