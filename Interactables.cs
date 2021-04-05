using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterAPI
{
    public class Interactables
    {
        public enum category
        { 
            Chests,
            Barrels,
            Shrines,
            Drones,
            Misc,
            Rare,
            Duplicator
        }

        private static List<interactableInfo> registeredInteractables = new List<interactableInfo>();

        static Interactables()
        {
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
        }

        private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (NetworkServer.active)
            {
                ClassicStageInfo stageInfo = SceneInfo.instance.GetComponent<ClassicStageInfo>();
                foreach(interactableInfo interactable in registeredInteractables)
                {
                    stageInfo.interactableCategories.AddCard((int)interactable.category, interactable.directorCard);
                }
                
            }
            orig(self);
        }

        public static void Add(InteractableTemplate interactable)
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

            var interactableDirectorCard = new DirectorCard();

            interactableDirectorCard.spawnCard = interactableSpawnCard;
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

        }

        private class interactableInfo
        {
            public DirectorCard directorCard;
            public category category;
            public interactableInfo(DirectorCard directorCard ,category category)
            {
                this.directorCard = directorCard;
                this.category = category;
            }
        }


        public class InteractableTemplate
        {
            public GameObject interactablePrefab;
            public category interactableCategory;
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

            public InteractableTemplate(GameObject interactablePrefab, category interactableCategory, int selectionWeight = 3, bool sendOverNetwork = true, HullClassification hullSize = HullClassification.Golem, MapNodeGroup.GraphType nodeGraphType = MapNodeGroup.GraphType.Ground, int directorCreditCost = 15, bool occupyPosition = true, SpawnCard.EliteRules eliteRules = SpawnCard.EliteRules.Default, bool orientToFloor = true, bool slightlyRandomizeOrientation = false, bool skipSpawnWhenSacrificeArtifactEnabled = false, NodeFlags requiredFlags = NodeFlags.None, NodeFlags forbiddenFlags = NodeFlags.None, DirectorCore.MonsterSpawnDistance spawnDistance = DirectorCore.MonsterSpawnDistance.Standard, bool allowAmbushSpawn = true, bool preventOverhead = false, int minimumStageCompletions = 0, UnlockableDef requiredUnlockableDef = null, UnlockableDef forbiddenUnlockableDef = null)
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
