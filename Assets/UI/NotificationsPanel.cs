using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerBuilder
{
    public class NotificationsPanel : MonoBehaviour
    {
        public GameObject bodyTextPrefab;

        // Transform bodyTextWrapper;
        TextMeshProUGUI bodyText;

        void Awake()
        {
            // bodyTextWrapper = transform.Find("Scroll View").Find("Viewport").Find("Content");
            // Debug.Log(bodyTextWrapper);
            // bodyText = Instantiate(bodyTextPrefab, bodyTextWrapper).GetComponent<TextMeshProUGUI>();
            // bodyText = bodyTextWrapper.Find("Text").GetComponent<TextMeshProUGUI>();
            bodyText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            var text = "";

            // create a new list to not affect the source list
            var notifications = new List<Notification>(WorldController.Get().notifications);
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