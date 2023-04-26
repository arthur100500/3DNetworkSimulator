using Objects.Player.Scripts;
using UI.DeviceSelect.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.DeviceSelect
{
    public class DeviceSelectorManager : MonoBehaviour
    {
        public PlaceableDevice[] devices = new PlaceableDevice[8];
        public GameObject cardPrefab;
        private bool _active = false;
        private PlayerMovement _playerMovement;
        private PlayerObjectPlaceControl _playerPlaceControl;
        [SerializeField] private Transform cardHolder;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Q)) return;
            
            // Freeze the player

            if (!_active)
                Show();
            else
                Hide();
        }
        
        private void Start()
        {
            _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            _playerPlaceControl = GameObject.FindWithTag("Player").GetComponent<PlayerObjectPlaceControl>();
            GenerateDevicePanels();
        }
        
        public void Show()
        {
            SetVisible(true);

            _playerMovement.InControl = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _active = true;
        }
        
        public void Hide()
        {
            SetVisible(false);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _playerMovement.InControl = true;

            _active = false;
        }

        private void SetVisible(bool state)
        {
            canvasGroup.interactable = state;
            canvasGroup.alpha = state ? 1 : 0;
            canvasGroup.blocksRaycasts = state;
        }

        private void GenerateDevicePanels()
        {
            foreach (var device in devices) 
            {
                var card = Instantiate(cardPrefab, cardHolder).GetComponent<DeviceCard>();
                card.Initialize(_playerPlaceControl, device, this);
                card.GetComponent<DeviceCard>();
            }
        }
    }
}
