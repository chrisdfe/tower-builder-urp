using UnityEngine;

namespace TowerBuilder
{
    public class OccupantBeingInHotelRoomTask : IOccupantTask
    {
        public string name
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

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        bool isCancelled = false;

        IOccupantTask currentSubTask;

        Occupant occupant;

        public OccupantBeingInHotelRoomTask(Occupant occupant)
        {
            this.occupant = occupant;
        }

        public void OnTick()
        {
            currentSubTask.OnTick();

            if (currentSubTask.isComplete)
            {
                if (isCancelled)
                {
                    _isComplete = true;
                }
                else
                {
                    TransitionToNextSubTask();
                }
            }
        }

        public void Setup()
        {
            TransitionToNextSubTask();
        }

        public void Teardown() { }

        public void Cancel()
        {
            isCancelled = true;

            currentSubTask?.Cancel();
        }

        void TransitionToNextSubTask()
        {
            IOccupantTask nextSubTask;
            if (occupant.currentRoom == occupant.hotelRoom)
            {
                // Wander about the hotel room
                var wanderingTask = new OccupantWanderingTask(occupant);
                nextSubTask = wanderingTask;

                if (occupant.currentRoom.behavior is HotelRoomBehavior)
                {
                    (occupant.currentRoom.behavior as HotelRoomBehavior).AddGuest();
                }
            }
            else
            {
                // travel to hotel room
                var destinationTile = occupant.hotelRoom.GetRandomTile();
                var routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, destinationTile);
                var route = routeFinder.FindRoute();

                if (route == null)
                {
                    // TODO - a notification as well
                    Debug.LogError($"{occupant} cannot find a route to their hotel room");

                    // TODO here - now what? Delete the occupant?

                    Cancel();
                    return;
                }

                nextSubTask = new OccupantTravelingToDestinationTask(occupant, route);
            }

            if (currentSubTask != null)
            {
                currentSubTask.Teardown();
            }

            currentSubTask = nextSubTask;
            currentSubTask.Setup();
        }
    }
}