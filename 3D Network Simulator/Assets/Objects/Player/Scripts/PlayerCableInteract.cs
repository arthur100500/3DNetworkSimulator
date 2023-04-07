using UnityEngine;
using Wire;

namespace Player
{
    public class PlayerCableInteract : MonoBehaviour
    {
        [SerializeField] private LayerMask PortLayer;
        [SerializeField] private GameObject floor;
        [SerializeField] private GameObject nCamera;

        [SerializeField] private Vector3 handStride;

        // Start is called before the first frame update
        private GameObject firstTarget;
        private GameObject inHandTarget;
        private bool isActive;
        private WireRenderer wireRenderer;

        public void Update()
        {
            CheckInteract();

            if (!isActive)
                return;

            // update item in hand position
            inHandTarget.transform.position = nCamera.transform.position +
                                              (nCamera.transform.forward + nCamera.transform.rotation * handStride);
            inHandTarget.transform.rotation = nCamera.transform.rotation;
        }

        private void SetActive(GameObject first)
        {
            var wire = first.GetComponent<AWire>();

            // If wire is available connect to it and give opposite end
            if (wire.IsAvailable())
            {
                firstTarget = wire.GetSelf();
                inHandTarget = new GameObject();
                var gennedPlug = wire.GetHandObject();
                gennedPlug.transform.parent = inHandTarget.transform;
                gennedPlug.transform.localPosition = handStride;

                // Add wire renderer
                CreateWireRenderer(wire);
                wire.VisualConnect();

                isActive = true;
            }

            // If wire is not available use its connected end as a primary
            else
            {
                wire.VisualDisconnect();
                firstTarget = wire.ConnectedWire.GetSelf();
                inHandTarget = new GameObject();
                var gennedPlug = wire.ConnectedWire.GetHandObject();

                wire.SingleDisconnect(wire.ConnectedWire);

                wire.ConnectedWire.Disconnect(wire);
                wire.Disconnect(wire.ConnectedWire);


                gennedPlug.transform.parent = inHandTarget.transform;
                gennedPlug.transform.localPosition = handStride;

                CreateWireRenderer(wire);
                isActive = true;
            }
        }

        private void CheckInteract()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(1) && isActive)
                Discard();

            if (!Input.GetMouseButtonDown(0))
                return;

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, PortLayer) && hit.collider != null)
            {
                var FoundObject = hit.collider.gameObject;

                if (!isActive)
                    SetActive(FoundObject);
                else
                    TryConnect(FoundObject);
            }
        }

        private void CreateWireRenderer(AWire wire)
        {
            GameObject wireHldr = new()
            {
                name = "Wire"
            };
            wireRenderer = wireHldr.AddComponent<WireRenderer>();
            wireRenderer.width = 0.006f;
            wireRenderer.p1 = firstTarget.transform;
            wireRenderer.p2 = inHandTarget.transform.GetChild(0).transform;
            wireRenderer.floor = floor.transform;
            wireRenderer.sagging = 3;
            wireRenderer.GetComponent<Renderer>().material = wire.GetWireMaterial();
        }

        private void TryConnect(GameObject target)
        {
            var expected = target.GetComponent<AWire>();
            var provided = firstTarget.GetComponent<AWire>();

            if (expected.GetInputType() == provided.GetOutputType()
                && expected.IsAvailable() && provided.IsAvailable()
                && expected != provided)
            {
                wireRenderer.p2 = target.transform;
                expected.wireRenderer = wireRenderer.gameObject;
                provided.wireRenderer = wireRenderer.gameObject;
                // Both ends get an event
                expected.Connect(provided);
                provided.Connect(expected);
                // Only 1 side is getting an event
                expected.SingleConnect(provided);
                Destroy(inHandTarget);
                isActive = false;
                expected.VisualConnect();
            }
        }

        private void Discard()
        {
            firstTarget.GetComponent<AWire>().VisualDisconnect();
            Destroy(inHandTarget);
            Destroy(wireRenderer.gameObject);
            isActive = false;
        }
    }
}