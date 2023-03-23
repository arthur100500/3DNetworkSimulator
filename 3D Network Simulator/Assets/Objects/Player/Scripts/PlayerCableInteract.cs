using System;
using UnityEngine;

public class PlayerCableInteract : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firstTarget;
    public GameObject inHandTarget;
    public GameObject lastWireJoint;
    public LayerMask PortLayer;
    public GameObject floor;
    public GameObject nCamera;
    public Vector3 handStride;

    public float MaxDist = 1;
    bool isActive = false;
    System.Random random = new();
    WireRenderer wireRenderer;

    public void SetActive(GameObject first)
    {
        IWire wire = first.GetComponent<IWire>();

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

            wire.ConnectedWire.Disconnect(wire);
            wire.Disconnect(wire.ConnectedWire);

            gennedPlug.transform.parent = inHandTarget.transform;
            gennedPlug.transform.localPosition = handStride;

            CreateWireRenderer(wire);
            isActive = true;
        }
    }

    void Update()
    {
        CheckInteract();

        if (!isActive)
            return;

        // update item in hand position
        inHandTarget.transform.position = nCamera.transform.position + (nCamera.transform.forward + (nCamera.transform.rotation * handStride));

        Debug.DrawLine(nCamera.transform.position, nCamera.transform.position + nCamera.transform.forward, Color.cyan);
        Debug.DrawLine(nCamera.transform.position, nCamera.transform.position + nCamera.transform.forward, Color.red);

        inHandTarget.transform.rotation = nCamera.transform.rotation;
    }

    void CheckInteract()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(1) && isActive)
            Discard();

        if (!Input.GetMouseButtonDown(0))
            return;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, PortLayer) && hit.collider != null)
        {
            var RaycastReturn = hit.collider.gameObject.name;
            var FoundObject = GameObject.Find(RaycastReturn);

            if (!isActive)
                SetActive(FoundObject);
            else
                TryConnect(FoundObject);
        }
    }

    void CreateWireRenderer(IWire wire)
    {
        GameObject wireHldr = new()
        {
            name = "Wire"
        };
        wireRenderer = wireHldr.AddComponent<WireRenderer>();
        wireRenderer.width = 0.01f;
        wireRenderer.p1 = firstTarget.transform;
        wireRenderer.p2 = inHandTarget.transform.GetChild(0).transform;
        wireRenderer.floor = floor.transform;
        wireRenderer.sagging = 3;
        wireRenderer.GetComponent<Renderer>().material = wire.GetWireMaterial();
    }

    void TryConnect(GameObject target)
    {
        IWire expected = target.GetComponent<IWire>();
        IWire provided = firstTarget.GetComponent<IWire>();

        if (expected.GetInputType() == provided.GetOutputType()
            && expected.IsAvailable() && provided.IsAvailable())
        {
            wireRenderer.p2 = target.transform;
            expected.wireRenderer = wireRenderer.gameObject;
            provided.wireRenderer = wireRenderer.gameObject;
            expected.Connect(provided);
            provided.Connect(expected);
            Destroy(inHandTarget);
            isActive = false;
            expected.VisualConnect();
        }
    }

    void Discard()
    {
        firstTarget.GetComponent<IWire>().VisualDisconnect();
        Destroy(inHandTarget);
        Destroy(wireRenderer.gameObject);
        isActive = false;
    }
}
