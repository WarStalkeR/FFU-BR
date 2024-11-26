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

public partial class patch_CondTrigger : CondTrigger {
    public int maxDepth { get; set; }

    public extern CondTrigger orig_Clone();
    public CondTrigger Clone() {
        patch_CondTrigger condTrigger = orig_Clone() as patch_CondTrigger;
        condTrigger.maxDepth = maxDepth;
        return condTrigger;
    }

    public extern bool orig_Triggered(CondOwner objOwner, string strIAStatsName = null, bool logOutcome = true);
    public bool Triggered(CondOwner objOwner, string strIAStatsName = null, bool logOutcome = true) {
		if (maxDepth > 0 && objOwner != null 
			&& GetDepth(objOwner) > maxDepth)
			return false;
        return orig_Triggered(objOwner, strIAStatsName, logOutcome);
    }

    public static int GetDepth(CondOwner objCO) {
        int currDepth = 0;
        CondOwner objParent = objCO.objCOParent;
        while (objParent != null) {
            objParent = objParent.objCOParent;
            currDepth++;
        }
        return currDepth;
    }
}

// Prototype Changes (for TEST BEGIN/TEST END):
/*
[MonoModReplace] public bool Triggered(CondOwner objOwner, string strIAStatsName = null, bool logOutcome = true) {
    //...ORIGINAL CODE...//
    objOwner.ValidateParent();
    // MAX DEPTH TEST BEGIN //
    if (maxDepth > 0 &&
        GetDepth(objOwner) > maxDepth)
        return false;
    // MAX DEPTH TEST END //
    SocialStats socStats = null;
    //...ORIGINAL CODE...//
}
*/

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
CondTrigger.Clone
CondTrigger.Triggered

public CondTrigger Clone()
{
	CondTrigger condTrigger = new CondTrigger();
	condTrigger.strName = strName;
	condTrigger.strCondName = strCondName;
	condTrigger._fChance = _fChance;
	condTrigger._fCount = _fCount;
	condTrigger.bAND = bAND;
	condTrigger.aReqs = aReqs;
	condTrigger.aForbids = aForbids;
	condTrigger.aTriggers = aTriggers;
	condTrigger.aTriggersConds = aTriggersConds;
	condTrigger.aTriggersForbid = aTriggersForbid;
	condTrigger.aTriggersForbidConds = aTriggersForbidConds;
	condTrigger._requiresHumans = _requiresHumans;
	condTrigger.strFailReason = strFailReason;
	condTrigger.strFailReasonLast = strFailReasonLast;
	condTrigger.strHigherCond = strHigherCond;
	condTrigger.aLowerConds = aLowerConds;
	condTrigger._isBlank = _isBlank;
	condTrigger._valuesWereChanged = false;
	return condTrigger;
}

