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
    }
}