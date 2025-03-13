using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerBuilder
{
    public class NotificationsPanel : MonoBehaviour
    {
        public GameObject bodyTextPrefab;

        TextMeshProUGUI bodyText;

        void Awake()
        {
            bodyText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            var text = "";

            // create a new list to not affect the source list
            var notifications = new List<Notification>(WorldController.Get().notificationsController.notifications);
            notifications.Reverse();
            int idx = notifications.Count;
            foreach (var notification in notifications)
            {
                text += $"{idx--}) {notification.message}";
                text += "\n\n";
            }

            bodyText.text = text;
        }
    }
}