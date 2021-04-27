using System;
using BepInEx;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", "1.3.3.1")]
    public class BetterAPIPlugin : BetterUnityPlugin<BetterAPIPlugin>
    {
        public override BaseUnityPlugin typeReference => throw new NotImplementedException();
    }
}
