namespace TowerBuilder
{
    public class Notification
    {
        public NotificationType type = NotificationType.Info;
        public string title;
        public string message;

        public bool isDismissable = true;

        public Notification() { }
    }
}
