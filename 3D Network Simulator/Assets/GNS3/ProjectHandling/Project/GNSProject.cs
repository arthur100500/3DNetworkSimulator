using System.Text;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using GNSJsonObject;

namespace GNSHandling
{
    public class GNSProject : IDisposable
    {
        public GNSJProject JProject;
        public string Name { get; private set; }
        private GNSProjectConfig config;
        private HttpClient httpClient;
        private string addrBegin;

        public GNSProject(GNSProjectConfig config, string name)
        {
            Name = name;
            // Establish constants
            httpClient = new();
            this.config = config;
            addrBegin = "http://" + config.Address + ":" + config.Port + "/v2/";

            // Create project
            /*
            curl -X POST "http://localhost:3080/v2/projects" -d '{"name": "test"}'
            {
                "name": "test",
                "project_id": "b8c070f7-f34c-4b7b-ba6f-be3d26ed073f",
            }
            */
            var res = MakeRequest("projects -d '{name: " + name + "}'");
            JProject = JsonConvert.DeserializeObject<GNSJProject>(res);
        }

        public string GetGNSVersion()
        {
            return MakeRequest("version");
        }

        public string MakeRequest(string endpoint)
        {
            using var request = new HttpRequestMessage(new HttpMethod("GET"), addrBegin + endpoint);
            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(config.User + ":" + config.Password));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

            var response = httpClient.SendAsync(request);
            return response.Result.Content.ToString();
        }

        public void Dispose()
        {

        }
    }
}