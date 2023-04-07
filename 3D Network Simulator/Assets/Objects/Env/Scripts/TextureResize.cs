using UnityEngine;

[ExecuteInEditMode]
public class TextureResize : MonoBehaviour
{
    public float scaleFactor = 5.0f;
    private Material mat;

    private void Start()
    {
        GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x / scaleFactor,
            transform.localScale.y / scaleFactor);
    }

    private void Update()
    {
        if (transform.hasChanged && Application.isEditor && !Application.isPlaying)
        {
            GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x / scaleFactor,
                transform.localScale.y / scaleFactor);
            transform.hasChanged = false;
        }
    }
}