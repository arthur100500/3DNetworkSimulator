using GNS3.JsonObjects.Basic;

namespace GNS3.JsonObjects
{
    public class GnsVpcsJNode : GnsJNode
    {
        public string compute_id { get; set; }
        public int console { get; set; }
        public string console_host { get; set; }
        public string console_type { get; set; }
        public string status { get; set; }

        public GnsVpcsJNode()
        {
        }
    }
}