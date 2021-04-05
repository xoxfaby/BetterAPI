using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public class Prefabs
    {
        internal static List<GameObject> prefabs;
        static Prefabs()
        {
            prefabs = new List<GameObject>();
        }
        public static void Add(GameObject prefab)
        {
            prefabs.Add(prefab);
        }

    }
}
