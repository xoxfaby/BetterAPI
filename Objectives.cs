using System;
using System.Collections.Generic;
using System.Text;
using RoR2.UI;

namespace BetterAPI
{
    public class Objectives
    {
        internal static List<ObjectiveInfo> objectives;
        internal static List<ObjectivePanelController.ObjectiveSourceDescriptor> sourceDescriptorList;
        static Objectives()
        {
            objectives = new List<ObjectiveInfo>();
            RoR2.UI.ObjectivePanelController.collectObjectiveSources += ObjectivePanelController_collectObjectiveSources;
        }

        private static void ObjectivePanelController_collectObjectiveSources(RoR2.CharacterMaster master, List<ObjectivePanelController.ObjectiveSourceDescriptor> list)
        {
            sourceDescriptorList = list;
            foreach (var objective in objectives)
            {
                if(objective.show)
                {
                    objective.sourceDescriptor = new ObjectivePanelController.ObjectiveSourceDescriptor
                    {
                        source = objective,
                        master = master,
                        objectiveType = typeof(ObjectiveTracker)
                    };
                    list.Add(objective.sourceDescriptor);
                }
            }
        }

        public static ObjectiveInfo AddObjective(string title = "", bool show = false)
        {
            ObjectiveInfo objectiveInfo = new ObjectiveInfo();
            objectives.Add(objectiveInfo);
            return objectiveInfo;
        }

        public static void RemoveObjective(ObjectiveInfo objectiveInfo)
        {
            sourceDescriptorList.Remove(objectiveInfo.sourceDescriptor);
            objectives.Remove(objectiveInfo);
        }

        public class ObjectiveInfo : UnityEngine.Object
        {
            public bool show = false;
            public bool dirty = false;
            public string baseToken;
            private string _title;
            public ObjectivePanelController.ObjectiveSourceDescriptor sourceDescriptor;
            public string title { 
                get { return _title; }
                set {
                    dirty = true;
                    _title = value;
                }
            }
            public ObjectiveInfo(string title = "", bool show = false ,string baseToken = null)
            {
                this.title = title;
                this.show = show;
                this.baseToken = baseToken;
            }

        }

        internal class ObjectiveTracker : ObjectivePanelController.ObjectiveTracker
        {
            public ObjectiveTracker()
            {
                //this.baseToken = (this.sourceDescriptor.source as ObjectiveInfo).baseToken;
            }
            public override bool IsDirty()
            {
                return (this.sourceDescriptor.source as ObjectiveInfo).dirty;
            }
            public override string GenerateString()
            {
                return (this.sourceDescriptor.source as ObjectiveInfo).title;
            }
        }
    }
}
