#pragma warning disable CS0108
#pragma warning disable CS0114
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using Ostranauts.UI.MegaToolTip;
using System.Collections.Generic;
using MonoMod;

public class patch_ConsoleResolver : ConsoleResolver {
    [MonoModReplace] private static bool KeywordGetCond(ref string strInput, string[] strings) {
        if (CrewSim.objInstance == null) {
            strInput += "\nCrewSim instance not found.";
            return false;
        }
        CondOwner cOwner = null;
        string currData = string.Empty;
        if (strings.Length == 2) {
            cOwner = CrewSim.GetSelectedCrew();
            if (cOwner == null) strInput += "\nCondOwner not found.";
            currData = strings[1];
        } else if (strings.Length == 3) {
            if (strings[1] == "[us]" || strings[1] == "player") {
                cOwner = CrewSim.GetSelectedCrew();
            } else if (strings[1] == "[them]") {
                if (!(GUIMegaToolTip.Selected != null)) {
                    strInput += "\nNo target selected for [them].";
                    return false;
                }
                cOwner = GUIMegaToolTip.Selected;
            } else {
                string cName = strings[1].Replace('_', ' ');
                if (!DataHandler.mapCOs.TryGetValue(cName, out cOwner)) {
                    List<CondOwner> cOs = CrewSim.shipCurrentLoaded.GetCOs(null, true, true, true);
                    foreach (CondOwner item in cOs) {
                        if (item.strNameFriendly == strings[1] || item.strNameFriendly == cName || 
							item.strName == strings[1] || item.strName == cName || item.strID == strings[1]) {
                            cOwner = item;
                            break;
                        }
                    }
                }
            }
            currData = strings[2];
            if (cOwner == null) 
				strInput += "\nCondOwner not found.";
        } else {
            if (strings.Length < 2) {
                strInput += "\nNot enough parameters.";
                return false;
            }
            if (strings.Length > 3) {
                strInput += "\nToo many parameters.";
                return false;
            }
        }
        if (cOwner != null && currData != string.Empty) {
            bool isFound = false;
            foreach (Condition refCond in cOwner.mapConds.Values) {
                if (currData == "*" || refCond.strName.IndexOf(currData) >= 0) {
                    strInput += "\n" + cOwner.strNameFriendly + "." + refCond.strName + " = " + refCond.fCount;
                    isFound = true;
                }
            }
            if (!isFound) strInput = strInput + "\nNo matching cond(s) on " + cOwner.strNameFriendly;
            return isFound;
        }
        return false;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
private static bool KeywordGetCond(ref string strInput, string[] strings)
{
	if (CrewSim.objInstance == null)
	{
		strInput += "\nCrewSim instance not found.";
		return false;
	}
	CondOwner value = null;
	string text = string.Empty;
	if (strings.Length == 2)
	{
		value = CrewSim.GetSelectedCrew();
		if (value == null)
		{
			strInput += "\nCondOwner not found.";
		}
		text = strings[1];
	}
	else if (strings.Length == 3)
	{
		if (strings[1] == "[us]" || strings[1] == "player")
		{
			value = CrewSim.GetSelectedCrew();
		}
		else if (strings[1] == "[them]")
		{
			if (!(GUIMegaToolTip.Selected != null))
			{
				strInput += "\nNo target selected for [them].";
				return false;
			}
			value = GUIMegaToolTip.Selected;
		}
		else
		{
			string text2 = strings[1].Replace('_', ' ');
			if (!DataHandler.mapCOs.TryGetValue(text2, out value))
			{
				List<CondOwner> cOs = CrewSim.shipCurrentLoaded.GetCOs(null, bSubObjects: true, bAllowDocked: true, bAllowLocked: true);
				foreach (CondOwner item in cOs)
				{
					if (item.strNameFriendly == strings[1] || item.strNameFriendly == text2 || item.strName == strings[1] || item.strName == text2 || item.strID == strings[1])
					{
						value = item;
						break;
					}
				}
			}
		}
		text = strings[2];
		if (value == null)
		{
			strInput += "\nCondOwner not found.";
		}
	}
	else
	{
		if (strings.Length < 2)
		{
			strInput += "\nNot enough parameters.";
			return false;
		}
		if (strings.Length > 3)
		{
			strInput += "\nToo many parameters.";
			return false;
		}
	}
	if (value != null && text != string.Empty)
	{
		bool flag = false;
		foreach (Condition value2 in value.mapConds.Values)
		{
			if (value2.strName.IndexOf(text) >= 0)
			{
				string text3 = strInput;
				strInput = text3 + "\n" + value.strNameFriendly + "." + value2.strName + " = " + value2.fCount;
				flag = true;
			}
		}
		if (!flag)
		{
			strInput = strInput + "\nNo matching cond(s) on " + value.strNameFriendly;
		}
		return flag;
	}
	return false;
}
*/