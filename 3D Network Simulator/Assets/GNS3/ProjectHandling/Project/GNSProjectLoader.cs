using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNSHandling
{
    /*
    Class for containg global parameters like project, last node name id etc
    */
    static class GlobalGNSParameters
    {
        private static int nid = 1;
        private static GNSProject project;
        public static int GetNextFreeID()
        {
            return nid++;
        }
        public static GNSProject GetProject()
        {
            if (project is null)
            {
                project = new(GNSProjectConfig.LocalGNSProjectConfig(), "unity_project");
                Debug.Log("Created project");
            }
            return project;
        }
    }
}
