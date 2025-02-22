using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class BuildingsController
    {
        public List<Building> buildings { get; private set; } = new();

        WorldController worldController;

        Transform buildingsContainer;
        Transform occupantsContainer;

        public delegate void BuildingEvent();
        public BuildingEvent onRoomBuilt;
        public BuildingEvent onRoomDestroyed;

        public BuildingsController(WorldController worldController)
        {
            this.worldController = worldController;

            buildingsContainer = GameObject.Find("BuildingsContainer").transform;
            occupantsContainer = GameObject.Find("OccupantsContainer").transform;
        }

        //
        // Public interface
        //
        public void OnTick()
        {
            foreach (var building in buildings)
            {
                building.OnTick();
                HandleBuildingRoomVacancies(building);
            }
        }

        // TODO - cache this number
        public int ResidentCount()
        {
            var result = 0;

            foreach (var building in buildings)
            {
                result += building.ResidentCount();
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

        public int RoomGroupCount()
        {
            int result = 0;

            foreach (var building in buildings)
            {
                result += building.roomGroups.Count;
            }

            return result;
        }

        // Warning - doesn't do any validation
        public void AddRoomAtTile(RoomDefinition roomDefinition, Tile tile)
        {
            var allAdjacentTiles = tile.GetAdjacentTilesIncludingSelf();

            // Search for a building adjacent
            // TODO - combine buildings?
            // var building = buildings.Find(building => building.ContainsRoomAtTiles(allAdjacentTiles.ToList()));

            // if (building == null)
            // {
            //     building = CreateBuilding();
            // }

            // DEBUG
            Building building;
            if (buildings.Count == 0)
            {
                building = CreateBuilding();
            }
            else
            {
                building = buildings[0];
            }

            building.AddRoom(tile, roomDefinition);
        }

        public void AddRoomAtCurrentTileIfValid()
        {
            if (!worldController.toolsController.buildTool.blueprintRoom.isValid)
            {
                foreach (var error in worldController.toolsController.buildTool.blueprintRoom.buildValidationErrors)
                {
                    worldController.AddNotification(error.message);
                }

                return;
            }

            var tile = worldController.GetMousePositionToTile();

            AddRoomAtTile(worldController.toolsController.buildTool.selectedRoomDefinition.current, tile);

            // TODO - this could get confusing
            onRoomBuilt?.Invoke();
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

                    onRoomDestroyed?.Invoke();
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

        public bool ContainsRoomsAtTile(Tile tile)
        {
            return FindRoomsAtTile(tile) != null;
        }

        public bool ContainsRoomAtTile(Tile tile, RoomLayer roomLayer)
        {
            return FindRoomAtTile(tile, roomLayer) != null;
        }

        public bool ContainsRoomsAtTiles(List<Tile> tiles)
        {
            return FindRoomsAtTiles(tiles).Count > 0;
        }

        public bool ContainsRoomsAtTiles(List<Tile> tiles, RoomLayer roomLayer)
        {
            return FindRoomsAtTiles(tiles, roomLayer).Count > 0;
        }

        public List<Room> FindRoomsAtTiles(List<Tile> tiles)
        {
            foreach (var building in buildings)
            {
                // Buildings can't overlap so we don't need to account for that
                var rooms = building.FindRoomsAtTiles(tiles);
                if (rooms.Count > 0)
                {
                    return rooms;
                }
            }

            return new();
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

        public Room FindRoomAtTile(Tile tile, RoomLayer roomLayer)
        {
            foreach (var building in buildings)
            {
                // Buildings can't overlap so we don't need to account for that
                var room = building.FindRoomAtTile(tile, roomLayer);
                if (room != null)
                {
                    return room;
                }
            }

            return new();
        }

        public List<Room> FindRoomsAtTiles(List<Tile> tiles, RoomLayer roomLayer)
        {
            foreach (var building in buildings)
            {
                // Buildings can't overlap so we don't need to account for that
                var rooms = building.FindRoomsAtTiles(tiles, roomLayer);
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

        Occupant CreateOccupant()
        {
            var occupantPrefab = worldController.occupantPrefab;
            var occupantGameObject = GameObject.Instantiate(occupantPrefab, occupantsContainer);
            var occupant = occupantGameObject.GetComponent<Occupant>();
            return occupant;
        }

        void HandleBuildingRoomVacancies(Building building)
        {
            // TODO - re-use the same occupants for residences/offices
            // create occupants for rooms that have residence slots available
            var availableResidenceRooms = building.GetRoomsWithAvailableResidenceSlots();

            foreach (var room in availableResidenceRooms)
            {
                var subTileOffset = room.residents.Count;

                var occupant = CreateOccupant();

                // TODO - pick random 'TileAddress' 
                occupant.SetTile(room.tiles[0]);
                occupant.SetSubTileOffset(subTileOffset * 0.3f);
                room.residents.Add(occupant);
                occupant.residence = room;
                occupant.title = $"{building.title} Occupant {building.ResidentCount()}";
                occupant.gameObject.name = occupant.title;

                worldController.notifications.Add(new Notification(occupant.title + " has moved into " + room.title));
            }

            // give occupants jobs if they are unemployed and there is work available
            var unemployedOccupants = building.GetUnemployedOccupants();

            if (unemployedOccupants.Count > 0)
            {
                var availableWorkerRooms = building.GetRoomsWithAvailableWorkerSlots();

                foreach (var room in availableWorkerRooms)
                {
                    foreach (var occupant in unemployedOccupants)
                    {
                        occupant.office = room;
                        room.workers.Add(occupant);
                        worldController.notifications.Add(new Notification(occupant.title + " has been assigned work at " + room.title));
                    }
                }
            }
        }
    }
}
