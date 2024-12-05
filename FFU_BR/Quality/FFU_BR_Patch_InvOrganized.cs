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
using System.Collections.Generic;
using UnityEngine;

public partial class patch_GUIInventory : GUIInventory {
    private List<Vector2> vBase;
    private float colMaxWidth = 0;
    private extern Vector2 orig_GetWindowPosition(GUIInventoryWindow winCurrent, GUIInventoryWindow winPrev);
    private Vector2 GetWindowPosition(GUIInventoryWindow winCurrent, GUIInventoryWindow winPrev) {
        if (!FFU_BR_Defs.OrgInventoryMode || FFU_BR_Defs.OrgInventoryTweaks.Length != 6) return orig_GetWindowPosition(winCurrent, winPrev);
        RectTransform tPaperDoll = base.transform.Find("PaperDoll").GetComponent<RectTransform>();
        if (activeWindows.IndexOf(winCurrent) == 0) {
            RectTransform tBackground = base.transform.Find("Background").GetComponent<RectTransform>();
            RectTransform tBorder = base.transform.Find("Border").GetComponent<RectTransform>();
            if (vBase == null) vBase = new List<Vector2>();
            if (vBase.Count == 0) {
                vBase.Add(new Vector2(tPaperDoll.localPosition.x, tPaperDoll.localPosition.y));
                vBase.Add(new Vector2(tBackground.localPosition.x, tBackground.localPosition.y));
                vBase.Add(new Vector2(tBorder.localPosition.x, tBorder.localPosition.y));
            }
            tPaperDoll.localPosition = new Vector3(vBase[0].x + FFU_BR_Defs.OrgInventoryTweaks[0], vBase[0].y, 0f);
            tBackground.localPosition = new Vector3(vBase[1].x + FFU_BR_Defs.OrgInventoryTweaks[0], vBase[1].y, 0f);
            tBorder.localPosition = new Vector3(vBase[2].x + FFU_BR_Defs.OrgInventoryTweaks[0], vBase[2].y, 0f);
        }
        float wScale = 1f / (1.5f * CanvasManager.CanvasRatio);
        float xOffset = wScale * (tPaperDoll.localPosition.x + tPaperDoll.rect.width / 2f + 5f) + 2f;
        float yOffsetRef = wScale * (tPaperDoll.localPosition.y + tPaperDoll.rect.height / 2f) - FFU_BR_Defs.OrgInventoryTweaks[1];
        float yOffset = yOffsetRef;
        if (winPrev != null) {
            xOffset = winPrev.transform.localPosition.x * wScale;
            yOffset = winPrev.transform.localPosition.y - winPrev.gridImageRect.rect.height - winPrev.tabImage.rectTransform.rect.height * 1.7f;
            yOffset *= wScale;
            float wHeight = (winCurrent.gridImageRect.rect.height + winCurrent.tabImage.rectTransform.rect.height * 1.7f) * FFU_BR_Defs.OrgInventoryTweaks[5];
            float prevMaxWidth = winPrev.tabImage.rectTransform.rect.width + 
                Mathf.Max(winPrev.gridLayout.gridMaxX - 4, 0) * FFU_BR_Defs.OrgInventoryTweaks[4];
            if (prevMaxWidth > colMaxWidth) colMaxWidth = prevMaxWidth;
            float yOffsetExpected = yOffset - wHeight;
            float yOffsetLimit = -yOffsetRef - 4f - FFU_BR_Defs.OrgInventoryTweaks[2];
            if (FFU_BR_Defs.ActLogging >= FFU_BR_Defs.ActLogs.Runtime) Debug.Log($"#Info# Inventory Window: {winCurrent.CO.strCODef}, yOffset: " +
                $"{yOffset}, wHeight: {wHeight}, yOffsetRef: {yOffsetRef}, yOffsetExpected: {yOffsetExpected}, yOffsetLimit: {yOffsetLimit}");
            if (yOffsetExpected < yOffsetLimit) {
                yOffset = yOffsetRef;
                xOffset += colMaxWidth + 42f + FFU_BR_Defs.OrgInventoryTweaks[3];
                colMaxWidth = 0f;
            }
        } else colMaxWidth = 0;
        return new Vector2(xOffset, yOffset);
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
GUIInventory.GetWindowPosition

private Vector2 GetWindowPosition(GUIInventoryWindow winCurrent, GUIInventoryWindow winPrev)
{
	RectTransform component = base.transform.Find("PaperDoll").GetComponent<RectTransform>();
	float num = 1f / (1.5f * CanvasManager.CanvasRatio);
	float num2 = num * (component.localPosition.x + component.rect.width / 2f + 5f);
	float num3 = num * (component.localPosition.y + component.rect.height / 2f);
	float num4 = num3;
	float num5 = num4;
	if (winPrev != null)
	{
		num2 = winPrev.transform.localPosition.x * num;
		num5 = winPrev.transform.localPosition.y - winPrev.gridImageRect.rect.height - winPrev.tabImage.rectTransform.rect.height * 1.7f;
		num5 *= num;
		float num6 = winCurrent.gridImageRect.rect.height + winCurrent.tabImage.rectTransform.rect.height * 1.7f;
		if (num5 - num6 < num4 - num3 * 2f)
		{
			num5 = num4;
			num2 += winCurrent.tabImage.rectTransform.rect.width + 40f;
		}
	}
	return new Vector2(num2, num5);
}
*/