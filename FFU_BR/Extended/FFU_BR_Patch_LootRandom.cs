using Ostranauts.Core.Models;
using System.Collections.Generic;
using UnityEngine;
using MonoMod;
using FFU_Beyond_Reach;
using System.Linq;

public partial class patch_Loot : Loot {
    [MonoModReplace] public List<CondTrigger> GetCTLoot(CondTrigger objUs, string strRandID = null) {
        List<CondTrigger> lootCTs = new List<CondTrigger>();
        int lootIdx = 0;
        float randOff = 0f;
        float fRandMax = 1f;
        foreach (List<LootUnit> aCOLootUnit in aCOLootUnits) {
            randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aCOLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootItm in aCOLootUnit) {
                float lootQty = lootItm.GetAmount(randOff, fRand);
                if (!lootItm.bPositive) lootQty = 0f - lootQty;
                randOff += lootItm.fChance;
                if (lootQty > 0f) {
                    CondTrigger lootCT = null;
                    lootCT = (objUs == null || !(lootItm.strName == "[us]")) ? 
                        DataHandler.GetCondTrigger(lootItm.strName) : objUs.Clone();
                    lootCT.fCount *= lootQty;
                    lootCTs.Add(lootCT);
                    break;
                }
            }
        }
        lootIdx = 0;
        fRandMax = 1f;
        foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits) {
            randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aOtherLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootItm in aOtherLootUnit) {
                float lootQty = lootItm.GetAmount(randOff, fRand);
                if (!lootItm.bPositive) lootQty = 0f - lootQty;
                randOff += lootItm.fChance;
                if (lootQty <= 0f) continue;
                List<CondTrigger> cTLoot = DataHandler.GetLoot(lootItm.strName).GetCTLoot(objUs, strRandID);
                foreach (CondTrigger lootCond in cTLoot) lootCond.fCount *= lootQty;
                lootCTs.AddRange(cTLoot);
                break;
            }
        }
        return lootCTs;
    }

    [MonoModReplace] public List<CondOwner> GetCOLoot(CondOwner objUs, bool bSuppressOverride, string strRandID = null) {
        List<CondOwner> lootCOs = new List<CondOwner>();
        int lootIdx = 0;
        float randOff = 0f;
        float fRandMax = 1f;
        foreach (List<LootUnit> aCOLootUnit in aCOLootUnits) {
            randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aCOLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootItm in aCOLootUnit) {
                float lootQty = lootItm.GetAmount(randOff, fRand);
                if (!lootItm.bPositive) lootQty = 0f - lootQty;
                randOff += lootItm.fChance;
                if (lootQty <= 0f) continue;
                int lootInt = Mathf.FloorToInt(lootQty);
                for (int i = 0; i < lootInt; i++) {
                    if (objUs != null && lootItm.strName == "[us]") lootCOs.Add(objUs);
                    else lootCOs.Add(DataHandler.GetCondOwner(lootItm.strName, 
                        null, null, !bSuppress || !bSuppressOverride));
                }
                break;
            }
        }
        lootIdx = 0;
        fRandMax = 1f;
        foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits) {
            randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aOtherLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootUnit in aOtherLootUnit) {
                float lootQty = lootUnit.GetAmount(randOff, fRand);
                if (!lootUnit.bPositive) lootQty = 0f - lootQty;
                randOff += lootUnit.fChance;
                if (lootQty <= 0f) continue;
                for (int i = 0; i < lootQty; i++) {
                    Loot lootRef = DataHandler.GetLoot(lootUnit.strName);
                    if (lootRef.strName != lootUnit.strName) Debug.Log(strName + 
                        " expected loot: " + lootUnit.strName + " but got " + lootRef.strName);
                    lootCOs.AddRange(lootRef.GetCOLoot(objUs, bSuppress && bSuppressOverride, strRandID));
                }
                break;
            }
        }
        if (bNested) {
            for (int coIdx = lootCOs.Count - 1; coIdx > 0; coIdx--) {
                for (int nestIdx = coIdx - 1; nestIdx >= 0; nestIdx--) {
                    lootCOs[coIdx] = lootCOs[nestIdx].AddCO(lootCOs[coIdx], 
                        bEquip: true, bOverflow: true, bIgnoreLocks: true);
                    if (lootCOs[coIdx] == null) {
                        lootCOs.RemoveAt(coIdx);
                        break;
                    }
                }
            }
        }
        return lootCOs;
    }

    [MonoModReplace] public List<string> GetLootNames(string strRandID = null, bool bOnlyCOs = false, string type = null) {
        List<string> lootNames = new List<string>();
        int lootIdx = 0;
        float randOff = 0f;
        float fRandMax = 1f;
        foreach (List<LootUnit> aCOLootUnit in aCOLootUnits) {
            if (!string.IsNullOrEmpty(type) && strType != type) break;
            randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aCOLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootItm in aCOLootUnit) {
                float lootQty = lootItm.GetAmount(randOff, fRand);
                randOff += lootItm.fChance;
                string lootName = lootItm.strName;
                if (lootQty < 0f) {
                    lootName = "-" + lootName;
                    lootQty = 0f - lootQty;
                }
                if (lootQty > 0f) {
                    for (int i = 0; i < lootQty; i++) 
                        lootNames.Add(lootName);
                    break;
                }
            }
        }
        if (bOnlyCOs) return lootNames;
        lootIdx = 0;
        fRandMax = 1f;
        foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits) {
            randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aOtherLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootItm in aOtherLootUnit) {
                float lootQty = lootItm.GetAmount(randOff, fRand);
                randOff += lootItm.fChance;
                string lootName = lootItm.strName;
                if (lootQty < 0f) {
                    lootName = "-" + lootName;
                    lootQty = 0f - lootQty;
                }
                if (lootQty <= 0f) continue;
                for (int j = 0; j < lootQty; j++) {
                    Loot lootRef = DataHandler.GetLoot(lootItm.strName);
                    if (string.IsNullOrEmpty(type) || !(lootRef.strType != type)) 
                        lootNames.AddRange(lootRef.GetLootNames(lootId));
                }
                break;
            }
        }
        return lootNames;
    }

    [MonoModReplace] public void ApplyCondLoot(CondOwner coUs, float fCoeff, string strRandID = null, float fCondRuleTrack = 0f) {
        if (coUs == null || strType != "condition") return;
        string[] condOwners = aCOs;
        foreach (string condOwner in condOwners) 
            coUs.ParseCondEquation(condOwner, fCoeff, fCondRuleTrack);
        int lootIdx = 0;
        float fRandMax = 1f;
        foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits) {
            float randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aOtherLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit item in aOtherLootUnit) {
                float lootQty = item.GetAmount(randOff, fRand);
                if (!item.bPositive) lootQty = 0f - lootQty;
                randOff += item.fChance;
                if (lootQty > 0f) {
                    for (int j = 0; j < lootQty; j++) {
                        Loot lootRef = DataHandler.GetLoot(item.strName);
                        lootRef.ApplyCondLoot(coUs, fCoeff, null, fCondRuleTrack);
                    }
                    break;
                }
            }
        }
    }

    [MonoModReplace] public Dictionary<string, double> GetCondLoot(float fCoeff, Dictionary<string, double> dictOut, string strRandID = null) {
        if (dictOut == null) dictOut = new Dictionary<string, double>();
        if (strType != "condition") return dictOut;
        string[] condOwners = aCOs;
        foreach (string condOwner in condOwners) {
            KeyValuePair<string, Tuple<double, double>> condEq = ParseCondEquation(condOwner);
            double fMult = condEq.Value.Item1;
            if (condEq.Value.Item2 != fMult) fMult = MathUtils.Rand(condEq.Value.Item1, 
                condEq.Value.Item2, MathUtils.RandType.Flat);
            if (condEq.Key != string.Empty && fMult != 0.0) {
                if (!dictOut.ContainsKey(condEq.Key)) 
                    dictOut[condEq.Key] = fMult * (double)fCoeff;
                else dictOut[condEq.Key] += fMult * (double)fCoeff;
            }
        }
        int lootIdx = 0;
        float fRandMax = 1f;
        foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits) {
            float randOff = 0f;
            string lootId = strRandID;
            if (lootId != null) lootId += lootIdx;
            lootIdx++;
            if (FFU_BR_Defs.DynamicRandomRange) {
                float fNewMax = 0f;
                foreach (LootUnit lootItm in aOtherLootUnit)
                    if (!FFU_BR_Defs.IgnoredKeys.Contains(lootItm.strName)) 
                        fNewMax += lootItm.fChance;
                fRandMax = fNewMax > 1f ? fNewMax : 1f;
            }
            float fRand = MathUtils.Rand(0f, fRandMax, MathUtils.RandType.Flat, lootId);
            foreach (LootUnit lootItm in aOtherLootUnit) {
                float lootQty = lootItm.GetAmount(randOff, fRand);
                if (!lootItm.bPositive) lootQty = 0f - lootQty;
                randOff += lootItm.fChance;
                if (lootQty > 0f) {
                    for (int j = 0; j < lootQty; j++) {
                        Loot lootRef = DataHandler.GetLoot(lootItm.strName);
                        lootRef.GetCondLoot(fCoeff, dictOut);
                    }
                    break;
                }
            }
        }
        return dictOut;
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* Loot.GetCTLoot
public List<CondTrigger> GetCTLoot(CondTrigger objUs, string strRandID = null)
{
	List<CondTrigger> list = new List<CondTrigger>();
	float num = 0f;
	int num2 = 0;
	foreach (List<LootUnit> aCOLootUnit in aCOLootUnits)
	{
		num = 0f;
		string text = strRandID;
		if (text != null)
		{
			text += num2;
		}
		num2++;
		float fRand = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text);
		foreach (LootUnit item in aCOLootUnit)
		{
			float num3 = item.GetAmount(num, fRand);
			if (!item.bPositive)
			{
				num3 = 0f - num3;
			}
			num += item.fChance;
			if (num3 > 0f)
			{
				CondTrigger condTrigger = null;
				condTrigger = ((objUs == null || !(item.strName == "[us]")) ? DataHandler.GetCondTrigger(item.strName) : objUs.Clone());
				condTrigger.fCount *= num3;
				list.Add(condTrigger);
				break;
			}
		}
	}
	num2 = 0;
	foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits)
	{
		num = 0f;
		string text2 = strRandID;
		if (text2 != null)
		{
			text2 += num2;
		}
		num2++;
		float fRand2 = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text2);
		foreach (LootUnit item2 in aOtherLootUnit)
		{
			float num4 = item2.GetAmount(num, fRand2);
			if (!item2.bPositive)
			{
				num4 = 0f - num4;
			}
			num += item2.fChance;
			if (!(num4 > 0f))
			{
				continue;
			}
			List<CondTrigger> cTLoot = DataHandler.GetLoot(item2.strName).GetCTLoot(objUs, strRandID);
			foreach (CondTrigger item3 in cTLoot)
			{
				item3.fCount *= num4;
			}
			list.AddRange(cTLoot);
			break;
		}
	}
	return list;
}
*/

