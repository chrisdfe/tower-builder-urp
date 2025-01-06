using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Building : MonoBehaviour
    {
        public List<Room> rooms { get; private set; } = new();

        // TODO - room type
        public Room AddRoom(Tile originTile, RoomDefinition roomDefinition)
        {
            var roomPrefab = WorldController.Get().roomPrefab;
            var position = originTile.ToWorldPosition();

            var roomGameObject = Instantiate(roomPrefab, position, Quaternion.identity, transform);
            var room = roomGameObject.GetComponent<Room>();

            // Initialize room
            room.definition = roomDefinition;
            room.CalculateAndInstantiateTilesFromOriginTile(originTile);

            rooms.Add(room);

            return room;
        }


        public bool ContainsRoom(Room room)
        {
            return rooms.Find(otherRoom => otherRoom == room) != null;
        }

        public bool ContainsRoomAtTile(Tile tile)
        {
            return rooms.Find(otherRoom => otherRoom.ContainsTile(tile)) != null;
        }

        public bool ContainsRoomAtTile(Tile[] tiles)
        {
            return FindRoomAtTiles(tiles) != null;
        }

        public Room FindRoomAtTiles(Tile[] tiles)
        {
            return rooms.Find(otherRoom => otherRoom.ContainsTile(tiles));
        }
    }
}