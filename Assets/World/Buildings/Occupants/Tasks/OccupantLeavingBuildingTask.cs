using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class OccupantLeavingBuildingTask : OccupantTaskBase
    {
        public override string name => "Leaving building";

        public OccupantLeavingBuildingTask(Occupant occupant) : base(occupant) { }

        public override void OnTick()
        {
            currentSubTask.OnTick();

            if (currentSubTask.isComplete)
            {
                // Don't transition to another task -
                // it's time to remove occupant from building
                WorldController.Get().buildingsController.FlagOccupantForRemoval(occupant);
            }
        }

        public override void Setup()
        {
            var entrance = occupant.currentRoom.building.GetEntrance();
            Assert.IsNotNull(entrance);
            var destinationTile = entrance.GetRandomTile();

            currentSubTask = new OccupantTravelingToDestinationTask(occupant, destinationTile, "building exit");
        }
    }
}