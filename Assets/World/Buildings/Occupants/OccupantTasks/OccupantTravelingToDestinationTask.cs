using UnityEngine;

namespace TowerBuilder
{
    public class OccupantTravelingToDestinationTask : IOccupantTask
    {
        bool _isComplete = false;
        public bool isComplete => _isComplete;

        Occupant occupant;
        OccupantRoute route;
        int currentIdx = 0;

        public OccupantTravelingToDestinationTask(Occupant occupant, OccupantRoute route)
        {
            this.occupant = occupant;
            this.route = route;
        }

        public void Setup()
        {
            Debug.Log($"Traveling to tile {route.path[route.path.Count - 1].tile}");
        }

        public void Teardown() { }

        public void OnTick()
        {
            // Walk to destination until we have reached the destination
            var currentNode = GetCurrentNode();

            occupant.SetTile(currentNode.tile);
            // set random subtile offset too?

            // TODO - start lerping to next node

            if (GetNextIdx() == -1)
            {
                _isComplete = true;

                // for now, manually set occupant's new state
                occupant.TransitionToTask(new OccupantIdleTask());
            }
            else
            {
                currentIdx++;
            }
        }

        OccupantRouteNode GetCurrentNode() => route.path[currentIdx];

        int GetNextIdx()
        {
            var nextIdx = currentIdx + 1;

            if (nextIdx > route.path.Count - 1)
            {
                return -1;
            }

            return nextIdx;
        }
    }
}