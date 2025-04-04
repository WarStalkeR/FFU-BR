using FFU_Beyond_Reach;
using MonoMod;
using System.Collections.Generic;
using System.Linq;

public partial class patch_JsonInteraction : JsonInteraction {
    public bool bForceVerbose { get; set; }
    public bool bRoomLookup { get; set; }
}

public partial class patch_Interaction : Interaction {
    public bool bForceVerbose;
    public bool bRoomLookup;
    public bool bWriteToLog;

    // Assign New Variables
    private extern void orig_SetData(JsonInteraction jsonIn, JsonInteractionSave jis = null);
    private void SetData(JsonInteraction jsonIn, JsonInteractionSave jis = null) {
        orig_SetData(jsonIn, jis);
        if (jsonIn != null) {
            bForceVerbose = (jsonIn as patch_JsonInteraction).bForceVerbose;
            bRoomLookup = (jsonIn as patch_JsonInteraction).bRoomLookup;
            bWriteToLog = FFU_BR_Defs.ActLogging >= FFU_BR_Defs.ActLogs.Interactions && bForceVerbose;
        }
    }

    // Logging Fail Reason
    private extern void orig_AddFailReason(string strKey, string strReason);
    private void AddFailReason(string strKey, string strReason) {
        if (bWriteToLog && strReason != Interaction.STR_IA_FAIL_DEFAULT)
            UnityEngine.Debug.Log($"#Interaction# {objUs?.strName ?? "Unknown"}:" +
                $"{objUs?.strID ?? "0"} => [Failed] {strKey}: " +
                $"{(string.IsNullOrEmpty(strReason) ? "N/A" : strReason)}");
        orig_AddFailReason(strKey, strReason);
    }

    // Enforce Verbosity
    private extern bool orig_TriggeredInternal(CondOwner objUs, CondOwner objThem, bool bStats = false,
        bool bIgnoreItems = false, bool bCheckPath = false, bool bFetchItems = true, List<string> aForbid3rds = null);
    private bool TriggeredInternal(CondOwner objUs, CondOwner objThem, bool bStats = false, 
        bool bIgnoreItems = false, bool bCheckPath = false, bool bFetchItems = true, List<string> aForbid3rds = null) {
        if (bForceVerbose) bVerboseTrigger = true;
        return orig_TriggeredInternal(objUs, objThem, bStats, bIgnoreItems, bCheckPath, bFetchItems, aForbid3rds);
    }

