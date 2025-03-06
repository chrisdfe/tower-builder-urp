using UnityEngine;

namespace TowerBuilder
{
    public class OccupantSleepingTask : OccupantTaskBase
    {
        public override string name => "Sleeping";

        public override bool isImmediatelyCancellable => true;

        public OccupantSleepingTask(Occupant occupant) : base(occupant) { }

        public override void Setup()
        {
            base.Setup();
            occupant.animationWrapper.SetIsLyingDown(true);
            occupant.currentRoom.AddAsleepOccupant(occupant);
        }

        public override void Teardown()
        {
            base.Teardown();
            occupant.animationWrapper.SetIsLyingDown(false);
            occupant.currentRoom.RemoveAsleepOccupant(occupant);
        }
    }
}