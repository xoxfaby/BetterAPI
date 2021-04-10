using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class MasterPrefabs
    {
        internal static List<GameObject> prefabs;
        static MasterPrefabs()
        {
            prefabs = new List<GameObject>();
        }

        public static void Add(GameObject prefab)
        {
            if (!prefabs.Contains(prefab))
            {
                prefabs.Add(prefab);
            }
        }

    }
}