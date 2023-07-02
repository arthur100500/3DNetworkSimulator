namespace GNS.ServerCommunication
{
    public class GnsUrl
    {
        private string _url;
        
        /// <summary>
        /// Gets the URL string without the last slash
        /// </summary>
        public string Url => _url.Trim('/');

        public GnsUrl(string addrBegin)
        {
            _url = addrBegin;
        }

        public GnsUrl Projects()
        {
            _url += "projects/";
            return this;
        }
        
        public GnsUrl Nodes()
        {
            _url += "nodes/";
            return this;
        }
        
        public GnsUrl With(string part)
        {
            _url += $"{part}/";
            return this;
        }

        public GnsUrl Project(string projectId)
        {
            _url += $"projects/{projectId}/";
            return this;
        }
        
        public GnsUrl Node(string nodeId)
        {
            _url += $"projects/{nodeId}/";
            return this;
        }
        
        public GnsUrl Links()
        {
            _url += "links/";
            return this;
        }
        
        public GnsUrl Link(string linkId)
        {
            _url += $"links/{linkId}/";
            return this;
        }

        public GnsUrl Start()
        {
            _url += "start/";
            return this;
        }
        
        public GnsUrl Stop()
        {
            _url += "stop/";
            return this;
        }
    }
}