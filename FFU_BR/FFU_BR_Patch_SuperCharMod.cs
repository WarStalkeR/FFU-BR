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
using System.Linq;
using UnityEngine;

public partial class patch_GUIChargenCareer : GUIChargenCareer {
    private void AddSkillTrait(JsonCareer jc, string strChosen) {
        bmpDot2.color = Color.white;
        List<string> list = new List<string>(coUser.mapConds.Keys);
        cgs.AddCareer(jc);
        CareerChosen latestCareer = cgs.GetLatestCareer();
        cgs.ApplyCareer(coUser, latestCareer, bEvents: true);
        latestCareer.bTermEnded = true;
        latestCareer.aEvents.Add(strChosen);
        double fAmount = 1.0;
        if (strChosen != null && strChosen.IndexOf("-") == 0) {
            strChosen = strChosen.Substring(1);
            fAmount = -1.0;
        }
        int traitYears = FFU_BR_Defs.NoSkillTraitCost ? 0 : GetTraitYears(strChosen);
        coUser.AddCondAmount("StatAge", traitYears);
        coUser.AddCondAmount(strChosen, fAmount);
        foreach (Condition value in coUser.mapConds.Values) {
            if (list.IndexOf(value.strName) < 0 && latestCareer.aSkillsChosen.IndexOf(value.strName) < 0 && value.nDisplaySelf == 2 && value.strName.IndexOf("Dc") != 0) {
                latestCareer.aSkillsChosen.Add(value.strName);
            }
        }
        UpdateSidebar();
        HideSidebarAlt();
        ClearMain();
        PageBranchChoice(latestCareer);
    }
}

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
/*
GUIChargenCareer.AddSkillTrait
Interaction.CalcRate

private void AddSkillTrait(JsonCareer jc, string strChosen)
{
	bmpDot2.color = Color.white;
	List<string> list = new List<string>(coUser.mapConds.Keys);
	cgs.AddCareer(jc);
	CareerChosen latestCareer = cgs.GetLatestCareer();
	cgs.ApplyCareer(coUser, latestCareer, bEvents: true);
	latestCareer.bTermEnded = true;
	latestCareer.aEvents.Add(strChosen);
	double fAmount = 1.0;
	if (strChosen != null && strChosen.IndexOf("-") == 0)
	{
		strChosen = strChosen.Substring(1);
		fAmount = -1.0;
	}
	int traitYears = GetTraitYears(strChosen);
	coUser.AddCondAmount("StatAge", traitYears);
	coUser.AddCondAmount(strChosen, fAmount);
	foreach (Condition value in coUser.mapConds.Values)
	{
		if (list.IndexOf(value.strName) < 0 && latestCareer.aSkillsChosen.IndexOf(value.strName) < 0 && value.nDisplaySelf == 2 && value.strName.IndexOf("Dc") != 0)
		{
			latestCareer.aSkillsChosen.Add(value.strName);
		}
	}
	UpdateSidebar();
	HideSidebarAlt();
	ClearMain();
	PageBranchChoice(latestCareer);
}

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