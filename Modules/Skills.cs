using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RoR2;
using RoR2.Skills;
using UnityEngine;


namespace BetterAPI
{
    public static class Skills
    {

        public static SkillDef Add(SkillDef skillDef, ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules = null, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].skillDefs.Add(skillDef);
            return skillDef;
        }

        public static SkillFamily AddFamily(SkillFamily skillFamily, ItemDisplays.CharacterItemDisplayRule[] characterItemDisplayRules = null, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].skillFamilies.Add(skillFamily);
            return skillFamily;
        }
    }
}
