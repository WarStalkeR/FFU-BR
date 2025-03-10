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



// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* ConsoleToGUI.DrawConsole
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
*/