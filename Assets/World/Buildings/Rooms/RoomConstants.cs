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
                shape = TileList.CreateBox(3, 1),
                type = RoomType.CommonArea,
                groupCategory = RoomGroupCategory.CommonArea,
            },

            new() {
                title = "Entrance/Exit",
                price = 4000,
                isEntrance = true,
                shape = TileList.CreateBox(1, 1),
                type = RoomType.CommonArea,
                groupCategory = RoomGroupCategory.CommonArea,
                // TODO - only one allowed per building
            },

            new() {
                //
                title = "Condo",
                price = 100_000,
                shape = TileList.CreateBox(5, 1),
                type = RoomType.Residential,
                capacity = 3,
            },

            new() {
                title = "Pod",
                price = 50_000,
                shape = TileList.CreateBox(2, 2),
                type = RoomType.Residential,
                capacity = 3,
            },

            new() {
                title = "Office",
                price = 75_000,
                shape = TileList.CreateBox(5, 1),
                type = RoomType.Office,
                capacity = 12,

                behaviorFactory = (Room room) => new OfficeRoomBehavior(room),
            },

            new() {
                title = "Recreation room",
                price = 50_000,
                shape = TileList.CreateBox(8, 1),
                type = RoomType.Recreation,
                capacity = 12,
            },

            new() {
                title = "Stairs",
                price = 20_000,
                shape = TileList.CreateBox(2, 1),
                type = RoomType.TransportationItem,
                // TODO - this will ultimately be part of the 'CommonArea' category
                //        right now the routefinding algorithm can't deal with non-rectangular rooms though
                groupCategory = RoomGroupCategory.Stairs,
            },

            new() {
                title = "Cabin",
                description = "Visitors can stay overnight here (for a price)",
                price = 70_000,
                shape = TileList.CreateBox(3, 1),
                capacity = 2,
                type = RoomType.Hotel,
                behaviorFactory = (Room room) => new HotelRoomBehavior(room),
            },

            new() {
                title = "Restaurant",
                description = "Occupants will eat lunch and dinner here",
                price = 90_000,
                shape = TileList.CreateBox(8, 1),
                capacity = 20,
                type = RoomType.Restaurant,
            }
        };
    }
}