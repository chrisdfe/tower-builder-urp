using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class Room : MonoBehaviour
    {
        public List<Tile> tiles { get; private set; } = new List<Tile>() { Tile.Zero() };

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