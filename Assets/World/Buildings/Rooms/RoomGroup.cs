using System.Collections;
using System.Collections.Generic;

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
    }
}