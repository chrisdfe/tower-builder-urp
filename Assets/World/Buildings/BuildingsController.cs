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
                HandleBuildingRoomVacancies(building);
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

        // Warning - doesn't do any validation
        public void AddRoomAtTile(RoomDefinition roomDefinition, Tile tile)
        {
            var allAdjacentTiles = tile.GetAdjacentTilesIncludingSelf();

            // Search for a building adjacent
            // TODO - combine buildings?
            var building = buildings.Find(building => building.ContainsRoomAtTile(allAdjacentTiles));

            if (building == null)
            {
                building = CreateBuilding();
            }

            building.AddRoom(tile, roomDefinition);
        }

        public void AddRoomAtCurrentTileIfValid()
        {
            if (!worldController.toolsController.buildTool.blueprintRoom.isValid)
            {
                worldController.AddNotification("You cannot build this room.");
                return;
            }

            var tile = worldController.mousePositionToTile();

            AddRoomAtTile(worldController.toolsController.buildTool.selectedRoomDefinition.current, tile);
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
            building.title = $"Building {buildings.Count}";
            buildingGameObject.name = building.title;
            return building;
        }

        Resident CreateResident()
        {
            var residentPrefab = worldController.residentPrefab;
            var residentGameObject = GameObject.Instantiate(residentPrefab, residentsContainer);
            var resident = residentGameObject.GetComponent<Resident>();
            return resident;
        }

        void HandleBuildingRoomVacancies(Building building)
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
                resident.title = $"{building.title} Resident {building.ResidentsCount()}";
                resident.gameObject.name = resident.title;

                worldController.notifications.Add(new Notification(resident.title + " has moved into " + room.title));
            }

            // give residents jobs if they are unemployed and there is work available
            var unemployedResidents = building.GetUnemployedResidents();

            if (unemployedResidents.Count > 0)
            {
                var availableWorkerRooms = building.GetRoomsWithAvailableWorkerSlots();

                foreach (var room in availableWorkerRooms)
                {
                    foreach (var resident in unemployedResidents)
                    {
                        resident.office = room;
                        room.workers.Add(resident);
                        worldController.notifications.Add(new Notification(resident.title + " has been assigned work at " + room.title));
                    }
                }
            }
        }
    }
}
