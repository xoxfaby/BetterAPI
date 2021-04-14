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
        public static void Add(BuffDef buffDef)
        {
            Add(buffDef, Assembly.GetCallingAssembly().GetName().Name);
        }
        public static void Add(BuffDef buffDef, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            ContentPacks.Packs[contentPackIdentifier].buffDefs.Add(buffDef);
        }

    }
}
