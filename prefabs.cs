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

        internal static Lazy<GameObject> _prefabParent;
        internal static GameObject prefabParent { get { return _prefabParent.Value; } }

        static Prefabs()
        {
            prefabs = new List<GameObject>();
            _prefabParent = new Lazy<GameObject>(() =>
            {
                var prefab = new GameObject("CustomPrefabs");
                UnityEngine.Object.DontDestroyOnLoad(prefab);
                prefab.SetActive(false);

                On.RoR2.Util.IsPrefab += (orig, obj) =>
                {
                    return obj.transform.parent && obj.transform.parent.gameObject.name == "CustomPrefabs" || orig(obj);
                };
                return prefab;
            });
        }

        public static GameObject FromGameObject(GameObject gameObject)
        {
            return UnityEngine.Object.Instantiate(gameObject, ((GameObject) prefabParent).transform);
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
