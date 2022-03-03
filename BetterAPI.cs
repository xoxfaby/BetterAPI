using System;
using BepInEx;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", BetterAPIPlugin.Version)]
    public class BetterAPIPlugin : BetterUnityPlugin.BetterUnityPlugin<BetterAPIPlugin>
    {
        public const string Version = "4.0.1";
        public override BaseUnityPlugin typeReference => throw new NotImplementedException();

        protected override void Awake()
        {
            base.Awake();

            this.gameObject.hideFlags |= UnityEngine.HideFlags.HideAndDontSave;
        }
    }
}