    // Room Lookup Integration
    [MonoModReplace] public void ApplyLogging(string strOwner, bool bTraitSuffix) {
        string msgText = null;
        if (bWriteToLog) {
            msgText = GetText(bTraitSuffix);
            UnityEngine.Debug.Log($"#Interaction# " +
            $"{objUs?.strName ?? "Unknown"}:" +
            $"{objUs?.strID ?? "0"} => {msgText}");
        }
        if (nLogging == Logging.NONE || bLogged) return;
        msgText = msgText ?? GetText(bTraitSuffix);
        List<CondOwner> coNotifyList = new List<CondOwner>();
        switch (nLogging) {
            case Logging.GROUP: {
                coNotifyList.Add(objUs);
                if (strThemType == TARGET_OTHER) coNotifyList.Add(objThem);
                break;
            }
            case Logging.ROOM: {
                if (objUs.currentRoom != null) {
                    coNotifyList.AddRange(objUs.ship.GetPeopleInRoom(objUs.currentRoom));
                } else if (bRoomLookup) {
                    List<CondOwner> listCOsAtPos = new List<CondOwner>();
                    CondTrigger isRoomCT = DataHandler.GetCondTrigger("TIsRoom");
                    objUs.ship.GetCOsAtWorldCoords1(objUs.GetPos(), isRoomCT, false, false, listCOsAtPos);
                    objUs.currentRoom = listCOsAtPos.First().currentRoom;
                    coNotifyList.AddRange(objUs.ship.GetPeopleInRoom(objUs.currentRoom));
                }
                if (!coNotifyList.Contains(objUs)) coNotifyList.Add(objUs);
                if (!coNotifyList.Contains(objThem)) coNotifyList.Add(objThem);
                break;
            }
            case Logging.SHIP: {
                if (objUs.ship != null) coNotifyList.AddRange(objUs.ship.GetPeople(true));
                break;
            }
        }
        foreach (CondOwner item in coNotifyList) {
            item.LogMessage(msgText, strColor, strOwner);
        }
        bLogged = true;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* Interaction.AddFailReason
private void AddFailReason(string strKey, string strReason)
{
	if (mapFails == null)
	{
		mapFails = new Dictionary<string, List<string>>();
	}
	if (!string.IsNullOrEmpty(strKey) && !string.IsNullOrEmpty(strReason))
	{
		if (mapFails.ContainsKey(strKey))
		{
			mapFails[strKey].Add(strReason);
			return;
		}
		mapFails[strKey] = new List<string> { strReason };
	}
}
*/

/* Interaction.TriggeredInternal
private bool TriggeredInternal(CondOwner objUs, CondOwner objThem, bool bStats = false, bool bIgnoreItems = false, bool bCheckPath = false, bool bFetchItems = true, List<string> aForbid3rds = null)
{
	string sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_DEFAULT;
	AddFailReason("main", sTR_IA_FAIL_DEFAULT);
	bAirlockBlocked = false;
	if (strThemType == TARGET_SELF && objUs != objThem)
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_THEM;
			AddFailReason("us", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (strThemType == TARGET_OTHER && objUs == objThem)
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_US;
			AddFailReason("them", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (bTargetOwned && (objThem == null || (objThem.ship != null && !objUs.OwnsShip(objThem.ship.strRegID))))
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_OWNED_US;
			AddFailReason("them", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (!string.IsNullOrEmpty(strFactionTest) && strFactionTest != "ALWAYS" && objThem != null)
	{
		bool flag = false;
		switch (strFactionTest)
		{
		case "DIFFERENT":
			flag = !objUs.SharesFactionsWith(objThem);
			break;
		case "SAME":
			flag = objUs.SharesFactionsWith(objThem);
			break;
		case "LIKES":
		{
			float factionScore = objUs.GetFactionScore(objThem.GetAllFactions());
			flag = JsonFaction.GetReputation(factionScore) != JsonFaction.Reputation.Likes;
			break;
		}
		case "DISLIKES":
		{
			float factionScore = objUs.GetFactionScore(objThem.GetAllFactions());
			flag = JsonFaction.GetReputation(factionScore) != JsonFaction.Reputation.Dislikes;
			break;
		}
		}
		if (flag)
		{
			if (bVerboseTrigger)
			{
				sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_FACTION;
				AddFailReason("us", sTR_IA_FAIL_DEFAULT);
			}
			return false;
		}
	}
	if (bHumanOnly && objUs != CrewSim.GetSelectedCrew())
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_PLAYER;
			AddFailReason("us", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (ShipTestUs != null && !ShipTestUs.Matches(objUs.ship, objUs))
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = DataHandler.GetString("IA_FAIL_SHIP_WRONG");
			AddFailReason("us", sTR_IA_FAIL_DEFAULT);
			sTR_IA_FAIL_DEFAULT = DataHandler.GetString("IA_FAIL_SHIP_WRONG");
			AddFailReason("debugus", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (ShipTestThem != null && !ShipTestThem.Matches(objThem.ship, objThem))
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = DataHandler.GetString("IA_FAIL_SHIP_WRONG");
			AddFailReason("them", sTR_IA_FAIL_DEFAULT);
			sTR_IA_FAIL_DEFAULT = DataHandler.GetString("IA_FAIL_SHIP_WRONG");
			AddFailReason("debugthem", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (CTTestUs != null && !CTTestUs.Triggered(objUs, strName))
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = CTTestUs.strFailReasonLast;
			AddFailReason("us", sTR_IA_FAIL_DEFAULT);
			sTR_IA_FAIL_DEFAULT = CTTestUs.strFailReasonLast;
			AddFailReason("debugus", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (PSpecTestThem != null)
	{
		if (objUs.pspec != null)
		{
			if (!objUs.pspec.IsCOMyMother(PSpecTestThem, objThem))
			{
				if (bVerboseTrigger)
				{
					Debug.Log(!PSpecTestThem.Matches(objThem));
					AddFailReason("main", "Didn't pass ptest us");
				}
				return false;
			}
		}
		else if (!PSpecTestThem.Matches(objThem))
		{
			if (bVerboseTrigger)
			{
				AddFailReason("main", "Didn't pass ptest them");
			}
			return false;
		}
	}
	if (CTTestThem != null && !CTTestThem.Triggered(objThem))
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = CTTestThem.strFailReasonLast;
			AddFailReason("them", sTR_IA_FAIL_DEFAULT);
			sTR_IA_FAIL_DEFAULT = CTTestThem.strFailReasonLast;
			AddFailReason("debugthem", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (obj3rd != null)
	{
		if (PSpecTest3rd != null)
		{
			if (!objUs.pspec.IsCOMyMother(PSpecTest3rd, obj3rd))
			{
				return false;
			}
		}
		else if (CTTest3rd != null && !CTTest3rd.Triggered(obj3rd))
		{
			return false;
		}
	}
	else if (PSpecTest3rd != null)
	{
		PersonSpec person = StarSystem.GetPerson(PSpecTest3rd, objUs.socUs, bForceUnrelated: false, aForbid3rds, ShipTest3rd);
		if (person != null)
		{
			obj3rd = person.GetCO();
		}
		if (obj3rd == null)
		{
			return false;
		}
	}
	else if (CTTest3rd != null)
	{
		List<CondOwner> list = new List<CondOwner>();
		foreach (Ship allLoadedShip in CrewSim.system.GetAllLoadedShips())
		{
			if (ShipTest3rd == null || ShipTest3rd.Matches(allLoadedShip, objUs))
			{
				list.AddRange(allLoadedShip.GetCOs(CTTest3rd, bSubObjects: true, bAllowDocked: false, bAllowLocked: true));
			}
		}
		if (aForbid3rds != null)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (aForbid3rds.Contains(list[num].strID))
				{
					list.RemoveAt(num);
				}
			}
		}
		if (list.Count <= 0)
		{
			if (bVerboseTrigger)
			{
				sTR_IA_FAIL_DEFAULT = CTTest3rd.strFailReasonLast;
				AddFailReason("3rd", sTR_IA_FAIL_DEFAULT);
				sTR_IA_FAIL_DEFAULT = CTTest3rd.strFailReasonLast;
				AddFailReason("debug3rd", sTR_IA_FAIL_DEFAULT);
			}
			return false;
		}
		obj3rd = list[MathUtils.Rand(0, list.Count, MathUtils.RandType.Flat)];
	}
	if (ShipTest3rd != null && obj3rd != null && !ShipTest3rd.Matches(obj3rd.ship, obj3rd))
	{
		if (bVerboseTrigger)
		{
			sTR_IA_FAIL_DEFAULT = DataHandler.GetString("IA_FAIL_SHIP_WRONG");
			AddFailReason("3rd", sTR_IA_FAIL_DEFAULT);
			sTR_IA_FAIL_DEFAULT = DataHandler.GetString("IA_FAIL_SHIP_WRONG");
			AddFailReason("debug3rd", sTR_IA_FAIL_DEFAULT);
		}
		return false;
	}
	if (CTTestRoom != null && !CTTestRoom.IsBlank() && objUs.ship != null)
	{
		Room roomAtWorldCoords = objUs.ship.GetRoomAtWorldCoords1(objUs.tf.position, bAllowDocked: true);
		if (roomAtWorldCoords != null && roomAtWorldCoords.CO != null && !CTTestRoom.Triggered(roomAtWorldCoords.CO, strName))
		{
			if (bVerboseTrigger)
			{
				sTR_IA_FAIL_DEFAULT = CTTestRoom.strFailReasonLast;
				AddFailReason("room", sTR_IA_FAIL_DEFAULT);
				sTR_IA_FAIL_DEFAULT = CTTestRoom.strFailReasonLast;
				AddFailReason("debugroom", sTR_IA_FAIL_DEFAULT);
			}
			return false;
		}
	}
	for (int i = 0; i < aSocialPrereqs.Length; i++)
	{
		if (objUs.socUs == null)
		{
			return false;
		}
		JsonPersonSpec personSpec = DataHandler.GetPersonSpec(aSocialPrereqs[i]);
		string matchingRelation = objUs.socUs.GetMatchingRelation(personSpec);
		if (matchingRelation == null)
		{
			return false;
		}
		aSocialPrereqsFound[i] = matchingRelation;
	}
	if (!(strTargetPoint == POINT_REMOTE))
	{
		if (bNoWalk)
		{
			bool flag2 = true;
			bool flag3 = objUs.GetCORef(objThem) != null;
			Tile tileAtWorldCoords = objUs.ship.GetTileAtWorldCoords1(objUs.tf.position.x, objUs.tf.position.y, bAllowDocked: true);
			Tile tile = tileAtWorldCoords;
			if (strTargetPoint != null && strTargetPoint != POINT_REMOTE && !flag3)
			{
				Vector2 pos = objThem.GetPos(strTargetPoint);
				tile = objUs.ship.GetTileAtWorldCoords1(pos.x, pos.y, bAllowDocked: true);
				flag2 = (float)TileUtils.TileRange(tileAtWorldCoords, tile) <= fTargetPointRange;
			}
			if (!flag2)
			{
				if (bVerboseTrigger)
				{
					sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_RANGE_START + objThem.FriendlyName + STR_IA_FAIL_RANGE_END;
					AddFailReason("main", sTR_IA_FAIL_DEFAULT);
				}
				return false;
			}
			if (!Visibility.IsCondOwnerLOSVisibleBlocks(objThem, tileAtWorldCoords.tf.position))
			{
				if (bVerboseTrigger)
				{
					sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_LOS_START + objThem.FriendlyName + STR_IA_FAIL_LOS_END;
					AddFailReason("main", sTR_IA_FAIL_DEFAULT);
				}
				return false;
			}
		}
		else if (bCheckPath)
		{
			Pathfinder component = objUs.GetComponent<Pathfinder>();
			if (component != null)
			{
				Tile tilDestNew = component.tilCurrent;
				if (strTargetPoint != null && strTargetPoint != POINT_REMOTE && objUs.GetCORef(objThem) == null)
				{
					Vector2 pos2 = objThem.GetPos(strTargetPoint);
					tilDestNew = objUs.ship.GetTileAtWorldCoords1(pos2.x, pos2.y, bAllowDocked: true);
				}
				bool bAllowAirlocks = objUs.HasAirlockPermission(bManual);
				PathResult pathResult = component.CheckGoal(tilDestNew, fTargetPointRange, objThem, bAllowAirlocks);
				if (pathResult.Length < 0)
				{
					if (bVerboseTrigger)
					{
						sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_PATH_START + objThem.FriendlyName + STR_IA_FAIL_PATH_END;
						AddFailReason("main", sTR_IA_FAIL_DEFAULT);
						sTR_IA_FAIL_DEFAULT = pathResult.FailReason(objUs);
						if (!string.IsNullOrEmpty(sTR_IA_FAIL_DEFAULT))
						{
							AddFailReason("main", sTR_IA_FAIL_DEFAULT);
						}
					}
					if (pathResult.bAirlockBlocked)
					{
						bAirlockBlocked = pathResult.bAirlockBlocked;
					}
					return false;
				}
			}
		}
	}
	if (strLedgerDef != null)
	{
		JsonLedgerDef ledgerDef = DataHandler.GetLedgerDef(strLedgerDef);
		if (ledgerDef != null && ledgerDef.bPaid && objUs.GetCondAmount(ledgerDef.strCurrency) < (double)ledgerDef.fAmount)
		{
			if (bVerboseTrigger)
			{
				sTR_IA_FAIL_DEFAULT = STR_IA_FAIL_MONEY;
				AddFailReason("them", sTR_IA_FAIL_DEFAULT);
			}
			return false;
		}
	}
	if (bIgnoreItems)
	{
		return true;
	}
	aLootItemGiveContract = null;
	aLootItemUseContract = null;
	aLootItemRemoveContract = null;
	aLootItemTakeContract = null;
	aSeekItemsForContract = new List<CondOwner>();
	if (strLootItmInputs != null)
	{
		bool flag4 = false;
		List<CondTrigger> cTLoot = DataHandler.GetLoot(strLootItmInputs).GetCTLoot(null);
		List<CondOwner> list2 = new List<CondOwner>();
		List<CondOwner> list3 = new List<CondOwner>();
		list3.Add(objThem);
		List<CondOwner> list4 = list3;
		list4.AddRange(objThem.GetLotCOs(bSubItems: true));
		foreach (CondOwner item in list4)
		{
			foreach (CondTrigger item2 in cTLoot)
			{
				if (item2.Triggered(item))
				{
					item2.fCount -= item.StackCount;
				}
				if (item2.fCount <= 0f)
				{
					cTLoot.Remove(item2);
					break;
				}
			}
		}
		if (cTLoot.Count == 0)
		{
			flag4 = true;
		}
		else
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (CondTrigger item3 in cTLoot)
			{
				hashSet.Add(item3.strName);
			}
			List<CondTrigger> list5 = new List<CondTrigger>();
			foreach (string item4 in hashSet)
			{
				list5.Add(DataHandler.GetCondTrigger(item4));
			}
			CondOwnerVisitorAddToHashSet condOwnerVisitorAddToHashSet = new CondOwnerVisitorAddToHashSet();
			CondOwnerVisitorLazyOrCondTrigger visitor = new CondOwnerVisitorLazyOrCondTrigger(condOwnerVisitorAddToHashSet, list5);
			CondOwnerVisitorAddToHashSet a = new CondOwnerVisitorAddToHashSet();
			CondOwnerVisitorLazyOrCondTrigger condOwnerVisitorLazyOrCondTrigger = new CondOwnerVisitorLazyOrCondTrigger(a, list5);
			objUs.VisitCOs(visitor, bAllowLocked: false);
			list2 = new List<CondOwner>(condOwnerVisitorAddToHashSet.aHashSet);
			objUs.ship.VisitCOs(visitor, bSubObjects: true, bAllowDocked: true, bAllowLocked: false);
			list4 = new List<CondOwner>(condOwnerVisitorAddToHashSet.aHashSet);
			foreach (CondOwner item5 in list2)
			{
				list4.Remove(item5);
			}
			list4.InsertRange(0, list2);
			list2.Clear();
			if (objThem.strPersistentCO != null)
			{
				CondOwner value = null;
				if (DataHandler.mapCOs.TryGetValue(objThem.strPersistentCO, out value))
				{
					int num2 = list4.IndexOf(value);
					if (num2 > 0)
					{
						list4.Remove(value);
						list4.Insert(0, value);
					}
				}
			}
			list2 = CheckItemsAvailable(cTLoot, list4, objUs, bSeekFirst: false, objThem.strPersistentCO, bUseTest: false);
			if (list2 != null)
			{
				if (!bFetchItems)
				{
					flag4 = true;
				}
				else
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>();
					foreach (CondTrigger item6 in cTLoot)
					{
						if (dictionary.ContainsKey(item6.strName))
						{
							dictionary[item6.strName]++;
						}
						else
						{
							dictionary[item6.strName] = 1;
						}
					}
					foreach (CondTrigger item7 in cTLoot)
					{
						JsonInteraction value2 = null;
						string text = "ACTFeedItem" + item7.strName;
						DataHandler.dictInteractions.TryGetValue(text, out value2);
						if (value2 == null)
						{
							Loot loot = DataHandler.GetLoot(text);
							if (loot.strName != text)
							{
								loot.strName = text;
								loot.strType = "trigger";
								loot.aCOs = new string[1] { item7.strName + "=1.0x1" };
								DataHandler.dictLoot[text] = loot;
								Debug.Log("Auto-generating Loot: " + text);
							}
							value2 = DataHandler.dictInteractions["ACTFeedItem"].Clone();
							value2.strName = text;
							if (!string.IsNullOrEmpty(objThem.strPlaceholderInstallReq))
							{
								value2.strDesc = value2.strDesc.Replace("[object]", DataHandler.GetCOShortName(objThem.strPlaceholderInstallReq));
							}
							else
							{
								value2.strDesc = value2.strDesc.Replace("[object]", item7.RulesInfo);
							}
							value2.aLootItms = new string[1] { "Give," + text + ",true,false" };
							value2.bLot = true;
							value2.fTargetPointRange = fTargetPointRange;
							DataHandler.dictInteractions[value2.strName] = value2;
						}
						Task2 task = new Task2();
						task.strName = value2.strName;
						task.strInteraction = value2.strName;
						task.strTargetCOID = objThem.strID;
						if (CrewSim.objInstance.workManager.taskUpstream != null)
						{
							task.CopyFrom(CrewSim.objInstance.workManager.taskUpstream);
							task.strDuty = CrewSim.objInstance.workManager.taskUpstream.strDuty;
						}
						else
						{
							task.strDuty = "Haul";
						}
						task.bManual = false;
						CrewSim.objInstance.workManager.AddTask(task, dictionary[item7.strName]);
						sTR_IA_FAIL_DEFAULT = "Items required first. Adding tasks now.";
						AddFailReason("main", sTR_IA_FAIL_DEFAULT);
						Debug.Log(sTR_IA_FAIL_DEFAULT);
					}
				}
			}
		}
		list4 = null;
		list2 = null;
		cTLoot = null;
		if (!flag4)
		{
			if (bStats && DataHandler.dictSocialStats.ContainsKey(strName))
			{
				DataHandler.dictSocialStats[strName].nMissingItem++;
			}
			return false;
		}
	}
	if (strLootCTsLacks != null)
	{
		List<CondOwner> list4 = objUs.GetCOs(bAllowLocked: false);
		List<CondOwner> list2 = new List<CondOwner>();
		CondTrigger condTrigger = DataHandler.GetCondTrigger("TIs[us]");
		List<CondTrigger> cTLootFlat = DataHandler.GetLoot(strLootCTsLacks).GetCTLootFlat(condTrigger);
		List<CondOwner> list6 = CheckItemsAvailable(cTLootFlat, list4, objUs, bSeekFirst: false, null, bUseTest: false);
		list4 = null;
		list2 = null;
		if (list6 != null)
		{
			if (bStats && DataHandler.dictSocialStats.ContainsKey(strName))
			{
				DataHandler.dictSocialStats[strName].nMissingItem++;
			}
			return false;
		}
	}
	if (strLootCTsGive != null || strLootCTsRemoveUs != null || strLootCTsUse != null)
	{
		List<CondOwner> list4 = objUs.GetCOsSafe(bAllowLocked: false);
		if (bGetItemBefore)
		{
			list4.AddRange(objUs.ship.GetCOs(ctNotCarried, bSubObjects: true, bAllowDocked: true, bAllowLocked: false));
		}
		CondTrigger condTrigger2 = DataHandler.GetCondTrigger("TIs[us]");
		List<CondTrigger> cTLootFlat2 = DataHandler.GetLoot(strLootCTsGive).GetCTLootFlat(condTrigger2);
		List<CondTrigger> cTLootFlat3 = DataHandler.GetLoot(strLootCTsUse).GetCTLootFlat(condTrigger2);
		List<CondTrigger> cTLootFlat4 = DataHandler.GetLoot(strLootCTsRemoveUs).GetCTLootFlat(condTrigger2);
		if (objThem.strPersistentCO != null)
		{
			CondOwner value3 = null;
			if (DataHandler.mapCOs.TryGetValue(objThem.strPersistentCO, out value3))
			{
				int num3 = list4.IndexOf(value3);
				if (num3 > 0)
				{
					list4.Remove(value3);
					list4.Insert(0, value3);
				}
			}
		}
		aLootItemGiveContract = CheckItemsAvailable(cTLootFlat2, list4, objUs, bSeekFirst: true, objThem.strPersistentCO, bUseTest: false);
		aLootItemRemoveContract = CheckItemsAvailable(cTLootFlat4, list4, objUs, bSeekFirst: true, objThem.strPersistentCO, bUseTest: false);
		list4.Remove(objThem);
		aLootItemUseContract = CheckItemsAvailable(cTLootFlat3, list4, objUs, bSeekFirst: true, objThem.strPersistentCO, strUseCase != null);
		bool flag5 = (cTLootFlat2.Count > 0 && aLootItemGiveContract == null) || (cTLootFlat3.Count > 0 && aLootItemUseContract == null) || (cTLootFlat4.Count > 0 && aLootItemRemoveContract == null);
		list4 = null;
		List<CondOwner> list2 = null;
		cTLootFlat2 = null;
		cTLootFlat3 = null;
		cTLootFlat4 = null;
		condTrigger2 = null;
		if (flag5)
		{
			aSeekItemsForContract.Clear();
			if (bStats && DataHandler.dictSocialStats.ContainsKey(strName))
			{
				DataHandler.dictSocialStats[strName].nMissingItem++;
			}
			return false;
		}
	}
	else
	{
		aLootItemGiveContract = new List<CondOwner>();
		aLootItemUseContract = new List<CondOwner>();
		aLootItemRemoveContract = new List<CondOwner>();
	}
	if (strLootCTsTake != null)
	{
		List<CondOwner> list4 = objThem.GetCOs(bAllowLocked: false);
		CondTrigger condTrigger3 = DataHandler.GetCondTrigger("TIs[us]");
		List<CondTrigger> cTLootFlat5 = DataHandler.GetLoot(strLootCTsTake).GetCTLootFlat(condTrigger3);
		aLootItemTakeContract = CheckItemsAvailable(cTLootFlat5, list4, objUs, bSeekFirst: false, null, bUseTest: false);
		list4 = null;
		List<CondOwner> list2 = null;
	}
	else
	{
		aLootItemTakeContract = new List<CondOwner>();
	}
	if (aLootItemTakeContract == null)
	{
		if (bStats && DataHandler.dictSocialStats.ContainsKey(strName))
		{
			DataHandler.dictSocialStats[strName].nMissingItem++;
		}
		return false;
	}
	mapFails["main"][0] = string.Empty;
	return true;
}
*/

/* Interaction.ApplyLogging
public void ApplyLogging(string strOwner, bool bTraitSuffix)
{
	if (nLogging == Logging.NONE || bLogged)
	{
		return;
	}
	string text = GetText(bTraitSuffix);
	List<CondOwner> list = new List<CondOwner>();
	switch (nLogging)
	{
	case Logging.SHIP:
		if (objUs.ship != null)
		{
			list.AddRange(objUs.ship.GetPeople(bAllowDocked: true));
		}
		break;
	case Logging.GROUP:
		list.Add(objUs);
		if (strThemType == TARGET_OTHER)
		{
			list.Add(objThem);
		}
		break;
	case Logging.ROOM:
		if (objUs.currentRoom != null)
		{
			list.AddRange(objUs.ship.GetPeopleInRoom(objUs.currentRoom));
		}
		if (!list.Contains(objUs))
		{
			list.Add(objUs);
		}
		if (!list.Contains(objThem))
		{
			list.Add(objThem);
		}
		break;
	}
	foreach (CondOwner item in list)
	{
		item.LogMessage(text, strColor, strOwner);
	}
	bLogged = true;
}
*/