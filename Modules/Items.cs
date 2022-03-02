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
            itemDef.tier = itemTemplate.tier ?? itemDef.tier;
            itemDef.canRemove = itemTemplate.canRemove ?? itemDef.canRemove;
            itemDef.unlockableDef = itemTemplate.unlockableDef ?? itemDef.unlockableDef;
            itemDef.pickupModelPrefab = itemTemplate.prefab ?? itemDef.pickupModelPrefab;
            itemDef.pickupIconSprite = itemTemplate.icon ?? itemDef.pickupIconSprite;
            itemDef.nameToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_NAME";
            itemDef.pickupToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_PICKUP";
            itemDef.descriptionToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_DESC";
            itemDef.loreToken = $"ITEM_{itemTemplate.internalName.ToUpper()}_LORE";
            itemDef.tags = itemTemplate.tags ?? itemDef.tags;

            Languages.AddTokenString(itemDef.nameToken, itemTemplate.name);
            Languages.AddTokenString(itemDef.pickupToken, itemTemplate.pickupText);
            Languages.AddTokenString(itemDef.descriptionToken, itemTemplate.descriptionText);
            Languages.AddTokenString(itemDef.loreToken, itemTemplate.loreText);

            return Add(itemDef, itemTemplate.characterItemDisplayRules, contentPackIdentifier);
        }

        public class ItemTemplate
        {
            public string name;
            public string internalName;
            public string pickupText;
            public string descriptionText;
            public string loreText;
            public GameObject prefab;
            public Sprite icon;
            public bool? canRemove;
            public ItemTier? tier;
            public ItemTag[] tags;
            public UnlockableDef unlockableDef;

            public ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules;
        }
    }
}
