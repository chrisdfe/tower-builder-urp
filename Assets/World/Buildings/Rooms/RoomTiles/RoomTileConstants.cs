using System.Collections.Generic;

namespace TowerBuilder
{
    using static RoomTileSegment;

    public static class RoomTileConstants
    {
        public static Dictionary<TilePosition, RoomTileSegment[]> SEGMENTS_FOR_TILE_POSITION = new Dictionary<TilePosition, RoomTileSegment[]>() {
            {
                TilePosition.None,
                new RoomTileSegment[] { }
            },

            // single (isolated)
            {
                TilePosition.Single,
                new[] {
                    Ceiling,
                    LeftWall,
                    RightWall,
                    Floor
                }
            },

            // middle (non-corner) edge tiles
            {
                TilePosition.Top,
                new[] {
                    Ceiling
                }
            },
            {
                TilePosition.Right,
                new[] {
                    RightWall
                }
            },
            {
                TilePosition.Bottom,
                new[] {
                    Floor
                }
            },
            {
                TilePosition.Left,
                new[] {
                    LeftWall
                }
            },

            // centers
            {
                TilePosition.Center,
                new RoomTileSegment[] {}
            },
            {
                TilePosition.HorizontalCenter,
                new[] {
                    Ceiling,
                    Floor
                }
            },
            {
                TilePosition.VerticalCenter,
                new[] {
                    LeftWall,
                    RightWall
                }
            },
            {
                TilePosition.LowToHighDiagonalCenter,
                new[] {
                    Ceiling,
                    LeftWall,
                    RightWall,
                    Floor
                }
            },
            {
                TilePosition.HighToLowDiagonalCenter,
                new[] {
                    Ceiling,
                    LeftWall,
                    RightWall,
                    Floor
                }
            },

            // corners
            {
                TilePosition.TopLeft,
                new[] {
                    Ceiling,
                    LeftWall
                }
            },
            {
                TilePosition.TopRight,
                new[] {
                    Ceiling,
                    RightWall
                }
            },
            {
                TilePosition.BottomRight,
                new[] {
                    RightWall,
                    Floor
                }
            },
            {
                TilePosition.BottomLeft,
                new[] {
                    LeftWall,
                    Floor
                }
            },

            // isolated cells
            {
                TilePosition.TopIsolated,
                new[] {
                    Ceiling,
                    LeftWall,
                    RightWall,
                }
            },
            {
                TilePosition.RightIsolated,
                new[] {
                    Ceiling,
                    RightWall,
                    Floor
                }
            },
            {
                TilePosition.BottomIsolated,
                new[] {
                    LeftWall,
                    RightWall,
                    Floor
                }
            },
            {
                TilePosition.LeftIsolated,
                new[] {
                    Ceiling,
                    LeftWall,
                    Floor
                }
            },

            {
                TilePosition.TopRightIsolated,
                new[] {
                    Ceiling,
                    RightWall,
                }
            },
            {
                TilePosition.BottomRightIsolated,
                new[] {
                    RightWall,
                    Floor
                }
            },
            {
                TilePosition.BottomLeftIsolated,
                new[] {
                    LeftWall,
                    Floor
                }
            },
            {
                TilePosition.TopLeftIsolated,
                new[] {
                    Ceiling,
                    LeftWall
                }
            },
        };

        public readonly static Dictionary<RoomTileSegment, RoomTileSegmentDefinition> SEGMENT_DEFINITIONS = new()
        {
            {
                Ceiling,
                new() {
                    rootNodeName = "Ceiling",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                LeftWall,
                new() {
                    rootNodeName = "LeftWall",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                RightWall,
                new() {
                    rootNodeName = "RightWall",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                Floor,
                new() {
                    rootNodeName = "Floor",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                BackWall,
                new() {
                    rootNodeName = "BackWall",
                    variants = new string[] { "Blank", "Window", "Porthole" },
                    defaultVariant = "Blank",
                }
            },
        };
    }
}