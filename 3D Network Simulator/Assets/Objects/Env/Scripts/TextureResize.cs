using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextureResize : MonoBehaviour
{
    public float scaleFactor = 5.0f;
    Material mat;
    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x / scaleFactor, transform.localScale.y / scaleFactor);
    }

    void Update()
    {
        if (transform.hasChanged && Application.isEditor && !Application.isPlaying)
        {
            GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x / scaleFactor, transform.localScale.y / scaleFactor);
            transform.hasChanged = false;
        }
    }
}