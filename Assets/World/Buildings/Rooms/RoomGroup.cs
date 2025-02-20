using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerBuilder
{
    public class RoomGroup : IEnumerable<Room>, IEnumerable
    {
        public string name;

        List<Room> rooms = new();

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

        public List<Room> GetList()
        {
            return rooms;
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

        public Tile GetBottomLeftTile()
        {
            var result = new Tile(int.MaxValue, int.MaxValue);

            foreach (var room in rooms)
            {
                var roomBottomLeft = room.GetBottomLeftTile();

                if (roomBottomLeft.x < result.x)
                {
                    result.x = roomBottomLeft.x;
                }

                if (roomBottomLeft.y < result.y)
                {
                    result.y = roomBottomLeft.y;
                }
            }

            return result;
        }
    }
}