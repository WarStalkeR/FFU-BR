public partial class patch_ConsoleResolver : ConsoleResolver {
    public static extern bool orig_ResolveString(ref string strInput);
    public static bool ResolveString(ref string strInput) {
        strInput = strInput.Trim();
        string[] array = strInput.Split(' ');
        array[0] = array[0].ToLower();
        switch (array[0]) {
            case "findcondcos": return KeywordFindCondCOs(ref strInput);
            case "repairship": return KeywordRepairShip(ref strInput);
            default: return orig_ResolveString(ref strInput);
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* ConsoleResolver.ResolveString
public static bool ResolveString(ref string strInput)
{
	strInput.Trim();
	string[] array = strInput.Split(' ');
	array[0] = array[0].ToLower();
	switch (array[0])
	{
	case "help":
		if (KeywordHelp(ref strInput, array))
		{
			return true;
		}
		return false;
	case "echo":
		strInput = strInput + "\n" + strInput.Remove(0, 5);
		return true;
	case "unlockdebug":
		return KeywordUnlockDebug(ref strInput, array);
	case "crewsim":
		return KeywordCrewSim(ref strInput, array);
	case "addcond":
		return KeywordAddCond(ref strInput, array);
	case "getcond":
		return KeywordGetCond(ref strInput, array);
	case "bugform":
		return KeywordBugForm(ref strInput);
	case "spawn":
		return KeywordSpawn(ref strInput, array);
	case "verify":
		return KeywordVerify(ref strInput);
	case "kill":
		return KeywordKill(ref strInput, array);
	case "addcrew":
		return KeywordAddCrew(ref strInput, array, makeCrew: true);
	case "addnpc":
		return KeywordAddCrew(ref strInput, array);
	case "damageship":
		return KeywordDamageShip(ref strInput, array);
	case "breakinship":
		return KeywordBreakInShip(ref strInput, array);
	case "meteor":
		return KeywordMeteor(ref strInput, array);
	case "oxygen":
		return KeywordOxygen(ref strInput, array);
	case "toggle":
		return KeywordToggle(ref strInput, array);
	case "ship":
		return KeywordShip(ref strInput, array);
	case "shipvis":
		return KeywordShipVis(ref strInput, array);
	case "lookup":
		return KeywordLookup(ref strInput, array);
	case "plot":
		return KeywordPlot(ref strInput, array);
	case "summon":
		return KeywordSummon(ref strInput, array);
	case "rel":
		return KeywordRelationship(ref strInput, array);
	case "skywalk":
		return KeywordSkywalk(ref strInput, array);
	case "detach":
		return KeywordDetach(ref strInput, array);
	case "attach":
		return KeywordAttach(ref strInput, array);
	case "meatstate":
		return KeywordMeatState(ref strInput, array);
	case "priceflips":
		return KeywordPriceFlips(ref strInput, array);
	case "clear":
	case "clr":
		return KeywordClear(ref strInput, array);
	default:
		strInput += "\nFailed to recognise command.";
		return false;
	}
}
*/