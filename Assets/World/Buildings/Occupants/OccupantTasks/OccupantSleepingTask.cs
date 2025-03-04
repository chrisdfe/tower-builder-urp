using UnityEngine;

namespace TowerBuilder
{
    public class OccupantSleepingTask : IOccupantTask
    {
        public string name => "Sleeping";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        Occupant occupant;

        public OccupantSleepingTask(Occupant occupant)
        {
            this.occupant = occupant;
        }

        public void OnTick() { }

        public void Setup()
        {
            occupant.animationWrapper.SetIsLyingDown(true);
            occupant.currentRoom.AddAsleepOccupant(occupant);
        }

        public void Teardown()
        {
            occupant.animationWrapper.SetIsLyingDown(false);
            occupant.currentRoom.RemoveAsleepOccupant(occupant);
        }

        public void Cancel()
        {
            _isComplete = true;
        }
    }
}