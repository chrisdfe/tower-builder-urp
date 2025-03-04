using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class RoomDefinition
    {
        public string title;
        public List<Tile> shape;
        public RoomType type = RoomType.CommonArea;
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
    }
}