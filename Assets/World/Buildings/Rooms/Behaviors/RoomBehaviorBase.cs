using System.Collections.Generic;

namespace TowerBuilder
{
    public abstract class RoomBehaviorBase
    {
        protected Room room;
        protected WorldController worldController;

        public RoomBehaviorBase(Room room)
        {
            this.room = room;
            worldController = WorldController.Get();
        }

        public virtual void OnTick() { }

        // Note - this is an instance method only because it's simpler to implement this way
        //        this is called once per tick per unique type of RoomBehaviorBase in a Building
        public virtual void OnTickAll(List<Room> rooms) { }
    }
}