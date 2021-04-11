using System;
using System.Collections.Generic;
using System.Text;
using MonoMod.Cil;
using RoR2;

namespace BetterAPI
{
    public static class Buffs
    {
        internal static List<BuffDef> buffDefs;
        static Buffs()
        {
            buffDefs = new List<BuffDef>();
            BuffCatalog.modHelper.getAdditionalEntries += (list) => list.AddRange(buffDefs);
        }

        public static void Add(BuffDef buffDef)
        {
            buffDefs.Add(buffDef);
        }

    }
}