/* Loot.GetCOLoot
public List<CondOwner> GetCOLoot(CondOwner objUs, bool bSuppressOverride, string strRandID = null)
{
	List<CondOwner> list = new List<CondOwner>();
	float num = 0f;
	int num2 = 0;
	foreach (List<LootUnit> aCOLootUnit in aCOLootUnits)
	{
		num = 0f;
		string text = strRandID;
		if (text != null)
		{
			text += num2;
		}
		num2++;
		float fRand = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text);
		foreach (LootUnit item in aCOLootUnit)
		{
			float num3 = item.GetAmount(num, fRand);
			if (!item.bPositive)
			{
				num3 = 0f - num3;
			}
			num += item.fChance;
			if (!(num3 > 0f))
			{
				continue;
			}
			int num4 = Mathf.FloorToInt(num3);
			for (int i = 0; i < num4; i++)
			{
				if (objUs != null && item.strName == "[us]")
				{
					list.Add(objUs);
				}
				else
				{
					list.Add(DataHandler.GetCondOwner(item.strName, null, null, !bSuppress || !bSuppressOverride));
				}
			}
			break;
		}
	}
	num2 = 0;
	foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits)
	{
		num = 0f;
		string text2 = strRandID;
		if (text2 != null)
		{
			text2 += num2;
		}
		num2++;
		float fRand2 = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text2);
		foreach (LootUnit item2 in aOtherLootUnit)
		{
			float num5 = item2.GetAmount(num, fRand2);
			if (!item2.bPositive)
			{
				num5 = 0f - num5;
			}
			num += item2.fChance;
			if (!(num5 > 0f))
			{
				continue;
			}
			for (int j = 0; (float)j < num5; j++)
			{
				Loot loot = DataHandler.GetLoot(item2.strName);
				if (loot.strName != item2.strName)
				{
					Debug.Log(strName + " expected loot: " + item2.strName + " but got " + loot.strName);
				}
				list.AddRange(loot.GetCOLoot(objUs, bSuppress && bSuppressOverride, strRandID));
			}
			break;
		}
	}
	if (bNested)
	{
		for (int num6 = list.Count - 1; num6 > 0; num6--)
		{
			for (int num7 = num6 - 1; num7 >= 0; num7--)
			{
				list[num6] = list[num7].AddCO(list[num6], bEquip: true, bOverflow: true, bIgnoreLocks: true);
				if (list[num6] == null)
				{
					list.RemoveAt(num6);
					break;
				}
			}
		}
	}
	return list;
}
*/

