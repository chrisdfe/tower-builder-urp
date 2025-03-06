using UnityEngine;

namespace TowerBuilder
{
    public class OccupantIdleTask : OccupantTaskBase
    {
        public override string name => "Idle";

        public override bool isImmediatelyCancellable => true;

        public OccupantIdleTask(Occupant occupant) : base(occupant) { }

        public override void OnTick()
        {
            // Nothing
        }
    }
}