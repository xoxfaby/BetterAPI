using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace BetterAPI
{
    public static class CostType
    {
        internal static List<CostTypeDef> costTypeDefs;
        static CostType()
        {
            costTypeDefs = new List<CostTypeDef>();
            IL.RoR2.CostTypeCatalog.GetCostTypeDef += CostTypeCatalog_GetCostTypeDef;
            CostTypeCatalog.modHelper.getAdditionalEntries += (list) => list.AddRange(costTypeDefs);
        }

        private static void CostTypeCatalog_GetCostTypeDef(ILContext il)
        {
            var c = new ILCursor(il);
            bool found = c.TryGotoNext(
                x => x.MatchLdcI4((int)CostTypeIndex.Count)
            );

            if (found) {
                c.Remove();
                c.Emit(OpCodes.Call, typeof(CostTypeCatalog).GetProperty("costTypeCount").GetGetMethod());
            }
        }

        public static void Add(CostTypeDef costTypeDef)
        {
            costTypeDefs.Add(costTypeDef);
        }
    }
}


}
