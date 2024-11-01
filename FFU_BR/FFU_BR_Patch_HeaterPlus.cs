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

using MonoMod;
using System.Collections.Generic;

public partial class patch_Heater : Heater {
    [MonoModReplace] private void Heat(double fTimePassed) {
        bool isEnabled = false;
        CondOwner heatCO = null;
        if (coUs.HasCond("IsOverrideOn")) isEnabled = true;
        else if (coUs.HasCond("IsOverrideOff")) isEnabled = false;
        else if (strRemoteID == null || strRemoteID == coUs.strID) {
            isEnabled = coUs.HasCond(strSignalCond);
            heatCO = coUs;
        } else {
            CondOwner cOByID = coUs.ship.GetCOByID(strRemoteID);
            if (cOByID != null && cOByID.HasCond(strSignalCond)) {
                isEnabled = true;
                heatCO = cOByID;
            }
        }
        coUs.mapInfo.Remove("Status");
        if (isEnabled) {
            CondOwner addPointCO = null;
            CondOwner subPointCO = null;
            double dirHeatCool = 1.0;
            if (strAddPoint != "ignore") {
                List<CondOwner> addList = new List<CondOwner>();
                coUs.ship.GetCOsAtWorldCoords1(coUs.GetPos(strAddPoint), ct, false, false, addList);
                if (addList.Count != 0) addPointCO = addList[0];
            }
            if (strSubPoint != "ignore") {
                List<CondOwner> subList = new List<CondOwner>();
                coUs.ship.GetCOsAtWorldCoords1(coUs.GetPos(strSubPoint), ct, false, false, subList);
                if (subList.Count != 0) subPointCO = subList[0];
            }
            if (addPointCO == null) {
                addPointCO = subPointCO;
                dirHeatCool = 0.0 - dirHeatCool;
                subPointCO = null;
            }
            if (addPointCO == null) return;
            double cMolHeatCap = 20.7;
            double cEmissCoef = 0.9;
			double cStefBoltz = 5.67E-08;
			double cStdAtmoPr = 101.30000305175781;
            GasContainer gasContainer = addPointCO.GasContainer;
            double gasTemp = addPointCO.GetCondAmount("StatGasTemp");
            double emitTemp = coUs.GetCondAmount("StatEmittedTemp");
			if (emitTemp == 0) emitTemp = coUs.GetCondAmount("StatSolidTemp");
            double heatArea = coUs.GetCondAmount("StatHeatArea");
            double gasMoles = gasContainer.mapGasMols1["StatGasMolTotal"];
            if (gasMoles == 0.0) gasMoles = double.PositiveInfinity;
            double heatDiff = emitTemp * emitTemp * emitTemp * emitTemp - gasTemp * gasTemp * gasTemp * gasTemp;
            double radPower = cEmissCoef * cStefBoltz * heatArea * heatDiff;
            double heatVol = coUs.GetCondAmount("StatHeatVol");
            double targetVol = addPointCO.GetCondAmount("StatVolume");
            if (targetVol == 0.0) targetVol = double.PositiveInfinity;
            double volRatio = heatVol / targetVol;
            double tempChange = radPower / cMolHeatCap / gasMoles * volRatio * fTimePassed * dirHeatCool;
            double gasPressure = addPointCO.GetCondAmount("StatGasPressure");
            if (gasPressure < cStdAtmoPr) tempChange *= gasPressure / cStdAtmoPr;
            gasContainer.fDGasTemp += tempChange;
            if (subPointCO == null) {
                coUs.mapInfo["Status"] = "Heating";
                return;
            }
            dirHeatCool = 0.0 - dirHeatCool;
            gasContainer = subPointCO.GasContainer;
            gasTemp = subPointCO.GetCondAmount("StatGasTemp");
            gasMoles = gasContainer.mapGasMols1["StatGasMolTotal"];
            if (gasMoles == 0.0) gasMoles = double.PositiveInfinity;
            targetVol = subPointCO.GetCondAmount("StatVolume");
            if (targetVol == 0.0) targetVol = double.PositiveInfinity;
            volRatio = heatVol / targetVol;
            tempChange = radPower / cMolHeatCap / gasMoles * volRatio * fTimePassed * dirHeatCool;
            gasPressure = subPointCO.GetCondAmount("StatGasPressure");
            if (gasPressure < cStdAtmoPr) tempChange *= gasPressure / cStdAtmoPr;
            gasContainer.fDGasTemp += tempChange;
            coUs.mapInfo["Status"] = "Cooling";
        } else {
            coUs.mapInfo["Status"] = "Idle";
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4
/*
private void Heat(double fTimePassed)
{
	bool flag = false;
	CondOwner condOwner = null;
	if (coUs.HasCond("IsOverrideOn"))
	{
		flag = true;
	}
	else if (coUs.HasCond("IsOverrideOff"))
	{
		flag = false;
	}
	else if (strRemoteID == null || strRemoteID == coUs.strID)
	{
		flag = coUs.HasCond(strSignalCond);
		condOwner = coUs;
	}
	else
	{
		CondOwner cOByID = coUs.ship.GetCOByID(strRemoteID);
		if (cOByID != null && cOByID.HasCond(strSignalCond))
		{
			flag = true;
			condOwner = cOByID;
		}
	}
	coUs.mapInfo.Remove("Status");
	if (flag)
	{
		CondOwner condOwner2 = null;
		CondOwner condOwner3 = null;
		double num = 1.0;
		if (strAddPoint != "ignore")
		{
			List<CondOwner> list = new List<CondOwner>();
			coUs.ship.GetCOsAtWorldCoords1(coUs.GetPos(strAddPoint), ct, bAllowDocked: false, bAllowLocked: false, list);
			if (list.Count != 0)
			{
				condOwner2 = list[0];
			}
		}
		if (strSubPoint != "ignore")
		{
			List<CondOwner> list2 = new List<CondOwner>();
			coUs.ship.GetCOsAtWorldCoords1(coUs.GetPos(strSubPoint), ct, bAllowDocked: false, bAllowLocked: false, list2);
			if (list2.Count != 0)
			{
				condOwner3 = list2[0];
			}
		}
		if (condOwner2 == null)
		{
			condOwner2 = condOwner3;
			num = 0.0 - num;
			condOwner3 = null;
		}
		if (condOwner2 == null)
		{
			return;
		}
		double num2 = 20.7;
		double num3 = 0.9;
		GasContainer gasContainer = condOwner2.GasContainer;
		double condAmount = condOwner2.GetCondAmount("StatGasTemp");
		double condAmount2 = coUs.GetCondAmount("StatSolidTemp");
		double condAmount3 = coUs.GetCondAmount("StatHeatArea");
		double num4 = gasContainer.mapGasMols1["StatGasMolTotal"];
		if (num4 == 0.0)
		{
			num4 = double.PositiveInfinity;
		}
		double num5 = condAmount2 * condAmount2 * condAmount2 * condAmount2 - condAmount * condAmount * condAmount * condAmount;
		double num6 = num3 * 5.67E-08 * condAmount3 * num5;
		double condAmount4 = coUs.GetCondAmount("StatHeatVol");
		double num7 = condOwner2.GetCondAmount("StatVolume");
		if (num7 == 0.0)
		{
			num7 = double.PositiveInfinity;
		}
		double num8 = condAmount4 / num7;
		double num9 = num6 / num2 / num4 * num8 * fTimePassed * num;
		double condAmount5 = condOwner2.GetCondAmount("StatGasPressure");
		if (condAmount5 < 101.30000305175781)
		{
			num9 *= condAmount5 / 101.30000305175781;
		}
		gasContainer.fDGasTemp += num9;
		if (condOwner3 == null)
		{
			coUs.mapInfo["Status"] = "Heating";
			return;
		}
		num = 0.0 - num;
		gasContainer = condOwner3.GasContainer;
		condAmount = condOwner3.GetCondAmount("StatGasTemp");
		num4 = gasContainer.mapGasMols1["StatGasMolTotal"];
		if (num4 == 0.0)
		{
			num4 = double.PositiveInfinity;
		}
		num7 = condOwner3.GetCondAmount("StatVolume");
		if (num7 == 0.0)
		{
			num7 = double.PositiveInfinity;
		}
		num8 = condAmount4 / num7;
		num9 = num6 / num2 / num4 * num8 * fTimePassed * num;
		condAmount5 = condOwner3.GetCondAmount("StatGasPressure");
		if (condAmount5 < 101.30000305175781)
		{
			num9 *= condAmount5 / 101.30000305175781;
		}
		gasContainer.fDGasTemp += num9;
		coUs.mapInfo["Status"] = "Cooling";
	}
	else
	{
		coUs.mapInfo["Status"] = "Idle";
	}
}
*/