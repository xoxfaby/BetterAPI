using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace BetterAPI
{
    public static class Interactables
    {
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
        public enum Stages : Int64
        {
            None = 0,
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
            VoidFields = 4096,
            MomentWhole = 8192,
            SkyMeadow = 16384,
            BullwarksAmbry = 32768,
            Commencement = 65536,
            SunderedGrove = 131072,
            BulwarksAmbry = 262144,
            VoidLocus = 524288,
            Planetarium = 1048576,
            AphelianSanctuary = 2097152,
            SimulacrumAphelianSanctuary = 4194304,
            SimulacrumAbyssalDepths = 8388608,
            SimulacrumRallypointDelta = 16777216,
            SimulacrumTitanicPlains = 33554432,
            SimulacrumAbandonedAquaduct = 67108864,
            SimulacrumCommencement = 134217728,
            SimulacrumSkyMeadow = 268435456,
            SiphonedForest = 536870912,
            SulfurPools = 1073741824,

            //Add new values before this point
            Last,
            All = (Last << 1) - 3,
            Stage1 = TitanicPlains | DistantRoost | SiphonedForest,
            Stage2 = WetlandAspect | AbandonedAqueduct | AphelianSanctuary,
            Stage3 = RallypointDelta | ScorchedAcres | SulfurPools,
            Stage4 = AbyssalDepths | SirensCall,
            Simulacrum = SimulacrumAbandonedAquaduct | SimulacrumAbyssalDepths | SimulacrumAphelianSanctuary | SimulacrumCommencement | SimulacrumRallypointDelta | SimulacrumSkyMeadow | SimulacrumTitanicPlains,
            Default = AbandonedAqueduct | AbyssalDepths | SiphonedForest | AphelianSanctuary | SulfurPools | DistantRoost | RallypointDelta | ScorchedAcres | SirensCall | SkyMeadow | SunderedGrove | TitanicPlains | WetlandAspect
        }

        public static Dictionary<Stages, List<string>> SceneNames = new Dictionary<Stages, List<string>>()
        {
            { Stages.TitanicPlains, new List<string>(){ "golemplains", "golemplains2", "golemplains trailer" } },
            { Stages.DistantRoost, new List<string>(){ "blackbeach", "blackbeach2", "blackbeachTest" } },
            { Stages.WetlandAspect, new List<string>(){ "foggyswamp" } },
            { Stages.AbandonedAqueduct, new List<string>(){ "goolake" } },
            { Stages.RallypointDelta, new List<string>(){ "frozenwall" } },
            { Stages.ScorchedAcres, new List<string>(){ "wispgraveyard" } },
            { Stages.AbyssalDepths, new List<string>(){ "dampcavesimple" } },
            { Stages.SirensCall, new List<string>(){ "shipgraveyard" } },
            { Stages.GildedCoast, new List<string>(){ "goldshores" } },
            { Stages.MomentFractured, new List<string>(){ "mysteryspace" } },
            { Stages.Bazaar, new List<string>(){ "bazaar" } },
            { Stages.VoidFields, new List<string>(){ "arena" } },
            { Stages.MomentWhole, new List<string>(){ "limbo" } },
            { Stages.SkyMeadow, new List<string>(){ "skymeadow" } },
            { Stages.BulwarksAmbry, new List<string>(){ "artifactworld" } },
            { Stages.BullwarksAmbry, new List<string>(){ "artifactworld" } },
            { Stages.Commencement, new List<string>(){ "moon", "moon2" } },
            { Stages.SunderedGrove, new List<string>(){ "rootjungle" } },
            { Stages.VoidLocus, new List<string>(){ "voidstage" } },
            { Stages.Planetarium, new List<string>(){ "voidraid" } },
            { Stages.AphelianSanctuary, new List<string>(){ "ancientloft" } },
            { Stages.SimulacrumAbandonedAquaduct, new List<string>(){ "itgoolake" } },
            { Stages.SimulacrumAbyssalDepths, new List<string>(){ "itdampcave" } },
            { Stages.SimulacrumAphelianSanctuary, new List<string>(){ "itancientloft" } },
            { Stages.SimulacrumCommencement, new List<string>(){ "itmoon" } },
            { Stages.SimulacrumRallypointDelta, new List<string>(){ "itfrozenwall" } },
            { Stages.SimulacrumSkyMeadow, new List<string>(){ "itskymeadow" } },
            { Stages.SimulacrumTitanicPlains, new List<string>(){ "itgolemplains" } },
            { Stages.SiphonedForest, new List<string>(){ "snowyforest" } },
            { Stages.SulfurPools, new List<string>(){ "sulfurpools" } },    
        };

        private static List<InteractableInfo> registeredInteractables = new List<InteractableInfo>();

        static Interactables()
        {
            BetterAPIPlugin.Hooks.Add<RoR2.SceneDirector>("Start", SceneDirector_Start);
            BetterAPIPlugin.Hooks.Add<RoR2.SceneDirector>("PopulateScene", SceneDirector_PopulateScene);
        }

        private static void SceneDirector_PopulateScene(Action<RoR2.SceneDirector> orig, SceneDirector self)
        {
            foreach (InteractableInfo interactable in registeredInteractables)
            {
                if (interactable.minimumCount > 0 && interactable.scenes.Contains(SceneManager.GetActiveScene().name))
                {
                    for (var i = 0; i < interactable.minimumCount; i++)
                    {
                        DirectorPlacementRule placementRule = new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Random
                        };

                        GameObject gameObject = null;
                        int counter = 0;
                        while (gameObject == null)
                        {
                            counter++;
                            gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(interactable.directorCard.spawnCard, placementRule, self.rng));
                            if (counter >= 10) break;
                        }

                        if (gameObject)
                        {
                            PurchaseInteraction component = gameObject.GetComponent<PurchaseInteraction>();
                            if (component && component.costType == CostTypeIndex.Money)
                            {
                                component.Networkcost = Run.instance.GetDifficultyScaledCost(component.cost);

                            }
                        }
                    }
                }
            }
            orig(self);
        }

        private static void SceneDirector_Start(Action<RoR2.SceneDirector> orig, SceneDirector self)
        {
            if (NetworkServer.active)
            {
                ClassicStageInfo stageInfo = SceneInfo.instance.GetComponent<ClassicStageInfo>();
                if (stageInfo != null)
                {
                    foreach (InteractableInfo interactable in registeredInteractables)
                    {
                        if (interactable.multiplayerOnly && RoR2Application.isInMultiPlayer || !interactable.multiplayerOnly)
                        {
                            //Debug.Log("Trying to add " + interactable.directorCard.spawnCard.prefab.name + " to " + SceneManager.GetActiveScene().name);
                            if (interactable.scenes.Contains(SceneManager.GetActiveScene().name))
                            {
                                stageInfo.interactableCategories.AddCard((int)interactable.category, interactable.directorCard);
                            }
                        }
                    }
                }
            }
            orig(self);
        }

        private static List<string> GetSceneNames(Stages scenes)
        {
            var names = new List<string>();

            foreach (var scene in SceneNames)
            {
                if (scenes.HasFlag(scene.Key))
                {
                    foreach (var name in scene.Value)
                    {
                        names.Add(name);
                    }
                }
            }

            return names;
        }
        public static InteractableInfo AddToStages(InteractableTemplate interactable, Stages stages)
        {
            return AddToStages(interactable, stages, Assembly.GetCallingAssembly().GetName().Name);
        }
        public static InteractableInfo AddToStages(InteractableTemplate interactable, Stages stages, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            var sceneNames = GetSceneNames(stages);

            return AddToStages(interactable, sceneNames, contentPackIdentifier);
        }
        public static InteractableInfo AddToStage(InteractableTemplate interactable, string sceneName)
        {
            return AddToStage(interactable, sceneName, Assembly.GetCallingAssembly().GetName().Name);
        }
        public static InteractableInfo AddToStage(InteractableTemplate interactable, string sceneName, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            var sceneNames = new List<string>();
            sceneNames.Add(sceneName);
            return AddToStages(interactable, sceneNames, contentPackIdentifier);
        }
        public static InteractableInfo AddToStages(InteractableTemplate interactable, List<string> sceneNames)
        {
            return AddToStages(interactable, sceneNames, Assembly.GetCallingAssembly().GetName().Name);
        }
        public static InteractableInfo AddToStages(InteractableTemplate interactable, List<string> sceneNames, String contentPackIdentifier = null)
        {
            contentPackIdentifier = contentPackIdentifier ?? Assembly.GetCallingAssembly().GetName().Name;
            var spawnCard = GenerateSpawnCard(interactable);
            var interactableDirectorCard = GenerateDirectorCard(interactable, spawnCard);

            InteractableInfo info = new InteractableInfo(interactableDirectorCard, interactable.interactableCategory, sceneNames, interactable, interactable.multiplayerOnly, interactable.minimumCount);

            if (interactable.interactablePrefab.GetComponent<NetworkIdentity>())
            {
                NetworkedPrefabs.Add(interactable.interactablePrefab, contentPackIdentifier);
            }

            registeredInteractables.Add(info);

            return info;
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
            //interactableDirectorCard.allowAmbushSpawn = interactable.allowAmbushSpawn;
            interactableDirectorCard.preventOverhead = interactable.preventOverhead;
            interactableDirectorCard.minimumStageCompletions = interactable.minimumStageCompletions;
            interactableDirectorCard.requiredUnlockableDef = interactable.requiredUnlockableDef;
            interactableDirectorCard.forbiddenUnlockableDef = interactable.forbiddenUnlockableDef;

            return interactableDirectorCard;
        }


        public class InteractableInfo
        {
            public DirectorCard directorCard;
            public Category category;
            public List<string> scenes;
            public InteractableTemplate template;
            public bool multiplayerOnly;
            public int minimumCount;
            public InteractableInfo(DirectorCard directorCard, Category category, List<string> scenes, InteractableTemplate template, bool multiplayerOnly, int minimumCount)
            {
                this.directorCard = directorCard;
                this.category = category;
                this.scenes = scenes;
                this.template = template;
                this.multiplayerOnly = multiplayerOnly;
                this.minimumCount = minimumCount;
            }
        }


        public class InteractableTemplate
        {
            public GameObject interactablePrefab;
            public Category interactableCategory;
            public int selectionWeight;
            public int minimumCount;
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
            public bool multiplayerOnly;


            public InteractableTemplate()
            {
                this.minimumCount = 0;
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
                this.multiplayerOnly = false;
            }

            public InteractableTemplate(GameObject interactablePrefab, Category interactableCategory, int selectionWeight = 3, bool sendOverNetwork = true, HullClassification hullSize = HullClassification.Golem, MapNodeGroup.GraphType nodeGraphType = MapNodeGroup.GraphType.Ground, int directorCreditCost = 15, bool occupyPosition = true, SpawnCard.EliteRules eliteRules = SpawnCard.EliteRules.Default, bool orientToFloor = true, bool slightlyRandomizeOrientation = false, bool skipSpawnWhenSacrificeArtifactEnabled = false, NodeFlags requiredFlags = NodeFlags.None, NodeFlags forbiddenFlags = NodeFlags.None, DirectorCore.MonsterSpawnDistance spawnDistance = DirectorCore.MonsterSpawnDistance.Standard, bool allowAmbushSpawn = true, bool preventOverhead = false, int minimumStageCompletions = 0, UnlockableDef requiredUnlockableDef = null, UnlockableDef forbiddenUnlockableDef = null, bool multiplayerOnly = false, int minimumCount = 0)
            {
                this.interactablePrefab = interactablePrefab;
                this.interactableCategory = interactableCategory;
                this.selectionWeight = selectionWeight;
                this.minimumCount = minimumCount;
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
                this.multiplayerOnly = multiplayerOnly;
            }
        }
    }
}
