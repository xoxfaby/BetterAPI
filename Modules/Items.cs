using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class Items
    {

        public static ItemDef Add(ItemDef itemDef, ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules = null, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].itemDefs.Add(itemDef);
            ItemDisplays.AddItemDisplayRules(itemDef, characterItemDisplayRules);
            return itemDef;
        }
        public static ItemDef Add(ItemTemplate itemTemplate, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemTemplate.internalName;
            itemDef.tier = itemTemplate.tier;
            itemDef.canRemove = itemTemplate.canRemove;
            itemDef.unlockableDef = itemTemplate.unlockableDef;
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

            return Add(itemDef, itemTemplate.characterItemDisplayRules, contentPackIdentifier);
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
            public bool canRemove;
            public ItemTier tier;
            public ItemTag[] tags;
            public UnlockableDef unlockableDef;

            public ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules;
        }
    }
}
