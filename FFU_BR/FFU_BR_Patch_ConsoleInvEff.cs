#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class patch_ConsoleResolver : ConsoleResolver {
    private static bool KeywordSyncInvEffects(ref string strInput) {
        int objCount = 0;
        List<patch_CondOwner> aRlevantCOs = GameObject.FindObjectsOfType<patch_CondOwner>()
            .Where(x => x != null && x.objCOParent != null && x.objCOParent.objContainer != null 
            && (x.objCOParent as patch_CondOwner).invSlotEffect != null).ToList();
        foreach (patch_CondOwner aRlevantCO in aRlevantCOs) {
            Debug.Log($"#Info# Synchronized inventory effects for CO [{aRlevantCO.strName}:{aRlevantCO.strID}]");
            Container refContainer = aRlevantCO.objCOParent.objContainer;
            refContainer.ClearIsInContainer(aRlevantCO);
            refContainer.SetIsInContainer(aRlevantCO);
            objCount++;
        }
        strInput += $"\nSynchronized inventory effects for {objCount} COs.";
        return true;
    }
}