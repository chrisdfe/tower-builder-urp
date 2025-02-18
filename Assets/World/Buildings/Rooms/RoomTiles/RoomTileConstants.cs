using System.Collections.Generic;

namespace TowerBuilder
{
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
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },

            // middle (non-corner) edge tiles
            {
                TilePosition.Top,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling
                }
            },
            {
                TilePosition.Right,
                new RoomTileSegment[] {
                    RoomTileSegment.RightWall
                }
            },
            {
                TilePosition.Bottom,
                new RoomTileSegment[] {
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.Left,
                new RoomTileSegment[] {
                    RoomTileSegment.LeftWall
                }
            },

            // centers
            {
                TilePosition.Center,
                new RoomTileSegment[] { }
            },
            {
                TilePosition.HorizontalCenter,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.VerticalCenter,
                new RoomTileSegment[] {
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.RightWall
                }
            },
            {
                TilePosition.LowToHighDiagonalCenter,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.HighToLowDiagonalCenter,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },

            // corners
            {
                TilePosition.TopLeft,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall
                }
            },
            {
                TilePosition.TopRight,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.RightWall
                }
            },
            {
                TilePosition.BottomRight,
                new RoomTileSegment[] {
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.BottomLeft,
                new RoomTileSegment[] {
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.Floor
                }
            },

            // isolated cells
            {
                TilePosition.TopIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.RightWall,
                }
            },
            {
                TilePosition.RightIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.BottomIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.LeftIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.Floor
                }
            },

            {
                TilePosition.TopRightIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.RightWall,
                }
            },
            {
                TilePosition.BottomRightIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.RightWall,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.BottomLeftIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.LeftWall,
                    RoomTileSegment.Floor
                }
            },
            {
                TilePosition.TopLeftIsolated,
                new RoomTileSegment[] {
                    RoomTileSegment.Ceiling,
                    RoomTileSegment.LeftWall
                }
            },
        };

        public readonly static Dictionary<RoomTileSegment, RoomTileSegmentDefinition> SEGMENT_DATA_MAP = new()
        {
            {
                RoomTileSegment.Ceiling,
                new RoomTileSegmentDefinition() {
                    rootNodeName = "Ceiling",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                RoomTileSegment.LeftWall,
                new RoomTileSegmentDefinition() {
                    rootNodeName = "LeftWall",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                RoomTileSegment.RightWall,
                new RoomTileSegmentDefinition() {
                    rootNodeName = "RightWall",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                RoomTileSegment.Floor,
                new RoomTileSegmentDefinition() {
                    rootNodeName = "Floor",
                    variants = new string[] { "Full" },
                    defaultVariant = "Full",
                }
            },
            {
                RoomTileSegment.BackWall,
                new RoomTileSegmentDefinition() {
                    rootNodeName = "BackWall",
                    variants = new string[] { "Blank", "Window", "Porthole", "LargeFlexibleWindow" },
                    defaultVariant = "Blank",
                }
            },
        };
    }
}