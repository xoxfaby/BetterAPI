using System;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace BetterAPI
{
    public static class Stats
    {

        public static StatsEventHandler health = new StatsEventHandler();
        public static StatsEventHandler regen = new StatsEventHandler();
        public static StatsEventHandler shield = new StatsEventHandler();
        public static StatsEventHandler damage = new StatsEventHandler();
        public static StatsEventHandler armor = new StatsEventHandler();
        public static StatsEventHandler speed = new StatsEventHandler();
        public static StatsEventHandler sprintSpeed = new StatsEventHandler();
        public static StatsEventHandler attackSpeed = new StatsEventHandler();
        public static StatsEventHandler critChance = new StatsEventHandler();
        public static StatsEventHandler jumps = new StatsEventHandler();
        public static StatsEventHandler curse = new StatsEventHandler();
        public static StatsEventHandler luck = new StatsEventHandler();
        static Stats()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
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
                c.EmitDelegate<Func<CharacterBody,float>>((characterBody) => health.getMultiplier(characterBody));
                c.Emit(OpCodes.Mul);
                c.Emit(OpCodes.Stloc, 50);
            }

        }


        public class StatsEventHandler
        {
            public delegate void StatsEvent(CharacterBody characterBody, StatsEventArgs e);

            public event StatsEvent collectBonuses;
            public event StatsEvent collectMultipliers;

            public float getBonus(CharacterBody characterBody)
            {
                if (collectBonuses != null)
                {
                    var eventArgs = new StatsEventArgs { stat = 0f };
                    collectBonuses.Invoke(characterBody, eventArgs);
                    return eventArgs.stat;
                }
                return 0f;
            }

            public float getMultiplier(CharacterBody characterBody)
            {
                if (collectMultipliers != null)
                {
                    var eventArgs = new StatsEventArgs { stat = 1f };
                    collectMultipliers.Invoke(characterBody, eventArgs);
                    return eventArgs.stat;
                }
                return 1f;
                
            }

            public class StatsEventArgs
            {
                public float stat;
            }
        }
    }
}
