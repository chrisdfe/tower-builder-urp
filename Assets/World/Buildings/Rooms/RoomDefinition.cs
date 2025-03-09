using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    using static RoomType;
    public class RoomDefinition
    {
        public string title = "No title";
        public string description = "No description.";
        public int price = 0;
        public List<Tile> shape;
        public RoomType type = CommonArea;
        public bool isEntrance = false;

        public RoomGroupCategory groupCategory = RoomGroupCategory.None;

        // How many occupants can:
        //   live in this room for Residential rooms
        //   work in this room for Office rooms
        //   recreate in this room for Recreation rooms
        public int capacity;

        public List<IRoomValidator> buildValidators = new(RoomBuildValidators.standardBuildValidators);

        public delegate RoomBehaviorBase RoomBehaviorFactory(Room room);
        public RoomBehaviorFactory behaviorFactory = (Room room) => null;

        //
        // Static interface
        //

        // A human-readable RoomType
        public static string GetRoomTypeName(RoomType roomType) =>
            roomType switch
            {
                CommonArea => "Common Area",
                TransportationItem => "Transportation Item",
                _ => roomType.ToString(),
            };
    }
}