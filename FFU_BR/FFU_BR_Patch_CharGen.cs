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

public class patch_GUIChargenCareer : GUIChargenCareer {
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
*/