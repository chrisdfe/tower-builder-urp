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

        public void Setup() { }

        public void Teardown() { }

        public void Cancel()
        {
            _isComplete = true;
        }
    }
}