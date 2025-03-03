using System.Collections;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantTravelingToDestinationTask : IOccupantTask
    {
        public string name => $"Traveling to tile {route.GetLastNode().tile}";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        bool isCancelled = false;

        Occupant occupant;
        OccupantRoute route;
        int currentIdx = 0;

        // TODO - this doesn't seem great
        WorldController worldController;

        public OccupantTravelingToDestinationTask(Occupant occupant, OccupantRoute route)
        {
            this.occupant = occupant;
            this.route = route;

            worldController = WorldController.Get();
        }

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            if (isCancelled)
            {
                _isComplete = true;
            }
            else
            {
                // Walk to destination until we have reached the destination
                var currentNode = GetCurrentNode();

                occupant.SetTile(currentNode.tile);

                // manage room transitions
                var previousRoom = occupant.currentRoom;
                var newRoom = worldController.buildingsController.FindRoomAtTile(currentNode.tile);

                if (previousRoom != newRoom)
                {
                    previousRoom.currentOccupants.Remove(occupant);
                    previousRoom.UpdateColor();
                    newRoom.currentOccupants.Add(occupant);
                    newRoom.UpdateColor();
                }

                occupant.SetCurrentRoom(newRoom);

                // animation
                occupant.animationWrapper.ResetPosition();


                // set random subtile offset too?

                if (GetNextIdx() == -1)
                {
                    _isComplete = true;
                }
                else
                {
                    StartAnimatingTransitionBetweenTiles();
                    currentIdx++;
                }
            }
        }

        void StartAnimatingTransitionBetweenTiles()
        {
            var startNode = GetCurrentNode();
            var nextNode = GetNextNode();

            occupant.animationWrapper.StartAnimatingTransitionBetweenTiles(startNode.tile, nextNode.tile);
        }

        public void Cancel()
        {
            isCancelled = true;
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

        OccupantRouteNode GetNextNode()
        {
            var nextIdx = GetNextIdx();

            if (nextIdx == -1)
            {
                return null;
            }

            return route.path[nextIdx];
        }
    }
}