public bool Triggered(CondOwner objOwner, string strIAStatsName = null, bool logOutcome = true)
{
	if (logReason)
	{
		logReason = logOutcome;
	}
	strFailReasonLast = string.Empty;
	if (objOwner == null)
	{
		return false;
	}
	if (IsBlank())
	{
		return true;
	}
	objOwner.ValidateParent();
	SocialStats value = null;
	if (strIAStatsName != null && DataHandler.dictSocialStats.TryGetValue(strIAStatsName, out value))
	{
		value.nChecked++;
	}
	if (!bChanceSkip && fChance < 1f)
	{
		float num = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat);
		if (num > fChance)
		{
			if (value != null)
			{
				value.nChecked++;
			}
			if (logReason)
			{
				strFailReasonLast = "Chance: " + num + " / " + fChance;
			}
			return false;
		}
	}
	Condition value2;
	if (bAND)
	{
		if (strHigherCond != null)
		{
			value2 = null;
			double num2 = 0.0;
			double num3 = 0.0;
			objOwner.mapConds.TryGetValue(strHigherCond, out value2);
			if (value2 != null)
			{
				num2 = value2.fCount;
			}
			string[] array = aLowerConds;
			foreach (string key in array)
			{
				value2 = null;
				num3 = (objOwner.mapConds.TryGetValue(key, out value2) ? value2.fCount : 0.0);
				if (num3 > num2)
				{
					return false;
				}
			}
		}
		string[] array2 = aReqs;
		foreach (string text in array2)
		{
			if (!objOwner.mapConds.TryGetValue(text, out value2))
			{
				StatsTrackReqs(strIAStatsName, text, 1f);
				if (logReason)
				{
					strFailReasonLast = "Lacking: " + text;
				}
				return false;
			}
			if (value2 == null || value2.fCount <= 0.0)
			{
				StatsTrackReqs(strIAStatsName, text, 1f);
				if (logReason)
				{
					strFailReasonLast = "Lacking: " + text;
				}
				return false;
			}
		}
		value2 = null;
		string[] array3 = aForbids;
		foreach (string text2 in array3)
		{
			if (objOwner.mapConds.TryGetValue(text2, out value2) && value2.fCount > 0.0)
			{
				StatsTrackForbids(strIAStatsName, text2, 1f);
				if (logReason)
				{
					strFailReasonLast = "Forbidden: " + text2;
				}
				return false;
			}
		}
		string[] array4 = aTriggers;
		foreach (string strTrig in array4)
		{
			CondTrigger trigger = GetTrigger(strTrig, CTDict.Triggers);
			if (!trigger.Triggered(objOwner, strIAStatsName, logReason))
			{
				if (logReason)
				{
					strFailReasonLast = trigger.strFailReasonLast;
				}
				return false;
			}
		}
		return true;
	}
	string[] array5 = aForbids;
	foreach (string text3 in array5)
	{
		if (objOwner.mapConds.TryGetValue(text3, out value2) && value2.fCount > 0.0)
		{
			StatsTrackForbids(strIAStatsName, text3, 1f);
			if (logReason)
			{
				strFailReasonLast = "Forbidden: " + text3;
			}
			return false;
		}
	}
	string[] array6 = aTriggersForbid;
	foreach (string strTrig2 in array6)
	{
		CondTrigger trigger2 = GetTrigger(strTrig2, CTDict.Forbids);
		if (!trigger2.Triggered(objOwner, strIAStatsName, logReason))
		{
			if (logReason)
			{
				strFailReasonLast = trigger2.strFailReasonLast;
			}
			return false;
		}
	}
	if (strHigherCond != null)
	{
		value2 = null;
		double num4 = 0.0;
		objOwner.mapConds.TryGetValue(strHigherCond, out value2);
		if (value2 != null)
		{
			num4 = value2.fCount;
		}
		string[] array7 = aLowerConds;
		foreach (string key2 in array7)
		{
			value2 = null;
			if (objOwner.mapConds.TryGetValue(key2, out value2) && value2.fCount <= num4)
			{
				return true;
			}
		}
	}
	string text4 = "Lacking: (";
	bool flag = false;
	string[] array8 = aReqs;
	foreach (string text5 in array8)
	{
		if (logReason)
		{
			text4 = text4 + text5 + " ";
		}
		flag = true;
		if (objOwner.mapConds.TryGetValue(text5, out value2) && value2 != null && value2.fCount > 0.0)
		{
			return true;
		}
	}
	if (flag && logReason)
	{
		strFailReasonLast = strFailReasonLast + text4 + ")";
	}
	if (logReason)
	{
		text4 = "Triggers Lacking: (";
	}
	flag = false;
	string[] array9 = aTriggers;
	foreach (string strTrig3 in array9)
	{
		CondTrigger trigger3 = GetTrigger(strTrig3, CTDict.Triggers);
		if (trigger3.Triggered(objOwner, strIAStatsName, logReason))
		{
			return true;
		}
		if (logReason)
		{
			text4 = text4 + trigger3.strFailReasonLast + " ";
		}
		flag = true;
	}
	if (flag && logReason)
	{
		strFailReasonLast = strFailReasonLast + text4 + ")";
	}
	if (aReqs.Length + aTriggers.Length == 0)
	{
		return true;
	}
	string[] array10 = aReqs;
	foreach (string strCond in array10)
	{
		StatsTrackReqs(strIAStatsName, strCond, 1f / (float)aReqs.Length);
	}
	return false;
}
*/