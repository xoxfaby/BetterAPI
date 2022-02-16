using System;
using BepInEx;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", BetterAPIPlugin.Version + "." + BuildNumber)]
    public class BetterAPIPlugin : BetterUnityPlugin<BetterAPIPlugin>
    {
        public const string Version = "2.0.0";
        public const string BuildNumber = "1";
        public override BaseUnityPlugin typeReference => throw new NotImplementedException();
    }
}
