using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class Unlockables
    {

        public static UnlockableDef Add(UnlockableDef unlockableDef, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].unlockableDefs.Add(unlockableDef);
            return unlockableDef;
        }
        public static UnlockableDef Add(UnlockableTemplate unlockableTemplate, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            UnlockableDef unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = unlockableTemplate.internalName;
            unlockableDef.hidden = unlockableTemplate.hidden;
            unlockableDef.displayModelPrefab = unlockableTemplate.displayModelPrefab;
            unlockableDef.nameToken = $"ITEM_{unlockableTemplate.internalName.ToUpper()}_NAME";

            Languages.AddTokenString(unlockableDef.nameToken, unlockableTemplate.name);

            return Add(unlockableDef, contentPackIdentifier);
        }

        public struct UnlockableTemplate
        {
            public string internalName;
            public string name;
            public bool hidden;
            public GameObject displayModelPrefab;
        }
    }
}
