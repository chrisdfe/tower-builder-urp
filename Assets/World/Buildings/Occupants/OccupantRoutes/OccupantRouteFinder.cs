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

            var firstAttempt = new OccupantRouteAttempt(
                building,
                startTile,
                destinationTile,
                new(),
                new(),
                attempts
            );

            firstAttempt.Start();
        }

        public List<OccupantRouteAttempt> GetSuccessfulAttempts() => attempts.FindAll(attempt => attempt.hasReachedDestination);

        public List<OccupantRouteAttempt> GetUnsuccessfulAttempts() => attempts.FindAll(attempt => !attempt.hasReachedDestination);

        public void DebugDrawPaths()
        {
            var successfulAttempts = GetSuccessfulAttempts();
            var unsuccessfulAttempts = GetUnsuccessfulAttempts();

            int pathIdx = 0;
            foreach (var attempt in successfulAttempts)
            {
                DrawPath(attempt.path, pathIdx, Color.green);
                pathIdx++;
            }

            foreach (var attempt in unsuccessfulAttempts)
            {
                DrawPath(attempt.path, pathIdx, Color.red);
                pathIdx++;
            }

            void DrawPath(List<OccupantRouteNode> path, int pathIdx, Color color)
            {
                int idx = 0;
                // draw the line
                foreach (var node in path)
                {
                    var nextIdx = idx + 1;
                    if (nextIdx < path.Count)
                    {
                        var nextNode = path[nextIdx];
                        Debug.DrawLine(
                            GetPositionWithOffset(node.tile.ToWorldPosition()),
                            GetPositionWithOffset(nextNode.tile.ToWorldPosition()),
                            color
                        );
                    }

                    idx++;
                }

                Vector3 GetPositionWithOffset(Vector3 position) =>
                    new Vector3(
                        position.x + (pathIdx * 0.02f),
                        position.y + (pathIdx * 0.02f),
                        position.z
                    );
            }
        }
    }
}