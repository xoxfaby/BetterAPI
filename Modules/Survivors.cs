using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class Survivors
    {

        public static SurvivorDef Add(SurvivorDef survivorDef, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].survivorDefs.Add(survivorDef);
            BodyPrefabs.Add(survivorDef.bodyPrefab);
            return survivorDef;
        }
        public static SurvivorDef Add(SurvivorTemplate survivorTemplate, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.cachedName = survivorTemplate.internalName;
            survivorDef.hidden = survivorTemplate.hidden;
            survivorDef.desiredSortPosition = survivorTemplate.desiredSortPosition;
            survivorDef.unlockableDef = survivorTemplate.unlockableDef;
            survivorDef.bodyPrefab = survivorTemplate.bodyPrefab;
            survivorDef.displayPrefab = survivorTemplate.displayPrefab;
            survivorDef.primaryColor = survivorTemplate.primaryColor;


            survivorDef.displayNameToken = $"CHARACTER_{survivorTemplate.internalName.ToUpper()}_NAME";
            survivorDef.descriptionToken = $"CHARACTER_{survivorTemplate.internalName.ToUpper()}_DESC";
            survivorDef.mainEndingEscapeFailureFlavorToken = $"CHARACTER_{survivorTemplate.internalName.ToUpper()}_FAILUREFLAVOR";
            survivorDef.outroFlavorToken = $"CHARACTER_{survivorTemplate.internalName.ToUpper()}_OUTROFLAVOR";

            Languages.AddTokenString(survivorDef.displayNameToken, survivorTemplate.name);
            Languages.AddTokenString(survivorDef.descriptionToken, survivorTemplate.descriptionText);
            Languages.AddTokenString(survivorDef.mainEndingEscapeFailureFlavorToken, survivorTemplate.mainEndingEscapeFailureFlavor);
            Languages.AddTokenString(survivorDef.outroFlavorToken, survivorTemplate.outroFlavor);

            return Add(survivorDef, contentPackIdentifier);
        }

        public struct SurvivorTemplate
        {
            public string internalName;
            public string name;
            public string descriptionText;
            public string mainEndingEscapeFailureFlavor;
            public string outroFlavor;
            public Color primaryColor;
            public bool hidden;
            public float desiredSortPosition;
            public UnlockableDef unlockableDef;
            public GameObject bodyPrefab;
            public GameObject displayPrefab;
        }
    }
}
