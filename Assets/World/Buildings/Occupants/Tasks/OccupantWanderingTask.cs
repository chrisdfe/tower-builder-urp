using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class OccupantWanderingTask : OccupantTaskBase
    {
        public override string name
        {
            get
            {
                if (currentSubTask is OccupantWanderingTask)
                {
                    return "Wandering";
                }

                return "Waiting around";
            }
        }

        public override bool isImmediatelyCancellable => true;

        Coroutine currentCoroutine;

        public float minWaitTime = 0.2f;
        public float maxWaitTime = 2f;

        public OccupantWanderingTask(Occupant occupant) : base(occupant) { }

        public override void Teardown()
        {
            if (currentCoroutine != null)
            {
                occupant.StopCoroutine(currentCoroutine);
            }

            base.Teardown();
        }

        protected override OccupantTaskBase GetNextSubTask()
        {
            if (currentSubTask is OccupantTravelingToDestinationTask)
            {
                // Wait around for a while
                return new OccupantWaitingTask(occupant, Random.Range(minWaitTime, maxWaitTime));
            }

            // Wander around
            // Find a random tile in the occupant's current room
            var nextTile = occupant.currentRoom.GetRandomTile();

            // Find a route to that tile

            return new OccupantTravelingToDestinationTask(occupant, nextTile, "Wandering");
        }
    }
}
