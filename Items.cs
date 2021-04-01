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
        internal readonly static Dictionary<String, Dictionary<UnityEngine.Object, ItemDisplayRule[]>> characterModelItemDisplayRulesDicts;
        static Items()
        {
            itemDefs = new List<ItemDef>();
            characterModelItemDisplayRulesDicts = new Dictionary<string, Dictionary<UnityEngine.Object, ItemDisplayRule[]>>();
        }

        public static ItemDef Add(ItemDef itemDef, CharacterItemDisplayRule[] characterItemDisplayRules = null)
        {
            itemDefs.Add(itemDef);
            if(characterItemDisplayRules != null)
            {
                foreach (var characterItemDisplayRule in characterItemDisplayRules)
                {
                    AddItemDisplayRule(itemDef, characterItemDisplayRule.itemDisplayRules, characterItemDisplayRule.characterModelName);
                }
            }
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

            return Add(itemDef, itemTemplate.characterItemDisplayRules);
        }
        public static void AddItemDisplayRule(UnityEngine.Object keyAsset, ItemDisplayRule[] itemDisplayRules, String characterModelName = "")
        {
            if (characterModelName == null) characterModelName = "";
            if (!characterModelItemDisplayRulesDicts.ContainsKey(characterModelName))
            {
                characterModelItemDisplayRulesDicts.Add(characterModelName,new Dictionary<UnityEngine.Object, ItemDisplayRule[]>());
            }
            characterModelItemDisplayRulesDicts[characterModelName].Add(keyAsset, itemDisplayRules);
        }

        internal static void ApplyCustomItemDisplayRules()
        {
            foreach (var bodyPrefab in BodyCatalog.allBodyPrefabs)
            {
                CharacterModel characterModel = bodyPrefab.GetComponentInChildren<CharacterModel>();
                if (characterModel && characterModel.itemDisplayRuleSet)
                {
                    Dictionary<UnityEngine.Object, ItemDisplayRule[]> characterModelItemDisplayRulesDict;
                    if (characterModelItemDisplayRulesDicts.TryGetValue(characterModel.name, out characterModelItemDisplayRulesDict)
                        || characterModelItemDisplayRulesDicts.TryGetValue("", out characterModelItemDisplayRulesDict))
                    {
                        foreach (var characterModelItemDisplayRule in characterModelItemDisplayRulesDict)
                        {
                            characterModel.itemDisplayRuleSet.SetDisplayRuleGroup(characterModelItemDisplayRule.Key, new DisplayRuleGroup { rules = characterModelItemDisplayRule.Value } );
                        }
                        characterModel.itemDisplayRuleSet.GenerateRuntimeValues();
                    }
                }
            }
        }

        public struct CharacterItemDisplayRule
        {
            public string characterModelName;
            public ItemDisplayRule[] itemDisplayRules;
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
            public CharacterItemDisplayRule[] characterItemDisplayRules;
        }
    }
}
