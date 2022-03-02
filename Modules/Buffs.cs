using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class Buffs
    {
        internal static Dictionary<BuffDef, BuffInfo> buffInfos = new Dictionary<BuffDef, BuffInfo>();
        public static BuffDef Add(BuffDef buffDef, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].buffDefs.Add(buffDef);
            return buffDef;
        }

        public static BuffDef Add(BuffTemplate buffTemplate, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffTemplate.internalName;
            buffDef.buffColor = buffTemplate.buffColor ?? buffDef.buffColor;
            buffDef.canStack = buffTemplate.canStack ?? buffDef.canStack;
            buffDef.eliteDef = buffTemplate.eliteDef ?? buffDef.eliteDef;
            buffDef.isCooldown = buffTemplate.isCooldown ?? buffDef.isCooldown;
            buffDef.isDebuff = buffTemplate.isDebuff ?? buffDef.isDebuff;
            buffDef.iconSprite = buffTemplate.iconSprite ?? buffDef.iconSprite;
            buffDef.isHidden = buffTemplate.isHidden ?? buffDef.isHidden;
            buffDef.m_CachedPtr = buffTemplate.m_CachedPtr ?? buffDef.m_CachedPtr;
            buffDef.startSfx = buffTemplate.startSfx ?? buffDef.startSfx;
            String nameToken = $"ARTIFACT_{buffTemplate.internalName.ToUpper()}_NAME";
            String descriptionToken = $"ARTIFACT_{buffTemplate.internalName.ToUpper()}_DESC";

            Languages.AddTokenString(nameToken, buffTemplate.name);
            Languages.AddTokenString(descriptionToken, buffTemplate.description);

            AddInfo(buffDef, nameToken, descriptionToken);

            return Add(buffDef, contentPackIdentifier);
        }

        public static void AddName(BuffDef buffDef, string nameToken)
        {
            AddInfo(buffDef, nameToken: nameToken);
        }
        public static void AddDescription(BuffDef buffDef, string descriptionToken)
        {
            AddInfo(buffDef, descriptionToken: descriptionToken);
        }

        public static void AddInfo(BuffDef buffDef, string nameToken = null, string descriptionToken = null)
        {
            buffInfos.TryGetValue(buffDef, out BuffInfo buffInfo);
            buffInfo.nameToken = buffInfo.nameToken ?? nameToken;
            buffInfo.descriptionToken = buffInfo.descriptionToken ?? descriptionToken;
            AddInfo(buffDef, buffInfo);
        }
        public static void AddInfo(BuffDef buffDef, BuffInfo buffInfo)
        {
            buffInfos[buffDef] = buffInfo;
        }

        public static string GetName(BuffDef buffDef)
        {
            buffInfos.TryGetValue(buffDef, out BuffInfo buffInfo);
            return string.IsNullOrEmpty(buffInfo.nameToken) ? buffDef.name : RoR2.Language.GetString(buffInfo.nameToken);
        }
        public static string GetDescription(BuffDef buffDef)
        {
            buffInfos.TryGetValue(buffDef, out BuffInfo buffInfo);
            return string.IsNullOrEmpty(buffInfo.descriptionToken) ? String.Empty : RoR2.Language.GetString(buffInfo.descriptionToken);
        }

        public struct BuffInfo
        {
            public string nameToken;
            public string descriptionToken;
        }

        public class BuffTemplate
        {
            public string internalName;
            public string name;
            public string description;
            public Color? buffColor;
            public bool? canStack;
            public EliteDef eliteDef;
            public bool? isCooldown;
            public bool? isDebuff;
            public Sprite iconSprite;
            public bool? isHidden;
            public IntPtr? m_CachedPtr;
            public NetworkSoundEventDef startSfx;
        }
    }
}
