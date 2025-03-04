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
            new() {
                title = "Lobby",
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                },
                type = RoomType.CommonArea,
                groupCategory = RoomGroupCategory.CommonArea,
            },

            new() {
                //
                title = "Entrance/Exit",
                isEntrance = true,
                shape = new() {
                    new(0, 0),
                },
                type = RoomType.CommonArea,
                groupCategory = RoomGroupCategory.CommonArea,
                // TODO - only one allowed per building
            },

            new() {
                //
                title = "Condo",
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                    new(4, 0),
                },
                type = RoomType.Residential,
                capacity = 3,
            },

            new() {
                title = "Pod",
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                },
                type = RoomType.Residential,
                capacity = 3,
            },

            new() {
                title = "Office",
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                },
                type = RoomType.Office,
                capacity = 12,
            },

            new() {
                title = "Recreation room",
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                    new(4, 0),
                },
                type = RoomType.Recreation,
                capacity = 12,
            },

            new() {
                title = "Stairs",
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                },
                type = RoomType.TransportationItem,
                // groupCategory = RoomGroupCategory.CommonArea,
                // TODO - this will ultimately be part of the 'CommonArea' category
                //        right now the routefinding algorithm can't deal with non-rectangular rooms though
                groupCategory = RoomGroupCategory.Stairs,
            },

            new() {
                title = "Cabin",
            }
        };
    }
}