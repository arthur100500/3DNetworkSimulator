using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNSJsonObject
{
    public class GNSJVPCSNode
    {
        public string compute_id { get; set; }
        public int console { get; set; }
        public string console_host { get; set; }
        public string console_type { get; set; }
        public string name { get; set; }
        public string node_id { get; set; }
        public string node_type { get; set; }
        public string project_id { get; set; }
        public string status { get; set; }
    }
}