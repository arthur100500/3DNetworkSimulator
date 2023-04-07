using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GNSJsonObject;
using Newtonsoft.Json;
using UnityEngine;

namespace GNSHandling
{
    public class GNSProject : IDisposable
    {
        private readonly string addrBegin;
        private readonly HttpClient httpClient;
        public GNSProjectConfig Config;
        public GNSJProject JProject;

        public GNSProject(GNSProjectConfig config, string name)
        {
            Name = name;
            // Establish constants
            httpClient = new HttpClient();

            Config = config;
            addrBegin = "http://" + config.Address + ":" + config.Port + "/v2/";

            var notification = "Creating project " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotification(InnerProjectCreate, notification, 4);
        }

        public string Name { get; }
        public string ID => JProject.project_id;

        public void Dispose()
        {
            MakeProjectDeleteRequest("");
        }

        public void InnerProjectCreate()
        {
            var msgID = Guid.NewGuid();

            var allProjects = GetAllProjects();
            var existingProject = allProjects.Find(p => p.name == Name);

            var res = "";

            if (existingProject is null)
                res = MakePostRequest("projects", "{\"name\": \"" + Name + "\"}");
            else
                res = MakePostRequest("projects/" + existingProject.project_id + "/open", "{}");

            JProject = JsonConvert.DeserializeObject<GNSJProject>(res);
        }

        public string GetGNSVersion()
        {
            return MakeGetRequest("version");
        }

        public string MakeGetRequest(string endpoint)
        {
            return MakeCustomRequest(endpoint, "GET");
        }

        public string MakeDeleteRequest(string endpoint)
        {
            return MakeCustomRequest(endpoint, "DELETE");
        }

        private string MakePostRequest(string endpoint, string type)
        {
            var task = MakePostRequestAsync(endpoint, type);
            return task.GetAwaiter().GetResult();
        }

        private string MakeCustomRequest(string endpoint, string type)
        {
            var task = MakeCustomRequestAsync(endpoint, type);
            return task.GetAwaiter().GetResult();
        }

        private async Task<string> MakeCustomRequestAsync(string endpoint, string type)
        {
            using var request = new HttpRequestMessage(new HttpMethod(type), addrBegin + endpoint);
            var base64authorization =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.User + ":" + Config.Password));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var toString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Debug.Log(type + "  " + addrBegin + endpoint + ": \n" + toString);
            return toString;
        }

        public async Task<string> MakePostRequestAsync(string endpoint, string data)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), addrBegin + endpoint);
            var base64authorization =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.User + ":" + Config.Password));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var toString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if ((int)response.StatusCode >= 200 || (int)response.StatusCode < 300)
            {
                Debug.Log("POST " + addrBegin + endpoint + " -d " + data + ": \n" + toString);
                return toString;
            }

            throw new BadResponseException();
        }

        public string MakeProjectGetRequest(string endpoint)
        {
            return MakeGetRequest("projects/" + JProject.project_id + "/" + endpoint);
        }

        public string MakeProjectPostRequest(string endpoint, string data)
        {
            return MakePostRequest("projects/" + JProject.project_id + "/" + endpoint, data);
        }

        public string MakeProjectDeleteRequest(string endpoint)
        {
            return MakeDeleteRequest("projects/" + JProject.project_id + "/" + endpoint);
        }

        private List<GNSJProject> GetAllProjects()
        {
            var res = MakeGetRequest("projects");
            return JsonConvert.DeserializeObject<List<GNSJProject>>(res);
        }
    }
}