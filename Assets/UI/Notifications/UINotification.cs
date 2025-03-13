using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class UINotification : MonoBehaviour
    {
        [System.Serializable]
        public class NotificationTypeColorEntry
        {
            public NotificationType type;
            public Color color;
        }

        // To be set on the prefab
        public List<NotificationTypeColorEntry> notificationTypeColors;

        Image backgroundImage;
        TextMeshProUGUI titleText;
        TextMeshProUGUI messageText;

        Notification notification;

        void Awake()
        {
            backgroundImage = GetComponent<Image>();
            titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
            messageText = transform.Find("Message").GetComponent<TextMeshProUGUI>();
        }

        public void SetNotification(Notification notification)
        {
            this.notification = notification;
            UpdateText();
            SetColorFromType();
        }

        void UpdateText()
        {
            Assert.IsNotNull(notification);
            titleText.text = notification.title;
            messageText.text = notification.message;
        }

        void SetColorFromType()
        {
            var entry = notificationTypeColors.Find((other) => other.type == notification.type);

            if (entry != null)
            {
                backgroundImage.color = entry.color;
            }
        }
    }
}
