using UnityEngine;

namespace TowerBuilder
{
    public class Tile
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Matches(Tile other)
        {
            return Tile.Matches(this, other);
        }

        public Vector3 ToWorldPosition()
        {
            return new Vector3(x, y, 0);
        }

        public Tile[] GetAdjacentTiles()
        {
            return new Tile[8] {
                new(x-1, y-1),
                new(x, y-1),
                new(x+1, y-1),
                new(x+1, y),
                new(x+1, y+1),
                new(x, y+1),
                new(x-1, y+1),
                new(x-1, y),
            };
        }

        public Tile[] GetAdjacentTilesIncludingSelf()
        {
            return new Tile[9] {
                new(x, y),
                new(x-1, y-1),
                new(x, y-1),
                new(x+1, y-1),
                new(x+1, y),
                new(x+1, y+1),
                new(x, y+1),
                new(x-1, y+1),
                new(x-1, y),
            };
        }

        public static Tile Zero()
        {
            return new Tile(0, 0);
        }

        public static bool Matches(Tile a, Tile b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }
}

