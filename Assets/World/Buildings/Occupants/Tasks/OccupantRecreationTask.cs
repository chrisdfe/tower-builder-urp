using UnityEngine;

namespace TowerBuilder
{
    public class OccupantRecreationTask : OccupantTaskBase
    {
        // TODO - 'traveling to {room.title}'
        public override string name
        {
            get
            {
                // if (currentSubTask is OccupantTravelingToDestinationTask)
                // {
                //     return "Traveling to Work";
                // }

                return "Hanging out";
            }
        }

        Room currentRecreationRoom;

        public OccupantRecreationTask(Occupant occupant) : base(occupant) { }

        protected override OccupantTaskBase GetNextSubTask()
        {
            var room = FindRecreationRoom();

            if (room == null)
            {
                // Nowhere to hang out 
                return null;
            }

            currentRecreationRoom = room;

            if (occupant.currentRoom == currentRecreationRoom)
            {
                // Wander about the room
                return new OccupantWanderingTask(occupant);
            }
            else
            {
                // travel to room
                var destinationTile = currentRecreationRoom.GetRandomTile();
                return new OccupantTravelingToDestinationTask(occupant, destinationTile, "recreation room");
            }
        }

        Room FindRecreationRoom()
        {
            var building = occupant.currentRoom.building;

            var recreationRooms = building.FindRoomsByType(RoomType.Recreation);

            // TODO -
            //      filter out inaccessible rooms
            //      find closest one
            // for now just go with the first one
            if (recreationRooms.Count > 0)
            {
                return recreationRooms[0];
            }

            return null;
        }
    }
}