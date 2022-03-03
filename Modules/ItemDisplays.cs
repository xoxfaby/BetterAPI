using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class ItemDisplays
    {
        internal readonly static Dictionary<String, Dictionary<UnityEngine.Object, ItemDisplayRule[]>> characterModelItemDisplayRulesDicts;
        internal readonly static Dictionary<String, Dictionary<UnityEngine.Object, ItemDisplayRule[]>> bodyPrefabItemDisplayRulesDicts;
        static ItemDisplays()
        {
            BodyCatalog.availability.CallWhenAvailable(() =>
            {
                ItemCatalog.availability.CallWhenAvailable(ItemDisplays.ApplyCustomItemDisplayRules);
            });
            characterModelItemDisplayRulesDicts = new Dictionary<string, Dictionary<UnityEngine.Object, ItemDisplayRule[]>>();
            bodyPrefabItemDisplayRulesDicts = new Dictionary<string, Dictionary<UnityEngine.Object, ItemDisplayRule[]>>();
        }

        public static void AddItemDisplayRules(UnityEngine.Object keyAsset, CharacterItemDisplayRule[] characterItemDisplayRules)
        {
            if(characterItemDisplayRules != null)
            {
                foreach (var characterItemDisplayRule in characterItemDisplayRules)
                {
                    AddItemDisplayRule(keyAsset, characterItemDisplayRule.itemDisplayRules, characterItemDisplayRule.characterModelName, characterItemDisplayRule.bodyPrefabName);
                }
            }
        }

        public static void AddItemDisplayRule(UnityEngine.Object keyAsset, ItemDisplayRule[] itemDisplayRules, String characterModelName = null, String bodyName = null)
        {
            foreach (var itemDisplayRule in itemDisplayRules)
            {
                if (itemDisplayRule.followerPrefab.GetComponentInChildren<ItemDisplay>() == null)
                {
                    itemDisplayRule.followerPrefab.AddComponent<ItemDisplay>();
                }
            }
            if (characterModelName == null && bodyName == null) characterModelName = "";
            if (characterModelName != null)
            {
                if (!characterModelItemDisplayRulesDicts.ContainsKey(characterModelName))
                {
                    characterModelItemDisplayRulesDicts.Add(characterModelName, new Dictionary<UnityEngine.Object, ItemDisplayRule[]>());
                }
                characterModelItemDisplayRulesDicts[characterModelName].Add(keyAsset, itemDisplayRules);
            }
            if (bodyName != null)
            {
                if (!bodyPrefabItemDisplayRulesDicts.ContainsKey(bodyName))
                {
                    bodyPrefabItemDisplayRulesDicts.Add(bodyName, new Dictionary<UnityEngine.Object, ItemDisplayRule[]>());
                }
                bodyPrefabItemDisplayRulesDicts[bodyName].Add(keyAsset, itemDisplayRules);
            }
        }

        internal static void ApplyCustomItemDisplayRules()
        {
            foreach (var bodyPrefab in BodyCatalog.allBodyPrefabs)
            {
                CharacterModel characterModel = bodyPrefab.GetComponentInChildren<CharacterModel>();
                if (characterModel)
                {
                    if ((bodyPrefabItemDisplayRulesDicts.TryGetValue(bodyPrefab.name, out var bodyPrefabItemDisplayRulesDict)
                        || bodyPrefabItemDisplayRulesDicts.TryGetValue("", out bodyPrefabItemDisplayRulesDict))
                        | (characterModelItemDisplayRulesDicts.TryGetValue(characterModel.name, out var characterModelItemDisplayRulesDict)
                        || characterModelItemDisplayRulesDicts.TryGetValue("", out characterModelItemDisplayRulesDict)))
                    {
                        if (!characterModel.itemDisplayRuleSet)
                        {
                            characterModel.itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
                        }
                        if (bodyPrefabItemDisplayRulesDict != null)
                        {
                            foreach (var bodyPrefabItemDisplayRule in bodyPrefabItemDisplayRulesDict)
                            {
                                characterModel.itemDisplayRuleSet.SetDisplayRuleGroup(bodyPrefabItemDisplayRule.Key, new DisplayRuleGroup { rules = bodyPrefabItemDisplayRule.Value });
                            }
                        }
                        if (characterModelItemDisplayRulesDict != null)
                        {
                            foreach (var bodyPrefabItemDisplayRule in characterModelItemDisplayRulesDict)
                            {
                                characterModel.itemDisplayRuleSet.SetDisplayRuleGroup(bodyPrefabItemDisplayRule.Key, new DisplayRuleGroup { rules = bodyPrefabItemDisplayRule.Value });
                            }
                        }
                        characterModel.itemDisplayRuleSet.GenerateRuntimeValues();
                    }
                }
            }
        }

        public class CharacterItemDisplayRuleSet
        {
            protected Dictionary<String, List<ItemDisplayRule>> characterModelItemDisplayRuleDict = new Dictionary<string, List<ItemDisplayRule>>();
            protected Dictionary<String, List<ItemDisplayRule>> bodyPrefabItemDisplayRuleDict = new Dictionary<string, List<ItemDisplayRule>>();

            public void AddDefaultRule(ItemDisplayRule itemDisplayRule)
            {
                if (!characterModelItemDisplayRuleDict.ContainsKey("")) characterModelItemDisplayRuleDict.Add("", new List<ItemDisplayRule>());
                characterModelItemDisplayRuleDict[""].Add(itemDisplayRule);
            }
            public void AddDefaultRule(ItemDisplayRule[] itemDisplayRules)
            {
                if (!characterModelItemDisplayRuleDict.ContainsKey("")) characterModelItemDisplayRuleDict.Add("", new List<ItemDisplayRule>());
                foreach (var itemDisplayRule in itemDisplayRules)
                {
                    characterModelItemDisplayRuleDict[""].Add(itemDisplayRule);
                }
            }

            public void AddCharacterModelRule(ItemDisplayRule itemDisplayRule, String characterModelName = "")
            {
                if (!characterModelItemDisplayRuleDict.ContainsKey(characterModelName)) characterModelItemDisplayRuleDict.Add(characterModelName, new List<ItemDisplayRule>());
                characterModelItemDisplayRuleDict[characterModelName].Add(itemDisplayRule);
            }
            public void AddCharacterModelRule(ItemDisplayRule[] itemDisplayRules, String characterModelName = "")
            {
                if (!characterModelItemDisplayRuleDict.ContainsKey(characterModelName)) characterModelItemDisplayRuleDict.Add(characterModelName, new List<ItemDisplayRule>());
                foreach (var itemDisplayRule in itemDisplayRules)
                {
                    characterModelItemDisplayRuleDict[characterModelName].Add(itemDisplayRule);
                }
            }
            public void AddCharacterModelRule(String characterModelName, ItemDisplayRule itemDisplayRule)
            {
                AddCharacterModelRule(itemDisplayRule, characterModelName);
            }

            public void AddBodyPrefabRule(ItemDisplayRule itemDisplayRule, String bodyPrefabName = "")
            {
                if (!bodyPrefabItemDisplayRuleDict.ContainsKey(bodyPrefabName)) bodyPrefabItemDisplayRuleDict.Add(bodyPrefabName, new List<ItemDisplayRule>());
                bodyPrefabItemDisplayRuleDict[bodyPrefabName].Add(itemDisplayRule);
            }
            public void AddBodyPrefabRule(ItemDisplayRule[] itemDisplayRules, String bodyPrefabName = "")
            {
                if (!bodyPrefabItemDisplayRuleDict.ContainsKey(bodyPrefabName)) bodyPrefabItemDisplayRuleDict.Add(bodyPrefabName, new List<ItemDisplayRule>());
                foreach (var itemDisplayRule in itemDisplayRules)
                {
                    bodyPrefabItemDisplayRuleDict[bodyPrefabName].Add(itemDisplayRule);
                }
            }
            public void AddBodyPrefabRule(String bodyPrefabName, ItemDisplayRule itemDisplayRule)
            {
                AddBodyPrefabRule(itemDisplayRule, bodyPrefabName);
            }

            public static implicit operator CharacterItemDisplayRule[](CharacterItemDisplayRuleSet characterItemDisplayRuleSet)
            {
                var characterItemDisplayRules = new List<CharacterItemDisplayRule>();
                foreach (var characterItemDisplayRule in characterItemDisplayRuleSet.characterModelItemDisplayRuleDict)
                {
                    characterItemDisplayRules.Add(new CharacterItemDisplayRule
                    {
                        characterModelName = characterItemDisplayRule.Key,
                        itemDisplayRules = characterItemDisplayRule.Value.ToArray()
                    }); ;
                }
                foreach (var characterItemDisplayRule in characterItemDisplayRuleSet.bodyPrefabItemDisplayRuleDict)
                {
                    characterItemDisplayRules.Add(new CharacterItemDisplayRule
                    {
                        bodyPrefabName = characterItemDisplayRule.Key,
                        itemDisplayRules = characterItemDisplayRule.Value.ToArray()
                    }); ;
                }
                return characterItemDisplayRules.ToArray();
            }
        }

        public struct CharacterItemDisplayRule
        {
            public string characterModelName;
            public string bodyPrefabName;
            public ItemDisplayRule[] itemDisplayRules;
        }
    }
}
