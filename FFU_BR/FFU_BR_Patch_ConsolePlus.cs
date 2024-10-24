﻿#pragma warning disable CS0108
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

public class patch_ConsoleToGUI : ConsoleToGUI {
	private string logHistoryPath = null;
	private extern void orig_LoadJson();
    private void LoadJson() {
		orig_LoadJson();
		logHistoryPath = Path.Combine(Application.dataPath, "console_history.txt");
        if (logHistoryPath != null && File.Exists(logHistoryPath)) {
			try {
                prevInputs = File.ReadAllText(logHistoryPath, 
					Encoding.UTF8).Split('\n').ToList();
			} catch { }
        }
    }

	private extern void orig_DrawConsole(int window);
    private void DrawConsole(int window) {
        orig_DrawConsole(window);
        if (Event.current.keyCode == KeyCode.Return && logHistoryPath != null) {
			try {
				File.WriteAllText(logHistoryPath, string.Join("\n", prevInputs.ToArray()));
			} catch { }
		}
    }
}

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
private void LoadJson()
{
	ConsoleData consoleData = ConsoleData.CreateFromJSON("console_params.json");
	if (consoleData != null)
	{
		myTitle = consoleData.consoleTitle;
		myLog = consoleData.startTxt;
		logName = consoleData.fileName;
		doLogs = consoleData.enableLog;
		doWarning = consoleData.enableWarning;
		doException = consoleData.enableException;
		doError = consoleData.enableError;
		doAssert = consoleData.enableAssert;
		doUnknown = consoleData.enableUnknown;
		saveToFile = consoleData.saveToFile;
		textSize = consoleData.textSize;
		kChars = consoleData.maxMessage;
		mChars = consoleData.maxTotal;
		iconSize = consoleData.popUpSize;
		guiButtonRect = new Rect(5f, 5f, iconSize, iconSize);
		windowRect = new Rect(windowRect.x, iconSize + 10, windowRect.width, windowRect.height);
	}
}

private void DrawConsole(int window)
{
	if (HandleInput())
	{
		return;
	}
	Rect rect = new Rect(10f, 20f, Screen.width / 2 - 60, 4000f);
	Rect position = new Rect(10f, 30f, Screen.width / 2 - 40, Screen.height / 2 - 90);
	Rect position2 = new Rect(10f, position.y + position.height + 5f, Screen.width / 2 - 60, textSize + 10);
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