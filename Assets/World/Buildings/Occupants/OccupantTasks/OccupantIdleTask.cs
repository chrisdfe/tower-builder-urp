using UnityEngine;

namespace TowerBuilder
{
    public class OccupantIdleTask : IOccupantTask
    {
        public string name => "Idle";

        public bool isComplete => true;

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            // Nothing for now
        }
    }
}