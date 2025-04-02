using MonoMod;
using System.Collections.Generic;

public partial class patch_Sensor : Sensor {
    [MonoModReplace] public void Run() {
        if (coUs == null || coUs.ship == null) return;
        bool targetSelf = string.IsNullOrEmpty(strPoint);
        List<CondOwner> listCOsAtPos = new List<CondOwner>();
        if (!targetSelf)
            coUs.ship.GetCOsAtWorldCoords1(coUs.GetPos(strPoint),
                null, false, false, listCOsAtPos);
        foreach (KeyValuePair<string, string> mapTest in mapTests) {
            CondTrigger condTrigger = DataHandler.GetCondTrigger(mapTest.Key);
            if (!targetSelf) {
                foreach (CondOwner targetCO in listCOsAtPos) {
                    if (condTrigger.Triggered(targetCO)) {
                        Interaction interaction = DataHandler.GetInteraction(mapTest.Value);
                        if (interaction != null && interaction.CTTestUs.Triggered(coUs)) {
                            coUs.QueueInteraction(targetCO, interaction);
                        }
                    }
                }
            } else {
                if (condTrigger.Triggered(coUs)) {
                    Interaction interaction = DataHandler.GetInteraction(mapTest.Value);
                    if (interaction != null && interaction.CTTestUs.Triggered(coUs)) {
                        coUs.QueueInteraction(coUs, interaction);
                    }
                }
            }
        }
        dfEpochLast = StarSystem.fEpoch;
    }
    [MonoModReplace] public void SetData(Dictionary<string, string> gpm) {
        if (gpm != null) {
            strPoint = gpm["strPoint"];
            string[] aStrings = gpm["mapTests"].Split(',');
            if (gpm.TryGetValue("dfUpdateInterval", out string strUpdIntVal)) {
                double.TryParse(strUpdIntVal, out double newUpdateInterval);
                if (newUpdateInterval > 0f) dfUpdateInterval = newUpdateInterval;
            }
            mapTests = DataHandler.ConvertStringArrayToDict(aStrings);
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* Sensor.Run
public void Run()
{
	if (coUs == null || coUs.ship == null)
	{
		return;
	}
	List<CondOwner> list = new List<CondOwner>();
	coUs.ship.GetCOsAtWorldCoords1(coUs.GetPos(strPoint), null, bAllowDocked: false, bAllowLocked: false, list);
	foreach (KeyValuePair<string, string> mapTest in mapTests)
	{
		CondTrigger condTrigger = DataHandler.GetCondTrigger(mapTest.Key);
		foreach (CondOwner item in list)
		{
			if (condTrigger.Triggered(item))
			{
				Interaction interaction = DataHandler.GetInteraction(mapTest.Value);
				if (interaction != null && interaction.CTTestUs.Triggered(coUs))
				{
					coUs.QueueInteraction(item, interaction);
				}
			}
		}
	}
	dfEpochLast = StarSystem.fEpoch;
}
*/

/* Sensor.SetData
public void SetData(Dictionary<string, string> gpm)
{
	if (gpm != null)
	{
		strPoint = gpm["strPoint"];
		string[] aStrings = gpm["mapTests"].Split(',');
		mapTests = DataHandler.ConvertStringArrayToDict(aStrings);
	}
}
*/