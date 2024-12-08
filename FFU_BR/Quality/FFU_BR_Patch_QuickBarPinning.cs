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
using UnityEngine;

public class patch_GUIQuickBar : GUIQuickBar {
    private extern void orig_Start();
    private void Start() {
        QuickBarOverride();
        orig_Start();
    }
    private extern void orig_ExpandCollapse(bool refreshSizeOnly = false);
    private void ExpandCollapse(bool refreshSizeOnly = false) {
        QuickBarOverride();
        orig_ExpandCollapse(refreshSizeOnly);
    }
    private void QuickBarOverride() {
        if (!FFU_BR_Defs.QuickBarPinning) return;
        if (FFU_BR_Defs.QuickBarTweaks.Length != 3) return;
        _pinPosition = new Vector3(FFU_BR_Defs.QuickBarTweaks[0],
            FFU_BR_Defs.QuickBarTweaks[1], 0);
        bExpanded = FFU_BR_Defs.QuickBarTweaks[2] == 1f;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
GUIQuickBar.Start
private void Start()
{
	TooltipPreviewButton.OnPreviewButtonClicked.AddListener(delegate(CondOwner selectedCO)
	{
		COTarget = selectedCO;
	});
	if (Slots.OnSlotContentUpdated == null)
	{
		Slots.OnSlotContentUpdated = new SlotUpdatedEvent();
	}
	Slots.OnSlotContentUpdated.AddListener(OnSlotsUpdated);
	_resetButton.onClick.AddListener(delegate
	{
		Refresh();
	});
	_pinButton.onClick.AddListener(delegate
	{
		OnPinButtonDown();
	});
	_closeButton.onClick.AddListener(OnCloseButtonDown);
	_expandButton.onClick.AddListener(delegate
	{
		ExpandCollapse();
	});
	Setup();
}

GUIQuickBar.ExpandCollapse
private void ExpandCollapse(bool refreshSizeOnly = false)
{
	if (!refreshSizeOnly)
	{
		bExpanded = !bExpanded;
	}
	if (bExpanded)
	{
		_expandIcon.sprite = _sprites[0];
		_panelObject.sizeDelta = new Vector2(_panelObject.sizeDelta.x, Mathf.Max(GetMinSize(), 2f + 16f * (float)aButtons.Count));
	}
	else
	{
		_expandIcon.sprite = _sprites[1];
		_panelObject.sizeDelta = new Vector2(_panelObject.sizeDelta.x, GetMinSize());
	}
}
*/