using System;
using System.Collections.Generic;
using RoR2;
using BepInEx;
using UnityEngine;
using RoR2.ContentManagement;
using System.Collections;

namespace BetterAPI
{
    [BepInPlugin("com.xoxfaby.BetterAPI", "BetterAPI", "1.3.0.1")]
    public class BetterAPI : BaseUnityPlugin, IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => "com.xoxfaby.BetterAPI";

        public void Awake()
        {
            RoR2.RoR2Application.isModded = true;
            RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            BodyCatalog.availability.CallWhenAvailable(Items.ApplyCustomItemDisplayRules);
        }

        public void Start()
        {
        }

        private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            this.contentPack.identifier = this.identifier;
            contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
            contentPack.itemDefs.Add(Items.itemDefs.ToArray());
            contentPack.bodyPrefabs.Add(BodyPrefabs.prefabs.ToArray());
            contentPack.masterPrefabs.Add(MasterPrefabs.prefabs.ToArray());
            contentPack.networkedObjectPrefabs.Add(NetworkedPrefabs.prefabs.ToArray());
            contentPack.projectilePrefabs.Add(ProjectilePrefabs.prefabs.ToArray());

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
}
