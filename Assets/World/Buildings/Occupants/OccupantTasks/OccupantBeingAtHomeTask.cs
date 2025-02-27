using UnityEngine;

namespace TowerBuilder
{
    public class OccupantBeingAtHomeTask : IOccupantTask
    {
        public string name => "Being at home";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        bool isCancelled = false;

        IOccupantTask currentSubTask;

        Occupant occupant;

        public OccupantBeingAtHomeTask(Occupant occupant)
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
            // We somehow ended up here erroneously if the occupant does not have a home
            if (occupant.residence == null)
            {
                Debug.LogError($"{occupant} cannot do working task without a residence");
                Cancel();
                return;
            }

            IOccupantTask nextTask;
            if (occupant.currentRoom == occupant.residence)
            {
                // Wander around home
                nextTask = new OccupantWanderingTask(occupant);
            }
            else
            {
                // travel home
                var destinationTile = occupant.residence.GetRandomTile();
                var routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, destinationTile);
                var route = routeFinder.FindRoute();

                if (route == null)
                {
                    // TODO - a notification as well
                    Debug.LogError($"{occupant} cannot find a route to their home");
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