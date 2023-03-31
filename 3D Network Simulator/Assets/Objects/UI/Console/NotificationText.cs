using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI.NotificationConsole
{
    public class NotificationText : MonoBehaviour
    {
        public string Text;

        public void Start()
        {
            Destroy(gameObject, 10);

            // Gen TextMeshPro text
            gameObject.AddComponent<CanvasRenderer>();
            var tmproComponent = gameObject.AddComponent<TextMeshProUGUI>();
            tmproComponent.text = Text;
            tmproComponent.fontSize = 16;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1000, 30);
            rectTransform.position = new Vector3(500, 30, 0);
        }
    }
}