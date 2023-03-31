using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GNSJsonObject;

namespace GNSHandling
{
    public class GNSNode
    {
        public GNSJNode JNode;
        private GNSProject project;

        public GNSNode(GNSProject project)
        {
            this.project = project;
            project.MakeRequest("");
        }
    }
}