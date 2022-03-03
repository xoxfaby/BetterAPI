using System;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;

namespace BetterAPI
{
    public static class Stats
    {
        public static Stat Health = Stat.withBaseStatHook(nameof(CharacterBody.baseMaxHealth));
        public static Stat Regen = Stat.withBaseStatHook(nameof(CharacterBody.baseRegen));
        public static Stat Shield = Stat.withBaseStatHook(nameof(CharacterBody.baseMaxShield));
        public static Stat Damage = Stat.withBaseStatHook(nameof(CharacterBody.baseDamage));
        public static Stat Armor = Stat.withBaseStatHook(nameof(CharacterBody.baseArmor));
        public static Stat MoveSpeed = Stat.withBaseStatHook(nameof(CharacterBody.baseMoveSpeed));
        public static Stat AttackSpeed = Stat.withBaseStatHook(nameof(CharacterBody.baseAttackSpeed));
        public static Stat CriticalChance = Stat.withBaseStatHook(nameof(CharacterBody.baseCrit));
        public static Stat CriticalDamage = Stat.withPropertyHook(nameof(CharacterBody.critMultiplier));
        public static Stat JumpPower = Stat.withBaseStatHook(nameof(CharacterBody.baseJumpPower));
        public static Stat JumpCount = Stat.withBaseStatHook(nameof(CharacterBody.baseJumpCount));
        public static Stat Curse = Stat.withPropertyHook(nameof(CharacterBody.cursePenalty));


        private const System.Reflection.BindingFlags allFlags = System.Reflection.BindingFlags.Public
        | System.Reflection.BindingFlags.NonPublic
        | System.Reflection.BindingFlags.Instance
        | System.Reflection.BindingFlags.Static;

        static Stats()
        {
            BetterAPIPlugin.Hooks.Add<RoR2.CharacterBody>("RecalculateStats", multiplierHook);


        }

        internal static void multiplierHook(ILContext il)
        {
            Health.multiplierEnabled =
                Regen.multiplierEnabled =
                Shield.multiplierEnabled =
                Damage.multiplierEnabled =
                Armor.multiplierEnabled =
                MoveSpeed.multiplierEnabled =
                AttackSpeed.multiplierEnabled =
                CriticalChance.multiplierEnabled =
                CriticalDamage.multiplierEnabled =
                JumpPower.multiplierEnabled =
                JumpCount.multiplierEnabled =
                Curse.multiplierEnabled = false;
            var c = new ILCursor(il);
            c.Index = c.Instrs.Count - 1;

            bool foundInsertionPoint = c.TryGotoPrev(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetProperty(nameof(CharacterBody.cursePenalty), allFlags).GetSetMethod(true))
            ) && c.TryGotoNext(MoveType.After,
                x => x.MatchLdarg(0)
            );

