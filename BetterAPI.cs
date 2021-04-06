using System;
using System.Collections.Generic;
using RoR2;
using BepInEx;
using UnityEngine;
using RoR2.ContentManagement;
using System.Collections;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", "1.2.0.1")]
    public class BetterAPI : BaseUnityPlugin
    {
        internal class BetterAPIContentPackProvider : IContentPackProvider
        {
            internal ContentPack contentPack = new ContentPack();
            public string identifier => "com.xoxfaby.BetterAPI";

            public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
            {
                this.contentPack.identifier = this.identifier;
                contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
                contentPack.itemDefs.Add(Items.itemDefs.ToArray());
                contentPack.networkedObjectPrefabs.Add(Prefabs.prefabs.ToArray());
                contentPack.bodyPrefabs.Add(Bodies.prefabs.ToArray());

                args.ReportProgress(1f);
                yield break;
            }

            public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
            {
                ContentPack.Copy(this.contentPack, args.output);
                args.ReportProgress(1f);
                yield break;
            }

            public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
            {
                args.ReportProgress(1f);
                yield break;
            }
        }

        public void Awake()
        {
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }
        public void Start()
        {
            Items.ApplyCustomItemDisplayRules();
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new BetterAPIContentPackProvider());
        }

    }
}
