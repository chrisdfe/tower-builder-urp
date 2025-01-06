using System.Collections.Generic;

namespace TowerBuilder
{
    public class RoomDefinition
    {
        public string title;
        public List<Tile> shape;

        public static RoomDefinition[] ALL_DEFINITIONS = new RoomDefinition[3] {
            new RoomDefinition() {
                //
                title = "Lobby",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(2, 0),
                }
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
                }
            },
            new RoomDefinition() {
                //
                title = "Pod",
                shape = new List<Tile>() {
                    new(0, 0),
                    new(1, 0),
                    new(0, 1),
                    new(1, 1),
                }
            },
        };
    }
}