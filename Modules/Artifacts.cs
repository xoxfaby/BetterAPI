using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class Artifacts
    {
        public static ArtifactDef Add(ArtifactDef artifactDef, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if(!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].artifactDefs.Add(artifactDef);
            return artifactDef;
        }
        public static ArtifactDef Add(ArtifactTemplate artifactTemplate, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ArtifactDef artifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            artifactDef.cachedName = artifactTemplate.internalName;
            artifactDef.pickupModelPrefab = artifactTemplate.prefab;
            artifactDef.unlockableDef = artifactTemplate.unlockableDef;
            artifactDef.smallIconDeselectedSprite = artifactTemplate.smallIconDeselectedSprite;
            artifactDef.smallIconSelectedSprite = artifactTemplate.smallIconSelectedSprite;
            artifactDef.nameToken = $"ARTIFACT_{artifactTemplate.internalName.ToUpper()}_NAME";
            artifactDef.descriptionToken = $"ARTIFACT_{artifactTemplate.internalName.ToUpper()}_DESC";

            Languages.AddTokenString(artifactDef.nameToken, artifactTemplate.name);
            Languages.AddTokenString(artifactDef.descriptionToken, artifactTemplate.descriptionText);

            return Add(artifactDef, contentPackIdentifier);
        }

        public struct ArtifactTemplate
        {
            public string internalName;
            public string name;
            public float cooldown;
            public bool isLunar;
            public bool isBoss;
            public bool canDrop;
            public BuffDef passiveBuffDef;
            public string pickupText;
            public string descriptionText;
            public string loreText;
            public GameObject prefab;
            public Sprite smallIconDeselectedSprite;
            public Sprite smallIconSelectedSprite;
            public UnlockableDef unlockableDef;
        }
    }
}
