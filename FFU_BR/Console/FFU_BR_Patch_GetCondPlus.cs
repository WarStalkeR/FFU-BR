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
#pragma warning disable IDE0051

using MonoMod;
using Ostranauts.UI.MegaToolTip;
using System.Collections.Generic;
using System;

public partial class patch_ConsoleResolver : ConsoleResolver {
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
            } else if (strings[1].Contains("[them]")) {
                if (!(GUIMegaToolTip.Selected != null)) {
                    strInput += "\nNo target selected for [them].";
                    return false;
                }
                cOwner = GUIMegaToolTip.Selected;
                if (strings[1].Contains("-")) {
                    string strNum = strings[1].Split('-')[1];
                    int.TryParse(strNum, out int pNum);
                    if (pNum > 0) {
                        CondOwner cParent = cOwner;
                        while (cParent != null && pNum > 0) {
                            cParent = cParent.objCOParent;
                            pNum--;
                        }
                        if (cParent == null) {
                            strInput += $"\nNo parent exists for [them] at depth {strNum}.";
                            return false;
                        }
                        cOwner = cParent;
                    }
                }
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
            if (currData == "*coParents") {
                strInput += $"\nFound condowner {cOwner.strNameFriendly} ({cOwner.strName})";
                CondOwner refParent = cOwner.objCOParent;
                while (refParent != null) {
                    strInput += $"\nIn condowner {refParent.strNameFriendly} ({refParent.strName})";
                    refParent = refParent.objCOParent;
                }
                return true;
            }
            if (currData == "*coRules") {
                if (cOwner.mapCondRules.Count > 0) {
                    strInput += $"\nFound condrules for {cOwner.strNameFriendly} ({cOwner.strName}):";
                    foreach (var condSet in cOwner.mapCondRules) {
                        if (condSet.Value != null) {
                            CondRule refRule = condSet.Value;
                            strInput += $"\n{refRule.strName}: " + Array.IndexOf(refRule.aThresholds,
                            refRule.GetCurrentThresh(cOwner)) + $" ({condSet.Key})";
                        }
                    }
                } else strInput += $"\nThe condowner {cOwner.strNameFriendly} " +
                    $"({cOwner.strName}) has no attached condrules.";
                return true;
            }
            if (currData == "*coTickers") {
                if (cOwner.aTickers.Count > 0) {
                    strInput += $"\nFound tickers for {cOwner.strNameFriendly} ({cOwner.strName}):";
                    foreach (var aTicker in cOwner.aTickers) {
                        if (aTicker != null) {
                            strInput += $"\n{aTicker.strName}: " +
                            $"{SmartString(aTicker.fTimeLeft * 3600)}s " +
                            $"({SmartString(aTicker.fPeriod * 3600)}s)";
                        }
                    }
                } else strInput += $"\nThe condowner {cOwner.strNameFriendly} " +
                    $"({cOwner.strName}) has no attached tickers.";
                return true;
            }
            bool isFirst = true;
            foreach (Condition refCond in cOwner.mapConds.Values) {
                if (currData == "*" || refCond.strName.IndexOf(currData) >= 0) {
                    if (isFirst) {
                        strInput += $"\nFound stats for {cOwner.strNameFriendly} ({cOwner.strName}):";
                        isFirst = false;
                    }
                    strInput += $"\n{refCond.strName} = {refCond.fCount}";
                    isFound = true;
                }
            }
            if (!isFound) strInput = strInput + "\nNo matching cond(s) on " + cOwner.strNameFriendly;
            return isFound;
        }
        return false;
    }

    public static string SmartString(double number) {
        int precision = number % 1 == 0 ? 0 : 1;
        string result;
        do {
            result = number.ToString($"N{precision}");
            precision++;
        } while (number < 0.1 && result.EndsWith("0") && precision <= 5);
        return result;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
ConsoleResolver.KeywordGetCond
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