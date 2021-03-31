using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public class Items
    {
        internal readonly static List<ItemDef> itemDefs;
        static Items()
        {
            itemDefs = new List<ItemDef>();
        }

        public static ItemDef Add(ItemDef itemDef)
        {
            itemDefs.Add(itemDef);
            return itemDef;
        }
        public static ItemDef Add(ItemTemplate itemTemplate)
        {
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemTemplate.internalName;
            itemDef.tier = itemTemplate.tier;
            itemDef.pickupModelPrefab = itemTemplate.prefab;
            itemDef.pickupIconSprite = itemTemplate.icon;
            itemDef.nameToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_NAME";
            itemDef.pickupToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_PICKUP";
            itemDef.descriptionToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_DESC";
            itemDef.loreToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_LORE";
            itemDef.tags = itemTemplate.tags ?? new ItemTag[] { };

            Languages.AddTokenString(itemDef.nameToken, itemTemplate.name);
            Languages.AddTokenString(itemDef.pickupToken, itemTemplate.pickupText);
            Languages.AddTokenString(itemDef.descriptionToken, itemTemplate.descriptionText);
            Languages.AddTokenString(itemDef.loreToken, itemTemplate.loreText);

            return Add(itemDef);
        }
        public struct ItemTemplate
        {
            public string internalName;
            public string name;
            public string pickupText;
            public string descriptionText;
            public string loreText;
            public GameObject prefab;
            public Sprite icon;
            public ItemTier tier;
            public ItemTag[] tags;
            public ItemDisplayRule[] itemDisplayRules;
        }
    }
}
