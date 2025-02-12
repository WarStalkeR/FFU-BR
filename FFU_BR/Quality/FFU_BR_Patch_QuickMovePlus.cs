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
using UnityEngine;
using Ostranauts.Core;
using Ostranauts.Objectives;

public partial class patch_GUIInventory : GUIInventory {
    [MonoModIgnore] public static patch_GUIInventory instance;
    public GUIInventoryWindow targetWindow = null;
}

public partial class patch_GUIInventoryItem : GUIInventoryItem {
    public extern void orig_OnShiftPointerDown();
    public void OnShiftPointerDown() {
        if (!FFU_BR_Defs.BetterInvTransfer ||
            patch_GUIInventory.instance.targetWindow == null ||
            patch_GUIInventory.instance.targetWindow.CO == null ||
            windowData == patch_GUIInventory.instance.targetWindow) {
            orig_OnShiftPointerDown();
            return;
        }
        CondOwner targetInv = patch_GUIInventory.instance.targetWindow.CO;
        if (windowData != null && windowData.type == InventoryWindowType.Container 
            && CO.RootParent() == targetInv) {
            if (!(CO.objCOParent.DropCO(CO, bAllowLocked: false) == null)) 
                AttachToCursor();
            return;
        }
        if (windowData == null) {
            if (!(CO.objCOParent.DropCO(CO, bAllowLocked: false) == null)) 
                AttachToCursor();
            return;
        }
        CO.RemoveFromCurrentHome();
        GUIInventory.RemoveTooltip();
        if (windowData != null) 
            windowData.RemoveAndDestroy(CO.strID);
        else Object.Destroy(base.gameObject);
        CondOwner addedCO = targetInv.AddCO(CO, bEquip: true, bOverflow: true, bIgnoreLocks: false);
        if (addedCO == null) {
            if (GUIInventory.instance.IsCOShown(CrewSim.coPlayer) && 
                CrewSim.coPlayer.HasCond("TutorialClothesWaiting")) {
                CrewSim.coPlayer.ZeroCondAmount("TutorialClothesWaiting");
                MonoSingleton<ObjectiveTracker>.Instance.CheckObjective(CrewSim.coPlayer.strID);
            }
            CrewSimTut.CheckHelmetAtmoTutorial();
            if (GUIInventory.CTOpenInv.Triggered(CO)) 
                GUIInventory.instance.SpawnInventoryWindow(CO, 
                    InventoryWindowType.Container, bFlyIn: true);
            GUIInventory.instance.RedrawAllWindows();
        } else {
            GUIInventoryItem gUIInventoryItem = SpawnInventoryItem(addedCO.strID);
            if (gUIInventoryItem != null) gUIInventoryItem.AttachToCursor();
        }
    }

