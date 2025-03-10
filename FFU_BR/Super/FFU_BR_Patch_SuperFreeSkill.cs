using FFU_Beyond_Reach;
using Ostranauts.ShipGUIs.Chargen;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class patch_GUIChargenCareer : GUIChargenCareer {
    private void AddSkillTrait(JsonCareer jc, string strChosen) {
        bmpDot2.color = Color.white;
        List<string> listConds = new List<string>(coUser.mapConds.Keys);
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
        int traitYears = FFU_BR_Defs.NoSkillTraitCost ? 0 : GetTraitYears(strChosen) * (int)fAmount;
        CondRuleThresh changedCRThresh = GetChangedCRThresh(0, traitYears);
        if (changedCRThresh != null && _promisedAgeLoot.TryGetValue(changedCRThresh.strLootNew, out var value) && value != null) {
            foreach (KeyValuePair<string, double> item in value) {
                coUser.AddCondAmount(item.Key, item.Value);
            }
        }
        coUser.bFreezeCondRules = true;
        coUser.AddCondAmount("StatAge", traitYears);
        coUser.bFreezeCondRules = false;
        coUser.AddCondAmount(strChosen, fAmount);
        foreach (Condition usrCond in coUser.mapConds.Values) {
            if (listConds.IndexOf(usrCond.strName) < 0 && latestCareer.aSkillsChosen.IndexOf(usrCond.strName) < 0 && usrCond.nDisplaySelf == 2 && usrCond.strName.IndexOf("Dc") != 0) {
                latestCareer.aSkillsChosen.Add(usrCond.strName);
            }
        }
    }

    private void RebuildMultiSelectSidebar() {
        if (_selectedSkills.Count == 0) {
            UpdateSidebar();
            return;
        }
        foreach (Transform item in tfSidebar) 
            Object.Destroy(item.gameObject);
        VerticalLayoutGroup layoutSidebar = tfSidebar.GetComponent<VerticalLayoutGroup>();
        layoutSidebar.spacing = 2f;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tfSidebar.GetComponent<RectTransform>());
        string @string = DataHandler.GetString("GUI_CAREER_SIDEBAR_COST_2");
        int yearsTotal = 0;
        SpawnSideBarHeader();
        foreach (SkillSelectionDTO selectedSkill in _selectedSkills) {
            int yearsChange = FFU_BR_Defs.NoSkillTraitCost ? 0 : GetTraitYears(selectedSkill.CondName) * selectedSkill.Change;
            selectedSkill.AgeConds = GetAgeRelatedConds(yearsTotal, yearsChange);
            SpawnAgeRelatedConds(selectedSkill.AgeConds);
            if (_dictCheckmarks.TryGetValue(selectedSkill.CondName, out var _)) {
                UpdateToggle(selectedSkill.Condition, selectedSkill.Change);
            }
            yearsTotal += yearsChange;
            GameObject goColumnLists = Object.Instantiate(pnlColumnLists, tfSidebar);
            GameObject goLabelRefA = Object.Instantiate(lblLeftTMP, goColumnLists.transform);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<b>");
            if (selectedSkill.Change < 0) {
                stringBuilder.Append("- ");
            } else {
                stringBuilder.Append("+ ");
            }
            stringBuilder.Append(selectedSkill.Condition.strNameFriendly);
            stringBuilder.AppendLine("</b> ");
            goLabelRefA.GetComponent<TMP_Text>().text = stringBuilder.ToString();
            GameObject goLabelRefB = Object.Instantiate(lblLeftTMP, goColumnLists.transform);
            stringBuilder = new StringBuilder();
            stringBuilder.Append(yearsChange);
            stringBuilder.AppendLine(@string);
            TMP_Text textCompRefA = goLabelRefB.GetComponent<TMP_Text>();
            textCompRefA.alignment = TextAlignmentOptions.Right;
            textCompRefA.text = stringBuilder.ToString();
        }
        Object.Instantiate(bmpLine01, tfSidebar);
        GameObject goLabelRefC = Object.Instantiate(lblLeftTMP, tfSidebar);
        TMP_Text textCompRefB = goLabelRefC.GetComponent<TMP_Text>();
        textCompRefB.alignment = TextAlignmentOptions.Right;
        textCompRefB.text = "<b>Total Cost: </b>" + yearsTotal + @string;
        AddApplyClearButtonSection(yearsTotal);
        StartCoroutine(CrewSim.objInstance.ScrollBottom(srSide));
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* GUIChargenCareer.AddSkillTrait
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

/* GUIChargenCareer.RebuildMultiSelectSidebar
private void RebuildMultiSelectSidebar()
{
	if (_selectedSkills.Count == 0)
	{
		UpdateSidebar();
		return;
	}
	foreach (Transform item in tfSidebar)
	{
		Object.Destroy(item.gameObject);
	}
	VerticalLayoutGroup component = tfSidebar.GetComponent<VerticalLayoutGroup>();
	component.spacing = 2f;
	LayoutRebuilder.ForceRebuildLayoutImmediate(tfSidebar.GetComponent<RectTransform>());
	string @string = DataHandler.GetString("GUI_CAREER_SIDEBAR_COST_2");
	int num = 0;
	SpawnSideBarHeader();
	foreach (SkillSelectionDTO selectedSkill in _selectedSkills)
	{
		int num2 = GetTraitYears(selectedSkill.CondName) * selectedSkill.Change;
		selectedSkill.AgeConds = GetAgeRelatedConds(num, num2);
		SpawnAgeRelatedConds(selectedSkill.AgeConds);
		if (_dictCheckmarks.TryGetValue(selectedSkill.CondName, out var _))
		{
			UpdateToggle(selectedSkill.Condition, selectedSkill.Change);
		}
		num += num2;
		GameObject gameObject = Object.Instantiate(pnlColumnLists, tfSidebar);
		GameObject gameObject2 = Object.Instantiate(lblLeftTMP, gameObject.transform);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<b>");
		if (selectedSkill.Change < 0)
		{
			stringBuilder.Append("- ");
		}
		else
		{
			stringBuilder.Append("+ ");
		}
		stringBuilder.Append(selectedSkill.Condition.strNameFriendly);
		stringBuilder.AppendLine("</b> ");
		gameObject2.GetComponent<TMP_Text>().text = stringBuilder.ToString();
		GameObject gameObject3 = Object.Instantiate(lblLeftTMP, gameObject.transform);
		stringBuilder = new StringBuilder();
		stringBuilder.Append(num2);
		stringBuilder.AppendLine(@string);
		TMP_Text component2 = gameObject3.GetComponent<TMP_Text>();
		component2.alignment = TextAlignmentOptions.Right;
		component2.text = stringBuilder.ToString();
	}
	Object.Instantiate(bmpLine01, tfSidebar);
	GameObject gameObject4 = Object.Instantiate(lblLeftTMP, tfSidebar);
	TMP_Text component3 = gameObject4.GetComponent<TMP_Text>();
	component3.alignment = TextAlignmentOptions.Right;
	component3.text = "<b>Total Cost: </b>" + num + @string;
	AddApplyClearButtonSection(num);
	StartCoroutine(CrewSim.objInstance.ScrollBottom(srSide));
}
*/