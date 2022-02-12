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
        internal static GameObject prefabParent { get { return _prefabParent.Value; } }

        static Utils()
        {
            _prefabParent = new Lazy<GameObject>(() =>
            {
                var prefab = new GameObject("CustomPrefabs");
                UnityEngine.Object.DontDestroyOnLoad(prefab);
                prefab.SetActive(false);

                BetterAPIPlugin.Hooks.Add<GameObject,bool>(typeof(RoR2.Util), "IsPrefab", Util_IsPrefab);
                return prefab;
            });
        }

        public static bool Util_IsPrefab(Func<GameObject, bool> orig, GameObject obj)
        {
            return obj.transform.parent && obj.transform.parent.gameObject.name == "CustomPrefabs" || orig(obj);
        }

        public static ItemDef[] ItemDefsFromTier(ItemTier itemTier)
        {
            return ItemDefsFromTier(itemTier, false);
        }
        public static ItemDef[] ItemDefsFromTier(ItemTier itemTier, bool onlyUnlocked = false)
        {
            var itemDefs = new List<ItemDef>();
            foreach (var itemDef in ItemCatalog.itemDefs)
            {

                if (itemDef.tier == itemTier)
                {
                    if (!onlyUnlocked || (Run.instance && Run.instance.IsItemAvailable(itemDef.itemIndex)))
                    {
                        itemDefs.Add(itemDef);
                    }
                    else if(!Run.instance)
                    {
                        foreach (var userProfile in UserProfile.loggedInProfiles)
                        {
                            if (userProfile.HasUnlockable(itemDef.unlockableDef)) itemDefs.Add(itemDef);
                            continue;
                        }
                    }
                }
            }
            return itemDefs.ToArray();
        }


        public static GameObject PrefabFromGameObject(GameObject gameObject,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            var prefab = UnityEngine.Object.Instantiate(gameObject, prefabParent.transform);
            var networkId = prefab.GetComponent<NetworkIdentity>();
            if (networkId)
            {
                networkId.m_AssetId = NetworkHash128.Parse(Hash128.Compute(sourceLineNumber + memberName + sourceFilePath).ToString());
            }
            return prefab;
        }
    }
}