/* Loot.GetLootNames
public List<string> GetLootNames(string strRandID = null, bool bOnlyCOs = false, string type = null)
{
	List<string> list = new List<string>();
	float num = 0f;
	int num2 = 0;
	foreach (List<LootUnit> aCOLootUnit in aCOLootUnits)
	{
		if (!string.IsNullOrEmpty(type) && strType != type)
		{
			break;
		}
		num = 0f;
		string text = strRandID;
		if (text != null)
		{
			text += num2;
		}
		num2++;
		float fRand = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text);
		foreach (LootUnit item in aCOLootUnit)
		{
			float num3 = item.GetAmount(num, fRand);
			num += item.fChance;
			string text2 = item.strName;
			if (num3 < 0f)
			{
				text2 = "-" + text2;
				num3 = 0f - num3;
			}
			if (num3 > 0f)
			{
				for (int i = 0; (float)i < num3; i++)
				{
					list.Add(text2);
				}
				break;
			}
		}
	}
	if (bOnlyCOs)
	{
		return list;
	}
	num2 = 0;
	foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits)
	{
		num = 0f;
		string text3 = strRandID;
		if (text3 != null)
		{
			text3 += num2;
		}
		num2++;
		float fRand2 = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text3);
		foreach (LootUnit item2 in aOtherLootUnit)
		{
			float num4 = item2.GetAmount(num, fRand2);
			num += item2.fChance;
			string text4 = item2.strName;
			if (num4 < 0f)
			{
				text4 = "-" + text4;
				num4 = 0f - num4;
			}
			if (!(num4 > 0f))
			{
				continue;
			}
			for (int j = 0; (float)j < num4; j++)
			{
				Loot loot = DataHandler.GetLoot(item2.strName);
				if (string.IsNullOrEmpty(type) || !(loot.strType != type))
				{
					list.AddRange(loot.GetLootNames(text3));
				}
			}
			break;
		}
	}
	return list;
}
*/

