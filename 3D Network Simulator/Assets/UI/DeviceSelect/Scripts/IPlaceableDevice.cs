using UnityEngine;

namespace UI.DeviceSelect.Scripts
{
    public interface IPlaceableDevice
    {
        public Vector3 Offset { get; }
        public GameObject GetPreview();
        public GameObject GetActual();
    }
}