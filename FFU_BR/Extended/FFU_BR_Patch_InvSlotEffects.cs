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
using UnityEngine;

public partial class patch_JsonCondOwner : JsonCondOwner {
    public string strInvSlotEffect { get; set; }
}

public partial class patch_CondOwner : CondOwner {
    public JsonSlotEffects jsInvSlotEffect;
    public extern void orig_SetData(JsonCondOwner jid, bool bLoot, JsonCondOwnerSave jCOSIn);
    public void SetData(patch_JsonCondOwner jid, bool bLoot, JsonCondOwnerSave jCOSIn) {
        orig_SetData(jid, bLoot, jCOSIn);
        ParseInvEffects(jid);
    }
    public void ParseInvEffects(patch_JsonCondOwner jid) {
        if (jid.strInvSlotEffect != null) {
            JsonSlotEffects slotEffect = DataHandler.GetSlotEffect(jid.strInvSlotEffect);
            if (slotEffect != null) {
                if (Container.GetSpace(this) < 1)
                    Debug.LogWarning($"Can't assign 'invSlotEffect' " +
                        $"for [{strName}] without inventory grid.");
                else jsInvSlotEffect = slotEffect;
            }
        }
    }
}

public partial class patch_Container : Container {
	[MonoModIgnore] patch_CondOwner CO => (patch_CondOwner)base.CO;
    [MonoModReplace] public void SetIsInContainer(CondOwner co) {
        if (CO == this) Debug.Log("ERROR: Assigning self as own parent.");
        co.objCOParent = CO;
        if (co.coStackHead == null) {
            co.tf.SetParent(CO.tf);
            co.tf.localPosition = new Vector3(0f, 0f, fZSubOffset);
            co.Visible = false;
        }
        if (!CO.HasCond("IsHuman")) {
            if (CO.jsInvSlotEffect != null)
                ApplyContainerEffects(co, CO.jsInvSlotEffect);
            co.AddCondAmount("IsInContainer", 1.0);
        }
        CondOwner targetCO = CO;
        while (targetCO != null) {
            if (targetCO.HasCond("IsHuman") || targetCO.HasCond("IsRobot")) {
                co.AddCondAmount("IsCarried", 1.0);
                CondOwnerVisitorAddCond condOwnerVisitorAddCond = new CondOwnerVisitorAddCond();
                condOwnerVisitorAddCond.strCond = "IsCarried";
                condOwnerVisitorAddCond.fAmount = 1.0;
                co.VisitCOs(condOwnerVisitorAddCond, true);
                break;
            }
            targetCO = targetCO.objCOParent;
        }
    }

    [MonoModReplace] public void ClearIsInContainer(CondOwner co) {
        if (CO.jsInvSlotEffect != null)
			ApplyContainerEffects(co, CO.jsInvSlotEffect, true);
        co.ZeroCondAmount("IsInContainer");
        co.ZeroCondAmount("IsCarried");
        CondOwnerVisitorZeroCond condOwnerVisitorZeroCond = new CondOwnerVisitorZeroCond();
        condOwnerVisitorZeroCond.strCond = "IsCarried";
        co.VisitCOs(condOwnerVisitorZeroCond, true);
        co.objCOParent = null;
    }

