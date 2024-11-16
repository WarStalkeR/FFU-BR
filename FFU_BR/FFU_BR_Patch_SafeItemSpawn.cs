#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using MonoMod;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

public partial class patch_Ship : Ship {
    [MonoModIgnore] public extern patch_Ship(GameObject go);
    private extern void orig_SpawnItems(List<JsonItem> aItemsPlusCrew, bool bTemplateOnly, Ship.Loaded nLoad, ref Dictionary<string, CondOwner> dictPlaceholders, ref List<CondOwner> aLootSpawners);
    private void SpawnItems(List<JsonItem> aItemsPlusCrew, bool bTemplateOnly, Ship.Loaded nLoad, ref Dictionary<string, CondOwner> dictPlaceholders, ref List<CondOwner> aLootSpawners) {
		{
			SyncModdedItems(aItemsPlusCrew, bTemplateOnly);
			RestoreMissingItems(aItemsPlusCrew);
			if (!bTemplateOnly) RestoreMissingCOs(aItemsPlusCrew);
		}
        orig_SpawnItems(aItemsPlusCrew, bTemplateOnly, nLoad, ref dictPlaceholders, ref aLootSpawners);
    }
    private void SyncModdedItems(List<JsonItem> aItemList, bool isTemplate) {
        foreach (JsonItem aItem in aItemList) {
            if (!string.IsNullOrEmpty(aItem.strSlotParentID)) {
                // Get Parent From ID
                JsonItem aParent = aItemList.Find(x => x.strID == aItem.strSlotParentID);
                if (aParent != null && patch_DataHandler.dictCOchanges.ContainsKey(aParent.strName)) {
                    if (patch_DataHandler.dictCOchanges[aParent.strName].ContainsKey(aItem.strName)) {
                        // Validate Mapped CO
                        string refTarget = patch_DataHandler.dictCOchanges[aParent.strName][aItem.strName];
                        Debug.Log($"#Info# Found the mismatched CO [{aItem.strName}:{aItem.strID}] " +
                            $"for the Parent CO [{aParent.strName}:{aParent.strID}] for remapping! " +
                            $"Syncing to the CO [{refTarget}] from the template.");
                        if (DataHandler.dictCOs.TryGetValue(refTarget, out JsonCondOwner refCO)) {
                            // Sync Existing Item
                            aItem.strName = refCO.strName;
                            if (!isTemplate && DataHandler.dictCOSaves.TryGetValue(aItem.strID, out JsonCondOwnerSave aSavedCO)
                                && DataHandler.dictCOs.TryGetValue(aParent.strName, out JsonCondOwner prntCO)) {
                                // Sync Saved CO Data
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
	private void RestoreMissingItems(List<JsonItem> aItemList) {
		List<JsonItem> aMissingItems = new List<JsonItem>();
        foreach (JsonItem aItem in aItemList) {
			if (patch_DataHandler.dictCOchanges.ContainsKey(aItem.strName) && 
				DataHandler.dictCOs.TryGetValue(aItem.strName, out JsonCondOwner refCO)) {
				if (refCO.aSlotsWeHave != null && refCO.aSlotsWeHave.Length > 0 
					&& refCO.strLoot != null && DataHandler.dictLoot.ContainsKey(refCO.strLoot)) {
					List<JsonItem> aSlottedItems = aItemList.FindAll(x => x.strSlotParentID == aItem.strID);
					List<string> aSlotStr = aSlottedItems.Select(x => x.strName).ToList();
                    //foreach (var _ in aSlotStr) Debug.Log($"#DEBUG_RestoreMissingItems# Found aSlotStr: {_} in {aItem.strName}");
                    List<string> aOrigStr = DataHandler.dictLoot[refCO.strLoot].GetAllLootNames();
                    //foreach (var _ in aOrigStr) Debug.Log($"#DEBUG_RestoreMissingItems# Found aOrigStr: {_} in {DataHandler.dictLoot[refCO.strLoot].strName}");
                    List<string> aItemAdd = aOrigStr.Except(aSlotStr).ToList();
					foreach (var _ in aItemAdd) Debug.LogWarning($"The CO [{aItem.strName}:{aItem.strID}] is missing sub-CO [{_}]");
				}
			}
		}
	}
    private void RestoreMissingCOs(List<JsonItem> aItemList) {
        foreach (JsonItem aItem in aItemList) {
            if (!DataHandler.mapCOs.ContainsKey(aItem.strID) && !DataHandler.dictCOSaves.ContainsKey(aItem.strID)) {
                Debug.LogWarning($"CO [{aItem.strName}:{aItem.strID}] is missing save data! Creating from template.");
                if (!string.IsNullOrEmpty(aItem.strSlotParentID)) {
                    JsonItem aParent = aItemList.Find(x => x.strID == aItem.strSlotParentID);
                    if (aParent != null && DataHandler.dictCOs.ContainsKey(aItem.strName) && DataHandler.dictCOs.ContainsKey(aParent.strName)) {
                        JsonCondOwner refCO = DataHandler.dictCOs[aItem.strName];
                        JsonCondOwner prntCO = DataHandler.dictCOs[aParent.strName];
                        if (refCO.strType == "Item") {
                            // Creating Save CO From Template
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
                            coSaveData.strRegIDLast = strRegID;

                            // Adding Saved COs Dictionary
                            DataHandler.dictCOSaves[aItem.strID] = coSaveData;
                        } else Debug.LogWarning($"Warning! The [{aItem.strName}] isn't item CO and not supported! Ignoring.");
                    } else Debug.LogWarning($"Warning! Template CO [{aItem.strName}] or parent item don't exist! Ignoring.");
                } else Debug.LogWarning($"Warning! The [{aItem.strName}] isn't slotted and not supported! Ignoring.");
            }
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
private void SpawnItems(List<JsonItem> aItemsPlusCrew, bool bTemplateOnly, Loaded nLoad, ref Dictionary<string, CondOwner> dictPlaceholders, ref List<CondOwner> aLootSpawners)
{
	GameObject gameObject = null;
	CondOwner condOwner = null;
	CondOwner condOwner2 = null;
	List<JsonItem> list = new List<JsonItem>();
	foreach (JsonItem item in aItemsPlusCrew)
	{
		if (DataHandler.mapCOs.ContainsKey(item.strID))
		{
			continue;
		}
		if (!bTemplateOnly && !DataHandler.dictCOSaves.ContainsKey(item.strID))
		{
			Debug.LogError("ERROR: Trying to load a CO (" + item.strName + ") with missing save data for ship: " + strRegID + ": " + item.strID + ". Skipping.");
			continue;
		}
		if (item.strParentID != null || item.strSlotParentID != null)
		{
			if (!bTemplateOnly || item.ForceLoad())
			{
				list.Add(item);
			}
			continue;
		}
		string strIDTemp = item.strID;
		if (bTemplateOnly && !CrewSim.bShipEdit)
		{
			strIDTemp = null;
		}
		gameObject = CreatePart(item, strIDTemp, bTemplateOnly);
		if (!(gameObject == null))
		{
			condOwner = gameObject.GetComponent<CondOwner>();
			if (bTemplateOnly)
			{
				mapIDRemap[item.strID] = condOwner.strID;
			}
			bool bTiles = nLoad > Loaded.Shallow;
			if (condOwner.mapGUIPropMaps != null && condOwner.mapGUIPropMaps.ContainsKey(GUITradeBase.ASYNCIDENTIFIER))
			{
				bTiles = false;
				condOwner.mapGUIPropMaps.Remove(GUITradeBase.ASYNCIDENTIFIER);
			}
			AddCO(condOwner, bTiles);
			if (condOwner.HasCond("IsLootSpawner"))
			{
				aLootSpawners.Add(condOwner);
				condOwner.ClaimShip(strRegID);
			}
			else if (condOwner.HasCond("IsPlaceholder"))
			{
				dictPlaceholders[condOwner.strID] = condOwner;
			}
		}
	}
	int num = -1;
	int num2 = -1;
	while (list.Count > 0)
	{
		if (num2 < 0)
		{
			if (num == 0)
			{
				Debug.Log("WARNING: " + list.Count + " unprocessed sub items on ship " + strRegID);
				break;
			}
			num2 = list.Count - 1;
			num = 0;
		}
		JsonItem jsonItem = list[num2];
		num2--;
		string text = jsonItem.strParentID;
		if (text == null)
		{
			text = jsonItem.strSlotParentID;
		}
		if (mapICOs.ContainsKey(text))
		{
			condOwner = mapICOs[text];
		}
		else
		{
			if (!mapIDRemap.ContainsKey(text))
			{
				continue;
			}
			condOwner = mapICOs[mapIDRemap[text]];
		}
		bool flag = jsonItem.ForceLoad() || condOwner.pspec != null;
		string strIDTemp2 = jsonItem.strID;
		if (bTemplateOnly && !flag && !CrewSim.bShipEdit)
		{
			strIDTemp2 = null;
		}
		gameObject = CreatePart(jsonItem, strIDTemp2, bTemplateOnly);
		if (gameObject == null)
		{
			continue;
		}
		condOwner2 = gameObject.GetComponent<CondOwner>();
		if (bTemplateOnly)
		{
			mapIDRemap[jsonItem.strID] = condOwner2.strID;
		}
		condOwner2.tf.localPosition = new Vector3(condOwner.tf.position.x, condOwner.tf.position.y, Container.fZSubOffset);
		bool flag2 = true;
		if (jsonItem.strSlotParentID != null)
		{
			if (condOwner.compSlots == null)
			{
				Debug.LogError("ERROR: Attempting to slot " + condOwner2.strCODef + " - " + condOwner2.strID + " into parent with no slot: " + condOwner.strCODef + " - " + condOwner.strID);
				flag2 = false;
			}
			else if (condOwner2.jCOS == null)
			{
				Debug.LogError("ERROR: Attempting to slot " + condOwner2.strCODef + " - " + condOwner2.strID + " but it has no jCOS data!");
				flag2 = false;
			}
			else if (!condOwner.compSlots.SlotItem(condOwner2.jCOS.strSlotName, condOwner2))
			{
				continue;
			}
		}
		else if (condOwner.objContainer != null && !condOwner.objContainer.Contains(condOwner2))
		{
			bool bAllowStacking = condOwner.objContainer.bAllowStacking;
			condOwner.objContainer.bAllowStacking = false;
			condOwner.objContainer.AddCOSimple(condOwner2, condOwner2.pairInventoryXY);
			condOwner.objContainer.bAllowStacking = bAllowStacking;
		}
		if (flag2)
		{
			mapICOs[condOwner2.strID] = condOwner2;
		}
		list.Remove(jsonItem);
		num++;
	}
}
*/