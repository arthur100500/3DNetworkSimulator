using Objects.Common;
using Objects.Devices.Common;
using Objects.Devices.Common.ConsoleDevice;
using UI.DeviceSelect;
using UI.DeviceSelect.Scripts;
using UnityEngine;

namespace Objects.Player.Scripts
{
    public class PlayerObjectPlaceControl : MonoBehaviour
    {
        private IPlaceableDevice _currentDevice;
        private GameObject _previewDevice;
        private Camera _camera;
        
        [SerializeField] private LayerMask placeableLayer;
        [SerializeField] private Project.Project project;

        public void SetDevice(IPlaceableDevice dev)
        {
            _currentDevice = dev;
            _previewDevice = Instantiate(_currentDevice.GetPreview());
        }
        
        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_currentDevice is null)
                return;

            if (_camera is null) return;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, placeableLayer) || hit.collider is null) return;

            var hitPosition = hit.point;
            
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject(hitPosition);
                return;
            }

            if (Input.GetMouseButtonDown(1))
                DestroyPreview();

            var angle = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
            _previewDevice.transform.position = hitPosition
                                                + angle * _currentDevice.PreviewOffset;
            _previewDevice.transform.rotation = angle;
        }

        private void PlaceObject(Vector3 position)
        {
            var angle = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);
            var actual = Instantiate(_currentDevice.GetActual());
            actual.transform.position = position + angle * _currentDevice.ActualOffset;
            actual.transform.rotation = angle;
            DestroyPreview();
            
            project.AddPlaceable(actual.GetComponent<APlaceable>());
        }

        private void DestroyPreview()
        {
            Destroy(_previewDevice);
            _previewDevice = null;
            _currentDevice = null;
        }
    }
}
