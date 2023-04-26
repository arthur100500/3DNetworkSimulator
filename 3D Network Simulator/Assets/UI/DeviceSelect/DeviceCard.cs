using Objects.Player.Scripts;
using TMPro;
using UI.DeviceSelect.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.DeviceSelect
{
    public class DeviceCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI devName;
        [SerializeField] private PlaceableDevice device;

        private PlayerObjectPlaceControl _playerPlaceControl;
        private DeviceSelectorManager _deviceSelectorManager;

        public void Initialize(PlayerObjectPlaceControl playerPlaceControl, PlaceableDevice dev,
            DeviceSelectorManager deviceSelectorManager)
        {
            devName.text = dev.name;
            device = dev;
            _playerPlaceControl = playerPlaceControl;
            _deviceSelectorManager = deviceSelectorManager;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _playerPlaceControl.SetDevice(device);
            _deviceSelectorManager.Hide();
        }
    }
}