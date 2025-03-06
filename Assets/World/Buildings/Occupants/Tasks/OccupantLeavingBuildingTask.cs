using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class OccupantLeavingBuildingTask : IOccupantTask
    {
        public string name => "Leaving building";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        IOccupantTask currentSubTask;

        Occupant occupant;

        public OccupantLeavingBuildingTask(Occupant occupant)
        {
            this.occupant = occupant;
        }

        public void OnTick()
        {
            currentSubTask.OnTick();

            if (currentSubTask.isComplete)
            {
                _isComplete = true;

                // remove occupant from building
                WorldController.Get().buildingsController.FlagOccupantForRemoval(occupant);
            }
        }

        public void Setup()
        {
            var entrance = occupant.currentRoom.building.GetEntrance();
            Assert.IsNotNull(entrance);
            var destinationTile = entrance.GetRandomTile();
            var routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, destinationTile);
            var route = routeFinder.FindRoute();

            if (route == null)
            {
                // TODO - a notification as well
                Debug.LogError($"{occupant} cannot find a route out of the building");
                Cancel();
                return;
            }

            currentSubTask = new OccupantTravelingToDestinationTask(occupant, route);
        }

        public void Teardown()
        {
            currentSubTask?.Teardown();
        }

        public void Cancel()
        {
            if (currentSubTask != null)
            {
                currentSubTask?.Cancel();
            }
            else
            {
                _isComplete = true;
            }
        }
    }
}