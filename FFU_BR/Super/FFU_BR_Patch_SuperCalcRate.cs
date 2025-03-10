using FFU_Beyond_Reach;
using System.Linq;
using UnityEngine;

public partial class patch_Interaction : Interaction {
    private void CalcRate() {
        if (strActionGroup != "Work" || bCTThemModifierCalculated) {
            return;
        }
        fCTThemModifierUs = 1f;
        if (strCTThemMultCondUs != null) {
            fCTThemModifierUs = (float)objUs.GetCondAmount(strCTThemMultCondUs);
            if (FFU_BR_Defs.AllowSuperChars && FFU_BR_Defs.SuperCharacters.Length > 0 &&
                FFU_BR_Defs.SuperCharacters.Contains(objUs.strName.ToLower())) {
                fCTThemModifierUs *= FFU_BR_Defs.SuperCharMultiplier;
            }
        }
        fCTThemModifierUs = Mathf.Clamp(fCTThemModifierUs, 1f, FFU_BR_Defs.ModifyUpperLimit ? FFU_BR_Defs.BonusUpperLimit : 10f);
        fCTThemModifierTools = 1f;
        if (strCTThemMultCondTools != null) {
            fCTThemModifierTools = 0f;
            if (aLootItemUseContract != null) {
                foreach (CondOwner item in aLootItemUseContract) {
                    if (item != null && strCTThemMultCondTools != null) {
                        fCTThemModifierTools += (float)item.GetCondAmount(strCTThemMultCondTools);
                    }
                }
            }
        }
        fCTThemModifierPenalty = (float)objUs.GetCondAmount("StatWorkSpeedPenalty");
        if ((double)fCTThemModifierPenalty > 0.99) {
            fCTThemModifierPenalty = 0.99f;
        }
        bCTThemModifierCalculated = true;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* Interaction.CalcRate
private void CalcRate()
{
	if (strActionGroup != "Work" || bCTThemModifierCalculated)
	{
		return;
	}
	fCTThemModifierUs = 1f;
	if (strCTThemMultCondUs != null)
	{
		fCTThemModifierUs = (float)objUs.GetCondAmount(strCTThemMultCondUs);
	}
	fCTThemModifierUs = Mathf.Clamp(fCTThemModifierUs, 1f, 10f);
	fCTThemModifierTools = 1f;
	if (strCTThemMultCondTools != null)
	{
		fCTThemModifierTools = 0f;
		if (aLootItemUseContract != null)
		{
			foreach (CondOwner item in aLootItemUseContract)
			{
				if (item != null && strCTThemMultCondTools != null)
				{
					fCTThemModifierTools += (float)item.GetCondAmount(strCTThemMultCondTools);
				}
			}
		}
	}
	fCTThemModifierPenalty = (float)objUs.GetCondAmount("StatWorkSpeedPenalty");
	if ((double)fCTThemModifierPenalty > 0.99)
	{
		fCTThemModifierPenalty = 0.99f;
	}
	bCTThemModifierCalculated = true;
}
*/