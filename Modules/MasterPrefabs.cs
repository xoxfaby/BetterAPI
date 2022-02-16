using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class MasterPrefabs
    {
        public static void Add(GameObject prefab, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            if (!ContentPacks.Packs[contentPackIdentifier].masterPrefabs.Contains(prefab))
            {
                ContentPacks.Packs[contentPackIdentifier].masterPrefabs.Add(prefab);
            }
        }
    }
}