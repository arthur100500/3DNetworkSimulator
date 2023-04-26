using UnityEngine;

namespace UI.DeviceSelect.Scripts
{
    public class PlaceableDevice : MonoBehaviour, IPlaceableDevice
    {
        public Vector3 offset;
        public GameObject prefab;
        public GameObject previewPrefab;
            
        public Vector3 Offset => offset;

        public GameObject GetPreview()
        {
            return previewPrefab;
        }

        public GameObject GetActual()
        {
            return prefab;
        }
    }
}