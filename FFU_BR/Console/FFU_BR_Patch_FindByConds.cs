using System.Linq;

public partial class patch_ConsoleResolver : ConsoleResolver {
    private static bool KeywordFindCondCOs(ref string strInput) {
        string[] aCondsToFind = strInput.Split(' ').Skip(1).Where(x => !x.StartsWith("!")).ToArray();
        string[] aCondsToAvoid = strInput.Split(' ').Skip(1).Where(x => x.StartsWith("!")).Select(x => x.Substring(1)).ToArray();
        if (aCondsToFind.Length > 0 || aCondsToAvoid.Length > 0) {
            int objCount = 0;
            strInput += $"\nCOs with corresponding conditions:";
            if (aCondsToAvoid.Length == 0) {
                foreach (JsonCondOwner aCO in DataHandler.dictCOs.Values) {
                    string[] aCondsList = aCO.aStartingConds.Select(x => x.Split('=')[0]).ToArray();
                    if (aCondsToFind.All(x => aCondsList.Contains(x))) {
                        strInput += $"\n> {aCO.strNameFriendly} ({aCO.strName})";
                        objCount++;
                    }
                }
            } else if (aCondsToFind.Length == 0) {
                foreach (JsonCondOwner aCO in DataHandler.dictCOs.Values) {
                    string[] aCondsList = aCO.aStartingConds.Select(x => x.Split('=')[0]).ToArray();
                    if (!aCondsToAvoid.Any(x => aCondsList.Contains(x))) {
                        strInput += $"\n> {aCO.strNameFriendly} ({aCO.strName})";
                        objCount++;
                    }
                }
            } else {
                foreach (JsonCondOwner aCO in DataHandler.dictCOs.Values) {
                    string[] aCondsList = aCO.aStartingConds.Select(x => x.Split('=')[0]).ToArray();
                    if (aCondsToFind.All(x => aCondsList.Contains(x)) && 
                        !aCondsToAvoid.Any(x => aCondsList.Contains(x))) {
                        strInput += $"\n> {aCO.strNameFriendly} ({aCO.strName})";
                        objCount++;
                    }
                }
            }
            strInput += $"\nFound {objCount} COs in total.";
        }
        return true;
    }
}