using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class Building : MonoBehaviour
    {
        public string title = "Building";

        public List<RoomGroup> roomGroups { get; private set; } = new();
        public List<Room> rooms { get; private set; } = new();

        //
        // Lifecycle
        //
        public void OnTick()
        {
            foreach (var room in rooms)
            {
                foreach (var resident in room.residents)
                {
                    resident.OnTick();
                }
            }
        }

        //
        // Public interface -
        // Write methods
        //
        public Room AddRoom(Tile originTile, RoomDefinition roomDefinition)
        {
            var roomPrefab = WorldController.Get().roomPrefab;
            var position = originTile.ToWorldPosition();

            var roomGameObject = Instantiate(roomPrefab, position, Quaternion.identity, transform);
            var room = roomGameObject.GetComponent<Room>();

            // Initialize room
            room.building = this;
            room.definition = roomDefinition;
            room.SetTitle($"{title} {room.definition.title} {GetRoomsByType(room.definition.type).Count + 1}");
            room.CalculateAndInstantiateTilesFromOriginTile(originTile);
            room.UpdateColor();
            room.SetZPosition();

            rooms.Add(room);

            // Add to roomGroup if room is groupable
            if (room.definition.groupCategory != RoomGroupCategory.None)
            {
                // check if the room we just created is next to another room of the same time
                var adjacentTiles = room.GetAdjacentTiles();

                var adjacentRooms = FindRoomsAtTiles(adjacentTiles);
                var adjacentRoomsOfTheSameType = adjacentRooms.FindAll(otherRoom => (
                    otherRoom.definition.groupCategory == room.definition.groupCategory
                )).ToList();

                if (adjacentRoomsOfTheSameType.Count > 0)
                {
                    var newRoomGroup = new RoomGroup { room };

                    List<RoomGroup> roomGroupsToDelete = new();

                    // transfer rooms from adjacent room group into new group and 
                    // and delete the original room groups
                    foreach (var adjacentRoom in adjacentRoomsOfTheSameType)
                    {
                        var adjacentRoomGroup = FindRoomGroupByRoom(adjacentRoom);

                        Assert.IsNotNull(adjacentRoomGroup);

                        roomGroupsToDelete.Add(adjacentRoomGroup);

                        foreach (var adjacentRoomGroupRoom in adjacentRoomGroup)
                        {
                            newRoomGroup.Add(adjacentRoomGroupRoom);
                        }
                    }

                    roomGroups.RemoveAll(roomGroup => roomGroupsToDelete.Contains(roomGroup));

                    newRoomGroup.name = $"{name} roomGroup #{roomGroups.Count + 1} - {room.definition.groupCategory}";

                    roomGroups.Add(newRoomGroup);

                    var allTilesInRooms = GetAllTilesInRooms(newRoomGroup.GetList());

                    // Now re-calculate positions/toggle segments in each of these rooms
                    foreach (var roomInNewRoomGroup in newRoomGroup)
                    {
                        roomInNewRoomGroup.CalculateSegmentsFromTileList(allTilesInRooms);
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
            foreach (var resident in room.residents)
            {
                Destroy(resident.gameObject);
            }

            // Don't delete workers, just unassign their place of work
            foreach (var worker in room.workers)
            {
                worker.SetOffice(null);
            }

            // TODO - decide what to do here!!!!!! move to a room at an adjacent tile probably.
            // write a GetAdjacentRooms() function
            // for now just set currentRoom to null
            foreach (var occupant in room.currentOccupants)
            {
                occupant.SetCurrentRoom(null);
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

        //
        // Public interface -
        // Read methods
        //
        public bool ContainsRoom(Room room) =>
            rooms.Find(otherRoom => otherRoom == room) != null;

        public bool ContainsRoomAtTile(Tile tile) =>
            FindRoomsAtTile(tile).Count > 0;

        public bool ContainsRoomAtTiles(List<Tile> tiles) =>
            FindRoomsAtTiles(tiles).Count > 0;

        public List<Room> FindRoomsAtTile(Tile tile) =>
            rooms.FindAll(otherRoom => otherRoom.ContainsTile(tile));

        public List<Room> FindRoomsAtTiles(List<Tile> tiles) =>
            rooms.FindAll(otherRoom => otherRoom.ContainsTiles(tiles));

        public Room FindRoomAtTile(Tile tile) =>
            rooms.Find(otherRoom => otherRoom.ContainsTile(tile));

        public List<Room> FindRoomsOnFloor(int floor) =>
            rooms.FindAll(otherRoom => otherRoom.ContainsFloor(floor));

        public RoomGroup FindRoomGroupByRoom(Room room) =>
            roomGroups.Find(roomGroup => roomGroup.Contains(room));

        // TODO - cache this number
        public int ResidentCount()
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
                if (room.residents.Count < room.definition.residentCapacity)
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
                foreach (var occupant in room.residents)
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

        //
        // static interface
        //
        public static List<Tile> GetAllTilesInRooms(List<Room> roomList)
        {
            var result = new List<Tile>();

            foreach (var room in roomList)
            {
                foreach (var tile in room.tiles)
                {
                    result.Add(tile);
                }
            }

            return result;
        }
    }
}