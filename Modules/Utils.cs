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
                BetterAPI.print("Prefab already has NetworkID");
                if (!prefabCounter.ContainsKey(gameObject)) prefabCounter.Add(gameObject, 0);
                prefabCounter[gameObject]++;
                BetterAPI.print($"Original ID: {networkId.assetId.ToString()}");
                BetterAPI.print($"Copy #:{prefabCounter[gameObject]}");
                var newnetworkId = NetworkHash128.Parse(networkId.assetId.ToString() + prefabCounter[gameObject]);
                BetterAPI.print($"New ID: {newnetworkId.ToString()}");
                ClientScene.RegisterPrefab(prefab, newnetworkId);
            }
            return prefab;
        }

    }
}
