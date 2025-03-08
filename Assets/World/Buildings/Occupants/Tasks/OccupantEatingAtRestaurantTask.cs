namespace TowerBuilder
{
    public class OccupantEatingAtRestaurantTask : OccupantTaskBase
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

        Room restaurant;

        public OccupantEatingAtRestaurantTask(Occupant occupant) : base(occupant) { }

        protected override OccupantTaskBase GetNextSubTask()
        {
            if (restaurant != null && occupant.currentRoom == restaurant)
            {
                // Occupant has arrived in restaurant
                // TODO - register this occupant as a patron to add to overall profit

                // TODO - don't wander, just sit
                return new OccupantWanderingTask(occupant);
            }

            // Find restaurant 
            restaurant = FindNearestRestaurant();
            if (restaurant == null)
            // No restaurants find in this building
            {
                return null;
            }

            var destinationTile = restaurant.GetRandomTile();
            return new OccupantTravelingToDestinationTask(occupant, destinationTile, "Traveling to restaurant");
        }

        Room FindNearestRestaurant()
        {
            var restaurants = occupant.currentRoom.building.FindRoomsByType(RoomType.Restaurant);

            // TODO - find nearest
            //        pay attention to capactity
            //        accessible from current location
            if (restaurants.Count > 0)
            {
                return restaurants[0];
            }

            return null;
        }
    }
}