using System.Collections.Generic;

namespace TowerBuilder
{
    public class RoomDefinition
    {
        public string name;
        public List<Tile> shape;

        public static RoomDefinition[] ALL_DEFINITIONS = new RoomDefinition[1] {
            new RoomDefinition() {
                //
                name = "lobby",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                }
            },
        };
    }
}