using System;
using System.Collections.Generic;
using GNS3.GNSConsole;
using GNS3.ProjectHandling.Link;
using GNS3.ProjectHandling.Project;
using Logger;

namespace GNS3.ProjectHandling.Node
{
    public abstract class GnsNode : IDisposable
    {
        private List<GnsJLink> _links;
        public string ID;
        public bool IsReady;

        public bool IsStarted;

        public string Name;
        protected GnsProject Project;

        public void Dispose()
        {
            Project.DeleteNode(this);
        }

        public IEventConsole GetTerminal()
        {
            var gnsWsUrl = "ws://" + Project.Config.Address + ":" + Project.Config.Port + "/v2/projects/" +
                           Project.ID + "/nodes/" + ID + "/console/ws";
            return new GnsConsole(gnsWsUrl, new DebugLogger());
        }

        protected void Init(string name, GnsProject project)
        {
            Name = name;
            Project = project;
            _links = new List<GnsJLink>();
        }

        public void Start()
        {
            Project.StartNode(this);
        }

        public void Stop()
        {
            Project.StopNode(this);
        }

        public void ConnectTo(GnsNode other, int selfAdapterID, int otherAdapterID)
        {
            var linkJson = "{\"nodes\": [{\"adapter_number\": 0, \"node_id\": \"" + ID + "\", \"port_number\": " +
                           selfAdapterID + "}, {\"adapter_number\": 0, \"node_id\": \"" + other.ID +
                           "\", \"port_number\": " + otherAdapterID + "}]}";

            void Callback(GnsJLink link)
            {
                other._links.Add(link);
                _links.Add(link);
            }

            Project.AddLink(linkJson, this, other, Callback);
        }

        public void DisconnectFrom(GnsNode other, int selfAdapterID, int otherAdapterID)
        {
            var selectedLink = _links.Find(a =>
                (a.nodes[0].node_id == ID &&
                 a.nodes[1].node_id == other.ID &&
                 a.nodes[0].port_number == selfAdapterID &&
                 a.nodes[1].port_number == otherAdapterID)
                ||
                (a.nodes[1].node_id == ID &&
                 a.nodes[0].node_id == other.ID &&
                 a.nodes[1].port_number == selfAdapterID &&
                 a.nodes[0].port_number == otherAdapterID)
            );


            Project.RemoveLink(selectedLink.link_id, this, other);
            other._links.Remove(selectedLink);
            _links.Remove(selectedLink);
        }
    }
}