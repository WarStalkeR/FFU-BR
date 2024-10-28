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
using MonoMod;
using System.Collections.Generic;
using UnityEngine;

public class patch_GUIHelmet : GUIHelmet {
    private double fPwrLast;
    [MonoModReplace] public void UpdateUI(CondOwner coRoomIn, CondOwner coRoomOut) {
        if (coRoomIn == null || !coRoomIn.HasCond("IsHuman")) {
            Style = HelmetStyle.None;
            Visible = false;
        } else {
            if (coRoomOut == null) return;
            if (!bInit) Init();
            Visible = true;
            List<CondOwner> suitSlots = new List<CondOwner>();
            bool noOxygen = true;
            bool isSuitHud = false;
            bool isBasicHud = false;
            if (coRoomIn.HasCond("IsEVAHUD")) {
                suitSlots = coRoomIn.compSlots.GetCOs("shirt_out", false, ctEVA);
                if (suitSlots.Count > 0) {
                    CondOwner firstSlot = suitSlots[0];
                    isSuitHud = true;
                    Style = HelmetStyle.Powered;
                    if (isSuitHud) {
                        suitSlots = firstSlot.GetICOs(false);
                        bool foundO2 = false;
						bool foundPwr = false;
						double maxO2Total = 0;
						double currO2Total = 0;
                        double maxPwrTotal = 0;
                        double currPwrTotal = 0;
                        foreach (CondOwner slotItem in suitSlots) {
							if (ctEVABottle.Triggered(slotItem)) {
								if (!foundO2) foundO2 = true;
								currO2Total += slotItem.GetCondAmount("StatGasMolO2");
								maxO2Total += slotItem.GetCondAmount("StatRef");
							} else if (ctEVABatt.Triggered(slotItem)) {
                                if (!foundPwr) foundPwr = true;
                                Powered refPower = slotItem.GetComponent<Powered>();
								double maxPower = slotItem.GetCondAmount("StatPowerMax");
								if (refPower != null) maxPower = refPower.PowerStoredMax;
                                if (maxPower == 0.0) maxPower = 1.0;
                                currPwrTotal += slotItem.GetCondAmount("StatPower");
								maxPwrTotal += maxPower;
                            }
                        }
						if (foundO2) {
                            noOxygen = false;
							double percOxygen = currO2Total / maxO2Total * 100.0;
                            txtO2.text = percOxygen.ToString("n2") + "%";
                            if (percOxygen != fO2Last) {
                                if (percOxygen < FFU_BR_Defs.SuitOxygenNotify) asO2Beep.Play();
                                fO2Last = percOxygen;
                            }
                        }
						if (foundPwr) {
							double percPower = currPwrTotal / maxPwrTotal * 100.0;
                            txtBatt.text = percPower.ToString("n2") + "%";
                            if (percPower != fPwrLast) {
                                if (percPower < FFU_BR_Defs.SuitPowerNotify) asO2Beep.Play();
                                fPwrLast = percPower;
                            }
                        }
                    }
                } else Style = HelmetStyle.Unpowered;
            } else if (coRoomIn.HasCond("IsPSHUD")) {
                isBasicHud = true;
                Style = HelmetStyle.Powered;
            }
            HUDOn = isSuitHud;
            GaugeOn = isBasicHud;
            bool flag5 = (int)Time.realtimeSinceStartup % 2 == 0;
            if (isBasicHud) {
                UpdatePSGauge(coRoomIn);
            } else if (isSuitHud) {
                if (noOxygen) txtO2.text = "ERROR";
                ghO2Int.Value = coRoomIn.GetCondAmount("StatGasPpO2");
                ghO2Ext.Value = coRoomOut.GetCondAmount("StatGasPpO2");
                ghPressInt.Value = coRoomIn.GetCondAmount("StatGasPressure");
                ghPressExt.Value = coRoomOut.GetCondAmount("StatGasPressure");
                ghTempInt.Value = coRoomIn.GetCondAmount("StatGasTemp");
                ghTempExt.Value = coRoomOut.GetCondAmount("StatGasTemp");
                ghPressExt.DangerLow = ghPressInt.Value - fPressureDiffMax;
                ghPressExt.DangerHigh = ghPressInt.Value + fPressureDiffMax;
            }
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
public void UpdateUI(CondOwner coRoomIn, CondOwner coRoomOut)
{
	if (coRoomIn == null || !coRoomIn.HasCond("IsHuman"))
	{
		Style = HelmetStyle.None;
		Visible = false;
	}
	else
	{
		if (coRoomOut == null)
		{
			return;
		}
		if (!bInit)
		{
			Init();
		}
		Visible = true;
		List<CondOwner> list = new List<CondOwner>();
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		if (coRoomIn.HasCond("IsEVAHUD"))
		{
			list = coRoomIn.compSlots.GetCOs("shirt_out", bAllowLocked: false, ctEVA);
			if (list.Count > 0)
			{
				CondOwner condOwner = list[0];
				flag2 = true;
				Style = HelmetStyle.Powered;
				if (flag2)
				{
					list = condOwner.GetICOs(bAllowLocked: false);
					bool flag4 = false;
					foreach (CondOwner item in list)
					{
						if (!flag4 && ctEVABottle.Triggered(item))
						{
							double num = item.GetCondAmount("StatGasMolO2") / item.GetCondAmount("StatRef") * 100.0;
							txtO2.text = num.ToString("n2") + "%";
							flag = false;
							if (num != fO2Last)
							{
								asO2Beep.Play();
								fO2Last = num;
							}
							flag4 = true;
						}
						else if (ctEVABatt.Triggered(item))
						{
							Powered component = item.GetComponent<Powered>();
							double num2 = item.GetCondAmount("StatPowerMax");
							if (component != null)
							{
								num2 = component.PowerStoredMax;
							}
							if (num2 == 0.0)
							{
								num2 = 1.0;
							}
							double num3 = item.GetCondAmount("StatPower") / num2 * 100.0;
							txtBatt.text = num3.ToString("n2") + "%";
						}
					}
				}
			}
			else
			{
				Style = HelmetStyle.Unpowered;
			}
		}
		else if (coRoomIn.HasCond("IsPSHUD"))
		{
			flag3 = true;
			Style = HelmetStyle.Powered;
		}
		HUDOn = flag2;
		GaugeOn = flag3;
		bool flag5 = (int)Time.realtimeSinceStartup % 2 == 0;
		if (flag3)
		{
			UpdatePSGauge(coRoomIn);
		}
		else if (flag2)
		{
			if (flag)
			{
				txtO2.text = "ERROR";
			}
			ghO2Int.Value = coRoomIn.GetCondAmount("StatGasPpO2");
			ghO2Ext.Value = coRoomOut.GetCondAmount("StatGasPpO2");
			ghPressInt.Value = coRoomIn.GetCondAmount("StatGasPressure");
			ghPressExt.Value = coRoomOut.GetCondAmount("StatGasPressure");
			ghTempInt.Value = coRoomIn.GetCondAmount("StatGasTemp");
			ghTempExt.Value = coRoomOut.GetCondAmount("StatGasTemp");
			ghPressExt.DangerLow = ghPressInt.Value - fPressureDiffMax;
			ghPressExt.DangerHigh = ghPressInt.Value + fPressureDiffMax;
		}
	}
}
*/