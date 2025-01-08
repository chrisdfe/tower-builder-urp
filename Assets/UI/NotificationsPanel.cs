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
            bodyText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            var text = "";

            // create a new list to not affect the source list
            var notifications = new List<Notification>(WorldController.Get().notifications);
            notifications.Reverse();
            foreach (var notification in notifications)
            {
                text += notification.message;
                text += "\n\n";
            }

            bodyText.text = text;
        }
    }
}