using UnityEngine;

namespace UI.DeviceSelect.Scripts
{
    public class PlaceableDevice : MonoBehaviour, IPlaceableDevice
    {
        public Vector3 actualOffset;
        public Vector3 previewOffset;
        public GameObject prefab;
        public GameObject previewPrefab;
            
        public Vector3 ActualOffset => actualOffset;
        public Vector3 PreviewOffset => previewOffset;

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