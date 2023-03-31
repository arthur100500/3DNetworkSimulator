using System.Text;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using GNSJsonObject;
using UnityEngine;
using System.Collections.Generic;

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

            var allProjects = GetAllProjects();
            var existingProject = allProjects.Find(p => p.name == name);

            var res = "";

            if (existingProject is null)
                res = MakePostRequest("projects", "{\"name\": \"" + name + "\"}");
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
            using var request = new HttpRequestMessage(new HttpMethod("GET"), addrBegin + endpoint);
            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(config.User + ":" + config.Password));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

            var response = httpClient.SendAsync(request);
            var toString = response.Result.Content.ReadAsStringAsync().Result;
            Debug.Log("GET  " + addrBegin + endpoint + ": \n" + toString);
            return toString;
        }

        public string MakePostRequest(string endpoint, string data)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), addrBegin + endpoint);
            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(config.User + ":" + config.Password));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

            var response = httpClient.SendAsync(request);
            var toString = response.Result.Content.ReadAsStringAsync().Result;
            Debug.Log("POST " + addrBegin + endpoint + " -d " + data + ": \n" + toString);
            return toString;
        }

        public string MakeProjectGetRequest(string endpoint)
        {
            return MakeGetRequest("projects/" + JProject.project_id + "/" + endpoint);
        }

        public string MakeProjectPostRequest(string endpoint, string data)
        {
            return MakePostRequest("projects/" + JProject.project_id + "/" + endpoint, data);
        }

        private List<GNSJProject> GetAllProjects()
        {
            var res = MakeGetRequest("projects");
            return JsonConvert.DeserializeObject<List<GNSJProject>>(res);
        }

        public void Dispose()
        {

        }
    }
}