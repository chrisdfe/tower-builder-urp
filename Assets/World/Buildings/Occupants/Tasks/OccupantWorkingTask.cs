using UnityEngine;

namespace TowerBuilder
{
    public class OccupantWorkingTask : IOccupantTask
    {
        public string name
        {
            get
            {
                if (currentSubTask is OccupantTravelingToDestinationTask)
                {
                    return "Traveling to Work";
                }

                return "Working";
            }
        }

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        bool isCancelled = false;

        IOccupantTask currentSubTask;

        Occupant occupant;

        public OccupantWorkingTask(Occupant occupant)
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

        public void Teardown()
        {
            if (occupant.currentRoom.behavior is OfficeRoomBehavior)
            {
                (occupant.currentRoom.behavior as OfficeRoomBehavior).workingOccupants.Remove(occupant);
            }
        }

        public void Cancel()
        {
            isCancelled = true;

            currentSubTask?.Cancel();
        }

        void TransitionToNextSubTask()
        {
            // We somehow ended up here erroneously if the occupant does not have a workplace
            if (occupant.office == null)
            {
                Debug.LogError($"{occupant} cannot do working task without a workplace");
                Cancel();
                return;
            }

            IOccupantTask nextSubTask;
            if (occupant.currentRoom == occupant.office)
            {
                // Wander about the office
                var wanderingTask = new OccupantWanderingTask(occupant);
                // Give a better sense of activity/busy-ness
                wanderingTask.minWaitTime = 0.4f;
                wanderingTask.maxWaitTime = 2f;
                nextSubTask = wanderingTask;

                // register this occupant as "working" in this office if this room has an office behavior
                if (occupant.currentRoom.behavior is OfficeRoomBehavior)
                {
                    (occupant.currentRoom.behavior as OfficeRoomBehavior).workingOccupants.Add(occupant);
                }
            }
            else
            {
                // travel to place of work
                var destinationTile = occupant.office.GetRandomTile();
                var routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, destinationTile);
                var route = routeFinder.FindRoute();

                if (route == null)
                {
                    // TODO - a notification as well
                    Debug.LogError($"{occupant} cannot find a route to their workplace");
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