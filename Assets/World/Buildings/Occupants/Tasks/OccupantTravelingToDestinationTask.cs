using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantTravelingToDestinationTask : OccupantTaskBase
    {
        public override string name => description ?? $"Traveling to {lastTile}";

        public Tile destinationTile { get; private set; }
        public OccupantRouteFinder routeFinder { get; private set; }
        OccupantRoute route;
        Tile lastTile;
        int currentIdx = 0;

        string description;

        WorldController worldController;

        public OccupantTravelingToDestinationTask(Occupant occupant, OccupantRouteFinder routeFinder, OccupantRoute route) : base(occupant)
        {
            this.routeFinder = routeFinder;
            this.route = route;

            worldController = WorldController.Get();
        }

        public OccupantTravelingToDestinationTask(Occupant occupant, Tile destinationTile) : base(occupant)
        {
            this.destinationTile = destinationTile;

            worldController = WorldController.Get();

            routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, destinationTile);
            route = routeFinder.FindRoute();

            if (route == null)
            {
                // TODO - a notification as well
                Debug.LogError($"{occupant} cannot find a route out of the building");
                Cancel();
                return;
            }

            lastTile = route.GetLastNode().tile;
        }

        public OccupantTravelingToDestinationTask(Occupant occupant, Tile destinationTile, string description) : this(occupant, destinationTile)
        {
            this.description = description;
        }

        // public override void Setup()
        // {

        // }

        public override void OnTick()
        {
            if (isCancelled)
            {
                isComplete = true;
            }
            else
            {
                // Walk to destination until we have reached the destination
                var currentNode = GetCurrentNode();

                occupant.SetTile(currentNode.tile);

                // Manage room transitions
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

                // TODO set random subtile offset too

                if (GetNextIdx() == -1)
                {
                    isComplete = true;
                }
                else
                {
                    StartAnimatingTransitionBetweenTiles();
                    currentIdx++;
                }
            }
        }

        protected override void TransitionToNextSubTask()
        {
            // TODO - rework things so I don't need to do this
        }

        void StartAnimatingTransitionBetweenTiles()
        {
            var startNode = GetCurrentNode();
            var nextNode = GetNextNode();

            occupant.animationWrapper.StartAnimatingTransitionBetweenTiles(startNode.tile, nextNode.tile);
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