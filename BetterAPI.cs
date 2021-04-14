using System;
using System.Collections.Generic;
using RoR2;
using BepInEx;
using UnityEngine;
using System.Collections;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", "1.3.0.1")]
    public class BetterAPI : BaseUnityPlugin
    {
        public void Awake()
        {
            RoR2.RoR2Application.isModded = true;
            BodyCatalog.availability.CallWhenAvailable(Items.ApplyCustomItemDisplayRules);
        }

    }
}
