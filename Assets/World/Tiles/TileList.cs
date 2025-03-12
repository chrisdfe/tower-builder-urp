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

        public static List<Tile> CreateBox(Tile startTile, int width, int height)
        {
            var result = new List<Tile>();

            for (var x = startTile.x; x < startTile.x + width; x++)
            {
                for (var y = startTile.y; y < startTile.y + height; y++)
                {
                    result.Add(new Tile(x, y));
                }
            }

            return result;
        }

        public static List<Tile> CreateBox(int width, int height) => CreateBox(Tile.zero, width, height);

        public static Dimensions GetScreenDimensions(List<Tile> tiles)
        {
            var ((highestX, lowestX), (highestY, lowestY)) = GetHighestAndLowestValues(tiles);

            var tileWidth = highestX - lowestX;
            var tileHeight = highestY - lowestY;

            var width = tileWidth * Tile.WORLD_WIDTH;
            var height = tileHeight * Tile.WORLD_HEIGHT;

            return new Dimensions(width, height);
        }

        // TODO - use GetScreenDimensions
        public static Rect GetScreenRect(List<Tile> tiles)
        {
            var ((highestX, lowestX), (highestY, lowestY)) = GetHighestAndLowestValues(tiles);

            // TODO - should I subtract Tile.WORLD_WIDTH / 2? is this returning the center of the tile
            var bottomLeft = Camera.main.WorldToScreenPoint(
                new Vector3(
                    lowestX * Tile.WORLD_WIDTH,
                    lowestY * Tile.WORLD_HEIGHT,
                    0
                )
            );

            var topRight = Camera.main.WorldToScreenPoint(
                new Vector3(
                    (highestX + 1) * Tile.WORLD_WIDTH,
                    (highestY + 1) * Tile.WORLD_HEIGHT,
                    0
                )
            );

            var x = bottomLeft.x;
            var y = bottomLeft.y;
            var width = topRight.x - bottomLeft.x;
            var height = topRight.y - bottomLeft.y;

            var tileScreenSize = GetTileScreenSize();
            x -= tileScreenSize.x / 2;
            y -= tileScreenSize.y / 2;

            return new Rect(x, y, width, height);
        }

        // TODO - Seems like not the right place for this but fine for now - maybe Camera
        // TODO - cache
        public static Vector2 GetTileScreenSize()
        {
            var originTileScreenPosition = Camera.main.WorldToScreenPoint(Vector3.zero);
            var oneTileScreenPosition = Camera.main.WorldToScreenPoint(new Vector3(Tile.WORLD_WIDTH, Tile.WORLD_HEIGHT, 0));

            var tileScreenSize = new Vector2(
                oneTileScreenPosition.x - originTileScreenPosition.x,
                oneTileScreenPosition.y - originTileScreenPosition.y
            );

            return tileScreenSize;
        }
    }
}