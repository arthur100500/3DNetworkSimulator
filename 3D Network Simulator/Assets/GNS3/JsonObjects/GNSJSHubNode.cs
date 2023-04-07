using System.Collections.Generic;

namespace GNSJsonObject
{
    public class DataLinkTypes
    {
        public string Ethernet { get; set; }
    }

    public class Label
    {
        public int rotation { get; set; }
        public object style { get; set; }
        public string text { get; set; }
        public object x { get; set; }
        public int y { get; set; }
    }

    public class Port
    {
        public int adapter_number { get; set; }
        public DataLinkTypes data_link_types { get; set; }
        public string link_type { get; set; }
        public string name { get; set; }
        public int port_number { get; set; }
        public string short_name { get; set; }
    }

    public class PortsMapping
    {
        public string name { get; set; }
        public int port_number { get; set; }
    }

    public class Properties
    {
        public List<PortsMapping> ports_mapping { get; set; }
    }

    public class GNSJSHubNode
    {
        public object command_line { get; set; }
        public string compute_id { get; set; }
        public object console { get; set; }
        public bool console_auto_start { get; set; }
        public string console_host { get; set; }
        public object console_type { get; set; }
        public List<object> custom_adapters { get; set; }
        public object first_port_name { get; set; }
        public int height { get; set; }
        public Label label { get; set; }
        public bool locked { get; set; }
        public string name { get; set; }
        public object node_directory { get; set; }
        public string node_id { get; set; }
        public string node_type { get; set; }
        public string port_name_format { get; set; }
        public int port_segment_size { get; set; }
        public List<Port> ports { get; set; }
        public string project_id { get; set; }
        public Properties properties { get; set; }
        public string status { get; set; }
        public string symbol { get; set; }
        public object template_id { get; set; }
        public int width { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }
}