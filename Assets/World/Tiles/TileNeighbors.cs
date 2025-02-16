using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    // Calculates and stores data about a Tile's direct neighboring tiles
    public class TileNeighbors
    {
        public class Neighbor
        {
            public TileOrientation tileOrientation;
            public bool isOccupied;

            public Neighbor(TileOrientation tileOrientation, bool isOccupied)
            {
                this.tileOrientation = tileOrientation;
                this.isOccupied = isOccupied;
            }
        }

        public Dictionary<TileOrientation, bool> occupiedMap { get; } = new();

        public TileNeighbors() { }

        public TileNeighbors(Tile tile, List<Tile> tileList)
        {
            occupiedMap = new Dictionary<TileOrientation, bool>() {
                { TileOrientation.Above,      Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesAbove()) },
                { TileOrientation.AboveRight, Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesAboveRight()) },
                { TileOrientation.Right,      Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesRight()) },
                { TileOrientation.BelowRight, Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesBelowRight()) },
                { TileOrientation.Below,      Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesBelow()) },
                { TileOrientation.BelowLeft,  Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesBelowLeft()) },
                { TileOrientation.Left,       Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesLeft()) },
                { TileOrientation.AboveLeft,  Tile.ListContainsTileThatMatches(tileList, tile.GetCoordinatesAboveLeft()) },
            };
        }

        public override string ToString()
        {
            var occupied = NeighborListToTileOrientation(GetOccupiedList());
            var notOccupied = NeighborListToTileOrientation(GetNotOccupiedList());

            string occupiedText = $"occupied: {occupied}";
            string notOccupiedText = $"not occupied: {notOccupied}";
            return $"{occupiedText}\n{notOccupiedText}";
        }

        public List<Neighbor> GetList()
        {
            List<Neighbor> result = new List<Neighbor>();

            foreach (KeyValuePair<TileOrientation, bool> entry in occupiedMap)
            {
                result.Add(new Neighbor(entry.Key, entry.Value));
            }

            return result;
        }

        public List<Neighbor> GetOccupiedList() => GetList().FindAll(neighbor => neighbor.isOccupied).ToList();

        public List<Neighbor> GetNotOccupiedList() => GetList().FindAll(neighbor => !neighbor.isOccupied).ToList();

        public List<Neighbor> GetOccupiedOrthogonalList() => GetOccupiedList().FindAll(neighbor => IsOrthogonal(neighbor.tileOrientation)).ToList();

        //
        public TilePosition GetOrthogonalTilePosition()
        {
            Debug.Log("GetOrthogonalTilePosition");
            var tileOrientation = NeighborListToTileOrientation(GetOccupiedOrthogonalList());
            Debug.Log("tileOrientation: " + tileOrientation);

            return tileOrientation switch
            {
                // 4 sides
                (TileOrientation.Above | TileOrientation.Right | TileOrientation.Below | TileOrientation.Left) =>
                    TilePosition.Center,

                // 3 sides
                (TileOrientation.Right | TileOrientation.Below | TileOrientation.Left) => TilePosition.Top,
                (TileOrientation.Below | TileOrientation.Left | TileOrientation.Above) => TilePosition.Right,
                (TileOrientation.Left | TileOrientation.Above | TileOrientation.Right) => TilePosition.Bottom,
                (TileOrientation.Above | TileOrientation.Right | TileOrientation.Below) => TilePosition.Left,

                // 2 sides
                // corners
                (TileOrientation.Above | TileOrientation.Right) => TilePosition.BottomLeft,
                (TileOrientation.Right | TileOrientation.Below) => TilePosition.TopLeft,
                (TileOrientation.Below | TileOrientation.Left) => TilePosition.TopRight,
                (TileOrientation.Left | TileOrientation.Above) => TilePosition.BottomRight,

                // centers
                (TileOrientation.Left | TileOrientation.Right) => TilePosition.HorizontalCenter,
                (TileOrientation.Above | TileOrientation.Below) => TilePosition.VerticalCenter,
                (TileOrientation.BelowLeft | TileOrientation.AboveRight) => TilePosition.LowToHighDiagonalCenter,
                (TileOrientation.AboveLeft | TileOrientation.BelowRight) => TilePosition.HighToLowDiagonalCenter,

                // 1 side
                (TileOrientation.Above) => TilePosition.BottomIsolated,
                (TileOrientation.AboveRight) => TilePosition.BottomLeftIsolated,

                (TileOrientation.Right) => TilePosition.LeftIsolated,
                (TileOrientation.BelowRight) => TilePosition.TopLeftIsolated,

                (TileOrientation.Below) => TilePosition.TopIsolated,
                (TileOrientation.BelowLeft) => TilePosition.TopRightIsolated,

                (TileOrientation.Left) => TilePosition.RightIsolated,
                (TileOrientation.AboveLeft) => TilePosition.BottomRightIsolated,

                // Default
                _ => TilePosition.Single
            };
        }

        // TODO - explain
        public TilePosition GetDiagonalTilePositions()
        {
            var tileOrientation = NeighborListToTileOrientation(GetOccupiedList());

            return tileOrientation switch
            {
                // centers
                (TileOrientation.BelowLeft | TileOrientation.AboveRight) => TilePosition.LowToHighDiagonalCenter,
                (TileOrientation.AboveLeft | TileOrientation.BelowRight) => TilePosition.HighToLowDiagonalCenter,

                // 1 side
                (TileOrientation.AboveRight) => TilePosition.BottomLeftIsolated,
                (TileOrientation.BelowRight) => TilePosition.TopLeftIsolated,
                (TileOrientation.BelowLeft) => TilePosition.TopRightIsolated,
                (TileOrientation.AboveLeft) => TilePosition.BottomRightIsolated,

                // Default
                _ => TilePosition.Single
            };
        }

        //
        // private interface
        //

        // TileOrientation is a bitmask
        TileOrientation NeighborListToTileOrientation(List<Neighbor> neighbors) =>
            neighbors.Aggregate(TileOrientation.None, (acc, neighbor) => (acc | neighbor.tileOrientation));

        bool IsOrthogonal(TileOrientation tileOrientation) =>
            tileOrientation == TileOrientation.Above ||
            tileOrientation == TileOrientation.Right ||
            tileOrientation == TileOrientation.Below ||
            tileOrientation == TileOrientation.Left;
    }
}