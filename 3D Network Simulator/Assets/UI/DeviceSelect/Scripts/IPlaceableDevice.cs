using UnityEngine;

namespace UI.DeviceSelect.Scripts
{
    public interface IPlaceableDevice
    {
        public Vector3 ActualOffset { get; }
        public Vector3 PreviewOffset { get; }
        
        public GameObject GetPreview();
        public GameObject GetActual();
    }
}