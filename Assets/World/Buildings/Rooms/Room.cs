using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Room : MonoBehaviour
    {
        public RoomDefinition definition;
        public List<Tile> tiles { get; private set; } = new List<Tile>() { Tile.Zero() };
        List<GameObject> roomTiles = new();

        // TODO - this isn't going to work for resizable rooms
        // TODO - make the originTile the center tile instead of bottom left
        public void CalculateAndInstantiateTilesFromOriginTile(Tile originTile)
        {
            // Calculate
            List<Tile> result = new();

            foreach (var tile in definition.shape)
            {
                var newTile = new Tile(originTile.x + tile.x, originTile.y + tile.y);
                result.Add(newTile);
            }

            tiles = result;

            // Instantiate
            var roomTilePrefab = WorldController.Get().roomTilePrefab;
            foreach (var tile in tiles)
            {
                var roomTile = Instantiate(roomTilePrefab, tile.ToWorldPosition(), Quaternion.identity, transform);
                roomTiles.Add(roomTile);
            }
        }

        public void SetTiles(List<Tile> tiles)
        {
            this.tiles = tiles;
        }


        public bool ContainsTile(Tile tile)
        {
            foreach (Tile t in tiles)
            {
                if (tile.Matches(t))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsTile(Tile[] targetTiles)
        {
            foreach (Tile tile in tiles)
            {
                foreach (Tile targetTile in targetTiles)
                {
                    if (tile.Matches(targetTile))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}