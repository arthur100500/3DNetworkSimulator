using UnityEngine;

namespace Objects.Player.Scripts
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