/* Loot.ApplyCondLoot
public void ApplyCondLoot(CondOwner coUs, float fCoeff, string strRandID = null, float fCondRuleTrack = 0f)
{
	if (coUs == null || strType != "condition")
	{
		return;
	}
	string[] array = aCOs;
	foreach (string strDef in array)
	{
		string text = coUs.ParseCondEquation(strDef, fCoeff, fCondRuleTrack);
	}
	int num = 0;
	foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits)
	{
		float num2 = 0f;
		string text2 = strRandID;
		if (text2 != null)
		{
			text2 += num;
		}
		num++;
		float fRand = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text2);
		foreach (LootUnit item in aOtherLootUnit)
		{
			float num3 = item.GetAmount(num2, fRand);
			if (!item.bPositive)
			{
				num3 = 0f - num3;
			}
			num2 += item.fChance;
			if (num3 > 0f)
			{
				for (int j = 0; (float)j < num3; j++)
				{
					Loot loot = DataHandler.GetLoot(item.strName);
					loot.ApplyCondLoot(coUs, fCoeff, null, fCondRuleTrack);
				}
				break;
			}
		}
	}
}
*/

/* Loot.GetCondLoot
public Dictionary<string, double> GetCondLoot(float fCoeff, Dictionary<string, double> dictOut, string strRandID = null)
{
	if (dictOut == null)
	{
		dictOut = new Dictionary<string, double>();
	}
	if (strType != "condition")
	{
		return dictOut;
	}
	string[] array = aCOs;
	foreach (string strDef in array)
	{
		KeyValuePair<string, Tuple<double, double>> keyValuePair = ParseCondEquation(strDef);
		double num = keyValuePair.Value.Item1;
		if (keyValuePair.Value.Item2 != num)
		{
			num = MathUtils.Rand(keyValuePair.Value.Item1, keyValuePair.Value.Item2, MathUtils.RandType.Flat);
		}
		if (keyValuePair.Key != string.Empty && num != 0.0)
		{
			if (!dictOut.ContainsKey(keyValuePair.Key))
			{
				dictOut[keyValuePair.Key] = num * (double)fCoeff;
			}
			else
			{
				dictOut[keyValuePair.Key] += num * (double)fCoeff;
			}
		}
	}
	int num2 = 0;
	foreach (List<LootUnit> aOtherLootUnit in aOtherLootUnits)
	{
		float num3 = 0f;
		string text = strRandID;
		if (text != null)
		{
			text += num2;
		}
		num2++;
		float fRand = MathUtils.Rand(0f, 1f, MathUtils.RandType.Flat, text);
		foreach (LootUnit item in aOtherLootUnit)
		{
			float num4 = item.GetAmount(num3, fRand);
			if (!item.bPositive)
			{
				num4 = 0f - num4;
			}
			num3 += item.fChance;
			if (num4 > 0f)
			{
				for (int j = 0; (float)j < num4; j++)
				{
					Loot loot = DataHandler.GetLoot(item.strName);
					loot.GetCondLoot(fCoeff, dictOut);
				}
				break;
			}
		}
	}
	return dictOut;
}
*/