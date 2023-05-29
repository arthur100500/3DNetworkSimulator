using TMPro;
using UnityEngine;

namespace UI.Console
{
    public class NotificationText : ANotification
    {
        public override void Configure(NotificationConsole self)
        {
            Destroy(gameObject, 100);

            // Gen TextMeshPro text
            gameObject.AddComponent<CanvasRenderer>();
            var tmpComponent = gameObject.AddComponent<TextMeshProUGUI>();
            tmpComponent.text = message;
            tmpComponent.fontSize = 16;
            tmpComponent.font = self.messageMaterial;
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1000, 30);
            rectTransform.position = new Vector3(510, 20, 0);
        }

        public override void SetText(string text)
        {
            if (text is null) return;

            var tmpComponent = gameObject.GetComponent<TextMeshProUGUI>();
            tmpComponent.text = text;
        }

        public override void DestroyAfterDelay(float delay)
        {
            Destroy(gameObject, delay);
        }

        public override void ShiftUp(int amount)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            var position = rectTransform.position;
            position = new Vector3(position.x, position.y + amount,
                position.z);
            rectTransform.position = position;
        }
    }
}