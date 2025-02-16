using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class RoomDefinition
    {
        public string title;
        public List<Tile> shape;
        public RoomLayer layer = RoomLayer.Default;
        public RoomType type = RoomType.CommonArea;

        // How many occupants can live in this room
        public uint occupantialCapacity = 0;

        // How many occupants can work in this room
        public uint workerCapacity = 0;

        public List<IRoomValidator> buildValidators = new(RoomBuildValidators.standardBuildValidators);
    }
}