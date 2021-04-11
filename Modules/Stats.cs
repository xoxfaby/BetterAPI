using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace BetterAPI
{
    public static class Stats
    {

        public static StatsEventHandler HealthMultiplier = new StatsEventHandler();
        static Stats()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        public static void Debug()
        {
            Stats.HealthMultiplier.getStatModifiers += HealthMultiplier_getStatModifiers;
        }

        private static void HealthMultiplier_getStatModifiers(object sender, Stats.StatsEventHandler.StatsEventArgs e)
        {
            e.stat *= 1000;
        }

        private static void CharacterBody_RecalculateStats(ILContext il)
        {
            var c = new ILCursor(il);
            var found = c.TryGotoNext(
                x => x.MatchLdloc(51),
                x => x.MatchMul(),
                x => x.MatchStloc(50)
            );
            c.Index += 3;
            if (found)
            {
                c.Emit(OpCodes.Ldloc, 50);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody,float>>((characterBody) => HealthMultiplier.getModifier(characterBody));
                c.Emit(OpCodes.Mul);
                c.Emit(OpCodes.Stloc, 50);
            }

        }

        public class StatsEventHandler
        {
            public event EventHandler<StatsEventArgs> getStatModifiers;

            public float getModifier(CharacterBody characterBody)
            {
                var eventArgs = new StatsEventArgs { stat = 1 };
                getStatModifiers.Invoke(characterBody, eventArgs);
                return eventArgs.stat;
            }

            public class StatsEventArgs
            {
                public float stat;
            }
        }
    }
}
