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
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using FFU_Beyond_Reach;
using System;

public partial class patch_ConsoleToGUI : ConsoleToGUI {
    private string logHistoryPath = null;
	private bool configLoaded = false;
    [MonoModReplace] private void DrawConsole(int window) {
        if (HandleInput()) return;
        if (!configLoaded) {
            configLoaded = true;
            mChars = FFU_BR_Defs.MaxLogTextSize;
            scrollPos = new Vector2(0f, mChars / 4);
            logHistoryPath = Path.Combine(Application.dataPath, "console_history.txt");
            if (logHistoryPath != null && File.Exists(logHistoryPath)) {
                try {
                    prevInputs = File.ReadAllText(logHistoryPath,
                        Encoding.UTF8).Split('\n').ToList();
                } catch (Exception ex) {
                    Debug.Log($"Failed to load console history!\n{ex.Message}\n{ex.StackTrace}");
                }
            }
        }
        Rect textView = new Rect(10f, 20f, Screen.width / 2 - 40, mChars / 4);
        Rect scrollView = new Rect(10f, 30f, Screen.width / 2 - 20, Screen.height / 2 - (_textActual + 55f));
        Rect textField = new Rect(10f, scrollView.y + scrollView.height + 5f, Screen.width / 2 - 40, _textActual + 10f);
        scrollPos = GUI.BeginScrollView(scrollView, scrollPos, textView, false, true);
        GUI.TextArea(textView, myLog, mChars, logStyle);
        GUI.EndScrollView();
        GUI.SetNextControlName("command");
        myInput = GUI.TextField(textField, myInput, txtStyle);
        if (Event.current.isKey && GUI.GetNameOfFocusedControl() == "command") {
            if (Event.current.keyCode == KeyCode.Return) {
                string[] commands = myInput.Split(';');
                if (!string.IsNullOrEmpty(myInput) && 
					prevInputs.LastOrDefault() != myInput)
                    prevInputs.Add(myInput);
                if (prevInputs.Count > prevMax)
                    prevInputs.RemoveRange(0, prevInputs.Count - prevMax);
                if (commands.Length > 1) {
                    myInput = "<color=" + multipleColor + "><b>[Command]</b></color>: " + myInput;
                    myLog = myLog + "\n" + myInput;
                }
                myInput = string.Empty;
                for (int i = 0; i < commands.Length; i++) {
                    if (!(commands[i] == string.Empty)) {
                        commands[i] = commands[i].Trim();
                        if (ConsoleResolver.ResolveString(ref commands[i]))
                            commands[i] = "<color=" + commandColor + "><b>[Command]</b></color>: " + commands[i];
                        else commands[i] = "<color=" + failedColor + "><b>[Command]</b></color>: " + commands[i];
                        myLog = myLog + "\n" + commands[i];
                    }
                }
                prevPointer = 0;
            } else if (Event.current.keyCode == KeyCode.UpArrow) {
                if (prevInputs.Count > 0) {
                    if (prevPointer == 0 && myInput != string.Empty) {
                        prevInputs.Add(myInput);
                        if (prevInputs.Count > prevMax)
                            prevInputs.RemoveRange(0, prevInputs.Count - prevMax);
                        prevPointer++;
                    }
                    prevPointer++;
                    if (prevPointer > prevInputs.Count) prevPointer = 1;
                    myInput = prevInputs[prevInputs.Count - prevPointer];
                }
            } else if (Event.current.keyCode == KeyCode.DownArrow) {
                if (prevInputs.Count > 0) {
                    if (prevPointer == 0 && myInput != string.Empty) {
                        prevInputs.Add(myInput);
                        if (prevInputs.Count > prevMax)
                            prevInputs.RemoveRange(0, prevInputs.Count - prevMax);
                        prevPointer--;
                    }
                    prevPointer--;
                    if (prevPointer < 1) prevPointer = prevInputs.Count;
                    myInput = prevInputs[prevInputs.Count - prevPointer];
                }
            } else prevPointer = 0;
        }
        if (doFocus) {
            GUI.FocusControl("command");
            doFocus = false;
        }
        if (GUI.GetNameOfFocusedControl() == "command") CrewSim.Typing = true;
        else CrewSim.Typing = false;
        if (Event.current.keyCode == KeyCode.Return && logHistoryPath != null) {
            try {
                File.WriteAllText(logHistoryPath,
                    string.Join("\n", prevInputs.ToArray()));
            } catch (Exception ex) {
                Debug.Log($"Failed to save console history!\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        GUI.DragWindow();
    }
}

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
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
private void DrawConsole(int window)
{
	if (HandleInput())
	{
		return;
	}
	Rect rect = new Rect(10f, 20f, Screen.width / 2 - 40, 4000f);
	Rect position = new Rect(10f, 30f, Screen.width / 2 - 20, (float)(Screen.height / 2) - (_textActual + 55f));
	Rect position2 = new Rect(10f, position.y + position.height + 5f, Screen.width / 2 - 40, _textActual + 10f);
	scrollPos = GUI.BeginScrollView(position, scrollPos, rect, alwaysShowHorizontal: false, alwaysShowVertical: true);
	GUI.TextArea(rect, myLog, mChars, logStyle);
	GUI.EndScrollView();
	GUI.SetNextControlName("command");
	myInput = GUI.TextField(position2, myInput, txtStyle);
	if (Event.current.isKey && GUI.GetNameOfFocusedControl() == "command")
	{
		if (Event.current.keyCode == KeyCode.Return)
		{
			string[] array = myInput.Split(';');
			if (array.Length > 1)
			{
				myInput = "<color=" + multipleColor + "><b>[Command]</b></color>: " + myInput;
				myLog = myLog + "\n" + myInput;
			}
			myInput = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] == string.Empty))
				{
					prevInputs.Add(array[i]);
					if (prevInputs.Count > prevMax)
					{
						prevInputs.RemoveRange(0, prevInputs.Count - prevMax);
					}
					if (ConsoleResolver.ResolveString(ref array[i]))
					{
						array[i] = "<color=" + commandColor + "><b>[Command]</b></color>: " + array[i];
					}
					else
					{
						array[i] = "<color=" + failedColor + "><b>[Command]</b></color>: " + array[i];
					}
					myLog = myLog + "\n" + array[i];
				}
			}
			prevPointer = 0;
		}
		else if (Event.current.keyCode == KeyCode.UpArrow)
		{
			if (prevInputs.Count > 0)
			{
				if (prevPointer == 0 && myInput != string.Empty)
				{
					prevInputs.Add(myInput);
					if (prevInputs.Count > prevMax)
					{
						prevInputs.RemoveRange(0, prevInputs.Count - prevMax);
					}
					prevPointer++;
				}
				prevPointer++;
				if (prevPointer > prevInputs.Count)
				{
					prevPointer = 1;
				}
				myInput = prevInputs[prevInputs.Count - prevPointer];
			}
		}
		else if (Event.current.keyCode == KeyCode.DownArrow)
		{
			if (prevInputs.Count > 0)
			{
				if (prevPointer == 0 && myInput != string.Empty)
				{
					prevInputs.Add(myInput);
					if (prevInputs.Count > prevMax)
					{
						prevInputs.RemoveRange(0, prevInputs.Count - prevMax);
					}
					prevPointer--;
				}
				prevPointer--;
				if (prevPointer < 1)
				{
					prevPointer = prevInputs.Count;
				}
				myInput = prevInputs[prevInputs.Count - prevPointer];
			}
		}
		else
		{
			prevPointer = 0;
		}
	}
	if (doFocus)
	{
		GUI.FocusControl("command");
		doFocus = false;
	}
	if (GUI.GetNameOfFocusedControl() == "command")
	{
		CrewSim.Typing = true;
	}
	else
	{
		CrewSim.Typing = false;
	}
	GUI.DragWindow();
}

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