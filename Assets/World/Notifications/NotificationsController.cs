using System.Collections.Generic;

namespace TowerBuilder
{
    public class NotificationsController
    {
        WorldController worldController;

        public List<Notification> notifications { get; private set; } = new();

        public delegate void NotificationEvent(Notification notification);
        public NotificationEvent onNotificationAdded;

        public NotificationsController(WorldController worldController)
        {
            this.worldController = worldController;
        }

        public void AddNotification(Notification notification)
        {
            notifications.Add(notification);
            onNotificationAdded?.Invoke(notification);
        }
    }
}