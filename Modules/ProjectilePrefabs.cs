using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class ProjectilePrefabs
    {
        static ProjectilePrefabs()
        {
        }
        public static void Add(GameObject prefab)
        {
            String contentPackIdentifier = Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            Add(prefab, contentPackIdentifier);
        }
        public static void Add(GameObject prefab, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            if (!ContentPacks.Packs[contentPackIdentifier].projectilePrefabs.Contains(prefab))
            {
                ContentPacks.Packs[contentPackIdentifier].projectilePrefabs.Add(prefab);
            }
        }
    }
}