using System.Collections.Generic;

namespace TowerBuilder
{
    public class WorldEvents
    {
        List<EventListener> listeners;

        public WorldEvents()
        {
            listeners = new List<EventListener>();
        }

        public void AddListener(EventListener listener)
        {
            listeners.Add(listener);
        }

        public void RemoveListener(EventListener listener)
        {
            listeners.Remove(listener);
        }

        public class OnTickPayload
        {
            public uint tick;
        }

        public void Fire(OnTickPayload payload)
        {
            foreach (var listener in listeners)
                listener.On(payload);
        }

        public class OnRoomAddedPayload
        {
            public Tile tile;
        }

        public void Fire(OnRoomAddedPayload payload)
        {
            foreach (var listener in listeners)
                listener.On(payload);
        }

        public class OnTileAddedPayload
        {
            // TODO
            public string message;
        }


        public void Fire(OnTileAddedPayload payload)
        {
            foreach (var listener in listeners)
                listener.On(payload);
        }


        public abstract class EventListener
        {
            public virtual void On(OnTickPayload payload) { }
            public virtual void On(OnRoomAddedPayload payload) { }
            public virtual void On(OnTileAddedPayload payload) { }
        }
    }
}