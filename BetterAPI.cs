using System;
using System.Collections.Generic;
using RoR2;
using BepInEx;
using UnityEngine;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", "1.2.0.1")]
    public class BetterAPI : BaseUnityPlugin
    {
            
        public void Awake()
        {
            On.RoR2.ContentManager.SetContentPacks += ContentManager_SetContentPacks;
        }


        public void Start()
        {
            Items.ApplyCustomItemDisplayRules();
        }

        private static void ContentManager_SetContentPacks(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            ContentPack contentPack = new ContentPack();

            contentPack.buffDefs = Buffs.buffDefs.ToArray();
            contentPack.itemDefs = Items.itemDefs.ToArray();
            contentPack.networkedObjectPrefabs = NetworkedPrefabs.prefabs.ToArray();
            contentPack.bodyPrefabs = BodyPrefabs.prefabs.ToArray();
            contentPack.masterPrefabs = MasterPrefabs.prefabs.ToArray();
            contentPack.projectilePrefabs = ProjectilePrefabs.prefabs.ToArray();

            newContentPacks.Add(contentPack);
            orig(newContentPacks);  
        }
    }
}
