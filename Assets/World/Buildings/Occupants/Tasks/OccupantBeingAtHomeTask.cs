using UnityEngine;

namespace TowerBuilder
{
    public class OccupantBeingAtHomeTask : OccupantTaskBase
    {
        public override string name
        {
            get
            {
                if (currentSubTask is OccupantTravelingToDestinationTask)
                {
                    return "Going home";
                }

                return "Being at home";
            }
        }

        public OccupantBeingAtHomeTask(Occupant occupant) : base(occupant) { }

        protected override OccupantTaskBase GetNextSubTask()
        {
            // We somehow ended up here erroneously if the occupant does not have a home
            if (occupant.residence == null)
            {
                Debug.LogError($"{occupant} does not have a residence on this map");
                Cancel();
                return null;
            }

            if (occupant.currentRoom == occupant.residence)
            {
                // Wander around home
                return new OccupantWanderingTask(occupant);
            }

            // Travel home
            var destinationTile = occupant.residence.GetRandomTile();
            return new OccupantTravelingToDestinationTask(occupant, destinationTile, "Traveling to their home");
        }
    }
}