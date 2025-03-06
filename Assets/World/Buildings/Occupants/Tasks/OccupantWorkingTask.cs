using UnityEngine;

namespace TowerBuilder
{
    public class OccupantWorkingTask : OccupantTaskBase
    {
        public override string name
        {
            get
            {
                if (currentSubTask is OccupantTravelingToDestinationTask)
                {
                    return "Traveling to Work";
                }

                return "Working";
            }
        }

        public override bool isImmediatelyCancellable => true;

        public OccupantWorkingTask(Occupant occupant) : base(occupant) { }

        public override void Teardown()
        {
            if (occupant.currentRoom.behavior is OfficeRoomBehavior)
            {
                (occupant.currentRoom.behavior as OfficeRoomBehavior).workingOccupants.Remove(occupant);
            }

            base.Teardown();
        }

        protected override OccupantTaskBase GetNextSubTask()
        {
            // We ended up here erroneously if the occupant does not have a workplace
            if (occupant.office == null)
            {
                Debug.LogError($"{occupant} cannot do working task without a workplace");
                Cancel();
                return null;
            }

            if (occupant.currentRoom == occupant.office)
            {
                // Register this occupant as "working" in this office if this room has an office behavior
                if (occupant.currentRoom.behavior is OfficeRoomBehavior)
                {
                    (occupant.currentRoom.behavior as OfficeRoomBehavior).workingOccupants.Add(occupant);
                }

                // Wander about the office
                var wanderingTask = new OccupantWanderingTask(occupant);

                // Give a better sense of activity/busy-ness
                wanderingTask.minWaitTime = 0.4f;
                wanderingTask.maxWaitTime = 2f;

                return wanderingTask;
            }

            // Travel to place of work
            var destinationTile = occupant.office.GetRandomTile();
            return new OccupantTravelingToDestinationTask(occupant, destinationTile);
        }
    }
}