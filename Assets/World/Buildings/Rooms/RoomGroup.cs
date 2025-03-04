using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerBuilder
{
    public class RoomGroup : IEnumerable<Room>, IEnumerable
    {
        public string title;

        public List<Room> rooms { get; private set; } = new();

        public Room this[int index]
        {
            get => rooms[index];
            set => rooms.Insert(index, value);
        }

        public void Add(Room room)
        {
            rooms.Add(room);
        }

        public void Remove(Room room)
        {
            rooms.Remove(room);
        }

        public IEnumerator<Room> GetEnumerator() => rooms.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(Room room) => rooms.Contains(room);

        public List<int> GetFloors()
        {
            HashSet<int> result = new();

            foreach (var room in rooms)
            {
                var floors = room.GetFloors();
                result.UnionWith(floors);
            }

            return result.ToList();
        }

        public Tile GetLowestXTileOnFloor(int floor)
        {
            Tile result = null;

            foreach (var room in rooms)
            {
                foreach (var tile in room.tiles)
                {
                    if (
                        tile.y == floor &&
                        (result == null || tile.x < result.x)
                    )
                    {
                        result = tile;
                    }
                }
            }

            return result;
        }
    }
}