using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantRouteFinder
    {
        // routes are limited within 1 building for now
        Building building;

        Tile startTile;

        Tile destinationTile;

        public List<OccupantRouteAttempt> attempts = new();

        public OccupantRouteFinder(Building building, Tile startTile, Tile destinationTile)
        {
            this.building = building;
            this.startTile = startTile;
            this.destinationTile = destinationTile;
        }

        public void Run()
        {
            attempts = new();

            var firstAttempt = new OccupantRouteAttempt(building, startTile, destinationTile, new(), attempts);
            firstAttempt.Start();
        }

        public void DebugDrawPaths()
        {

            var successfulAttempts = attempts.FindAll(attempt => attempt.hasReachedDestination);
            var unsuccessfullAttempts = attempts.FindAll(attempt => !attempt.hasReachedDestination);

            foreach (var attempt in successfulAttempts)
            {
                DrawPath(attempt.path, Color.green);
            }

            foreach (var attempt in unsuccessfullAttempts)
            {
                DrawPath(attempt.path, Color.red);
            }

            void DrawPath(List<OccupantRouteNode> path, Color color)
            {
                int idx = 0;
                // draw the line
                foreach (var node in path)
                {
                    var nextIdx = idx + 1;
                    if (nextIdx < path.Count)
                    {
                        var nextNode = path[nextIdx];
                        Debug.DrawLine(node.tile.ToWorldPosition(), nextNode.tile.ToWorldPosition(), color);
                    }

                    idx++;
                }
            }
        }
    }
}