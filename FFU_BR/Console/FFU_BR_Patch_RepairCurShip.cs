#pragma warning disable CS0108
#pragma warning disable CS0162
#pragma warning disable CS0414
#pragma warning disable CS0618
#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable IDE1006
#pragma warning disable IDE0019
#pragma warning disable IDE0002

using System.Collections.Generic;

public partial class patch_ConsoleResolver : ConsoleResolver {
    private static bool KeywordRepairShip(ref string strInput) {
        if (CrewSim.objInstance == null) {
            strInput += "\nCrewSim instance not found.";
            return false;
        }
        if (CrewSim.shipCurrentLoaded == null) {
            strInput += "\nNo ship currently loaded.";
            return false;
        }
        CondTrigger isInstalled = new CondTrigger("TIsInstalledObject", 
            new string[] {"IsInstalled"}, null, null, null);
        List<CondOwner> shipCOs = CrewSim.shipCurrentLoaded
            .GetCOs(isInstalled, true, false, true);
        foreach (CondOwner shipCO in shipCOs) {
            shipCO.SetUpBehaviours();
            shipCO.ZeroCondAmount("StatDamage");
            if (shipCO.Item != null)
                shipCO.Item.VisualizeDamage();
            shipCO.UpdateStats();
        }
        strInput += $"\nRepaired {shipCOs.Count} installed COs on the ship.";
        return true;
    }
}