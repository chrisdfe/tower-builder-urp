using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    // Global place for hardcoded room data
    public static class RoomConstants
    {
        public const float NORMAL_ROOM_Z_OFFSET = 2f;
        public const float BLUEPRINT_ROOM_Z_OFFSET = 2.1f;

        // Room definitions
        public static readonly RoomDefinition[] ALL_DEFINITIONS = new RoomDefinition[] {
            new() {
                title = "Lobby",
                price = 5000,
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                },
                type = RoomType.CommonArea,
                groupCategory = RoomGroupCategory.CommonArea,
            },

            new() {
                title = "Entrance/Exit",
                price = 4000,
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
                price = 100_000,
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
                price = 50_000,
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
                price = 75_000,
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                    new(3, 0),
                },
                type = RoomType.Office,
                capacity = 12,

                roomBehaviorFactory = (Room room) => new OfficeRoomBehavior(room),
            },

            new() {
                title = "Recreation room",
                price = 50_000,
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
                price = 20_000,
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
                price = 70_000,
                shape = new() {
                    new(0, 0),
                    new(1, 0),
                },
                type = RoomType.Hotel,
            }
        };
    }
}