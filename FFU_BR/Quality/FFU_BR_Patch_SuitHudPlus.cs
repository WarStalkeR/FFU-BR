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

public partial class patch_GUIHelmet : GUIHelmet {
    private double fPwrLast;
    private extern void orig_Init();
    private void Init() {
        orig_Init();
        ghTempInt.DangerLow = 289.15;
        ghTempInt.DangerHigh = 315.15;
    }
    [MonoModReplace] public void UpdateUI(CondOwner coRoomIn, CondOwner coRoomOut) {
        if (coRoomIn == null || !coRoomIn.HasCond("IsHuman")) {
            Style = HelmetStyle.None;
            Visible = false;
        } else {
            if (coRoomOut == null) return;
            if (!bInit) Init();
            Visible = true;
            List<CondOwner> suitSlots = new List<CondOwner>();
            bool noPower = true;
            bool noOxygen = true;
            bool isSuitHud = false;
            bool isBasicHud = false;
            if (coRoomIn.HasCond("IsEVAHUD")) {
                suitSlots = coRoomIn.compSlots.GetCOs("shirt_out", false, ctEVA);
                if (suitSlots.Count > 0) {
                    CondOwner firstSlot = suitSlots[0];
                    isSuitHud = true;
					if (isSuitHud) {
						suitSlots = firstSlot.GetCOs(false);
						bool foundO2 = false;
						bool foundPwr = false;
						if (suitSlots != null) {
							double currO2Total = 0;
							double currPwrTotal = 0;
							double maxO2Total = 0;
							double maxPwrTotal = 0;
							double percO2 = 0;
							double percPwr = 0;
							foreach (CondOwner slotItem in suitSlots) {
								if (ctEVABottle.Triggered(slotItem)) {
									if (!foundO2) foundO2 = true;
									double currO2 = slotItem.GetCondAmount("StatGasMolO2");
									double maxO2 = slotItem.GetCondAmount("StatRef");
									if (FFU_BR_Defs.ShowEachO2Battery) {
										percO2 += currO2 / maxO2 * 100.0;
									} else {
										currO2Total += currO2;
										maxO2Total += maxO2;
									}
								} else if (ctEVABatt.Triggered(slotItem)) {
									if (!foundPwr) foundPwr = true;
									Powered refPower = slotItem.GetComponent<Powered>();
									double currPwr = slotItem.GetCondAmount("StatPower");
									double maxPwr = slotItem.GetCondAmount("StatPowerMax") 
                                        * slotItem.GetDamageState();
									if (refPower != null) maxPwr = refPower.PowerStoredMax;
									if (maxPwr == 0.0) maxPwr = 1.0;
									if (FFU_BR_Defs.ShowEachO2Battery) {
										percPwr += currPwr / maxPwr * 100.0;
									} else {
										currPwrTotal += currPwr;
										maxPwrTotal += maxPwr;
									}
								}
							}
							if (foundO2) {
								noOxygen = false;
								if (!FFU_BR_Defs.ShowEachO2Battery)
									percO2 = currO2Total / maxO2Total * 100.0;
								txtO2.text = percO2.ToString("n2") + "%";
								if (percO2 != fO2Last) {
									if (percO2 < FFU_BR_Defs.SuitOxygenNotify) asO2Beep.Play();
									fO2Last = percO2;
								}
							}
							if (foundPwr) {
                                noPower = false;
                                if (!FFU_BR_Defs.ShowEachO2Battery)
									percPwr = currPwrTotal / maxPwrTotal * 100.0;
								txtBatt.text = percPwr.ToString("n2") + "%";
								if (percPwr != fPwrLast) {
									if (percPwr < FFU_BR_Defs.SuitPowerNotify) asO2Beep.Play();
									fPwrLast = percPwr;
								}
							}
						}
					}
                }
				if (noPower) Style = HelmetStyle.Unpowered;
				else Style = HelmetStyle.Powered;
            } else if (coRoomIn.HasCond("IsPSHUD")) {
                isBasicHud = true;
                Style = HelmetStyle.Powered;
            }
            HUDOn = isSuitHud;
            GaugeOn = isBasicHud;
            bool isEvenTime = (int)Time.realtimeSinceStartup % 2 == 0;
            double amountO2 = coRoomIn.GetCondAmount("StatGasPpO2");
            double amountCO2 = coRoomIn.GetCondAmount("StatGasPpCO2");
            if (amountO2 <= fO2PPMin || amountCO2 >= fCO2Max) TriggerTutorial();
            if (isBasicHud) UpdatePSGauge(amountO2, amountCO2);
            else if (isSuitHud) {
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
GUIHelmet.UpdateUI
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
					list = condOwner.GetCOs(bAllowLocked: false);
					bool flag4 = false;
					if (list != null)
					{
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
		double condAmount = coRoomIn.GetCondAmount("StatGasPpO2");
		double condAmount2 = coRoomIn.GetCondAmount("StatGasPpCO2");
		if (condAmount <= fO2PPMin || condAmount2 >= fCO2Max)
		{
			TriggerTutorial();
		}
		if (flag3)
		{
			UpdatePSGauge(condAmount, condAmount2);
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