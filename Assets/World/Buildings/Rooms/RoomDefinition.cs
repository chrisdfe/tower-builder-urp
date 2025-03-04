using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    using static RoomType;
    public class RoomDefinition
    {
        public string title = "No title";
        public string description = "No description.";
        public List<Tile> shape;
        public RoomType type = CommonArea;
        public bool isEntrance = false;

        public RoomGroupCategory groupCategory = RoomGroupCategory.None;

        // How many occupants can:
        //   live in this room for Residential rooms
        //   work in this room for Office  rooms
        //   recreate in this room for Recreation rooms
        public uint capacity;

        // public uint residentCapacity = 0;

        // public uint workerCapacity = 0;

        public List<IRoomValidator> buildValidators = new(RoomBuildValidators.standardBuildValidators);

        // A human-readable RoomType
        public static string GetRoomTypeName(RoomType roomType)
        {
            switch (roomType)
            {
                case CommonArea: return "Common Area";
                case TransportationItem: return "Transportation Item";
                case Residential: return "Residential";
                case Office: return "Office";
                case Recreation: return "Recreation";
            }

            throw new System.Exception($"Can't get name for roomType: {roomType}");
        }
    }
}