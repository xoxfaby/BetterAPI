using System;
using System.Collections.Generic;
using System.Text;

namespace BetterAPI
{
    public class Interactables
    {
        public static RoR2.TemporaryVisualEffect myTempEffect;
        static Interactables()
        {
            //On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;
            //On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects;
        }

        private static void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, RoR2.CharacterBody self)
        {
            orig(self);
            self.UpdateSingleTemporaryVisualEffect(ref myTempEffect, "Prefabs/TemporaryVisualEffects/BucklerDefense", self.radius, true, "");
        }

        private static WeightedSelection<RoR2.DirectorCard> SceneDirector_GenerateInteractableCardSelection(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, RoR2.SceneDirector self)
        {
            return orig(self);
        }
    }
}
