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
            interactableSpawnCard.prefab = interactable.spawnCard.interactablePrefab;
            interactableSpawnCard.sendOverNetwork = interactable.spawnCard.sendOverNetwork;
            interactableSpawnCard.hullSize = interactable.spawnCard.hullSize;
            interactableSpawnCard.nodeGraphType = interactable.spawnCard.nodeGraphType;
            interactableSpawnCard.requiredFlags = interactable.spawnCard.requiredFlags;
            interactableSpawnCard.forbiddenFlags = interactable.spawnCard.forbiddenFlags;
            interactableSpawnCard.directorCreditCost = interactable.spawnCard.directorCreditCost;
            interactableSpawnCard.occupyPosition = interactable.spawnCard.occupyPosition;
            interactableSpawnCard.eliteRules = interactable.spawnCard.eliteRules;
            interactableSpawnCard.orientToFloor = interactable.spawnCard.orientToFloor;
            interactableSpawnCard.slightlyRandomizeOrientation = interactable.spawnCard.slightlyRandomizeOrientation;
            interactableSpawnCard.skipSpawnWhenSacrificeArtifactEnabled = interactable.spawnCard.skipSpawnWhenSacrificeArtifactEnabled;

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

            Prefabs.Add(interactable.spawnCard.interactablePrefab);

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


        public class SpawnCardTemplate
        {
            public GameObject interactablePrefab;
            public bool sendOverNetwork;
            public HullClassification hullSize;
            public MapNodeGroup.GraphType nodeGraphType;
            public NodeFlags requiredFlags;
            public NodeFlags forbiddenFlags;
            public int directorCreditCost;
            public bool occupyPosition;
            public SpawnCard.EliteRules eliteRules;
            public bool orientToFloor;
            public bool slightlyRandomizeOrientation;
            public bool skipSpawnWhenSacrificeArtifactEnabled;

            public SpawnCardTemplate()
            {
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
        }

        public class InteractableTemplate
        {
            public SpawnCardTemplate spawnCard;
            public int selectionWeight;
            public DirectorCore.MonsterSpawnDistance spawnDistance;
            public bool allowAmbushSpawn;
            public bool preventOverhead;
            public int minimumStageCompletions;
            public UnlockableDef requiredUnlockableDef;
            public UnlockableDef forbiddenUnlockableDef;
            public category interactableCategory;

            public InteractableTemplate()
            {
                this.selectionWeight = 3;
                this.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;
                this.allowAmbushSpawn = true;
                this.preventOverhead = false;
                this.minimumStageCompletions = 0;
            }
        }

    }
}
