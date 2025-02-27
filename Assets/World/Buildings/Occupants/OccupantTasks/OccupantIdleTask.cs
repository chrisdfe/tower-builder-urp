using UnityEngine;

namespace TowerBuilder
{
    public class OccupantIdleTask : IOccupantTask
    {
        public string name => "Idle";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            // Nothing for now
        }

        public void Cancel()
        {
            _isComplete = true;
        }
    }
}