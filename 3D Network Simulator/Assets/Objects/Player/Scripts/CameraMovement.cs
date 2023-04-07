using UnityEngine;

namespace Player
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform cameraPos;

        public void Update()
        {
            transform.position = cameraPos.position;
        }
    }
}