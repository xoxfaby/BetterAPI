using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MonoMod.Cil;
using RoR2;

namespace BetterAPI
{
    public static class Buffs
    {
        internal static Dictionary<BuffDef, BuffInfo> buffInfos = new Dictionary<BuffDef, BuffInfo>();
        public static void Add(BuffDef buffDef, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            if (!ContentPacks.assemblyDict.ContainsKey(contentPackIdentifier)) ContentPacks.assemblyDict[contentPackIdentifier] = Assembly.GetCallingAssembly();
            ContentPacks.Packs[contentPackIdentifier].buffDefs.Add(buffDef);
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
    }
}
