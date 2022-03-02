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
            equipmentDef.cooldown = equipmentTemplate.cooldown ?? equipmentDef.cooldown;
            equipmentDef.isLunar = equipmentTemplate.isLunar ?? equipmentDef.isLunar;
            equipmentDef.isBoss = equipmentTemplate.isBoss ?? equipmentDef.isBoss;
            equipmentDef.canDrop = equipmentTemplate.canDrop ?? equipmentDef.canDrop;
            equipmentDef.appearsInSinglePlayer = equipmentTemplate.appearsInSinglePlayer ?? equipmentDef.appearsInSinglePlayer;
            equipmentDef.appearsInMultiPlayer = equipmentTemplate.appearsInMultiPlayer ?? equipmentDef.appearsInMultiPlayer;
            equipmentDef.enigmaCompatible = equipmentTemplate.appearsInSinglePlayer ?? equipmentDef.enigmaCompatible;
            equipmentDef.colorIndex = equipmentTemplate.colorIndex ?? equipmentDef.colorIndex;
            equipmentDef.unlockableDef = equipmentTemplate.unlockableDef ?? equipmentDef.unlockableDef;
            equipmentDef.passiveBuffDef = equipmentTemplate.passiveBuffDef ?? equipmentDef.passiveBuffDef;
            equipmentDef.pickupModelPrefab = equipmentTemplate.prefab ?? equipmentDef.pickupModelPrefab;
            equipmentDef.pickupIconSprite = equipmentTemplate.icon ?? equipmentDef.pickupIconSprite;
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

        public class EquipmentTemplate
        {
            public string internalName;
            public string name;
            public float? cooldown;
            public bool? isLunar;
            public bool? isBoss;
            public bool? canDrop;
            public bool? appearsInSinglePlayer;
            public bool? appearsInMultiPlayer;
            public bool? enigmaCompatible;
            public ColorCatalog.ColorIndex? colorIndex;
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
