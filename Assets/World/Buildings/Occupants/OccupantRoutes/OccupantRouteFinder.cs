using System.Collections.Generic;
using System.Linq;
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
        public List<OccupantRouteAttempt> successfulAttempts = new();
        public List<OccupantRouteAttempt> unsuccessfulAttempts = new();

        // The output
        public OccupantRoute route;

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

            //
            foreach (var attempt in attempts)
            {
                if (attempt.hasReachedDestination)
                {
                    successfulAttempts.Add(attempt);
                }
                else
                {
                    unsuccessfulAttempts.Add(attempt);
                }
            }

            // TODO - choose the shortest route
            //        for now just choose the first one if it exists
            // TODO - make sure we use the full route when comparing lengths to find the shortest one
            if (successfulAttempts.Count > 0)
            {
                route = new OccupantRoute(CreateFullRouteFromSuccessfulAttempt(successfulAttempts[0]));
            }
        }

        List<OccupantRouteNode> CreateFullRouteFromSuccessfulAttempt(OccupantRouteAttempt attempt)
        {
            List<OccupantRouteNode> result = new();

            int idx = 0;
            foreach (var node in attempt.path)
            {
                var nextIdx = idx + 1;
                // TODO - something about this isn't working, it's skipping some tiles and duplicating others
                if (nextIdx <= attempt.path.Count - 1)
                {
                    var nextNode = attempt.path[nextIdx];
                    // first travel vertically
                    if (nextNode.tile.y != node.tile.y)
                    {
                        var range = CreateRange(node.tile.y, nextNode.tile.y);
                        foreach (var y in CreateRange(node.tile.y, nextNode.tile.y))
                        {
                            result.Add(new() { tile = new Tile(node.tile.x, y) });
                        }
                    }

                    // then horizontally
                    if (nextNode.tile.x != node.tile.x)
                    {
                        var range = CreateRange(node.tile.x, nextNode.tile.x);
                        foreach (var x in range)
                        {
                            result.Add(new() { tile = new Tile(x, node.tile.y) });
                        }
                    }
                }

                idx++;
            }

            var lastNode = attempt.path[attempt.path.Count - 1];
            result.Add(new() { tile = lastNode.tile });

            return result;

            // non-inclusive: includes a but not b
            List<int> CreateRange(int a, int b)
            {
                var result = new List<int>();

                if (a < b)
                {
                    for (var i = a; i < b; i++)
                    {
                        result.Add(i);
                    }
                }
                else if (b < a)
                {
                    for (var i = a; i > b; i--)
                    {
                        result.Add(i);
                    }
                }
                else
                // a == b
                {
                    result = new();
                }

                return result;
            }
        }

        public void DebugDrawPaths()
        {
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