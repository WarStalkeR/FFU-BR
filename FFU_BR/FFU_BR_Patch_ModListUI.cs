#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using UnityEngine;
using MonoMod;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;

public partial class patch_GUIOptions : GUIOptions {
    [MonoModReplace] private void Init() {
        cgOptions = base.transform.parent.GetComponent<CanvasGroup>();
        btnQuit = base.transform.Find("pnlMenu/btnQuit/btn").GetComponent<Button>();
        btnQuit.onClick.AddListener(delegate
        {
            Exit();
        });
        btnVideo = base.transform.Find("pnlMenu/btnVideo/btn").GetComponent<Button>();
        btnVideo.onClick.AddListener(delegate
        {
            State = cgVideo;
        });
        chkScreen = base.transform.Find("pnlVideo/pnlStyle/chkScreen").GetComponent<Toggle>();
        chkScreen.isOn = Screen.fullScreen;
        chkScreen.onValueChanged.AddListener(delegate
        {
            SetRes();
        });
        sldFPS = base.transform.Find("pnlVideo/pnlFPS/sliderFPS/Slider").GetComponent<Slider>();
        sldFPS.value = PlayerPrefs.GetInt("TargetFPS", 60) / 10;
        sldFPS.onValueChanged.AddListener(delegate
        {
            SetFPS(sldFPS.value);
        });
        txtFPS = base.transform.Find("pnlVideo/pnlFPS/txtFPS").GetComponent<TMP_Text>();
        txtFPS.text = PlayerPrefs.GetInt("TargetFPS", 60).ToString();
        sldAO = base.transform.Find("pnlVideo/pnlAO/sliderAO/Slider").GetComponent<Slider>();
        sldAO.value = PlayerPrefs.GetInt("AmbientOcclusion", 8) / 2;
        sldAO.onValueChanged.AddListener(delegate
        {
            SetAO(sldAO.value);
        });
        txtAO = base.transform.Find("pnlVideo/pnlAO/txtAO").GetComponent<TMP_Text>();
        txtAO.text = PlayerPrefs.GetInt("AmbientOcclusion", 8).ToString();
        if (txtAO.text == "0") {
            txtAO.text = "Off";
        }
        chkParallax = base.transform.Find("pnlVideo/pnlParallax/chkParallax").GetComponent<Toggle>();
        chkParallax.isOn = PlayerPrefs.GetInt("Parallax", 1) == 1;
        chkParallax.onValueChanged.AddListener(delegate
        {
            ToggleParallax(chkParallax.isOn);
        });
        chkLoS = base.transform.Find("pnlVideo/pnlLoS/chkLoS").GetComponent<Toggle>();
        chkLoS.isOn = PlayerPrefs.GetInt("LineOfSight", 1) == 1;
        chkLoS.onValueChanged.AddListener(delegate
        {
            ToggleLoS(chkLoS.isOn);
        });
        chkA = base.transform.Find("pnlVideo/pnlTurbo/chkA").GetComponent<Toggle>();
        chkA.isOn = PlayerPrefs.GetInt("TurboA", 1) == 1;
        chkA.onValueChanged.AddListener(delegate
        {
            TurboA(chkA.isOn);
        });
        chkB = base.transform.Find("pnlVideo/pnlTurbo/chkB").GetComponent<Toggle>();
        chkB.isOn = PlayerPrefs.GetInt("TurboB", 1) == 1;
        chkB.onValueChanged.AddListener(delegate
        {
            TurboB(chkB.isOn);
        });
        btnSaveVideo = base.transform.Find("pnlVideo/pnlOutputs/btnSave/btn").GetComponent<Button>();
        btnSaveVideo.onClick.AddListener(delegate
        {
            SavePrefs();
        });
        txtPrefsVideo = base.transform.Find("pnlVideo/pnlOutputs/imgLCD/TextMeshPro - InputField").GetComponent<TMP_InputField>();
        txtPrefsVideo.text = PlayerPrefs.GetString("PrefsVideo");
        LoadPrefs();
        ddRes = base.transform.Find("pnlVideo/pnlResolution/ddRes").GetComponent<TMP_Dropdown>();
        List<string> resList = new List<string>();
        dictRes = new Dictionary<string, Resolution>();
        int resIndex = 0;
        ddRes.ClearOptions();
        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++) {
            Resolution refRes = resolutions[i];
            string resEntry = refRes.width + "x" + refRes.height;
            float resRatio = (float)refRes.width / (float)refRes.height;
            if ((double)resRatio >= 1.77 && (double)resRatio <= 1.78) {
                resEntry += "*";
            }
            if (resList.IndexOf(resEntry) < 0) {
                resList.Add(resEntry);
                dictRes[resEntry] = refRes;
                if (Screen.width == refRes.width && Screen.height == refRes.height) {
                    resIndex = resList.Count - 1;
                }
            }
        }
        ddRes.AddOptions(resList);
        ddRes.value = resIndex;
        ddRes.onValueChanged.AddListener(delegate
        {
            SetRes();
        });
        cgVideo = base.transform.Find("pnlVideo").GetComponent<CanvasGroup>();
        btnAudio = base.transform.Find("pnlMenu/btnAudio/btn").GetComponent<Button>();
        btnAudio.onClick.AddListener(delegate
        {
            State = cgAudio;
        });
        cgAudio = base.transform.Find("pnlAudio").GetComponent<CanvasGroup>();
        ledMaster = base.transform.Find("pnlAudio/pnlLedsMaster").GetComponent<GUILedMeter>();
        ledMaster.SetState(2);
        slidMaster = base.transform.Find("pnlAudio/SliderMaster").GetComponent<Slider>();
        VolMaster = VolMaster;
        slidMaster.value = VolMaster;
        slidMaster.onValueChanged.AddListener(delegate
        {
            VolMaster = slidMaster.value;
        });
        ledMusic = base.transform.Find("pnlAudio/pnlLedsMusic").GetComponent<GUILedMeter>();
        ledMusic.SetState(2);
        slidMusic = base.transform.Find("pnlAudio/SliderMusic").GetComponent<Slider>();
        VolMusic = VolMusic;
        slidMusic.value = VolMusic;
        slidMusic.onValueChanged.AddListener(delegate
        {
            VolMusic = slidMusic.value;
        });
        ledEffects = base.transform.Find("pnlAudio/pnlLedsEffects").GetComponent<GUILedMeter>();
        ledEffects.SetState(2);
        slidEffects = base.transform.Find("pnlAudio/SliderEffects").GetComponent<Slider>();
        VolEffects = VolEffects;
        slidEffects.value = VolEffects;
        slidEffects.onValueChanged.AddListener(delegate
        {
            VolEffects = slidEffects.value;
        });
        btnControls = base.transform.Find("pnlMenu/btnControls/btn").GetComponent<Button>();
        btnControls.onClick.AddListener(delegate
        {
            State = cgControls;
        });
        cgControls = base.transform.Find("pnlControls").GetComponent<CanvasGroup>();
        btnInterface = base.transform.Find("pnlMenu/btnInterface/btn").GetComponent<Button>();
        btnInterface.onClick.AddListener(delegate
        {
            State = cgInterface;
        });
        cgInterface = base.transform.Find("pnlInterface").GetComponent<CanvasGroup>();
        ddDateFormat = base.transform.Find("pnlInterface/ddDateFormat").GetComponent<TMP_Dropdown>();
        dictDateFormat = new Dictionary<string, string>();
        dictDateFormat.Add("ISO 8601: YYYY-MM-DD", MathUtils.DateFormat.YYYY_MM_DD.ToString());
        dictDateFormat.Add("DD-MM-YYYY", MathUtils.DateFormat.DD_MM_YYYY.ToString());
        dictDateFormat.Add("MM-DD-YYYY", MathUtils.DateFormat.MM_DD_YYYY.ToString());
        ddDateFormat.ClearOptions();
        ddDateFormat.AddOptions(new List<string>(dictDateFormat.Keys));
        foreach (TMP_Dropdown.OptionData dateFormat in ddDateFormat.options) {
            if (DataHandler.GetUserSettings().strDateFormat == dictDateFormat[dateFormat.text]) {
                ddDateFormat.value = ddDateFormat.options.IndexOf(dateFormat);
                break;
            }
        }
        ddDateFormat.onValueChanged.AddListener(delegate
        {
            ChangeDateFormat();
        });
        ddTempUnits = base.transform.Find("pnlInterface/ddTempUnits").GetComponent<TMP_Dropdown>();
        dictTempUnits = new Dictionary<string, string>();
        dictTempUnits.Add("Kelvin", MathUtils.TemperatureUnit.K.ToString());
        dictTempUnits.Add("Celsius", MathUtils.TemperatureUnit.C.ToString());
        ddTempUnits.ClearOptions();
        ddTempUnits.AddOptions(new List<string>(dictTempUnits.Keys));
        foreach (TMP_Dropdown.OptionData tempUnit in ddTempUnits.options) {
            if (DataHandler.GetUserSettings().strTemperatureUnit == dictTempUnits[tempUnit.text]) {
                ddTempUnits.value = ddTempUnits.options.IndexOf(tempUnit);
                break;
            }
        }
        ddTempUnits.onValueChanged.AddListener(delegate
        {
            ChangeTempUnits();
        });
        ddautoSaveInt = base.transform.Find("pnlInterface/ddAutoSaveInterval").GetComponent<TMP_Dropdown>();
        ddautoSaveInt.ClearOptions();
        ddautoSaveInt.AddOptions(new List<string>(dictAutoSaveInterval.Values));
        if (dictAutoSaveInterval.TryGetValue(DataHandler.GetUserSettings().nAutosaveInterval, out var saveIntVal)) {
            foreach (TMP_Dropdown.OptionData saveInt in ddautoSaveInt.options) {
                if (saveIntVal == saveInt.text) {
                    ddautoSaveInt.value = ddautoSaveInt.options.IndexOf(saveInt);
                    break;
                }
            }
        }
        ddautoSaveInt.onValueChanged.AddListener(ChangeAutoSaveInterval);
        ddautoSaveMaxCount = base.transform.Find("pnlInterface/ddautoSaveMaxCount").GetComponent<TMP_Dropdown>();
        ddautoSaveMaxCount.ClearOptions();
        ddautoSaveMaxCount.AddOptions(new List<string>(dictAutoSaveMaxCount.Values));
        if (dictAutoSaveMaxCount.TryGetValue(DataHandler.GetUserSettings().nAutosaveMaxCount, out var saveMaxVal)) {
            foreach (TMP_Dropdown.OptionData saveMax in ddautoSaveMaxCount.options) {
                if (saveMaxVal == saveMax.text) {
                    ddautoSaveMaxCount.value = ddautoSaveMaxCount.options.IndexOf(saveMax);
                    break;
                }
            }
        }
        ddautoSaveMaxCount.onValueChanged.AddListener(ChangeAutoSaveMaxCount);
        btnFiles = base.transform.Find("pnlMenu/btnFiles/btn").GetComponent<Button>();
        btnFiles.onClick.AddListener(delegate
        {
            State = cgFiles;
        });
        cgFiles = base.transform.Find("pnlFiles").GetComponent<CanvasGroup>();
        Button component = base.transform.Find("pnlFiles/btnMods/btn").GetComponent<Button>();
        component.onClick.AddListener(delegate
        {
            TryOpenModFolder();
        });
        base.transform.Find("pnlFiles/btnMods/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = DataHandler.strModFolder;
        component = base.transform.Find("pnlFiles/btnModsHelp").GetComponent<Button>();
        component.onClick.AddListener(delegate
        {
            try {
                Application.OpenURL("https://docs.google.com/document/d/1MVRows7dK7nS8DsQqSSG_UxGJRlOFmp0wv7eEN7SLGk/edit?usp=sharing");
            } catch (Exception ex5) {
                Debug.LogError(ex5.Message + "\n" + ex5.StackTrace.ToString());
            }
        });
        base.transform.Find("pnlFiles/btnScreenshots/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.persistentDataPath + "/Screenshots";
        component = base.transform.Find("pnlFiles/btnScreenshots/btn").GetComponent<Button>();
        component.onClick.AddListener(delegate
        {
            try {
                Application.OpenURL(Application.persistentDataPath + "/Screenshots");
            } catch (Exception ex4) {
                Debug.LogError(ex4.Message + "\n" + ex4.StackTrace.ToString());
            }
        });
        base.transform.Find("pnlFiles/btnScreenshots/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.persistentDataPath + "/Screenshots";
        component = base.transform.Find("pnlFiles/btnManuals/btn").GetComponent<Button>();
        component.onClick.AddListener(delegate
        {
            try {
                Application.OpenURL(Application.streamingAssetsPath + "/images/manuals");
            } catch (Exception ex3) {
                Debug.LogError(ex3.Message + "\n" + ex3.StackTrace.ToString());
            }
        });
        base.transform.Find("pnlFiles/btnManuals/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.streamingAssetsPath + "/images/manuals";
        component = base.transform.Find("pnlFiles/btnSave1/btn").GetComponent<Button>();
        component.onClick.AddListener(delegate
        {
            try {
                Application.OpenURL(Application.persistentDataPath);
            } catch (Exception ex2) {
                Debug.LogError(ex2.Message + "\n" + ex2.StackTrace.ToString());
            }
        });
        base.transform.Find("pnlFiles/btnSave1/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.persistentDataPath;
        component = base.transform.Find("pnlFiles/btnAssets/btn").GetComponent<Button>();
        component.onClick.AddListener(delegate
        {
            try {
                Application.OpenURL(Application.streamingAssetsPath);
            } catch (Exception ex) {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace.ToString());
            }
        });
        base.transform.Find("pnlFiles/btnAssets/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.streamingAssetsPath;
        aCGs = new List<CanvasGroup> { cgVideo, cgAudio, cgControls, cgInterface, cgFiles };
        tfList = base.transform.Find("pnlFiles/pnlList/Viewport/pnlListContent");
        JsonModList modList = null;
        GUIModRow prefabModRow = Resources.Load<GameObject>("prefabModRow").GetComponent<GUIModRow>();
        if (!DataHandler.dictModList.TryGetValue("Mod Loading Order", out modList)) {
            return;
        }
        string[] aLoadOrder = modList.aLoadOrder;
        foreach (string aLoadItem in aLoadOrder) {
			// Core Entry Already Exists
			if (aLoadItem == "core") continue;
			
			// Proceed With Mod List Rendering
            GUIModRow instModRow = UnityEngine.Object.Instantiate(prefabModRow, tfList);
            JsonModInfo modInfo = null;
            if (aLoadItem != null && aLoadItem != string.Empty && DataHandler.dictModInfos.TryGetValue(aLoadItem, out modInfo)) {
                instModRow.txtName.text = modInfo.strName;
                instModRow.Status = modInfo.Status;
            } else {
                instModRow.txtName.text = aLoadItem;
                instModRow.Status = GUIModRow.ModStatus.Missing;
            }
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
private void Init()
{
	cgOptions = base.transform.parent.GetComponent<CanvasGroup>();
	btnQuit = base.transform.Find("pnlMenu/btnQuit/btn").GetComponent<Button>();
	btnQuit.onClick.AddListener(delegate
	{
		Exit();
	});
	btnVideo = base.transform.Find("pnlMenu/btnVideo/btn").GetComponent<Button>();
	btnVideo.onClick.AddListener(delegate
	{
		State = cgVideo;
	});
	chkScreen = base.transform.Find("pnlVideo/pnlStyle/chkScreen").GetComponent<Toggle>();
	chkScreen.isOn = Screen.fullScreen;
	chkScreen.onValueChanged.AddListener(delegate
	{
		SetRes();
	});
	chkScreen.GetComponent<GUIAudioToggle>().requiresInit = false;
	sldFPS = base.transform.Find("pnlVideo/pnlFPS/sliderFPS/Slider").GetComponent<Slider>();
	sldFPS.value = PlayerPrefs.GetInt("TargetFPS", 60) / 10;
	sldFPS.onValueChanged.AddListener(delegate
	{
		SetFPS(sldFPS.value);
	});
	txtFPS = base.transform.Find("pnlVideo/pnlFPS/txtFPS").GetComponent<TMP_Text>();
	txtFPS.text = PlayerPrefs.GetInt("TargetFPS", 60).ToString();
	sldAO = base.transform.Find("pnlVideo/pnlAO/sliderAO/Slider").GetComponent<Slider>();
	sldAO.value = PlayerPrefs.GetInt("AmbientOcclusion", 8) / 2;
	sldAO.onValueChanged.AddListener(delegate
	{
		SetAO(sldAO.value);
	});
	txtAO = base.transform.Find("pnlVideo/pnlAO/txtAO").GetComponent<TMP_Text>();
	txtAO.text = PlayerPrefs.GetInt("AmbientOcclusion", 8).ToString();
	if (txtAO.text == "0")
	{
		txtAO.text = "Off";
	}
	chkParallax = base.transform.Find("pnlVideo/pnlParallax/chkParallax").GetComponent<Toggle>();
	chkParallax.isOn = PlayerPrefs.GetInt("Parallax", 1) == 1;
	chkParallax.onValueChanged.AddListener(delegate
	{
		ToggleParallax(chkParallax.isOn);
	});
	chkParallax.GetComponent<GUIAudioToggle>().requiresInit = false;
	chkLoS = base.transform.Find("pnlVideo/pnlLoS/chkLoS").GetComponent<Toggle>();
	chkLoS.isOn = PlayerPrefs.GetInt("LineOfSight", 1) == 1;
	chkLoS.onValueChanged.AddListener(delegate
	{
		ToggleLoS(chkLoS.isOn);
	});
	chkLoS.GetComponent<GUIAudioToggle>().requiresInit = false;
	chkA = base.transform.Find("pnlVideo/pnlScreenShake/chkSS").GetComponent<Toggle>();
	chkA.isOn = PlayerPrefs.GetFloat("ScreenShakeMod", 1f) == 1f;
	chkA.onValueChanged.AddListener(delegate
	{
		ScreenShake(chkA.isOn);
	});
	chkA.GetComponent<GUIAudioToggle>().requiresInit = false;
	chkB = base.transform.Find("pnlVideo/pnlTurbo/chkB").GetComponent<Toggle>();
	chkB.isOn = PlayerPrefs.GetInt("TurboB", 1) == 1;
	chkB.onValueChanged.AddListener(delegate
	{
		TurboB(chkB.isOn);
	});
	btnSaveVideo = base.transform.Find("pnlVideo/pnlOutputs/btnSave/btn").GetComponent<Button>();
	btnSaveVideo.onClick.AddListener(delegate
	{
		SavePrefs();
	});
	txtPrefsVideo = base.transform.Find("pnlVideo/pnlOutputs/imgLCD/TextMeshPro - InputField").GetComponent<TMP_InputField>();
	txtPrefsVideo.text = PlayerPrefs.GetString("PrefsVideo");
	LoadPrefs();
	ddRes = base.transform.Find("pnlVideo/pnlResolution/ddRes").GetComponent<TMP_Dropdown>();
	List<string> list = new List<string>();
	dictRes = new Dictionary<string, Resolution>();
	int value = 0;
	ddRes.ClearOptions();
	Resolution[] resolutions = Screen.resolutions;
	for (int i = 0; i < resolutions.Length; i++)
	{
		Resolution value2 = resolutions[i];
		string text = value2.width + "x" + value2.height;
		float num = (float)value2.width / (float)value2.height;
		if ((double)num >= 1.77 && (double)num <= 1.78)
		{
			text += "*";
		}
		if (list.IndexOf(text) < 0)
		{
			list.Add(text);
			dictRes[text] = value2;
			if (Screen.width == value2.width && Screen.height == value2.height)
			{
				value = list.Count - 1;
			}
		}
	}
	ddRes.AddOptions(list);
	ddRes.value = value;
	ddRes.onValueChanged.AddListener(delegate
	{
		SetRes();
	});
	cgVideo = base.transform.Find("pnlVideo").GetComponent<CanvasGroup>();
	btnAudio = base.transform.Find("pnlMenu/btnAudio/btn").GetComponent<Button>();
	btnAudio.onClick.AddListener(delegate
	{
		State = cgAudio;
	});
	cgAudio = base.transform.Find("pnlAudio").GetComponent<CanvasGroup>();
	ledMaster = base.transform.Find("pnlAudio/pnlLedsMaster").GetComponent<GUILedMeter>();
	ledMaster.SetState(2);
	slidMaster = base.transform.Find("pnlAudio/SliderMaster").GetComponent<Slider>();
	VolMaster = VolMaster;
	slidMaster.value = VolMaster;
	slidMaster.onValueChanged.AddListener(delegate
	{
		VolMaster = slidMaster.value;
	});
	ledMusic = base.transform.Find("pnlAudio/pnlLedsMusic").GetComponent<GUILedMeter>();
	ledMusic.SetState(2);
	slidMusic = base.transform.Find("pnlAudio/SliderMusic").GetComponent<Slider>();
	VolMusic = VolMusic;
	slidMusic.value = VolMusic;
	slidMusic.onValueChanged.AddListener(delegate
	{
		VolMusic = slidMusic.value;
	});
	ledEffects = base.transform.Find("pnlAudio/pnlLedsEffects").GetComponent<GUILedMeter>();
	ledEffects.SetState(2);
	slidEffects = base.transform.Find("pnlAudio/SliderEffects").GetComponent<Slider>();
	VolEffects = VolEffects;
	slidEffects.value = VolEffects;
	slidEffects.onValueChanged.AddListener(delegate
	{
		VolEffects = slidEffects.value;
	});
	btnControls = base.transform.Find("pnlMenu/btnControls/btn").GetComponent<Button>();
	btnControls.onClick.AddListener(delegate
	{
		State = cgControls;
	});
	cgControls = base.transform.Find("pnlControls").GetComponent<CanvasGroup>();
	btnInterface = base.transform.Find("pnlMenu/btnInterface/btn").GetComponent<Button>();
	btnInterface.onClick.AddListener(delegate
	{
		State = cgInterface;
	});
	cgInterface = base.transform.Find("pnlInterface").GetComponent<CanvasGroup>();
	ddDateFormat = base.transform.Find("pnlInterface/ddDateFormat").GetComponent<TMP_Dropdown>();
	dictDateFormat = new Dictionary<string, string>();
	dictDateFormat.Add("ISO 8601: YYYY-MM-DD", MathUtils.DateFormat.YYYY_MM_DD.ToString());
	dictDateFormat.Add("DD-MM-YYYY", MathUtils.DateFormat.DD_MM_YYYY.ToString());
	dictDateFormat.Add("MM-DD-YYYY", MathUtils.DateFormat.MM_DD_YYYY.ToString());
	ddDateFormat.ClearOptions();
	ddDateFormat.AddOptions(new List<string>(dictDateFormat.Keys));
	foreach (TMP_Dropdown.OptionData option in ddDateFormat.options)
	{
		if (DataHandler.GetUserSettings().strDateFormat == dictDateFormat[option.text])
		{
			ddDateFormat.value = ddDateFormat.options.IndexOf(option);
			break;
		}
	}
	ddDateFormat.onValueChanged.AddListener(delegate
	{
		ChangeDateFormat();
	});
	ddTempUnits = base.transform.Find("pnlInterface/ddTempUnits").GetComponent<TMP_Dropdown>();
	dictTempUnits = new Dictionary<string, string>();
	dictTempUnits.Add("Kelvin", MathUtils.TemperatureUnit.K.ToString());
	dictTempUnits.Add("Celsius", MathUtils.TemperatureUnit.C.ToString());
	ddTempUnits.ClearOptions();
	ddTempUnits.AddOptions(new List<string>(dictTempUnits.Keys));
	foreach (TMP_Dropdown.OptionData option2 in ddTempUnits.options)
	{
		if (DataHandler.GetUserSettings().strTemperatureUnit == dictTempUnits[option2.text])
		{
			ddTempUnits.value = ddTempUnits.options.IndexOf(option2);
			break;
		}
	}
	ddTempUnits.onValueChanged.AddListener(delegate
	{
		ChangeTempUnits();
	});
	ddautoSaveInt = base.transform.Find("pnlInterface/ddAutoSaveInterval").GetComponent<TMP_Dropdown>();
	ddautoSaveInt.ClearOptions();
	ddautoSaveInt.AddOptions(new List<string>(dictAutoSaveInterval.Values));
	if (dictAutoSaveInterval.TryGetValue(DataHandler.GetUserSettings().nAutosaveInterval, out var value3))
	{
		foreach (TMP_Dropdown.OptionData option3 in ddautoSaveInt.options)
		{
			if (value3 == option3.text)
			{
				ddautoSaveInt.value = ddautoSaveInt.options.IndexOf(option3);
				break;
			}
		}
	}
	ddautoSaveInt.onValueChanged.AddListener(ChangeAutoSaveInterval);
	ddautoSaveMaxCount = base.transform.Find("pnlInterface/ddautoSaveMaxCount").GetComponent<TMP_Dropdown>();
	ddautoSaveMaxCount.ClearOptions();
	ddautoSaveMaxCount.AddOptions(new List<string>(dictAutoSaveMaxCount.Values));
	if (dictAutoSaveMaxCount.TryGetValue(DataHandler.GetUserSettings().nAutosaveMaxCount, out var value4))
	{
		foreach (TMP_Dropdown.OptionData option4 in ddautoSaveMaxCount.options)
		{
			if (value4 == option4.text)
			{
				ddautoSaveMaxCount.value = ddautoSaveMaxCount.options.IndexOf(option4);
				break;
			}
		}
	}
	ddautoSaveMaxCount.onValueChanged.AddListener(ChangeAutoSaveMaxCount);
	btnFiles = base.transform.Find("pnlMenu/btnFiles/btn").GetComponent<Button>();
	btnFiles.onClick.AddListener(delegate
	{
		State = cgFiles;
	});
	cgFiles = base.transform.Find("pnlFiles").GetComponent<CanvasGroup>();
	Button component = base.transform.Find("pnlFiles/btnMods/btn").GetComponent<Button>();
	component.onClick.AddListener(delegate
	{
		TryOpenModFolder();
	});
	base.transform.Find("pnlFiles/btnMods/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = DataHandler.strModFolder;
	component = base.transform.Find("pnlFiles/btnModsHelp").GetComponent<Button>();
	component.onClick.AddListener(delegate
	{
		try
		{
			Application.OpenURL("https://docs.google.com/document/d/1MVRows7dK7nS8DsQqSSG_UxGJRlOFmp0wv7eEN7SLGk/edit?usp=sharing");
		}
		catch (Exception ex5)
		{
			Debug.LogError(ex5.Message + "\n" + ex5.StackTrace.ToString());
		}
	});
	base.transform.Find("pnlFiles/btnScreenshots/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.persistentDataPath + "/Screenshots";
	component = base.transform.Find("pnlFiles/btnScreenshots/btn").GetComponent<Button>();
	component.onClick.AddListener(delegate
	{
		try
		{
			Application.OpenURL(Application.persistentDataPath + "/Screenshots");
		}
		catch (Exception ex4)
		{
			Debug.LogError(ex4.Message + "\n" + ex4.StackTrace.ToString());
		}
	});
	base.transform.Find("pnlFiles/btnScreenshots/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.persistentDataPath + "/Screenshots";
	component = base.transform.Find("pnlFiles/btnManuals/btn").GetComponent<Button>();
	component.onClick.AddListener(delegate
	{
		try
		{
			Application.OpenURL(Application.streamingAssetsPath + "/images/manuals");
		}
		catch (Exception ex3)
		{
			Debug.LogError(ex3.Message + "\n" + ex3.StackTrace.ToString());
		}
	});
	base.transform.Find("pnlFiles/btnManuals/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.streamingAssetsPath + "/images/manuals";
	component = base.transform.Find("pnlFiles/btnSave1/btn").GetComponent<Button>();
	component.onClick.AddListener(delegate
	{
		try
		{
			Application.OpenURL(Application.persistentDataPath);
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.Message + "\n" + ex2.StackTrace.ToString());
		}
	});
	base.transform.Find("pnlFiles/btnSave1/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.persistentDataPath;
	component = base.transform.Find("pnlFiles/btnAssets/btn").GetComponent<Button>();
	component.onClick.AddListener(delegate
	{
		try
		{
			Application.OpenURL(Application.streamingAssetsPath);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace.ToString());
		}
	});
	base.transform.Find("pnlFiles/btnAssets/boxFilePath/txt").GetComponent<TextMeshProUGUI>().text = Application.streamingAssetsPath;
	aCGs = new List<CanvasGroup> { cgVideo, cgAudio, cgControls, cgInterface, cgFiles };
	tfList = base.transform.Find("pnlFiles/pnlList/Viewport/pnlListContent");
	JsonModList value5 = null;
	GUIModRow component2 = Resources.Load<GameObject>("prefabModRow").GetComponent<GUIModRow>();
	if (!DataHandler.dictModList.TryGetValue("Mod Loading Order", out value5))
	{
		return;
	}
	string[] aLoadOrder = value5.aLoadOrder;
	foreach (string text2 in aLoadOrder)
	{
		GUIModRow gUIModRow = UnityEngine.Object.Instantiate(component2, tfList);
		JsonModInfo value6 = null;
		if (text2 != null && text2 != string.Empty && DataHandler.dictModInfos.TryGetValue(text2, out value6))
		{
			gUIModRow.txtName.text = value6.strName;
			gUIModRow.Status = value6.Status;
		}
		else
		{
			gUIModRow.txtName.text = text2;
			gUIModRow.Status = GUIModRow.ModStatus.Missing;
		}
	}
}
*/