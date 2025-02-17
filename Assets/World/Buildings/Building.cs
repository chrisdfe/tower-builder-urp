using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class Building : MonoBehaviour
    {
        public string title = "Building";

        public List<List<Room>> roomGroups { get; private set; } = new();
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

            if (room.definition.isGroupable)
            {
                // check if the room we just created is next to another room of the same time
                var adjacentTiles = room.GetAdjacentTiles();
                var adjacentRooms = FindRoomsAtTiles(adjacentTiles);
                var adjacentRoomsOfTheSameType = adjacentRooms.FindAll(otherRoom => (
                    otherRoom.definition == room.definition
                )).ToList();

                if (adjacentRoomsOfTheSameType.Count > 0)
                {
                    // TODO here - combine all roomGroups now into a single roomGroup

                    foreach (var adjacentRoom in adjacentRoomsOfTheSameType)
                    {
                        var roomGroup = FindRoomGroupByRoom(adjacentRoom);

                        // TODO - assert that this roomGroup exists
                        if (roomGroup == null)
                        {
                            throw new System.Exception($"Unable to find expected roomGroup for room {room}");
                        }

                        roomGroup.Add(room);

                        // if not, then
                        //      create a new room group, and add both rooms to it
                        // if the room is not next to another room of the same type,
                        //      create a new room group with this room group in it
                        // then re-calculate positions/toggle segments in each of these rooms

                        // TODO - put into a function?
                        var allTilesInRooms = roomGroup.Aggregate(new List<Tile>(), (tileList, room) =>
                        {
                            foreach (var tile in room.tiles)
                            {
                                tileList.Add(tile);
                            }

                            return tileList;
                        });

                        foreach (var roomInGroup in roomGroup)
                        {
                            //
                            roomInGroup.CalculateSegmentsFromTileList(allTilesInRooms);
                        }
                    }
                }
                else
                {
                    // Create a new roomGroup with only this room in it for now
                    roomGroups.Add(new() { room });
                }
            }

            return room;
        }

        // TODO - 'destroy validation'
        public void RemoveRoom(Room room)
        {
            rooms.Remove(room);

            // Delete all occupants for now
            foreach (var occupant in room.occupants)
            {
                Destroy(occupant.gameObject);
            }

            // Don't delete workers, just unassign their place of work
            foreach (var worker in room.workers)
            {
                worker.office = null;
            }

            // Remove room from all room groups
            foreach (var roomGroup in roomGroups)
            {
                if (roomGroup.Contains(room))
                {
                    roomGroup.Remove(room);
                }
            }

            Destroy(room.gameObject);
        }

        public bool ContainsRoom(Room room)
        {
            return rooms.Find(otherRoom => otherRoom == room) != null;
        }

        public bool ContainsRoomAtTile(Tile tile)
        {
            return FindRoomsAtTile(tile).Count > 0;
        }

        public bool ContainsRoomAtTile(List<Tile> tiles, RoomLayer roomLayer)
        {
            return FindRoomsAtTiles(tiles, roomLayer).Count > 0;
        }

        public bool ContainsRoomAtTiles(List<Tile> tiles)
        {
            return FindRoomsAtTiles(tiles).Count > 0;
        }

        public bool ContainsRoomAtTiles(List<Tile> tiles, RoomLayer roomLayer)
        {
            return FindRoomsAtTiles(tiles, roomLayer).Count > 0;
        }

        public List<Room> FindRoomsAtTile(Tile tile)
        {
            return rooms.FindAll(otherRoom => otherRoom.ContainsTile(tile));
        }

        public List<Room> FindRoomsAtTiles(List<Tile> tiles)
        {
            return rooms.FindAll(otherRoom => otherRoom.ContainsTiles(tiles));
        }

        public Room FindRoomAtTile(Tile tile, RoomLayer roomLayer)
        {
            return rooms.Find(otherRoom => otherRoom.ContainsTile(tile) && otherRoom.definition.layer == roomLayer);
        }

        public List<Room> FindRoomsAtTiles(List<Tile> tiles, RoomLayer roomLayer)
        {
            return rooms.FindAll(otherRoom => otherRoom.ContainsTiles(tiles) && otherRoom.definition.layer == roomLayer);
        }

        public List<Room> FindRoomGroupByRoom(Room room) =>
            roomGroups.Find(roomGroup => roomGroup.Contains(room));

        // TODO - cache this number
        public int OccupantsCount()
        {
            var result = 0;

            foreach (var room in rooms)
            {
                result += room.occupants.Count;
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
                if (room.occupants.Count < room.definition.occupantialCapacity)
                {
                    result.Add(room);
                }
            }

            return result;
        }

        public List<Occupant> GetUnemployedOccupants()
        {
            var result = new List<Occupant>();

            foreach (var room in rooms)
            {
                foreach (var occupant in room.occupants)
                {
                    if (occupant.office == null)
                    {
                        result.Add(occupant);
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