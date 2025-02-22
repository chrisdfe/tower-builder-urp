using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    // Not its own actual type - a collection of utils for dealing with List<Tile>
    public static class TileList
    {
        // TODO - this data structure could be better
        // ((highestX, lowestX), (highestY, lowestY))
        public static ((int, int), (int, int)) GetHighestAndLowestValues(List<Tile> tiles)
        {
            var highestX = int.MinValue;
            var lowestX = int.MaxValue;
            var highestY = int.MinValue;
            var lowestY = int.MaxValue;

            foreach (var tile in tiles)
            {
                if (tile.x < lowestX)
                {
                    lowestX = tile.x;
                }

                if (tile.x > highestX)
                {
                    highestX = tile.x;
                }

                if (tile.y < lowestY)
                {
                    lowestY = tile.y;
                }

                if (tile.y > highestY)
                {
                    highestY = tile.y;
                }

            }

            return ((highestX, lowestX), (highestY, lowestY));
        }

        public static Vector2 GetRelativeCenterPoint(List<Tile> tiles)
        {
            var ((highestX, lowestX), (highestY, lowestY)) = GetHighestAndLowestValues(tiles);

            return new Vector2(
                (highestX - lowestX) / 2f,
                (highestY - lowestY) / 2f
            );
        }
    }
}