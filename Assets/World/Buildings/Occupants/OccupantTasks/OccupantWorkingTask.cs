using UnityEngine;

namespace TowerBuilder
{
    public class OccupantWorkingTask : IOccupantTask
    {
        // TODO - if traveling still, then "traveling to work"
        public string name => "Working";

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

        public void Teardown() { }

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

            IOccupantTask nextTask;
            if (occupant.currentRoom == occupant.office)
            {
                // Wander about the office
                var wanderingTask = new OccupantWanderingTask(occupant);
                // Give a better sense of business
                wanderingTask.minWaitTime = 0.4f;
                wanderingTask.maxWaitTime = 2f;
                nextTask = wanderingTask;
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

                nextTask = new OccupantTravelingToDestinationTask(occupant, route);
            }

            if (currentSubTask != null)
            {
                currentSubTask.Teardown();
            }

            currentSubTask = nextTask;
            currentSubTask.Setup();
        }
    }
}