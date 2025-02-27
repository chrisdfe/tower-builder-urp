using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class RoomDefinition
    {
        public string title;
        public List<Tile> shape;
        public RoomType type = RoomType.CommonArea;

        public RoomGroupCategory groupCategory = RoomGroupCategory.None;

        // How many occupants can live in this room
        public uint residentCapacity = 0;

        // How many occupants can work in this room
        public uint workerCapacity = 0;

        public List<IRoomValidator> buildValidators = new(RoomBuildValidators.standardBuildValidators);
    }
}