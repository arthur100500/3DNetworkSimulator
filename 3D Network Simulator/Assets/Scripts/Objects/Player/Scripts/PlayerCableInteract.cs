using Objects.Parts.Wire;
using UnityEngine;
using Wire;

namespace Objects.Player.Scripts
{
    public class PlayerCableInteract : MonoBehaviour
    {
        [SerializeField] private LayerMask portLayer;
        [SerializeField] private GameObject floor;
        [SerializeField] private GameObject nCamera;

        [SerializeField] private Vector3 handStride;

        // Start is called before the first frame update
        private GameObject _firstTarget;
        private GameObject _inHandTarget;
        private bool _isActive;
        private WireRenderer _wireRenderer;

        public void Update()
        {
            CheckInteract();

            if (!_isActive)
                return;

            var rotation = nCamera.transform.rotation;
            _inHandTarget.transform.position = nCamera.transform.position +
                                               (nCamera.transform.forward + rotation * handStride);
            _inHandTarget.transform.rotation = rotation;
        }

        private void SetActive(GameObject first)
        {
            var wire = first.GetComponent<AWire>();

            // If wire is available connect to it and give opposite end
            if (wire.IsAvailable())
            {
                _firstTarget = wire.GetSelf();
                _inHandTarget = new GameObject();
                var gennedPlug = wire.GetHandObject();
                gennedPlug.transform.parent = _inHandTarget.transform;
                gennedPlug.transform.localPosition = handStride;

                // Add wire renderer
                CreateWireRenderer(wire);
                wire.VisualConnect();

                _isActive = true;
            }

            // If wire is not available use its connected end as a primary
            else
            {
                wire.VisualDisconnect();
                _firstTarget = wire.connectedWire.GetSelf();
                _inHandTarget = new GameObject();
                var gennedPlug = wire.connectedWire.GetHandObject();

                wire.SingleDisconnect(wire.connectedWire);

                wire.connectedWire.Disconnect(wire);
                wire.Disconnect(wire.connectedWire);


                gennedPlug.transform.parent = _inHandTarget.transform;
                gennedPlug.transform.localPosition = handStride;

                CreateWireRenderer(wire);
                _isActive = true;
            }
        }

        private void CheckInteract()
        {
            if (Camera.main == null) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(1) && _isActive)
                Discard();

            if (!Input.GetMouseButtonDown(0))
                return;

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, portLayer) || hit.collider == null) return;
            
            var foundObject = hit.collider.gameObject;

            if (!_isActive)
                SetActive(foundObject);
            else
                TryConnect(foundObject);
        }

        private void CreateWireRenderer(AWire wire)
        {
            GameObject wireHdr = new()
            {
                name = "Wire"
            };
            _wireRenderer = wireHdr.AddComponent<WireRenderer>();
            _wireRenderer.width = 0.006f;
            _wireRenderer.p1 = _firstTarget.transform;
            _wireRenderer.p2 = _inHandTarget.transform.GetChild(0).transform;
            _wireRenderer.floor = floor.transform;
            _wireRenderer.sagging = 3;
            _wireRenderer.GetComponent<Renderer>().material = wire.GetWireMaterial();
        }

        private void TryConnect(GameObject target)
        {
            var expected = target.GetComponent<AWire>();
            var provided = _firstTarget.GetComponent<AWire>();

            if (expected.GetInputType() != provided.GetOutputType()
                || !expected.IsAvailable() || !provided.IsAvailable()
                || expected == provided) return;
            
            
            _wireRenderer.p2 = target.transform;
            var o = _wireRenderer.gameObject;
            expected.wireRenderer = o;
            provided.wireRenderer = o;
            // Both ends get an event
            expected.Connect(provided);
            provided.Connect(expected);
            // Only 1 side is getting an event
            expected.SingleConnect(provided);
            Destroy(_inHandTarget);
            _isActive = false;
            expected.VisualConnect();
        }

        private void Discard()
        {
            _firstTarget.GetComponent<AWire>().VisualDisconnect();
            Destroy(_inHandTarget);
            Destroy(_wireRenderer.gameObject);
            _isActive = false;
        }
    }
}