    public extern bool orig_MoveInventories(GUIInventoryWindow destination, Vector2 position, bool canPlaceSelf);
    public bool MoveInventories(GUIInventoryWindow destination, Vector2 position, bool canPlaceSelf) {
        bool aResult = orig_MoveInventories(destination, position, canPlaceSelf);
        if (aResult) patch_GUIInventory.instance.targetWindow = destination;
        return aResult;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
GUIInventoryItem.OnShiftPointerDown
public void OnShiftPointerDown()
{
	CondOwner inventoryCrew = GetInventoryCrew();
	if (windowData != null && windowData.type == InventoryWindowType.Container && CO.RootParent() == inventoryCrew)
	{
		if (!(CO.objCOParent.DropCO(CO, bAllowLocked: false) == null))
		{
			AttachToCursor();
		}
		return;
	}
	if (windowData == null)
	{
		if (!(CO.objCOParent.DropCO(CO, bAllowLocked: false) == null))
		{
			AttachToCursor();
		}
		return;
	}
	CO.RemoveFromCurrentHome();
	GUIInventory.RemoveTooltip();
	if (windowData != null)
	{
		windowData.RemoveAndDestroy(CO.strID);
	}
	else
	{
		Object.Destroy(base.gameObject);
	}
	CondOwner condOwner = inventoryCrew.AddCO(CO, bEquip: true, bOverflow: true, bIgnoreLocks: false);
	if (condOwner == null)
	{
		if (GUIInventory.instance.IsCOShown(CrewSim.coPlayer) && CrewSim.coPlayer.HasCond("TutorialClothesWaiting"))
		{
			CrewSim.coPlayer.ZeroCondAmount("TutorialClothesWaiting");
			MonoSingleton<ObjectiveTracker>.Instance.CheckObjective(CrewSim.coPlayer.strID);
		}
		CrewSimTut.CheckHelmetAtmoTutorial();
		if (GUIInventory.CTOpenInv.Triggered(CO))
		{
			GUIInventory.instance.SpawnInventoryWindow(CO, InventoryWindowType.Container, bFlyIn: true);
		}
		GUIInventory.instance.RedrawAllWindows();
	}
	else
	{
		GUIInventoryItem gUIInventoryItem = SpawnInventoryItem(condOwner.strID);
		if (gUIInventoryItem != null)
		{
			gUIInventoryItem.AttachToCursor();
		}
	}
}

GUIInventoryItem.MoveInventories
public bool MoveInventories(GUIInventoryWindow destination, Vector2 position, bool canPlaceSelf)
{
	if (CO == null || destination == null)
	{
		return false;
	}
	if (CO.HasCond("IsSocialItem") && destination != windowData)
	{
		return false;
	}
	if (!RectTransformUtility.RectangleContainsScreenPoint(destination.gridImage.rectTransform, position, CrewSim.objInstance.UICamera))
	{
		return false;
	}
	if (destination.type == InventoryWindowType.Container && destination.CO.objContainer != null && !destination.CO.objContainer.AllowedCO(CO))
	{
		return false;
	}
	int x = (int)(itemRect.rect.width - 24f * CanvasManager.CanvasRatio);
	int y = (int)(itemRect.rect.height - 24f * CanvasManager.CanvasRatio);
	if (MathUtils.IsRotationVertical(fRotLast))
	{
		MathUtils.Swap(ref x, ref y);
	}
	PairXY pairXY = destination.PairXYFromPosition(position, x, y);
	CondOwner condOwner = null;
	GUIInventoryItem gUIInventoryItem = null;
	int num = 0;
	for (int i = 0; i < itemWidthOnGrid; i++)
	{
		for (int j = 0; j < itemHeightOnGrid; j++)
		{
			PairXY pairXY2 = new PairXY(pairXY.x + i, pairXY.y + j);
			GUIInventoryItem inventoryItem = destination.gridLayout.GetInventoryItem(pairXY2);
			if (inventoryItem != null && inventoryItem != this)
			{
				if (inventoryItem.IsBlocker())
				{
					return false;
				}
				if (gUIInventoryItem != inventoryItem)
				{
					num++;
				}
				gUIInventoryItem = inventoryItem;
				condOwner = destination.gridLayout.GetCO(pairXY2);
			}
			if (destination.type == InventoryWindowType.Ground && !destination.IsValidPlacementTile(pairXY2))
			{
				return false;
			}
		}
	}
	if (num > 1)
	{
		return false;
	}
	if (windowData != null)
	{
		Debug.Log("MoveInventories from " + windowData.name + "." + CO.name + " to " + destination.name + "(" + pairXY.x + "," + pairXY.y + ")");
	}
	CondOwner objCOParent = CO.objCOParent;
	Ship ship = CO.ship;
	AudioEmitter component = CO.GetComponent<AudioEmitter>();
	if (condOwner == CO)
	{
		if (!canPlaceSelf)
		{
			return false;
		}
		condOwner = null;
	}
	if (condOwner != null)
	{
		if (condOwner.CanStackOnItem(CO) > 0)
		{
			CO.RemoveFromCurrentHome();
			CondOwner coRemainder = condOwner.StackCO(CO);
			if (component != null)
			{
				component.StartTrans();
			}
			Object.Destroy(base.gameObject);
			ProcessRemainder(coRemainder, destination, objCOParent, ship);
			return true;
		}
		if (condOwner.objContainer != null && condOwner.objContainer.gridLayout.FindFirstUnoccupiedTile(this).IsValid() && StackOrAddToContainer(condOwner.objContainer))
		{
			if (component != null)
			{
				component.StartTrans();
			}
			destination.Redraw();
			if (windowData != null)
			{
				windowData.Redraw();
			}
			return true;
		}
	}
	if (gUIInventoryItem != null && gUIInventoryItem.CO == null)
	{
		gUIInventoryItem = null;
	}
	else if (gUIInventoryItem != null)
	{
		destination.Remove(gUIInventoryItem.CO.strID);
	}
	if (!IsGoodPlacement(destination, pairXY))
	{
		if (gUIInventoryItem != null)
		{
			gUIInventoryItem.AddToWindow(destination);
		}
		destination.Redraw();
		return false;
	}
	CO.RemoveFromCurrentHome();
	if (windowData == null)
	{
		Object.Destroy(base.gameObject);
	}
	Item item = CO.Item;
	if (item != null)
	{
		item.fLastRotation = fRotLast;
	}
	CondOwner condOwner2 = null;
	Ship previousShip = null;
	if (condOwner != null)
	{
		condOwner2 = condOwner.objCOParent;
		previousShip = condOwner.ship;
		condOwner.RemoveFromCurrentHome();
	}
	if (destination.type == InventoryWindowType.Ground)
	{
		if (!PlaceOnGround(destination, pairXY))
		{
			return false;
		}
	}
	else
	{
		destination.CO.objContainer.AddCOSimple(CO, pairXY);
	}
	if (condOwner != null)
	{
		GUIInventoryItem gUIInventoryItem2 = GUIInventory.GetInventoryItemFromCO(condOwner);
		if (gUIInventoryItem2 == null)
		{
			gUIInventoryItem2 = SpawnInventoryItem(condOwner.strID);
		}
		gUIInventoryItem2.AttachToCursor(previousShip);
	}
	if (destination != null)
	{
		destination.Redraw();
	}
	if (windowData != null)
	{
		windowData.Redraw();
	}
	if (component != null)
	{
		component.StartTrans();
	}
	if (MonoSingleton<GUIQuickBar>.Instance.COTarget == CO)
	{
		MonoSingleton<GUIQuickBar>.Instance.Refresh(ignoreInput: true);
	}
	return true;
}
*/