using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    // Global place for hardcoded room data
    public static class RoomConstants
    {
        public const float NORMAL_ROOM_Z_OFFSET = 2f;
        public const float BLUEPRINT_ROOM_Z_OFFSET = 2f;

        // TODO - these should ultimately be materials or all part of the same shader or something
        public static readonly Color ROOM_MARKED_FOR_DELETION_COLOR = Color.red;
        public static readonly Color ROOM_INSPECTION_HOVERED_COLOR = Color.yellow;
        public static readonly Color ROOM_INSPECTED_COLOR = Color.white;

        // Room definitions
        public static readonly RoomDefinition[] ALL_DEFINITIONS = new RoomDefinition[] {
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
                title = "Condo",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                    new(4, 0),
                },
                type = RoomType.Residential,

                // residentCapacity = 12,
                residentCapacity = 3,
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
                type = RoomType.Residential,
                residentCapacity = 3,
            },

            new RoomDefinition() {
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
                title = "Stairs",

                groupCategory = RoomGroupCategory.CommonArea,

                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                },
                type = RoomType.TransportationItem,
            },
        };
    }
}