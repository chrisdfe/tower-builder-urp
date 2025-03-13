
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class UINotification : MonoBehaviour
    {
        TextMeshProUGUI titleText;
        TextMeshProUGUI messageText;

        Notification notification;

        void Awake()
        {
            titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
            messageText = transform.Find("Message").GetComponent<TextMeshProUGUI>();
        }

        public void SetNotification(Notification notification)
        {
            this.notification = notification;
            UpdateText();
        }

        void UpdateText()
        {
            Assert.IsNotNull(notification);
            titleText.text = notification.title;
            messageText.text = notification.message;
        }
    }
}
