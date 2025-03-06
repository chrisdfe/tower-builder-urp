using UnityEngine;

namespace TowerBuilder
{
    public class OccupantBeingInHotelRoomTask : OccupantTaskBase
    {
        public override string name
        {
            get
            {
                if (currentSubTask is OccupantTravelingToDestinationTask)
                {
                    return "Traveling to hotel room";
                }

                return "Hanging out in hotel room";
            }
        }

        public OccupantBeingInHotelRoomTask(Occupant occupant) : base(occupant) { }

        protected override OccupantTaskBase GetNextSubTask()
        {
            if (occupant.currentRoom == occupant.hotelRoom)
            {
                // Occupant has arrived in hotel room
                if (occupant.currentRoom.behavior is HotelRoomBehavior)
                {
                    (occupant.currentRoom.behavior as HotelRoomBehavior).AddGuest();
                }

                // Wander about the hotel room
                return null;
            }

            // Travel to hotel room
            var destinationTile = occupant.hotelRoom.GetRandomTile();
            return new OccupantTravelingToDestinationTask(occupant, destinationTile, "Traveling to their hotel room");
        }
    }
}