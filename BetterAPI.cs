using System;
using BepInEx;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", BetterAPIPlugin.Version + "+" + ReleaseType + "-" + BuildNumber)]
    public class BetterAPIPlugin : BetterUnityPlugin.BetterUnityPlugin<BetterAPIPlugin>
    {
        public const string Version = "2.0.0";
        public const string ReleaseType = "release";
        public const string BuildNumber = "1";
        public override BaseUnityPlugin typeReference => throw new NotImplementedException();
    }
}
