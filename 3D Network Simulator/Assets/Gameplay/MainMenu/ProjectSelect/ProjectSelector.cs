using System.Collections.Generic;
using Gameplay.MainMenu.InterScene;
using GNS3.ProjectHandling.Project;
using Logger;
using Menu.Json;
using Menu.ProjectSelect;
using Tasks.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.MainMenu.ProjectSelect
{
    public class ProjectSelector : MonoBehaviour
    {
        [SerializeField] private Transform layoutGroup;
        [SerializeField] private GameObject projectPrefab;

        public void Init(List<NsJProject> projectJsons)
        {
            foreach (var projectJson in projectJsons)
            {
                var go = Instantiate(projectPrefab, layoutGroup);
                var projectEntryComponent = go.GetComponent<ProjectEntry>();
                projectEntryComponent.SetText(projectJson.Name);
                projectEntryComponent.AddOnClickListener(() => LoadProject(projectJson));
            }
        }

        private static void LoadProject(NsJProject proj)
        {
            MenuToGameExchanger.ProjectConfig = GnsProjectConfig.ProxyGnsProjectConfig();
            MenuToGameExchanger.InitialProject = proj;
            MenuToGameExchanger.RequestMaker = new WebRequestMaker(new VoidLogger());
            
            SceneManager.LoadScene("GameScene");
        }
    }
}
