using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;

namespace BetterAPI
{
    public static class ContentPacks
    {
        internal static ContentPackCollection Packs = new ContentPackCollection();
        internal static Dictionary<string, Assembly> assemblyDict = new Dictionary<string, Assembly>();

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
            internal bool TryGetValue(string key, out ContentPackProvider outPut)
            {
                return contentPackProviders.TryGetValue(key, out outPut);
            }
        }
        public static Assembly FindAssembly(String contentPackIdentifier)
        {
            if (assemblyDict.TryGetValue(contentPackIdentifier, out var assembly))
            {
                return assembly;
            }
            return null;
        }

        public static ContentPackProvider GetContentPackProvider(String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            return Packs[contentPackIdentifier];
        }

        public class ContentPackProvider : IContentPackProvider
        {
            public readonly List<ArtifactDef> artifactDefs = new List<ArtifactDef>();
            public readonly List<GameObject> bodyPrefabs = new List<GameObject>();
            public readonly List<BuffDef> buffDefs = new List<BuffDef>();
            public readonly List<EquipmentDef> equipmentDefs = new List<EquipmentDef>();
            public readonly List<ItemDef> itemDefs = new List<ItemDef>();
            public readonly List<GameObject> masterPrefabs = new List<GameObject>();
            public readonly List<GameObject> networkedPrefabs = new List<GameObject>();
            public readonly List<GameObject> projectilePrefabs = new List<GameObject>();
            public readonly List<RoR2.Skills.SkillDef> skillDefs = new List<RoR2.Skills.SkillDef>();
            public readonly List<RoR2.Skills.SkillFamily> skillFamilies = new List<RoR2.Skills.SkillFamily>();
            public readonly List<SurvivorDef> survivorDefs = new List<SurvivorDef>();
            public readonly List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();

            public string identifier { get; private set; }
            public readonly ContentPack contentPack = new ContentPack();
            internal ContentPackProvider(String identifier)
            {
                this.identifier = identifier;
                RoR2.RoR2Application.isModded = true;
                RoR2.NetworkModCompatibilityHelper.networkModList = RoR2.NetworkModCompatibilityHelper.networkModList.Append(identifier);
                RoR2.ContentManagement.ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            }
            private void ContentManager_collectContentPackProviders(RoR2.ContentManagement.ContentManager.AddContentPackProviderDelegate addContentPackProvider)
            {
                addContentPackProvider(this);
            }
            public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
            {
                this.contentPack.identifier = this.identifier;
                contentPack.artifactDefs.Add(artifactDefs.ToArray());
                contentPack.bodyPrefabs.Add(bodyPrefabs.ToArray());
                contentPack.buffDefs.Add(buffDefs.ToArray());
                contentPack.equipmentDefs.Add(equipmentDefs.ToArray());
                contentPack.itemDefs.Add(itemDefs.ToArray());
                contentPack.masterPrefabs.Add(masterPrefabs.ToArray());
                contentPack.networkedObjectPrefabs.Add(networkedPrefabs.ToArray());
                contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());
                contentPack.skillDefs.Add(skillDefs.ToArray());
                contentPack.skillFamilies.Add(skillFamilies.ToArray());
                contentPack.survivorDefs.Add(survivorDefs.ToArray());
                contentPack.unlockableDefs.Add(unlockableDefs.ToArray());
                


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
