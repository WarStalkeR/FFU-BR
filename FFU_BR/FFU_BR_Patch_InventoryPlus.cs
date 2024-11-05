#pragma warning disable CS0108
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
using Ostranauts.Core;
using Ostranauts.Objectives;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class patch_JsonCondOwner : JsonCondOwner {
    public string invSlotEffect { get; set; }
}

public partial class patch_CondOwner : CondOwner {
    public JsonSlotEffects invSlotEffect;
    public extern void orig_SetData(JsonCondOwner jid, bool bLoot, JsonCondOwnerSave jCOSIn);
    public void SetData(patch_JsonCondOwner jid, bool bLoot, JsonCondOwnerSave jCOSIn) {
        orig_SetData(jid, bLoot, jCOSIn);
        if (jid.invSlotEffect != null) {
            JsonSlotEffects slotEffect = DataHandler.GetSlotEffect(jid.invSlotEffect);
            if (slotEffect != null) {
                if (Container.GetSpace(this) < 1)
					Debug.LogWarning($"Can't assign 'invSlotEffect' " +
						$"for [{strName}] without inventory grid."); 
				else invSlotEffect = slotEffect;
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
            if (CO.invSlotEffect != null)
                ApplyContainerEffects(co, CO.invSlotEffect);
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
        if (CO.invSlotEffect != null)
			ApplyContainerEffects(co, CO.invSlotEffect, true);
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

public partial class patch_Slot : Slot {
	[MonoModIgnore] public extern patch_Slot(JsonSlot jslot);
	[MonoModReplace] public bool CanFit(CondOwner coFit) {
        if (aCOs == null) return false;
        foreach (CondOwner condOwner in aCOs) {
            if (bHoldSlot && condOwner == null && 
				coFit.mapSlotEffects.ContainsKey(strName)) {
                return true;
            }
            CondOwner coParent = condOwner.objCOParent;
            while (coParent != null) {
                if (coFit == coParent) return false;
                coParent = coParent.objCOParent;
            }
            if (condOwner != null && condOwner.objContainer != null && 
				(condOwner.objContainer.ctAllowed == null || 
				condOwner.objContainer.ctAllowed.Triggered(coFit)) && 
				condOwner.objContainer.GetSpaceAvailable() > 0) {
                return true;
            }
        }
        return false;
    }
}

public partial class patch_Container : Container {
    [MonoModReplace] public bool AllowedCO(CondOwner coIn) {
        if (coIn == null || coIn == CO)
			return false;
        CondOwner coParent = CO.objCOParent;
		while (coParent != null) {
			if (coIn == coParent) return false;
			coParent = coParent.objCOParent;
		}
        if (ctAllowed != null)
			return ctAllowed.Triggered(coIn);
        return true;
    }
}

public partial class patch_GUIInventory : GUIInventory {
    public void SpawnInventoryWindow(patch_CondOwner CO, InventoryWindowType type, bool bFlyIn) {
        if (CO == null) return;
        CanvasManager.ShowCanvasGroup(canvasGroup);
        bool isValid = type == InventoryWindowType.Ground || CO.objContainer != null;
        for (int i = 0; i < activeWindows.Count; i++) {
            if (!isValid) break;
            if (activeWindows[i].CO == CO && type == activeWindows[i].type) 
				isValid = false;
        }
        if (isValid) {
            GameObject invGO = Object.Instantiate(inventoryGridPrefab, base.transform);
            GUIInventoryWindow winCurr = invGO.GetComponent<GUIInventoryWindow>();
            activeWindows.Add(winCurr);
            winCurr.SetData(CO, type);
            int winIndex = activeWindows.IndexOf(winCurr);
            GUIInventoryWindow winPrev = null;
            if (winIndex > 0) winPrev = activeWindows[winIndex - 1];
            winCurr.transform.localPosition = GetWindowPosition(winCurr, winPrev) 
				* 1.5f * CanvasManager.CanvasRatio;
            CanvasManager.SetAnchorsToCorners(winCurr.transform);
            StartCoroutine(FlyIn(winCurr));
        }
		List<Slot> sortedSlots = FFU_BR_Defs.StrictInvSorting ? 
			CO.GetSortedSlots() : CO.GetSlots(true);
        foreach (Slot slot in sortedSlots) {
            if (slot.strName == "social") {
                CondOwner coSlotted = slot.aCOs.FirstOrDefault();
                SpawnSocialMovesWindow(coSlotted);
            } else {
                if (slot.bHide) continue;
                CondOwner[] aCOs = slot.aCOs;
                foreach (CondOwner condOwner in aCOs) {
                    if (condOwner != null && condOwner.objContainer != null 
						&& CTOpenInv.Triggered(condOwner)) {
                        SpawnInventoryWindow(condOwner, InventoryWindowType.Container, bFlyIn);
                    }
                }
            }
        }
        if (CrewSim.coPlayer.HasCond("TutorialLockerWaiting") && 
            instance.IsCOShown(CrewSim.coPlayer) && (CO.HasCond("TutorialLockerTarget") || 
            (CO.HasCond("IsStorageFurniture") && CrewSim.coPlayer.HasCond("IsENCFirstDockOKLG")))) {
            CrewSim.coPlayer.ZeroCondAmount("TutorialLockerWaiting");
            MonoSingleton<ObjectiveTracker>.Instance.CheckObjective(CrewSim.coPlayer.strID);
        }
    }
}

public partial class patch_CondOwner : CondOwner {
    public List<Slot> GetSortedSlots() {
        if (compSlots == null) 
			return _emptySlotsResult ?? 
				(_emptySlotsResult = new List<Slot>());
        List<Slot> sortedSlots = new List<Slot>();
        sortedSlots = RecursiveSlots(compSlots.aSlots.ToList(),
			FFU_BR_Defs.ActLogging >= FFU_BR_Defs.ActLogs.Runtime);
        return sortedSlots;

		// Depth Sorting Method
        int SortByDepth(Slot s1, Slot s2) {
            if (s1 == null || s2 == null) return 0;
            return s1.nDepth.CompareTo(s2.nDepth);
        }

		// Recursive Slot Sorting
		List<Slot> RecursiveSlots(List<Slot> refSlots, bool dLog, bool dSort = true, int sDepth = 0) {
			if (refSlots != null && refSlots.Count > 0) {
				sortedSlots = new List<Slot>();
				refSlots.Sort(SortByDepth);
				foreach (Slot refSlot in refSlots) {
					if (dLog) Debug.Log($"#Info# Sorted Slot " +
						$"{string.Empty.PadLeft(sDepth, '=')}> {refSlot.strName}");
					sortedSlots.Add(refSlot);
					sortedSlots.AddRange(RecursiveSlots(
						refSlot.GetSlots(false, false), dLog, dSort, sDepth + 1));
				}
				return sortedSlots;
			} else return new List<Slot>();
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
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

public void ClearIsInContainer(CondOwner co)
{
	co.ZeroCondAmount("IsInContainer");
	co.ZeroCondAmount("IsCarried");
	CondOwnerVisitorZeroCond condOwnerVisitorZeroCond = new CondOwnerVisitorZeroCond();
	condOwnerVisitorZeroCond.strCond = "IsCarried";
	co.VisitCOs(condOwnerVisitorZeroCond, bAllowLocked: true);
	co.objCOParent = null;
}

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

public bool CanFit(CondOwner coFit)
{
	if (aCOs == null)
	{
		return false;
	}
	CondOwner[] array = aCOs;
	foreach (CondOwner condOwner in array)
	{
		if (bHoldSlot && condOwner == null && coFit.mapSlotEffects.ContainsKey(strName))
		{
			return true;
		}
		if (condOwner != null && condOwner.objContainer != null && (condOwner.objContainer.ctAllowed == null || condOwner.objContainer.ctAllowed.Triggered(coFit)) && condOwner.objContainer.GetSpaceAvailable() > 0)
		{
			return true;
		}
	}
	return false;
}

public bool AllowedCO(CondOwner coIn)
{
	if (coIn == null || coIn == CO)
	{
		return false;
	}
	if (ctAllowed != null)
	{
		return ctAllowed.Triggered(coIn);
	}
	return true;
}

public void SpawnInventoryWindow(CondOwner CO, InventoryWindowType type, bool bFlyIn)
{
	if (CO == null)
	{
		return;
	}
	CanvasManager.ShowCanvasGroup(canvasGroup);
	bool flag = type == InventoryWindowType.Ground || CO.objContainer != null;
	for (int i = 0; i < activeWindows.Count; i++)
	{
		if (!flag)
		{
			break;
		}
		if (activeWindows[i].CO == CO && type == activeWindows[i].type)
		{
			flag = false;
		}
	}
	if (flag)
	{
		GameObject gameObject = Object.Instantiate(inventoryGridPrefab, base.transform);
		GUIInventoryWindow component = gameObject.GetComponent<GUIInventoryWindow>();
		activeWindows.Add(component);
		component.SetData(CO, type);
		int num = activeWindows.IndexOf(component);
		GUIInventoryWindow winPrev = null;
		if (num > 0)
		{
			winPrev = activeWindows[num - 1];
		}
		component.transform.localPosition = GetWindowPosition(component, winPrev) * 1.5f * CanvasManager.CanvasRatio;
		CanvasManager.SetAnchorsToCorners(component.transform);
		StartCoroutine(FlyIn(component));
	}
	foreach (Slot slot in CO.GetSlots(bDeep: true))
	{
		if (slot.strName == "social")
		{
			CondOwner coSlotted = slot.aCOs.FirstOrDefault();
			SpawnSocialMovesWindow(coSlotted);
		}
		else
		{
			if (slot.bHide)
			{
				continue;
			}
			CondOwner[] aCOs = slot.aCOs;
			foreach (CondOwner condOwner in aCOs)
			{
				if (condOwner != null && condOwner.objContainer != null && CTOpenInv.Triggered(condOwner))
				{
					SpawnInventoryWindow(condOwner, InventoryWindowType.Container, bFlyIn);
				}
			}
		}
	}
	if (CrewSim.coPlayer.HasCond("TutorialLockerWaiting") && instance.IsCOShown(CrewSim.coPlayer) && (CO.HasCond("TutorialLockerTarget") || (CO.HasCond("IsStorageFurniture") && CrewSim.coPlayer.HasCond("IsENCFirstDockOKLG"))))
	{
		CrewSim.coPlayer.ZeroCondAmount("TutorialLockerWaiting");
		MonoSingleton<ObjectiveTracker>.Instance.CheckObjective(CrewSim.coPlayer.strID);
	}
}

public List<Slot> GetSlotsDepthFirst(bool bDeep)
{
	List<Slot> list = new List<Slot>(aSlots);
	if (bDeep)
	{
		foreach (Slot aSlot in aSlots)
		{
			list.AddRange(aSlot.GetSlots(bDeep, bChildFirst: false));
		}
	}
	list.Sort(SortBySlotDepth);
	return list;
}
*/