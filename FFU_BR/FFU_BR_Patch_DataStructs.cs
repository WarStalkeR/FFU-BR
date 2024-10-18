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

public class patch_JsonModInfo : JsonModInfo {
    public Dictionary<string, string[]> removeIds { get; set; }
}