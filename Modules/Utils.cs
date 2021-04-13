using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterAPI
{
    public static class Utils
    {

        internal static Lazy<GameObject> _prefabParent;
        internal static Dictionary<GameObject, int> prefabCounter = new Dictionary<GameObject, int>();
        internal static GameObject prefabParent { get { return _prefabParent.Value; } }

        static Utils()
        {
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
        public static ItemDef[] ItemDefsFromTier(ItemTier itemTier)
        {
            var itemDefs = new List<ItemDef>();
            foreach (var itemDef in ItemCatalog.itemDefs)
            {

                if (itemDef.tier == itemTier)
                {
                    itemDefs.Add(itemDef);
                }
            }
            return itemDefs.ToArray();
        }

        public static GameObject PrefabFromGameObject(GameObject gameObject)
        {
            var prefab = UnityEngine.Object.Instantiate(gameObject, prefabParent.transform);
            var networkId = prefab.GetComponent<NetworkIdentity>();
            if (networkId)
            {
                if (!prefabCounter.ContainsKey(gameObject)) prefabCounter.Add(gameObject, 0);
                prefabCounter[gameObject]++;
                Hash128 newHash = Hash128.Parse(networkId.assetId.ToString());
                networkId.assetId.Reset();
                for (int i = 0; i < prefabCounter[gameObject]; i++)
                {
                    newHash = Hash128.Compute(newHash.ToString());
                }
                networkId.m_AssetId = NetworkHash128.Parse(newHash.ToString());
            }
            return prefab;
        }

    }
}
