using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class Tile
    {
        // Map of tilePosition enum -> fbx node names
        public static Dictionary<TileType, string> TypeLabelMap =
            new() {
                { TileType.None,       "None" },
                { TileType.Single,     "Single" },
                { TileType.Horizontal, "Horizontal" },
                { TileType.Vertical,   "Vertical" },
                { TileType.Diagonal,   "Diagonal" },
                { TileType.Full,       "Full" },
            };

        public int x = 0;
        public int y = 0;

        // position relative to neighbors
        // only uses top, right, bottom, and left
        public TilePosition orthogonalPosition = TilePosition.Single;

        // only uses topRight, bottomRight, bottomLeft, and topLeft
        public TilePosition diagonalPosition = TilePosition.Single;

        // TODO - "full" position too? or does that not make sense

        public Tile() { }

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"({x}, {y})";

        public (int, int) AsTuple() => (x, y);

        //
        // public interface
        //
        public void CalculatePositionFromTileList(List<Tile> tiles)
        {
            var neighbors = new TileNeighbors(this, tiles);
            orthogonalPosition = neighbors.GetOrthogonalTilePosition();
            diagonalPosition = neighbors.GetDiagonalTilePositions();
        }

        public Tile Add(Tile b) => Add(this, b);

        public Tile Subtract(Tile b) => Subtract(this, b);

        public bool Matches(Tile b) => Matches(this, b);

        public Tile Clone() => new Tile(x, y);

        public Vector3 ToWorldPosition() => new Vector3(x, y, 0);

        public Tile[] GetOrthagonalAdjacentTiles() =>
            new Tile[] {
                new(x, y - 1),
                new(x + 1, y),
                new(x, y + 1),
                new(x - 1, y),
            };

        public Tile[] GetAdjacentTiles() =>
            new Tile[] {
                new(x - 1, y - 1),
                new(x, y - 1),
                new(x + 1, y - 1),
                new(x + 1, y),
                new(x + 1, y + 1),
                new(x, y + 1),
                new(x - 1, y + 1),
                new(x - 1, y),
            };

        public Tile[] GetAdjacentTilesIncludingSelf() =>
             new Tile[] {
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

        public Tile GetCoordinatesAbove() => new Tile(x, y + 1);

        public Tile GetCoordinatesAboveRight() => new Tile(x + 1, y + 1);

        public Tile GetCoordinatesRight() => new Tile(x + 1, y);

        public Tile GetCoordinatesBelowRight() => new Tile(x + 1, y - 1);

        public Tile GetCoordinatesBelow() => new Tile(x, y - 1);

        public Tile GetCoordinatesBelowLeft() => new Tile(x - 1, y - 1);

        public Tile GetCoordinatesLeft() => new Tile(x - 1, y);

        public Tile GetCoordinatesAboveLeft() => new Tile(x - 1, y + 1);

        //
        // static interface
        //
        public static Tile Add(Tile a, Tile b) =>
            new Tile(a.x + b.x, a.y + b.y);

        public static Tile Subtract(Tile a, Tile b) =>
            new Tile(a.x - b.x, a.y - b.y);

        public static bool Matches(Tile a, Tile b) =>
            (
                a.x == b.x &&
                a.y == b.y
            );

        public static Tile zero => new Tile(0, 0);
        public static Tile one => new Tile(1, 1);

        // TODO - this doesn't exactly seem the right place for this
        public static bool ListContainsTileThatMatches(List<Tile> tileList, Tile tile) => tileList.Find(otherTile => otherTile.Matches(tile)) != null;

        public static Tile FromTuple((int, int) tuple)
        {
            var (x, y) = tuple;
            return new Tile(x, y);
        }
    }
}