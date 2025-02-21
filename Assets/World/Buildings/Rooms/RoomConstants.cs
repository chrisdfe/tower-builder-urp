using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    // Global place for hardcoded room data
    public static class RoomConstants
    {
        // Z-offsets
        public static Dictionary<RoomLayer, float> ROOM_LAYER_Z_OFFSETS = new() {
            { RoomLayer.Default, 0 },
            { RoomLayer.TransportationItem, 0.5f },
        };

        public static float BLUEPRINT_Z_OFFSET = 2f;

        // Colors
        public static Dictionary<RoomType, Color> ROOM_TYPE_COLORS = new() {
            { RoomType.CommonArea, Color.gray },
            { RoomType.Occupantial, Color.green },
            // { RoomType.TransportationItem, new Color(1f, 1f, 1f, 0.5f) },
            { RoomType.TransportationItem, new Color(0.992f, 0.811f, 0.721f) },
            { RoomType.Office, Color.magenta }
        };

        // TODO - these should ultimately be materials or all part of the same shader or something
        public static Color ROOM_MARKED_FOR_DELETION_COLOR = Color.red;
        public static Color ROOM_INSPECTION_HOVERED_COLOR = Color.yellow;
        public static Color ROOM_INSPECTED_COLOR = Color.white;

        // Room definitions
        public static RoomDefinition[] ALL_DEFINITIONS = new RoomDefinition[] {
            new RoomDefinition() {
                //
                title = "Lobby",

                groupCategory = RoomGroupCategory.CommonArea,

                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                },
                type = RoomType.CommonArea,
            },

            new RoomDefinition() {
                //
                title = "Entrance/Exit",

                groupCategory = RoomGroupCategory.CommonArea,

                shape = new List<Tile>() {
                    new(0, 0),
                },
                type = RoomType.CommonArea,

                // TODO - only one allowed per building
            },

            new RoomDefinition() {
                //
                title = "Barracks",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                    new(4, 0),
                },
                type = RoomType.Occupantial,
                occupantialCapacity = 12,
            },

            new RoomDefinition() {
                //
                title = "Pod",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                },
                type = RoomType.Occupantial,
                occupantialCapacity = 3,
            },

            new RoomDefinition() {
                //
                title = "Office",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                },
                type = RoomType.Office,
                workerCapacity = 12,
            },

            new RoomDefinition() {
                //
                title = "Stairs",

                groupCategory = RoomGroupCategory.CommonArea,

                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                },
                type = RoomType.TransportationItem,

                // TODO - stairs validators
                // buildValidators = new()
            },
        };
    }
}