    private void ApplyContainerEffects(CondOwner co, JsonSlotEffects jse, bool bRemove = false) {
        if (CO == null || co == null || jse == null) return;
        co.ValidateParent();
        Slots.ApplyIAEffects(CO, co, jse, bRemove, false);
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
CondOwner.SetData
public void SetData(JsonCondOwner jid, bool bLoot = true, JsonCondOwnerSave jCOSIn = null)
{
	if (jid == null)
	{
		return;
	}
	jCOS = jCOSIn;
	strName = jid.strName;
	strNameFriendly = jid.strNameFriendly;
	strNameShort = jid.strNameShort;
	strDesc = jid.strDesc;
	strCODef = jid.strCODef;
	if (strCODef == null)
	{
		strCODef = jid.strName;
	}
	strItemDef = jid.strItemDef;
	strPortraitImg = jid.strPortraitImg;
	nStackLimit = Mathf.Max(1, jid.nStackLimit);
	bSaveMessageLog = jid.bSaveMessageLog;
	bSlotLocked = jid.bSlotLocked;
	strType = jid.strType;
	if (jid.aInteractions != null)
	{
		aInteractions = new List<string>(jid.aInteractions);
	}
	else
	{
		aInteractions = new List<string>();
	}
	CheckInteractionFlag();
	if (jCOSIn != null)
	{
		strSourceInteract = jCOSIn.strSourceInteract;
		strSourceCO = jCOSIn.strSourceCO;
		strPersistentCT = jCOSIn.strPersistentCT;
		strPersistentCO = jCOSIn.strPersistentCO;
		strIdleAnim = jCOSIn.strIdleAnim;
		strLastSocial = jCOSIn.strLastSocial;
		pairInventoryXY = new PairXY(jCOSIn.inventoryX, jCOSIn.inventoryY);
	}
	else
	{
		strSourceInteract = null;
		strSourceCO = null;
		strPersistentCO = null;
		strPersistentCT = null;
		strLastSocial = null;
	}
	if (jid.aSlotsWeHave != null)
	{
		compSlots = base.gameObject.AddComponent<Slots>();
		string[] aSlotsWeHave = jid.aSlotsWeHave;
		foreach (string text in aSlotsWeHave)
		{
			compSlots.AddSlot(DataHandler.GetSlot(text));
		}
	}
	if (jid.strContainerCT != null)
	{
		objContainer = base.gameObject.AddComponent<Container>();
		CondOwner cO = objContainer.CO;
		objContainer.ctAllowed = DataHandler.GetCondTrigger(jid.strContainerCT);
		if (!CrewSim.bSaveUsesOldContainerGrids && jid.nContainerHeight != 0 && jid.nContainerWidth != 0)
		{
			objContainer.gridLayout = new GridLayout(jid.nContainerWidth, jid.nContainerHeight);
		}
	}
	bLogConds = false;
	string[] array = jid.aStartingCondRules;
	if (jCOSIn != null)
	{
		array = jCOSIn.aCondRules;
	}
	if (array != null)
	{
		if (array.Contains("DEFAULT"))
		{
			List<string> list = array.ToList();
			list.Remove("DEFAULT");
			JsonCondOwner condOwnerDef = DataHandler.GetCondOwnerDef(strCODef);
			if (condOwnerDef != null && condOwnerDef.aStartingCondRules != null)
			{
				list.AddRange(condOwnerDef.aStartingCondRules);
			}
			array = list.ToArray();
		}
		bool flag = bIgnoreKill;
		if (jCOSIn == null)
		{
			bIgnoreKill = true;
		}
		string[] array2 = array;
		foreach (string strCondRule in array2)
		{
			AddCondRule(strCondRule, bApplyEffects: false);
		}
		bIgnoreKill = flag;
	}
	bFreezeCondRules = true;
	if (jid.aUpdateCommands != null)
	{
		string[] aUpdateCommands = jid.aUpdateCommands;
		foreach (string strDef in aUpdateCommands)
		{
			AddCommand(strDef);
		}
	}
	if (nStackLimit > 1)
	{
		AddCondAmount("IsStacking", nStackLimit - 1);
	}
	string[] array3 = jid.aStartingConds;
	bool flag2 = false;
	if (jCOSIn != null)
	{
		array3 = jCOSIn.aConds;
		flag2 = jCOSIn.aCondReveals != null && array3.Length == 2 * jCOSIn.aCondReveals.Length;
	}
	if (array3 != null)
	{
		if (array3.Contains("DEFAULT"))
		{
			List<string> list2 = array3.ToList();
			list2.Remove("DEFAULT");
			JsonCondOwner condOwnerDef2 = DataHandler.GetCondOwnerDef(strCODef);
			list2.AddRange(condOwnerDef2.aStartingConds);
			array3 = list2.ToArray();
		}
		for (int l = 0; l < array3.Length; l++)
		{
			string text2 = array3[l];
			string[] array4 = text2.Split('=');
			if (array4.Length > 1)
			{
				ZeroCondAmount(array4[0]);
			}
			string text3 = ParseCondEquation(text2);
			Condition value = null;
			if (text3 != null && mapConds != null && mapConds.TryGetValue(text3, out value))
			{
				if (flag2)
				{
					value.nDisplaySelf = jCOSIn.aCondReveals[2 * l];
					value.nDisplayOther = jCOSIn.aCondReveals[2 * l + 1];
				}
				if (value.bRemoveOnLoad)
				{
					ZeroCondAmount(text3);
				}
			}
		}
	}
	if (jCOSIn == null)
	{
		foreach (CondRule value2 in mapCondRules.Values)
		{
			AddCondRuleEffects(value2, 1f);
			if (value2.fPref != 0.0)
			{
				hashCondsImportant.Add(value2.strCond);
			}
		}
		bFreezeCondRules = false;
	}
	bLogConds = true;
	if (jCOSIn != null)
	{
		bFreezeConds = true;
	}
	JsonTicker[] array5 = null;
	if (jCOSIn != null)
	{
		array5 = jCOSIn.aTickers;
		JsonTicker[] array6 = new JsonTicker[aTickers.Count];
		aTickers.CopyTo(array6);
		JsonTicker[] array7 = array6;
		foreach (JsonTicker jtRemove in array7)
		{
			RemoveTicker(jtRemove);
		}
	}
	else if (jid.aTickers != null)
	{
		array5 = new JsonTicker[jid.aTickers.Length];
		for (int n = 0; n < jid.aTickers.Length; n++)
		{
			array5[n] = DataHandler.GetTicker(jid.aTickers[n]);
		}
	}
	if (array5 != null)
	{
		JsonTicker[] array8 = array5;
		foreach (JsonTicker jsonTicker in array8)
		{
			if (jsonTicker != null)
			{
				jsonTicker.SetTimeLeft(jsonTicker.fTimeLeft);
				AddTicker(jsonTicker.Clone());
			}
		}
	}
	mapSlotEffects = new Dictionary<string, JsonSlotEffects>();
	if (jid.mapSlotEffects != null)
	{
		Dictionary<string, string> dictionary = DataHandler.ConvertStringArrayToDict(jid.mapSlotEffects);
		foreach (KeyValuePair<string, string> item6 in dictionary)
		{
			JsonSlotEffects slotEffect = DataHandler.GetSlotEffect(item6.Value);
			if (slotEffect != null)
			{
				mapSlotEffects[item6.Key] = slotEffect;
				slotEffect.strSlotPrimary = item6.Key;
				if (slotEffect.aSlotsSecondary != null)
				{
					string[] aSlotsSecondary = slotEffect.aSlotsSecondary;
					foreach (string key in aSlotsSecondary)
					{
						mapSlotEffects[key] = slotEffect;
					}
				}
			}
			else
			{
				Debug.Log(strName + " was unable to load sloteffect Key: " + item6.Key + " Value: " + item6.Value);
			}
		}
	}
	mapChargeProfiles = new Dictionary<string, JsonChargeProfile>();
	if (jid.mapChargeProfiles != null)
	{
		Dictionary<string, string> dictionary2 = DataHandler.ConvertStringArrayToDict(jid.mapChargeProfiles);
		foreach (KeyValuePair<string, string> item7 in dictionary2)
		{
			JsonChargeProfile chargeProfile = DataHandler.GetChargeProfile(item7.Value);
			if (chargeProfile != null)
			{
				mapChargeProfiles[item7.Key] = chargeProfile;
			}
		}
	}
	if (jid.mapAltItemDefs != null)
	{
		mapAltItemDefs = DataHandler.ConvertStringArrayToDict(jid.mapAltItemDefs);
	}
	else
	{
		mapAltItemDefs = new Dictionary<string, string>();
	}
	if (jid.mapAltSlotImgs != null)
	{
		mapAltSlotImgs = DataHandler.ConvertStringArrayToDict(jid.mapAltSlotImgs);
	}
	else
	{
		mapAltSlotImgs = new Dictionary<string, string>();
	}
	if (bLoot && jCOSIn == null)
	{
		List<CondOwner> cOLoot = DataHandler.GetLoot(jid.strLoot).GetCOLoot(this, bSuppressOverride: false);
		foreach (CondOwner item8 in cOLoot)
		{
			if (compSlots != null && item8.mapSlotEffects.Keys.Count > 0)
			{
				foreach (string key2 in item8.mapSlotEffects.Keys)
				{
					if (compSlots.SlotItem(key2, item8))
					{
						break;
					}
				}
			}
			else
			{
				CondOwner condOwner = AddCO(item8, bEquip: true, bOverflow: true, bIgnoreLocks: true);
				if (condOwner != null)
				{
					condOwner.Destroy();
				}
			}
		}
	}
	if (jid.mapPoints != null)
	{
		string[] array9 = jid.mapPoints;
		foreach (string text4 in array9)
		{
			string[] array10 = text4.Split(',');
			float result = 0f;
			if (array10[1] == "9e99")
			{
				result = float.PositiveInfinity;
			}
			else
			{
				float.TryParse(array10[1], out result);
			}
			float result2 = 0f;
			if (array10[2] == "9e99")
			{
				result2 = float.PositiveInfinity;
			}
			else
			{
				float.TryParse(array10[2], out result2);
			}
			mapPoints[array10[0]] = new Vector2(result, result2);
		}
	}
	if (jid.jsonPI != null)
	{
		pwr = base.gameObject.AddComponent<Powered>();
		pwr.SetData(jid.jsonPI);
	}
	if (jid.mapGUIPropMaps != null)
	{
		Dictionary<string, string> dictionary3 = DataHandler.ConvertStringArrayToDict(jid.mapGUIPropMaps);
		foreach (string key3 in dictionary3.Keys)
		{
			mapGUIPropMaps[key3] = DataHandler.GetGUIPropMap(dictionary3[key3]);
		}
	}
	if (jid.aComponents != null)
	{
		string[] aComponents = jid.aComponents;
		foreach (string typeName in aComponents)
		{
			Type type = Type.GetType(typeName);
			base.gameObject.AddComponent(type);
		}
	}
	if (jCOSIn != null)
	{
		bAlive = jCOSIn.bAlive;
		if (jid.bSaveMessageLog)
		{
			if (jCOSIn.aMsgColors != null)
			{
				for (int num5 = 0; num5 < jCOSIn.aMsgColors.Length && num5 < jCOSIn.aMessages.Length; num5++)
				{
					JsonLogMessage jsonLogMessage = new JsonLogMessage();
					jsonLogMessage.strName = Guid.NewGuid().ToString();
					jsonLogMessage.strMessage = jCOSIn.aMessages[num5];
					jsonLogMessage.strColor = jCOSIn.aMsgColors[num5];
					jsonLogMessage.strOwner = "n/a";
					if (jsonLogMessage.strMessage.IndexOf(jCOSIn.strID) == 0)
					{
						jsonLogMessage.strOwner = jCOSIn.strID;
					}
					jsonLogMessage.fTime = StarSystem.fEpoch - (double)jCOSIn.aMsgColors.Length + (double)num5;
					this.aMessages.Add(jsonLogMessage);
				}
			}
			if (jCOSIn.aMessages2 != null)
			{
				JsonLogMessage[] aMessages = jCOSIn.aMessages2;
				foreach (JsonLogMessage item in aMessages)
				{
					this.aMessages.Add(item);
				}
			}
		}
		if (jCOSIn.aCondZeroes != null)
		{
			string[] array11 = jCOSIn.aCondZeroes;
			foreach (string item2 in array11)
			{
				aCondZeroes.Add(item2);
			}
		}
		if (jCOSIn.mapDGasMols != null)
		{
			GasContainer gasContainer = GasContainer;
			if (gasContainer != null)
			{
				float result3 = 0f;
				gasContainer.fDGasTemp = jCOSIn.fDGasTemp;
				if (gasContainer.fDGasTemp == 0.0)
				{
					gasContainer.fDGasTemp = 0.001;
				}
				string[] mapDGasMols = jCOSIn.mapDGasMols;
				foreach (string text5 in mapDGasMols)
				{
					string[] array12 = text5.Split(',');
					float.TryParse(array12[1], out result3);
					gasContainer.mapDGasMols[array12[0]] = result3;
				}
			}
		}
		fLastICOUpdate = jCOSIn.fLastICOUpdate;
		fMSRedamageAmount = jCOSIn.fMSRedamageAmount;
		if (jCOSIn.aQueue != null)
		{
			JsonInteractionSave[] array13 = jCOSIn.aQueue;
			foreach (JsonInteractionSave jsonInteractionSave in array13)
			{
				Interaction interaction = DataHandler.GetInteraction(jsonInteractionSave.strName, jsonInteractionSave);
				if (interaction == null)
				{
					Debug.Log("Interaction " + jsonInteractionSave.strName + " is missing from the DataHandler. This should probably be a crash...");
					continue;
				}
				aQueue.Add(interaction);
				if (interaction.strName == "Walk" && interaction.strTargetPoint == null)
				{
					interaction.strTargetPoint = "use";
				}
			}
		}
		if (jCOSIn.aReplies != null)
		{
			ReplyThread[] array14 = jCOSIn.aReplies;
			foreach (ReplyThread replyThread in array14)
			{
				ReplyThread replyThread2 = new ReplyThread();
				replyThread2.fEpoch = replyThread.fEpoch;
				replyThread2.jis = replyThread.jis;
				replyThread2.strID = replyThread.strID;
				aReplies.Add(replyThread2);
			}
		}
		if (jCOSIn.aAttackIAs != null && !CrewSim.objInstance.VersionIsOlderThan(CrewSim.aSaveVersion, new int[4] { 0, 14, 0, 12 }))
		{
			ApplyAModes(jCOSIn.aAttackIAs, bRebuildQAB: false);
		}
		if (jCOSIn.dictRecentlyTried != null)
		{
			foreach (KeyValuePair<string, double> item9 in jCOSIn.dictRecentlyTried)
			{
				dictRecentlyTried.Add(item9.Key, item9.Value);
			}
		}
		if (jCOSIn.dictRememberScores != null)
		{
			foreach (KeyValuePair<string, double> dictRememberScore in jCOSIn.dictRememberScores)
			{
				dictRememberScores.Add(dictRememberScore.Key, dictRememberScore.Value);
			}
		}
		if (jCOSIn.aRememberIAs != null)
		{
			string[] array15 = jCOSIn.aRememberIAs;
			foreach (string item3 in array15)
			{
				aRememberIAs.Add(item3);
			}
		}
		if (jCOSIn.aMyShips != null)
		{
			string[] array16 = jCOSIn.aMyShips;
			foreach (string item4 in array16)
			{
				aMyShips.Add(item4);
			}
		}
		if (jCOSIn.aFactions != null)
		{
			string[] array17 = jCOSIn.aFactions;
			foreach (string item5 in array17)
			{
				aFactions.Add(item5);
			}
		}
		if (jCOSIn.mapIAHist2 != null)
		{
			this.mapIAHist = new Dictionary<string, CondHistory>();
			if (jCOSIn.mapIAHist2 != null)
			{
				this.mapIAHist = new Dictionary<string, CondHistory>();
				JsonCondHistory[] mapIAHist = jCOSIn.mapIAHist2;
				foreach (JsonCondHistory jsonCondHistory in mapIAHist)
				{
					if (!CrewSim.bSaveHasENCPoliceBoard || jsonCondHistory.strCondName.IndexOf("ENCPoliceBoard") != 0)
					{
						this.mapIAHist[jsonCondHistory.strCondName] = jsonCondHistory.GetData();
					}
				}
			}
		}
		if (jCOSIn.social != null)
		{
			socUs = base.gameObject.AddComponent<Social>();
			socUs.Init(jCOSIn.social);
		}
		if (jCOSIn.cgs != null)
		{
			GUIChargenStack gUIChargenStack = base.gameObject.AddComponent<GUIChargenStack>();
			gUIChargenStack.Init(jCOSIn.cgs);
			JsonPersonSpec jsonPersonSpec = new JsonPersonSpec();
			jsonPersonSpec.bAlive = bAlive;
			int nAgeMax = (jsonPersonSpec.nAgeMin = Convert.ToInt32(GetCondAmount("StatAge")));
			jsonPersonSpec.nAgeMax = nAgeMax;
			if (gUIChargenStack.GetLatestCareer() != null)
			{
				jsonPersonSpec.strCareerNow = gUIChargenStack.GetLatestCareer().GetJC().strName;
			}
			jsonPersonSpec.strFirstName = gUIChargenStack.strFirstName;
			jsonPersonSpec.strLastName = gUIChargenStack.strLastName;
			jsonPersonSpec.strGender = "IsMale";
			if (HasCond("IsFemale"))
			{
				jsonPersonSpec.strGender = "IsFemale";
			}
			else if (HasCond("IsNB"))
			{
				jsonPersonSpec.strGender = "IsNB";
			}
			string strHomeworldFind = (jsonPersonSpec.strHomeworldSet = gUIChargenStack.GetHomeworld().strATCCode);
			jsonPersonSpec.strHomeworldFind = strHomeworldFind;
			pspec = new PersonSpec(jsonPersonSpec, bNew: false);
			pspec.nStrata = gUIChargenStack.Strata;
			pspec.strCO = strID;
		}
		if (jCOSIn.aPledges != null)
		{
			JsonPledgeSave[] aPledges = jCOSIn.aPledges;
			foreach (JsonPledgeSave jsonPledgeSave in aPledges)
			{
				if (CrewSim.bSaveHasMissingPledgeUs && jsonPledgeSave.strUsID != jCOSIn.strID)
				{
					string strUsID = jsonPledgeSave.strUsID;
					jsonPledgeSave.strUsID = jCOSIn.strID;
					if (jsonPledgeSave.strThemID == strUsID)
					{
						jsonPledgeSave.strThemID = jsonPledgeSave.strUsID;
					}
				}
				Pledge2 pledge = PledgeFactory.Factory(jsonPledgeSave);
				if (pledge != null)
				{
					AddPledge(pledge);
				}
			}
		}
		if (jCOSIn.strComp != null)
		{
			Company = CrewSim.system.GetCompany(jCOSIn.strComp);
		}
	}
	if (jid.strAudioEmitter != null)
	{
		JsonAudioEmitter audioEmitter = DataHandler.GetAudioEmitter(jid.strAudioEmitter);
		if (audioEmitter != null)
		{
			AudioEmitter audioEmitter2 = base.gameObject.AddComponent<AudioEmitter>();
			audioEmitter2.SetData(audioEmitter);
			audioEmitter2.RandomizePitchAll();
			audioEmitter2.FadeInSteady();
		}
	}
	SetUpBehaviours();
}

Container.SetIsInContainer
public void SetIsInContainer(CondOwner co)
{
	if (CO == this)
	{
		Debug.Log("ERROR: Assigning self as own parent.");
	}
	co.objCOParent = CO;
	if (co.coStackHead == null)
	{
		co.tf.SetParent(CO.tf);
		co.tf.localPosition = new Vector3(0f, 0f, fZSubOffset);
		co.Visible = false;
	}
	if (!CO.HasCond("IsHuman"))
	{
		co.AddCondAmount("IsInContainer", 1.0);
	}
	CondOwner condOwner = CO;
	while (condOwner != null)
	{
		if (condOwner.HasCond("IsHuman") || condOwner.HasCond("IsRobot"))
		{
			co.AddCondAmount("IsCarried", 1.0);
			CondOwnerVisitorAddCond condOwnerVisitorAddCond = new CondOwnerVisitorAddCond();
			condOwnerVisitorAddCond.strCond = "IsCarried";
			condOwnerVisitorAddCond.fAmount = 1.0;
			co.VisitCOs(condOwnerVisitorAddCond, bAllowLocked: true);
			break;
		}
		condOwner = condOwner.objCOParent;
	}
}

Container.ClearIsInContainer
public void ClearIsInContainer(CondOwner co)
{
	co.ZeroCondAmount("IsInContainer");
	co.ZeroCondAmount("IsCarried");
	CondOwnerVisitorZeroCond condOwnerVisitorZeroCond = new CondOwnerVisitorZeroCond();
	condOwnerVisitorZeroCond.strCond = "IsCarried";
	co.VisitCOs(condOwnerVisitorZeroCond, bAllowLocked: true);
	co.objCOParent = null;
}

Slots.ApplySlotEffects
private void ApplySlotEffects(Slot slot, CondOwner co, JsonSlotEffects jse, bool bRemove = false)
{
	if (co == null || slot == null || jse == null)
	{
		return;
	}
	co.ValidateParent();
	if (!co.mapSlotEffects.TryGetValue(slot.strName, out jse))
	{
		return;
	}
	ApplyIAEffects(slot.compSlots.coUs, co, jse, bRemove, bParent: false);
	Crew crew = coUs.Crew;
	if (crew == null)
	{
		crew = GetComponentInParent<Crew>();
	}
	ApplyMeshEffects(slot, co, crew, bRemove);
	if (jse.aSlotsAdded == null)
	{
		return;
	}
	string[] aSlotsAdded = jse.aSlotsAdded;
	foreach (string text in aSlotsAdded)
	{
		if (bRemove)
		{
			RemoveSlot(text);
			continue;
		}
		JsonSlot slot2 = DataHandler.GetSlot(text);
		if (slot2 != null)
		{
			AddSlot(slot2);
		}
	}
}
*/