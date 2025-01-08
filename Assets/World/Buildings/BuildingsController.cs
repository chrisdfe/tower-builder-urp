using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class BuildingsController
    {
        public List<Building> buildings { get; private set; } = new();

        WorldController worldController;

        Transform buildingsContainer;
        Transform residentsContainer;

        public BuildingsController(WorldController worldController)
        {
            this.worldController = worldController;

            buildingsContainer = GameObject.Find("BuildingsContainer").transform;
            residentsContainer = GameObject.Find("ResidentsContainer").transform;
        }

        //
        // Public interface
        //
        public void OnTick()
        {
            foreach (var building in buildings)
            {
                // TODO - re-use the same residents for residences/offices
                // create residents for rooms that have residence slots available
                var availableResidenceRooms = building.GetRoomsWithAvailableResidenceSlots();

                foreach (var room in availableResidenceRooms)
                {
                    var subTileOffset = room.residents.Count;

                    var resident = CreateResident();

                    // TODO - this will update the position twice. not a huge deal
                    resident.SetTile(room.tiles[0]);
                    resident.SetSubTileOffset(subTileOffset * 0.3f);
                    room.residents.Add(resident);
                    resident.residence = room;
                }

                // create workers for rooms that have worker slots available
                var availableWorkerRooms = building.GetRoomsWithAvailableWorkerSlots();

                foreach (var room in availableWorkerRooms)
                {
                    var subTileOffset = room.workers.Count;

                    var resident = CreateResident();

                    // TODO - this will update the position twice. not a huge deal
                    resident.SetTile(room.tiles[0]);
                    resident.SetSubTileOffset(subTileOffset * 0.3f);
                    room.workers.Add(resident);
                    resident.office = room;
                }
            }
        }


        // TODO - cache this number
        public int ResidentsCount()
        {
            var result = 0;

            foreach (var building in buildings)
            {
                result += building.ResidentsCount();
            }

            return result;
        }

        // TODO - cache this number
        public int WorkerCount()
        {
            var result = 0;

            foreach (var building in buildings)
            {
                result += building.WorkerCount();
            }

            return result;
        }

        public int RoomsCount()
        {
            int result = 0;

            foreach (var building in buildings)
            {
                result += building.rooms.Count;
            }

            return result;
        }

        public void AddRoomAtCurrentTileIfValid()
        {
            if (!worldController.toolsController.blueprintRoom.isValid)
            {
                worldController.AddNotification("You cannot build this room.");
                return;
            }

            var tile = worldController.mousePositionToTile();
            var allAdjacentTiles = tile.GetAdjacentTilesIncludingSelf();

            // Search for a building adjacent
            // TODO - combine buildings?
            var building = buildings.Find(building => building.ContainsRoomAtTile(allAdjacentTiles));

            if (building == null)
            {
                building = CreateBuilding();
            }

            building.AddRoom(tile, worldController.toolsController.selectedRoomDefinition.current);
        }

        public void RemoveFrontmostRoomAtCurrentTile()
        {
            var room = FindFrontmostRoomAtTile(WorldController.Get().hoveredTile.current);

            if (room != null)
            {
                var building = FindBuildingByRoom(room);

                if (building != null)
                {
                    building.RemoveRoom(room);

                    if (building.rooms.Count == 0)
                    {
                        RemoveBuilding(building);
                    }
                }
            }
        }

        public void RemoveBuilding(Building building)
        {
            buildings.Remove(building);
            GameObject.Destroy(building.gameObject);
        }

        public Room FindFrontmostRoomAtTile(Tile tile)
        {
            var rooms = FindRoomsAtTile(tile);
            if (rooms.Count > 0)
            {
                // TODO - order by z-offset 
                return rooms[0];
            }

            return null;
        }

        public List<Room> FindRoomsAtTile(Tile tile)
        {
            foreach (var building in buildings)
            {
                // Buildings can't overlap so we don't need to account for that
                var rooms = building.FindRoomsAtTile(tile);
                if (rooms.Count > 0)
                {
                    return rooms;
                }
            }

            return new();
        }

        public Building FindBuildingByRoom(Room room)
        {
            foreach (var building in buildings)
            {
                if (building.ContainsRoom(room))
                {
                    return building;
                }
            }

            return null;
        }

        //
        // Private interface
        //
        Building CreateBuilding()
        {
            var buildingGameObject = GameObject.Instantiate(
                worldController.buildingPrefab,
                Vector3.zero,
                Quaternion.identity,
                buildingsContainer
            );
            var building = buildingGameObject.GetComponent<Building>();
            buildings.Add(building);
            return building;
        }

        Resident CreateResident()
        {
            var residentPrefab = worldController.residentPrefab;
            var residentGameObject = GameObject.Instantiate(residentPrefab, residentsContainer);
            var resident = residentGameObject.GetComponent<Resident>();
            return resident;
        }
    }
}
