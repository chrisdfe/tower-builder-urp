using UnityEngine;

namespace TowerBuilder
{
    public struct Tile
    {
        public int x;
        public int y;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Tile other)
        {
            return x == other.x && y == other.y;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, 0);
        }

        public static Tile Zero()
        {
            return new Tile(0, 0);
        }
    }
}

