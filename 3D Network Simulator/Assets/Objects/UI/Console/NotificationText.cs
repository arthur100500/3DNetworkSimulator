using TMPro;
using UnityEngine;

namespace UI.NotificationConsole
{
    public class NotificationText : ANotification
    {
        public override void Configure(NotificationConsole self)
        {
            Destroy(gameObject, 100);

            // Gen TextMeshPro text
            gameObject.AddComponent<CanvasRenderer>();
            var tmproComponent = gameObject.AddComponent<TextMeshProUGUI>();
            tmproComponent.text = Text;
            tmproComponent.fontSize = 16;
            tmproComponent.font = self.MessageMaterial;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1000, 30);
            rectTransform.position = new Vector3(510, 20, 0);
        }

        public override void SetText(string text)
        {
            var tmproComponent = gameObject.GetComponent<TextMeshProUGUI>();
            tmproComponent.text = text;
        }

        public override void DestroyAfterDelay(float delay)
        {
            Destroy(gameObject, delay);
        }

        public override void ShiftUp(int amnt)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y + amnt,
                rectTransform.position.z);
        }
    }
}