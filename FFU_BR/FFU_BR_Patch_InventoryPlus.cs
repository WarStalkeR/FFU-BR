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
        if (CO == this) {
            Debug.Log("ERROR: Assigning self as own parent.");
        }
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
                co.VisitCOs(condOwnerVisitorAddCond, bAllowLocked: true);
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
        if (coIn == null || coIn == CO) {
            return false;
        }
		CondOwner coParent = CO.objCOParent;
		while (coParent != null) {
			if (coIn == coParent) return false;
			coParent = coParent.objCOParent;
		}
        if (ctAllowed != null) {
            return ctAllowed.Triggered(coIn);
        }
        return true;
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
*/