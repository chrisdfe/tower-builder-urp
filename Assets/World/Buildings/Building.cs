using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Building : MonoBehaviour
    {
        public string title = "Building";

        public List<Room> rooms { get; private set; } = new();

        public Room AddRoom(Tile originTile, RoomDefinition roomDefinition)
        {
            var roomPrefab = WorldController.Get().roomPrefab;
            var position = originTile.ToWorldPosition();

            var roomGameObject = Instantiate(roomPrefab, position, Quaternion.identity, transform);
            var room = roomGameObject.GetComponent<Room>();

            // Initialize room
            room.definition = roomDefinition;
            room.CalculateAndInstantiateTilesFromOriginTile(originTile);
            room.UpdateColor();
            room.SetZPosition();

            rooms.Add(room);
            room.title = $"{title} {room.definition.title} {GetRoomsByType(room.definition.type).Count}";
            roomGameObject.name = room.title;

            return room;
        }

        // TODO - 'destroy validation'
        public void RemoveRoom(Room room)
        {
            rooms.Remove(room);

            // Delete all residents for now
            foreach (var resident in room.residents)
            {
                Destroy(resident.gameObject);
            }

            // Don't delete workers, just unassign their place of work
            foreach (var worker in room.workers)
            {
                worker.office = null;
            }

            Destroy(room.gameObject);
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
            return FindRoomsAtTiles(tiles) != null;
        }

        public List<Room> FindRoomsAtTile(Tile tile)
        {
            return rooms.FindAll(otherRoom => otherRoom.ContainsTile(tile));
        }

        public List<Room> FindRoomsAtTiles(Tile[] tiles)
        {
            return rooms.FindAll(otherRoom => otherRoom.ContainsTile(tiles));
        }

        // TODO - cache this number
        public int ResidentsCount()
        {
            var result = 0;

            foreach (var room in rooms)
            {
                result += room.residents.Count;
            }

            return result;
        }

        // TODO - cache this number
        public int WorkerCount()
        {
            var result = 0;

            foreach (var room in rooms)
            {
                result += room.workers.Count;
            }

            return result;
        }

        public List<Room> GetRoomsWithAvailableResidenceSlots()
        {
            var result = new List<Room>();

            //
            foreach (var room in rooms)
            {
                if (room.residents.Count < room.definition.residentialCapacity)
                {
                    result.Add(room);
                }
            }

            return result;
        }

        public List<Resident> GetUnemployedResidents()
        {
            var result = new List<Resident>();

            foreach (var room in rooms)
            {
                foreach (var resident in room.residents)
                {
                    if (resident.office == null)
                    {
                        result.Add(resident);
                    }
                }
            }

            return result;
        }

        public List<Room> GetRoomsWithAvailableWorkerSlots()
        {
            var result = new List<Room>();

            //
            foreach (var room in rooms)
            {
                if (room.workers.Count < room.definition.workerCapacity)
                {
                    result.Add(room);
                }
            }

            return result;
        }

        public List<Room> GetRoomsByType(RoomType type)
        {
            var result = new List<Room>();

            foreach (var room in rooms)
            {
                if (room.definition.type == type)
                {
                    result.Add(room);
                }
            }

            return result;
        }
    }
}