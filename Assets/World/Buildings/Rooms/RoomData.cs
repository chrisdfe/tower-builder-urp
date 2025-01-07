using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    // Global place for hardcoded room data
    public static class RoomData
    {
        public static Dictionary<RoomLayer, float> ROOM_LAYER_Z_OFFSETS = new() {
            { RoomLayer.Default, 0 },
            { RoomLayer.TransportationItem, 0.5f },
        };

        public static float BLUEPRINT_Z_OFFSET = 2f;

        public static Dictionary<RoomType, Color> ROOM_TYPE_COLORS = new() {
            { RoomType.CommonArea, Color.gray },
            { RoomType.Residential, Color.green },
            { RoomType.TransportationItem, new Color(1f, 1f, 1f, 0.5f) }
        };

        public static RoomDefinition[] ALL_DEFINITIONS = new RoomDefinition[4] {
            new RoomDefinition() {
                //
                title = "Lobby",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                },
                type = RoomType.CommonArea,
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
                type = RoomType.Residential,
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
            },
            new RoomDefinition() {
                //
                title = "Stairs",
                layer = RoomLayer.TransportationItem,
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                },
                type = RoomType.TransportationItem
            },
        };
    }
}