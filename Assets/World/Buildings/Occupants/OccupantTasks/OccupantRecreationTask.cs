using UnityEngine;

namespace TowerBuilder
{
    public class OccupantRecreationTask : IOccupantTask
    {
        // TODO - 'traveling to {room.title}'
        public string name
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

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        bool isCancelled = false;

        IOccupantTask currentSubTask;

        Occupant occupant;
        Room currentRecreationRoom;

        public OccupantRecreationTask(Occupant occupant)
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
            Debug.Log("starting recreation task");
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
            var room = FindRecreationRoom();

            if (room == null)
            {
                // Nowhere to hang out - just go home instead
                occupant.TransitionToTask(new OccupantBeingAtHomeTask(occupant));
                return;
            }

            currentRecreationRoom = room;

            IOccupantTask nextSubTask;
            if (occupant.currentRoom == currentRecreationRoom)
            {
                // Wander about the room
                var wanderingTask = new OccupantWanderingTask(occupant);
                // Give a better sense of activity/busy-ness
                nextSubTask = wanderingTask;
            }
            else
            {
                // travel to room
                var destinationTile = currentRecreationRoom.GetRandomTile();
                var routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, destinationTile);
                var route = routeFinder.FindRoute();

                if (route == null)
                {
                    // TODO - a notification as well
                    Debug.LogError($"{occupant} cannot find a route to recreation room {currentRecreationRoom}");
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