using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerBuilder
{
    public class NotificationsPanel : MonoBehaviour
    {
        public GameObject uiNotificationPrefab;
        List<UINotification> uiNotifications = new();

        void Start()
        {
            WorldController.Get().notificationsController.onNotificationAdded += OnNotificationAdded;
        }

        void OnNotificationAdded(Notification notification)
        {
            var uiNotification = CreateNotification(notification);
            uiNotifications.Add(uiNotification);
        }

        UINotification CreateNotification(Notification notification)
        {
            var uiNotificationGameObject = Instantiate(uiNotificationPrefab, Vector3.zero, Quaternion.identity, transform);
            // Insert at the top 
            uiNotificationGameObject.transform.SetSiblingIndex(0);
            var uiNotification = uiNotificationGameObject.GetComponent<UINotification>();
            uiNotification.SetNotification(notification);
            return uiNotification;
        }
    }
}