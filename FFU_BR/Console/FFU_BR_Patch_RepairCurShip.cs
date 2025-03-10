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