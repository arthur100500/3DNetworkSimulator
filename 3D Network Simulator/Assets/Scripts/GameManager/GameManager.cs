using System;
using GNS3.ProjectHandling.Project;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            GlobalGnsParameters.Cleanup();
        }
    }
}
