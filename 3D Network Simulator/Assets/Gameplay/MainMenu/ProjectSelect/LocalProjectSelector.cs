using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Gameplay.MainMenu.InterScene;
using GNS.JsonObjects;
using GNS3.ProjectHandling.Project;
using Logger;
using Menu.Json;
using Menu.ProjectSelect;
using Newtonsoft.Json;
using Tasks.Requests;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.MainMenu.ProjectSelect
{
    public class LocalProjectSelector : MonoBehaviour
    {
        [SerializeField] private Button newProject;
        [SerializeField] private Transform layoutGroup;
        [SerializeField] private GameObject projectPrefab;
        
        private const string Path = "./LocalGnsProjects";
        private const string RequestUrl = "http://127.0.0.1:3080/v2/projects";
        private List<GameObject> _projectEntries;
        
        public void Init()
        {
            _projectEntries = new List<GameObject>();
            
            InitSaveFolder();
            var prList = GetProjects();
            DisplayProjects(prList);
            
            newProject.onClick.AddListener(CreateNewProject);
        }

        private static void InitSaveFolder()
        {
            Directory.CreateDirectory(Path);
        }

        private void CreateNewProject()
        {
            var prCount = GetProjects().Count();
            var newProjectName = $"{{\"name\": \"Local Project {prCount + 1}\"}}";

            var request = new UnityWebRequest(RequestUrl);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(newProjectName));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            
            StartCoroutine(SendRequest(request, PostCreateNewProject));
        }

        private void PostCreateNewProject(UnityWebRequest result)
        {
            if (result.responseCode is < 200 or >= 300)
                return;

            var created = JsonConvert.DeserializeObject<GnsJProject>(result.downloadHandler.text);
            if (created is null)
                return;
            
            var prCount = GetProjects().Count();
            var newProjectName = $"Project {prCount + 1}";

            var emptyNsProject = new NsJProject
            {
                Name = newProjectName,
                GnsID = created.project_id,
                Id = prCount,
                JsonAnnotation = "[]",
                OwnerId = "local",
            };
            
            File.WriteAllText($"{Path}/{newProjectName}", JsonConvert.SerializeObject(emptyNsProject));

            DisplayProjects(GetProjects());
        }

        IEnumerator SendRequest(UnityWebRequest request, Action<UnityWebRequest> callback)
        {
            yield return request.SendWebRequest();

            callback(request);
        }
        
        private static IEnumerable<NsJProject> GetProjects()
        {
            NsJProject DeserializeSafe(string content)
            {
                try
                {
                    return JsonConvert.DeserializeObject<NsJProject>(content);
                }
                catch
                {
                    return null;
                }
            }

            var res = Directory.GetFiles(Path)
                .Select(File.ReadAllText)
                .Select(DeserializeSafe)
                .Where(deserialized => deserialized is not null)
                .ToList();
            
            return res;
        }

        private void DisplayProjects(IEnumerable<NsJProject> projects)
        {
            _projectEntries.ForEach(Destroy);
            _projectEntries.Clear();
            
            foreach (var project in projects)
            {
                var go = Instantiate(projectPrefab, layoutGroup);
                var projectEntryComponent = go.GetComponent<ProjectEntry>();
                projectEntryComponent.SetText(project.Name);
                projectEntryComponent.AddOnClickListener(() => LoadProject(project));
                
                _projectEntries.Add(go);
            }
        }
        
        private static void LoadProject(NsJProject project)
        {
            MenuToGameExchanger.UseLocalGns = true;
            MenuToGameExchanger.InitialProject = project;
            MenuToGameExchanger.RequestMaker = new WebRequestMaker(new VoidLogger());
            MenuToGameExchanger.ProjectConfig = GnsProjectConfig.LocalGnsProjectConfig();
            
            SceneManager.LoadScene("GameScene");
        }
    }
}
