using Gameplay.MainMenu.InterScene;
using Menu.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.MainMenu.ProjectSelect
{
    public class LocalProjectSelector : MonoBehaviour
    {
        public void Init()
        {
            
        }

        private void GetProjects()
        {
            
        }

        private void DisplayProjects()
        {
            
        }
        
        private static void LoadProject(string projName)
        {
            var proj = new NsJProject
            {
                Name = projName
            };

            MenuToGameExchanger.UseLocalGns = true;
            MenuToGameExchanger.InitialProject = proj;
            
            SceneManager.LoadScene("GameScene");
        }
    }
}
