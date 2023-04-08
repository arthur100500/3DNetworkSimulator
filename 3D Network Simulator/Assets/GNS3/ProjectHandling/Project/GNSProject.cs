using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.ProjectHandling.Exceptions;
using Newtonsoft.Json;
using UnityEngine;

namespace GNS3.ProjectHandling.Project
{
    public class GnsProject : IDisposable
    {
        private readonly string _addrBegin;
        private readonly HttpClient _httpClient;
        public readonly GnsProjectConfig Config;
        private GnsJProject _jProject;

        public GnsProject(GnsProjectConfig config, string name)
        {
            Name = name;
            _httpClient = new HttpClient();

            Config = config;
            _addrBegin = "http://" + config.Address + ":" + config.Port + "/v2/";

            var notification = "Creating project " + Name;
            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(InnerProjectCreate, notification, 4);
        }

        private string Name { get; }
        public string ID => _jProject.project_id;

        public void Dispose()
        {
            MakeProjectDeleteRequest("");
        }

        private void InnerProjectCreate()
        {
            var msgID = Guid.NewGuid();

            var allProjects = GetAllProjects();
            var existingProject = allProjects.Find(p => p.name == Name);

            var res = "";

            res = existingProject is null
                ? MakePostRequest("projects", "{\"name\": \"" + Name + "\"}")
                : MakePostRequest("projects/" + existingProject.project_id + "/open", "{}");

            _jProject = JsonConvert.DeserializeObject<GnsJProject>(res);
        }

        public string GetGnsVersion()
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
            using var request = new HttpRequestMessage(new HttpMethod(type), _addrBegin + endpoint);
            var base64Authorization =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.User + ":" + Config.Password));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64Authorization}");
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var toString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Debug.Log(type + "  " + _addrBegin + endpoint + ": \n" + toString);
            return toString;
        }

        public async Task<string> MakePostRequestAsync(string endpoint, string data)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), _addrBegin + endpoint);
            var base64Authorization =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(Config.User + ":" + Config.Password));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64Authorization}");

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var toString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if ((int)response.StatusCode >= 200 || (int)response.StatusCode < 300)
            {
                Debug.Log("POST " + _addrBegin + endpoint + " -d " + data + ": \n" + toString);
                return toString;
            }

            throw new BadResponseException();
        }

        public string MakeProjectGetRequest(string endpoint)
        {
            return MakeGetRequest("projects/" + _jProject.project_id + "/" + endpoint);
        }

        public string MakeProjectPostRequest(string endpoint, string data)
        {
            return MakePostRequest("projects/" + _jProject.project_id + "/" + endpoint, data);
        }

        public string MakeProjectDeleteRequest(string endpoint)
        {
            return MakeDeleteRequest("projects/" + _jProject.project_id + "/" + endpoint);
        }

        private List<GnsJProject> GetAllProjects()
        {
            var res = MakeGetRequest("projects");
            return JsonConvert.DeserializeObject<List<GnsJProject>>(res);
        }
    }
}