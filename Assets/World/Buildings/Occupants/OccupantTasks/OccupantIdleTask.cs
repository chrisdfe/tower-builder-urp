using UnityEngine;

namespace TowerBuilder
{
    public class OccupantIdleTask : IOccupantTask
    {
        public bool isComplete => true;

        public void Setup()
        {
            Debug.Log("starting idle task");
        }

        public void Teardown() { }

        public void OnTick()
        {
            // Nothing for now
        }
    }
}