#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002
#pragma warning disable IDE0051

using FFU_Beyond_Reach;
using System.Collections.Generic;
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

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
GUIChargenCareer.AddSkillTrait
private void AddSkillTrait(JsonCareer jc, string strChosen)
{
	bmpDot2.color = Color.white;
	List<string> list = new List<string>(coUser.mapConds.Keys);
	cgs.AddCareer(jc);
	CareerChosen latestCareer = cgs.GetLatestCareer();
	cgs.ApplyCareer(coUser, latestCareer, bEvents: true);
	latestCareer.bTermEnded = true;
	latestCareer.aEvents.Add(strChosen);
	double num = 1.0;
	if (strChosen != null && strChosen.IndexOf("-") == 0)
	{
		strChosen = strChosen.Substring(1);
		num = -1.0;
	}
	int num2 = GetTraitYears(strChosen) * (int)num;
	CondRuleThresh changedCRThresh = GetChangedCRThresh(0, num2);
	if (changedCRThresh != null && _promisedAgeLoot.TryGetValue(changedCRThresh.strLootNew, out var value) && value != null)
	{
		foreach (KeyValuePair<string, double> item in value)
		{
			coUser.AddCondAmount(item.Key, item.Value);
		}
	}
	coUser.bFreezeCondRules = true;
	coUser.AddCondAmount("StatAge", num2);
	coUser.bFreezeCondRules = false;
	coUser.AddCondAmount(strChosen, num);
	foreach (Condition value2 in coUser.mapConds.Values)
	{
		if (list.IndexOf(value2.strName) < 0 && latestCareer.aSkillsChosen.IndexOf(value2.strName) < 0 && value2.nDisplaySelf == 2 && value2.strName.IndexOf("Dc") != 0)
		{
			latestCareer.aSkillsChosen.Add(value2.strName);
		}
	}
}
*/