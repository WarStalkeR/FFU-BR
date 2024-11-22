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

public partial class patch_JsonSlot : JsonSlot {
    public int? sOrder { get; set; }
}

public partial class patch_Slot : Slot {
    public int sOrder;
    [MonoModIgnore] public extern patch_Slot(JsonSlot jslot);
    [MonoModOriginal] public extern void orig_Slot(JsonSlot jslot);
    [MonoModConstructor] public void Slot(patch_JsonSlot jslot) {
        orig_Slot(jslot);
        sOrder = jslot.sOrder != null ? (int)jslot.sOrder : jslot.nDepth;
    }
}

public partial class patch_GUIInventory : GUIInventory {
    [MonoModReplace] public void SpawnInventoryWindow(patch_CondOwner CO, InventoryWindowType type, bool bFlyIn) {
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
        GetSlotsRecursive(ref sortedSlots, compSlots?.aSlots?.ToList(),
            FFU_BR_Defs.ActLogging >= FFU_BR_Defs.ActLogs.Runtime);
        return sortedSlots;

        // Depth Sorting Method
        int SortByDepth(Slot s1, Slot s2) {
            if ((s1 as patch_Slot) == null || (s2 as patch_Slot) == null) return 0;
            return (s1 as patch_Slot).sOrder.CompareTo((s2 as patch_Slot).sOrder);
        }

        // Recursive Slot Sorting
        void GetSlotsRecursive(ref List<Slot> srtSlots, List<Slot> refSlots,
            bool dLog, bool dSort = true, int sDepth = 0) {
            if (refSlots != null && refSlots.Count > 0) {
                refSlots.Sort(SortByDepth);
                foreach (Slot refSlot in refSlots) {
                    if (dLog) Debug.Log($"#Info# Sorted Slot " +
                        $"{string.Empty.PadLeft(sDepth, '=')}> {refSlot.strName}");
                    srtSlots.Add(refSlot);
                    foreach (CondOwner subCO in refSlot.aCOs) {
                        List<Slot> subSlots = subCO?.compSlots?.aSlots?.ToList();
                        GetSlotsRecursive(ref srtSlots, subSlots, dLog, dSort, sDepth + 1);
                    }
                }
            }
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
GUIInventory.SpawnInventoryWindow
Slots.GetSlotsDepthFirst

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