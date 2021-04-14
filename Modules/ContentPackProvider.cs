using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;

namespace BetterAPI
{
    internal static class ContentPacks
    {
        internal static ContentPackCollection Packs = new ContentPackCollection();
        internal class ContentPackCollection
        {
            private Dictionary<String, ContentPackProvider> contentPackProviders = new Dictionary<String, ContentPackProvider>();
            internal ContentPackProvider this[String identifier]
            {
                get {
                    if (!contentPackProviders.ContainsKey(identifier)) contentPackProviders.Add(identifier, new ContentPackProvider(identifier));
                    return contentPackProviders[identifier];
                }
            }
        }
        internal class ContentPackProvider : IContentPackProvider
        {
            internal readonly List<BuffDef> buffDefs = new List<BuffDef>();
            internal readonly List<ItemDef> itemDefs = new List<ItemDef>();
            internal readonly List<GameObject> bodyPrefabs = new List<GameObject>();
            internal readonly List<GameObject> masterPrefabs = new List<GameObject>();
            internal readonly List<GameObject> networkedPrefabs = new List<GameObject>();
            internal readonly List<GameObject> projectilePrefabs = new List<GameObject>();

            public string identifier { get; private set; }
            private readonly ContentPack contentPack = new ContentPack();
            internal ContentPackProvider(String identifier)
            {
                this.identifier = identifier;
                RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders; 
            }
            private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
            {
                addContentPackProvider(this);
            }
            public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
            {
                this.contentPack.identifier = this.identifier;
                contentPack.buffDefs.Add(buffDefs.ToArray());
                contentPack.itemDefs.Add(itemDefs.ToArray());
                contentPack.bodyPrefabs.Add(bodyPrefabs.ToArray());
                contentPack.masterPrefabs.Add(masterPrefabs.ToArray());
                contentPack.networkedObjectPrefabs.Add(networkedPrefabs.ToArray());
                contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());

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
}
