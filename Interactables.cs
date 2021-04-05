using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BetterAPI
{
    public class Interactables
    {
        public static RoR2.TemporaryVisualEffect myTempEffect;
        public enum Category
        { 
            Chests,
            Barrels,
            Shrines,
            Drones,
            Misc,
            Rare,
            Duplicator
        }

        [Flags]
        public enum Stages
        {
            TitanicPlains = 2,
            DistantRoost = 4,
            WetlandAspect = 8,
            AbandonedAqueduct = 16,
            RallypointDelta = 32,
            ScorchedAcres = 64,
            AbyssalDepths = 128,
            SirensCall = 256,
            GildedCoast = 512,
            MomentFractured = 1024,
            Bazaar = 2048,
            VoidCell = 4096,
            MomentWhole = 8192,
            SkyMeadow = 16384,
            BullwarksAmbry = 32768,
            Commencement = 65536,
            SunderedGrove = 131072
        }

        public static Dictionary<Stages, string> SceneNames = new Dictionary<Stages, string>()
        {
            { Stages.TitanicPlains, "golemplains" },
            { Stages.DistantRoost, "blackbeach" },
            { Stages.WetlandAspect, "foggyswamp" },
            { Stages.AbandonedAqueduct, "goolake" },
            { Stages.RallypointDelta, "frozenwall" },
            { Stages.ScorchedAcres, "wispgraveyard" },
            { Stages.AbyssalDepths, "dampcavesimple" },
            { Stages.SirensCall, "shipgraveyard" },
            { Stages.GildedCoast, "goldshores" },
            { Stages.MomentFractured, "mysteryspace" },
            { Stages.Bazaar, "bazaar" },
            { Stages.VoidCell, "arena" },
            { Stages.MomentWhole, "limbo" },
            { Stages.SkyMeadow, "skymeadow" },
            { Stages.BullwarksAmbry, "artifactworld" },
            { Stages.Commencement, "moon" },
            { Stages.SunderedGrove, "rootjungle" }
        };

        private static List<interactableInfo> registeredInteractables = new List<interactableInfo>();

        static Interactables()
        {
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection; ;
            On.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += CharacterBody_UpdateAllTemporaryVisualEffects; ;
        }

        private static void CharacterBody_UpdateAllTemporaryVisualEffects(On.RoR2.CharacterBody.orig_UpdateAllTemporaryVisualEffects orig, CharacterBody self)
        {

            orig(self);
            self.UpdateSingleTemporaryVisualEffect(ref myTempEffect, "Prefabs/TemporaryVisualEffects/BucklerDefense", self.radius, true, "");
        }

        private static WeightedSelection<DirectorCard> SceneDirector_GenerateInteractableCardSelection(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, SceneDirector self)
        {
            if (ClassicStageInfo.instance && ClassicStageInfo.instance.interactableCategories)
            {
                foreach (var cat in ClassicStageInfo.instance.interactableCategories.categories)
                {
                    BetterAPI.print($"{cat.name} - {cat.selectionWeight}");
                    foreach (var card in cat.cards)
                    {
                        BetterAPI.print($"{ card.spawnCard.name} - {card.selectionWeight}");
                    }
                }
            }

            return orig(self);
        }

        private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (NetworkServer.active)
            {
                ClassicStageInfo stageInfo = SceneInfo.instance.GetComponent<ClassicStageInfo>();
                foreach(interactableInfo interactable in registeredInteractables)
                {
                    if (interactable.scenes.Contains(SceneManager.GetActiveScene().name))
                    {
                        stageInfo.interactableCategories.AddCard((int)interactable.category, interactable.directorCard);
                    }
                }
                
            }
            orig(self);
        }

        private static List<string> GetSceneNames(Stages scenes)
        {
            var names = new List<string>();

            foreach( var scene in SceneNames)
            {
                if (scenes.HasFlag(scene.Key)) names.Add(scene.Value);
            }

            return names;
        }

        public static void AddToStages(InteractableTemplate interactable, Stages stages)
        {
            var sceneNames = GetSceneNames(stages);

            var spawnCard = GenerateSpawnCard(interactable);
            var interactableDirectorCard = GenerateDirectorCard(interactable, spawnCard);

            interactableInfo info = new interactableInfo(interactableDirectorCard, interactable.interactableCategory, sceneNames);

            Prefabs.Add(interactable.interactablePrefab);

            registeredInteractables.Add(info);
        }

        public static void AddToStages(InteractableTemplate interactable, List<string> sceneNames)
        {
            var spawnCard = GenerateSpawnCard(interactable);
            var interactableDirectorCard = GenerateDirectorCard(interactable, spawnCard);

            interactableInfo info = new interactableInfo(interactableDirectorCard, interactable.interactableCategory, sceneNames);

            Prefabs.Add(interactable.interactablePrefab);

            registeredInteractables.Add(info);
        }

        public static void AddToStage(InteractableTemplate interactable, string sceneName)
        {
            var sceneNames = new List<string>();

            sceneNames.Add(sceneName);


            var spawnCard = GenerateSpawnCard(interactable);
            var interactableDirectorCard = GenerateDirectorCard(interactable, spawnCard);


            interactableInfo info = new interactableInfo(interactableDirectorCard, interactable.interactableCategory, sceneNames);

            Prefabs.Add(interactable.interactablePrefab);

            registeredInteractables.Add(info);
        }

        public static InteractableSpawnCard GenerateSpawnCard(InteractableTemplate interactable)
        {
            var interactableSpawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            interactableSpawnCard.prefab = interactable.interactablePrefab;
            interactableSpawnCard.sendOverNetwork = interactable.sendOverNetwork;
            interactableSpawnCard.hullSize = interactable.hullSize;
            interactableSpawnCard.nodeGraphType = interactable.nodeGraphType;
            interactableSpawnCard.requiredFlags = interactable.requiredFlags;
            interactableSpawnCard.forbiddenFlags = interactable.forbiddenFlags;
            interactableSpawnCard.directorCreditCost = interactable.directorCreditCost;
            interactableSpawnCard.occupyPosition = interactable.occupyPosition;
            interactableSpawnCard.eliteRules = interactable.eliteRules;
            interactableSpawnCard.orientToFloor = interactable.orientToFloor;
            interactableSpawnCard.slightlyRandomizeOrientation = interactable.slightlyRandomizeOrientation;
            interactableSpawnCard.skipSpawnWhenSacrificeArtifactEnabled = interactable.skipSpawnWhenSacrificeArtifactEnabled;

            return interactableSpawnCard;
        }

        public static DirectorCard GenerateDirectorCard(InteractableTemplate interactable, SpawnCard spawnCard)
        {
            var interactableDirectorCard = new DirectorCard();

            interactableDirectorCard.spawnCard = spawnCard;
            interactableDirectorCard.selectionWeight = interactable.selectionWeight;
            interactableDirectorCard.spawnDistance = interactable.spawnDistance;
            interactableDirectorCard.allowAmbushSpawn = interactable.allowAmbushSpawn;
            interactableDirectorCard.preventOverhead = interactable.preventOverhead;
            interactableDirectorCard.minimumStageCompletions = interactable.minimumStageCompletions;
            interactableDirectorCard.requiredUnlockableDef = interactable.requiredUnlockableDef;
            interactableDirectorCard.forbiddenUnlockableDef = interactable.forbiddenUnlockableDef;

            interactableInfo info = new interactableInfo(interactableDirectorCard, interactable.interactableCategory);

            Prefabs.Add(interactable.interactablePrefab);

            registeredInteractables.Add(info);
            return interactableDirectorCard;
        }


        private class interactableInfo
        {
            public DirectorCard directorCard;
            public Category category;
            public List<string> scenes;
            public interactableInfo(DirectorCard directorCard, Category category, List<string> scenes)
            {
                this.directorCard = directorCard;
                this.category = category;
                this.scenes = scenes;
            }
        }


        public class InteractableTemplate
        {
            public GameObject interactablePrefab;
            public Category interactableCategory;
            public int selectionWeight;
            public bool sendOverNetwork;
            public HullClassification hullSize;
            public MapNodeGroup.GraphType nodeGraphType;
            public int directorCreditCost;
            public bool occupyPosition;
            public SpawnCard.EliteRules eliteRules;
            public bool orientToFloor;
            public bool slightlyRandomizeOrientation;
            public bool skipSpawnWhenSacrificeArtifactEnabled;
            public NodeFlags requiredFlags;
            public NodeFlags forbiddenFlags;
            public DirectorCore.MonsterSpawnDistance spawnDistance;
            public bool allowAmbushSpawn;
            public bool preventOverhead;
            public int minimumStageCompletions;
            public UnlockableDef requiredUnlockableDef;
            public UnlockableDef forbiddenUnlockableDef;
            

            public InteractableTemplate()
            {
                this.selectionWeight = 3;
                this.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;
                this.allowAmbushSpawn = true;
                this.preventOverhead = false;
                this.minimumStageCompletions = 0;
                this.sendOverNetwork = true;
                this.hullSize = HullClassification.Golem;
                this.nodeGraphType = MapNodeGroup.GraphType.Ground;
                this.requiredFlags = NodeFlags.None;
                this.forbiddenFlags = NodeFlags.None;
                this.directorCreditCost = 15;
                this.occupyPosition = true;
                this.eliteRules = SpawnCard.EliteRules.Default;
                this.orientToFloor = true;
                this.slightlyRandomizeOrientation = false;
                this.skipSpawnWhenSacrificeArtifactEnabled = false;
            }

            public InteractableTemplate(GameObject interactablePrefab, Category interactableCategory, int selectionWeight = 3, bool sendOverNetwork = true, HullClassification hullSize = HullClassification.Golem, MapNodeGroup.GraphType nodeGraphType = MapNodeGroup.GraphType.Ground, int directorCreditCost = 15, bool occupyPosition = true, SpawnCard.EliteRules eliteRules = SpawnCard.EliteRules.Default, bool orientToFloor = true, bool slightlyRandomizeOrientation = false, bool skipSpawnWhenSacrificeArtifactEnabled = false, NodeFlags requiredFlags = NodeFlags.None, NodeFlags forbiddenFlags = NodeFlags.None, DirectorCore.MonsterSpawnDistance spawnDistance = DirectorCore.MonsterSpawnDistance.Standard, bool allowAmbushSpawn = true, bool preventOverhead = false, int minimumStageCompletions = 0, UnlockableDef requiredUnlockableDef = null, UnlockableDef forbiddenUnlockableDef = null)
            {
                this.interactablePrefab = interactablePrefab;
                this.interactableCategory = interactableCategory;
                this.selectionWeight = selectionWeight;
                this.sendOverNetwork = sendOverNetwork;
                this.hullSize = hullSize;
                this.nodeGraphType = nodeGraphType;
                this.directorCreditCost = directorCreditCost;
                this.occupyPosition = occupyPosition;
                this.eliteRules = eliteRules;
                this.orientToFloor = orientToFloor;
                this.slightlyRandomizeOrientation = slightlyRandomizeOrientation;
                this.skipSpawnWhenSacrificeArtifactEnabled = skipSpawnWhenSacrificeArtifactEnabled;
                this.requiredFlags = requiredFlags;
                this.forbiddenFlags = forbiddenFlags;
                this.spawnDistance = spawnDistance;
                this.allowAmbushSpawn = allowAmbushSpawn;
                this.preventOverhead = preventOverhead;
                this.minimumStageCompletions = minimumStageCompletions;
                this.requiredUnlockableDef = requiredUnlockableDef;
                this.forbiddenUnlockableDef = forbiddenUnlockableDef;
            }
        }
    }
}
