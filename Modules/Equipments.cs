using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class Equipments
    {
        public static EquipmentDef Add(EquipmentDef equipmentDef, ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules = null, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if(!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].equipmentDefs.Add(equipmentDef);
            ItemDisplays.AddItemDisplayRules(equipmentDef, characterItemDisplayRules);
            return equipmentDef;
        }
        public static EquipmentDef Add(EquipmentTemplate equipmentTemplate, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            EquipmentDef equipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
            equipmentDef.name = equipmentTemplate.internalName;
            equipmentDef.cooldown = equipmentTemplate.cooldown;
            equipmentDef.isLunar = equipmentTemplate.isLunar;
            equipmentDef.isBoss = equipmentTemplate.isBoss;
            equipmentDef.canDrop = equipmentTemplate.canDrop;
            equipmentDef.appearsInSinglePlayer = equipmentTemplate.appearsInSinglePlayer;
            equipmentDef.appearsInMultiPlayer = equipmentTemplate.appearsInMultiPlayer;
            equipmentDef.enigmaCompatible = equipmentTemplate.appearsInSinglePlayer;
            equipmentDef.colorIndex = equipmentTemplate.colorIndex;
            equipmentDef.unlockableDef = equipmentTemplate.unlockableDef;
            equipmentDef.passiveBuffDef = equipmentTemplate.passiveBuffDef;
            equipmentDef.pickupModelPrefab = equipmentTemplate.prefab;
            equipmentDef.pickupIconSprite = equipmentTemplate.icon;
            equipmentDef.nameToken = $"EQUIPMENT_{equipmentTemplate.internalName.ToUpper()}_NAME";
            equipmentDef.pickupToken = $"EQUIPMENT_{equipmentTemplate.internalName.ToUpper()}_PICKUP";
            equipmentDef.descriptionToken = $"EQUIPMENT_{equipmentTemplate.internalName.ToUpper()}_DESC";
            equipmentDef.loreToken = $"EQUIPMENT_{equipmentTemplate.internalName.ToUpper()}_LORE";


            Languages.AddTokenString(equipmentDef.nameToken, equipmentTemplate.name);
            Languages.AddTokenString(equipmentDef.pickupToken, equipmentTemplate.pickupText);
            Languages.AddTokenString(equipmentDef.descriptionToken, equipmentTemplate.descriptionText);
            Languages.AddTokenString(equipmentDef.loreToken, equipmentTemplate.loreText);

            return Add(equipmentDef, equipmentTemplate.characterItemDisplayRules, contentPackIdentifier);
        }

        public struct EquipmentTemplate
        {
            public string internalName;
            public string name;
            public float cooldown;
            public bool isLunar;
            public bool isBoss;
            public bool canDrop;
            public bool appearsInSinglePlayer;
            public bool appearsInMultiPlayer;
            public bool enigmaCompatible;
            public ColorCatalog.ColorIndex colorIndex;
            public BuffDef passiveBuffDef;
            public string pickupText;
            public string descriptionText;
            public string loreText;
            public GameObject prefab;
            public Sprite icon;
            public ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules;
            public UnlockableDef unlockableDef;
        }
    }
}
