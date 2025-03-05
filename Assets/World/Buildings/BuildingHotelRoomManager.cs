using UnityEngine;

namespace TowerBuilder
{
    public class BuildingHotelRoomManager
    {
        Building building;

        WorldController worldController;

        public BuildingHotelRoomManager(Building building)
        {
            this.building = building;

            worldController = WorldController.Get();
        }

        public void OnTick()
        {
            // 6pm
            // TODO - some variability
            var checkinTime = new DayTimeValue(18, 0);

            var hotelRooms = building.GetRoomsByType(RoomType.Hotel);

            if (hotelRooms.Count > 0 && worldController.timeController.timeValue.ToDayTimeValue().Matches(checkinTime))
            {
                Debug.Log("It's checkin time");

                foreach (var hotelRoom in hotelRooms)
                {
                    // TODO 
                    // create a random number of occupants between 1 - the room's capacity
                    // put them in the building's entrance
                    // give them a BeingInHotelRoom task (which should include traveling to the hotel room)
                }
            }
        }
    }
}