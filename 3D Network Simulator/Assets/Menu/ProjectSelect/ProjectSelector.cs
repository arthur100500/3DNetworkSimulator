using System.Collections.Generic;
using Menu.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.ProjectSelect
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

        private void LoadProject(NsJProject proj)
        {
            Debug.Log(proj.Name);
        }
    }
}
