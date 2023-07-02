using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Gameplay.MainMenu.InterScene;
using GNS3.ProjectHandling.Project;
using Logger;
using Menu.Json;
using Menu.ProjectSelect;
using Newtonsoft.Json;
using Tasks.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.MainMenu.ProjectSelect
{
    public class ProjectSelector : MonoBehaviour
    {
        [SerializeField] private Transform layoutGroup;
        [SerializeField] private GameObject projectPrefab;
        [SerializeField] private Button newProjectButton;
        private const string NewRequestUrl = "http://127.0.0.1:10203/ns/new";
        private const string RequestUrlList = "http://127.0.0.1:10203/ns/projects";
        
        private string _authCookie;
        private List<GameObject> _projectEntries;
        

        public void Init(string cookie)
        {
            _authCookie = cookie;
            _projectEntries = new List<GameObject>();
            newProjectButton.onClick.AddListener(CreateNewProject);
            
            ReloadProjects();
        }

        private void ReloadProjects()
        {
            _projectEntries.ForEach(Destroy);
            _projectEntries.Clear();
            
            
            var request = new UnityWebRequest(RequestUrlList);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbGET;
            request.SetRequestHeader("Cookie", _authCookie);
            
            StartCoroutine(SendRequest(request, ReloadProjectsEvent));
        }
        
        private void ReloadProjectsEvent(UnityWebRequest request)
        {
            var nsJProjectListJson = request.downloadHandler.text;
            var nsJProjectList = JsonConvert.DeserializeObject<List<NsJProject>>(nsJProjectListJson);

            foreach (var projectJson in nsJProjectList)
            {
                var go = Instantiate(projectPrefab, layoutGroup);
                var projectEntryComponent = go.GetComponent<ProjectEntry>();
                projectEntryComponent.SetText(projectJson.Name);
                projectEntryComponent.AddOnClickListener(() => LoadProject(projectJson));
                
                _projectEntries.Add(go);
            }
        }
        
        private void CreateNewProject()
        {
            var request = new UnityWebRequest(NewRequestUrl);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Cookie", _authCookie);
            
            StartCoroutine(SendRequest(request, FinishCreating));
        }

        private void FinishCreating(UnityWebRequest request)
        {
            ReloadProjects();
        }
        
        IEnumerator SendRequest(UnityWebRequest request, Action<UnityWebRequest> callback)
        {
            yield return request.SendWebRequest();

            callback(request);
        }


        private static void LoadProject(NsJProject proj)
        {
            MenuToGameExchanger.UseLocalGns = false;
            MenuToGameExchanger.ProjectConfig = GnsProjectConfig.ProxyGnsProjectConfig();
            MenuToGameExchanger.InitialProject = proj;
            MenuToGameExchanger.RequestMaker = new WebRequestMaker(new VoidLogger());
            
            SceneManager.LoadScene("GameScene");
        }
    }
}