            if (foundInsertionPoint)
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<CharacterBody>>(characterBody =>
                {
                    characterBody.maxHealth *= Health.getMultiplier(characterBody);
                    characterBody.regen *= Regen.getMultiplier(characterBody);
                    characterBody.maxShield *= Shield.getMultiplier(characterBody);
                    characterBody.damage *= Damage.getMultiplier(characterBody);
                    characterBody.armor *= Armor.getMultiplier(characterBody);
                    characterBody.moveSpeed *= MoveSpeed.getMultiplier(characterBody);
                    characterBody.attackSpeed *= AttackSpeed.getMultiplier(characterBody);
                    characterBody.crit *= CriticalChance.getMultiplier(characterBody);
                    characterBody.critMultiplier *= CriticalDamage.getMultiplier(characterBody);
                    characterBody.jumpPower *= JumpPower.getMultiplier(characterBody);
                    characterBody.maxJumpCount *= (int)JumpCount.getMultiplier(characterBody);
                    characterBody.cursePenalty *= Curse.getMultiplier(characterBody);
                });
                Health.multiplierEnabled =
                    Regen.multiplierEnabled =
                    Shield.multiplierEnabled =
                    Damage.multiplierEnabled =
                    Armor.multiplierEnabled =
                    MoveSpeed.multiplierEnabled =
                    AttackSpeed.multiplierEnabled =
                    CriticalChance.multiplierEnabled =
                    CriticalDamage.multiplierEnabled =
                    JumpPower.multiplierEnabled =
                    JumpCount.multiplierEnabled =
                    Curse.multiplierEnabled = true;
            }
        }



        public class Stat
        {

            public class StatMultiplierArgs
            {
                public List<float> additiveMultipliers = new List<float>() { 1f };
                public List<float> multiplicativeMultipliers = new List<float>() { 1f };
            }
            public class StatBonusArgs
            {
                public List<float> FlatBonuses = new List<float>() { 0f };
                public List<float> PerLevelBonuses = new List<float>() { 0f };
            }

            public delegate void StatMultiplierEvent(CharacterBody characterBody, StatMultiplierArgs e);
            public delegate void StatBonusEvent(CharacterBody characterBody, StatBonusArgs e);

            public delegate void BonusHook(ILContext il);

            public bool bonusEnabled = false;
            public event StatBonusEvent collectBonuses;
            public bool multiplierEnabled = false;
            public event StatMultiplierEvent collectMultipliers;

            private string baseStatFieldName;
            private string statPropertyName;

            internal static Stat withBaseStatHook(string baseStatFieldName)
            {
                var statEventHandler = new Stat();
                statEventHandler.baseStatFieldName = baseStatFieldName;
                BetterAPIPlugin.Hooks.Add<RoR2.CharacterBody>("RecalculateStats", statEventHandler.baseStatHook);
                return statEventHandler;
            }
            internal static Stat withPropertyHook(string statPropertyName)
            {
                var statEventHandler = new Stat();
                statEventHandler.statPropertyName = statPropertyName;
                BetterAPIPlugin.Hooks.Add<RoR2.CharacterBody>("RecalculateStats", statEventHandler.setPropertyHook);
                return statEventHandler;
            }

            private Stat() {
                RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
                

            }
            private void Run_onRunStartGlobal(Run obj)
            {
                if (!bonusEnabled) UnityEngine.Debug.LogError($"Bonus hook on {baseStatFieldName ?? statPropertyName } failed");
                if (!multiplierEnabled) UnityEngine.Debug.LogError($"Multiplier hook on {baseStatFieldName ?? statPropertyName } failed");
            }

            internal void baseStatHook(ILContext il)
            {
                bonusEnabled = false;
                var c = new ILCursor(il);
                bool foundField = c.TryGotoNext(MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld(typeof(CharacterBody).GetField(baseStatFieldName, allFlags))
                );


                if (foundField)
                {
                    var type = typeof(CharacterBody).GetField(baseStatFieldName, allFlags).FieldType;
                    if (type == typeof(int))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, int>>(characterBody => (int)getBonus(characterBody));
                        c.Emit(OpCodes.Add);
                    }
                    else if (type == typeof(float))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>(characterBody => getBonus(characterBody));
                        c.Emit(OpCodes.Add);
                    }
                    else
                    {
                        return;
                    }
                    bonusEnabled = true;
                }
            }

            internal void setPropertyHook(ILContext il)
            {
                var c = new ILCursor(il);
                bool foundPropertySetter = c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetProperty(statPropertyName, allFlags).GetSetMethod(true))
                );
                
                if (foundPropertySetter)
                {
                    var type = typeof(CharacterBody).GetProperty(statPropertyName, allFlags).PropertyType;
                    if (type == typeof(int))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, int>>(characterBody => (int) getBonus(characterBody));
                        c.Emit(OpCodes.Add);
                    } 
                    else if (type == typeof(float))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<CharacterBody, float>>(characterBody => getBonus(characterBody));
                        c.Emit(OpCodes.Add);
                    }
                    else
                    {
                        return;
                    }
                    bonusEnabled = true;
                }
            }

            public float getBonus(CharacterBody characterBody)
            {
                if (bonusEnabled && collectBonuses != null)
                {
                    var eventArgs = new StatBonusArgs();
                    collectBonuses.Invoke(characterBody, eventArgs);
                    var flatBonus = eventArgs.FlatBonuses.Aggregate((x, y) => x + y);
                    var perLevelBonus = eventArgs.PerLevelBonuses.Aggregate((x, y) => x + y) * characterBody.level;
                    return flatBonus + perLevelBonus;
                }
                return 0f;
            }

            public float getMultiplier(CharacterBody characterBody)
            {
                if (multiplierEnabled && collectMultipliers != null)
                {
                    var eventArgs = new StatMultiplierArgs();
                    collectMultipliers.Invoke(characterBody, eventArgs);
                    var additiveMultipliers = eventArgs.additiveMultipliers.Aggregate((x, y) => x + y);
                    var multiplicativeMultipliers = eventArgs.multiplicativeMultipliers.Aggregate((x, y) => x * y) ;
                    return additiveMultipliers * multiplicativeMultipliers;
                }
                return 1f;
            }
        }
    }
}
