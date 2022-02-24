using System;
using BepInEx;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", BetterAPIPlugin.Version)]
    public class BetterAPIPlugin : BetterUnityPlugin.BetterUnityPlugin<BetterAPIPlugin>
    {
        public const string Version = "2.0.0";
        public override BaseUnityPlugin typeReference => throw new NotImplementedException();
    }
}
