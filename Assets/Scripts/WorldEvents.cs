using System.Collections.Generic;

namespace TowerBuilder
{
    public static class WorldEvents
    {
        public class OnTileAddedPayload
        {
            string message;
        }

        public abstract class EventEmitter
        {
            List<EventListener> listeners;

            public void OnTileAdded(OnTileAddedPayload payload)
            {
                listeners.ForEach(listener =>
                {
                    listener.OnTileAdded(payload);
                });
            }
        }

        public abstract class EventListener
        {
            public virtual void OnTileAdded(OnTileAddedPayload payload) { }
        }
    }

}