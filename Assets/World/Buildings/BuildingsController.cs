using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    // manages buildings AND occupants (for now)
    public class BuildingsController
    {
        public List<Building> buildings { get; private set; } = new();
        public List<Occupant> occupants { get; private set; } = new();
        List<Occupant> occupantsToRemove = new();

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
        // Lifecycle
        //
        public void OnTick()
        {
            foreach (var building in buildings)
            {
                building.OnTick();
            }

            foreach (var occupant in occupants)
            {
                occupant.OnTick();
            }

            if (occupantsToRemove.Count > 0)
            {
                foreach (var occupant in occupantsToRemove)
                {
                    RemoveOccupant(occupant);
                }

                occupantsToRemove = new();
            }
        }

        //
        // Public interface
        //

        //
        // Write methods
        //

        // Warning - doesn't do any validation
        public void AddRoomAtTile(RoomDefinition roomDefinition, Tile tile)
        {

            // Search for a building adjacent
            // TODO - combine buildings?
            // var allAdjacentTiles = tile.GetAdjacentTilesIncludingSelf();
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


        // TODO - this function and AddRoomAtTile do similar things - clarify
        //        AddRoomAtTile is intended for internal use
        //        AddRoomAtCurrentTileIfValid is intended for use from the UI
        // Return value = whether room was built
        public bool AddRoomAtCurrentTileIfValid()
        {
            if (!worldController.toolsController.buildTool.blueprintRoom.isValid)
            {
                var buildErrors = worldController.toolsController.buildTool.blueprintRoom.buildValidationErrors;
                var errorIdx = -1;
                var message = buildErrors
                    .Aggregate("", (acc, error) =>
                    {
                        acc += error.message;
                        if (++errorIdx < buildErrors.Count - 1)
                        {
                            acc += " & ";
                        }

                        return acc;
                    });
                worldController.notificationsController.AddNotification(new()
                {
                    title = "Build error",
                    message = message
                });

                return false;
            }


            // Pay for the room
            worldController.walletController.ReduceFunds(worldController.toolsController.buildTool.selectedRoomDefinition.current.price);

            // Add it
            var tile = worldController.GetMousePositionToTile();
            AddRoomAtTile(worldController.toolsController.buildTool.selectedRoomDefinition.current, tile);

            // TODO - this could get confusing
            onRoomBuilt?.Invoke();

            return true;
        }

        public void RemoveFrontmostRoomAtCurrentTile()
        {
            var room = FindRoomAtTile(worldController.hoveredTile.current);

            if (room != null)
            {
                var building = FindBuildingByRoom(room);

                if (building != null)
                {
                    // residents of a room get destroyed when room is destroyed
                    occupants.RemoveAll(occupant => room.residents.Contains(occupant));

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

        //
        // Read methods
        //
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

        public bool ContainsRoomAtTile(Tile tile)
        {
            return FindRoomAtTile(tile) != null;
        }

        public bool ContainsRoomsAtTiles(List<Tile> tiles)
        {
            return FindRoomsAtTiles(tiles).Count > 0;
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

        public Room FindRoomAtTile(Tile tile)
        {
            foreach (var building in buildings)
            {
                // Buildings can't overlap so we don't need to account for that
                var room = building.FindRoomAtTile(tile);
                if (room != null)
                {
                    return room;
                }
            }

            return null;
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

        //
        // Buildings
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
            building.Setup();
            buildings.Add(building);
            building.title = $"Building {buildings.Count}";
            buildingGameObject.name = building.title;
            return building;
        }

        //
        // Occupants
        // 
        public Occupant CreateOccupant()
        {
            var occupantGameObject = GameObject.Instantiate(worldController.occupantPrefab, occupantsContainer);
            var occupant = occupantGameObject.GetComponent<Occupant>();
            occupants.Add(occupant);
            return occupant;
        }

        public Occupant CreateOccupantAtBuildingEntrance(Building building)
        {
            var entrance = building.GetEntrance();
            var occupant = CreateOccupant();

            occupant.SetTitle($"{building.title} Occupant {building.ResidentCount()}");
            occupant.SetTile(entrance.GetRandomTile());
            occupant.SetCurrentRoom(entrance);
            entrance.AddCurrentOccupant(occupant);
            occupant.SetRandomSubTileOffset();

            return occupant;
        }

        public void RemoveOccupant(Occupant occupant)
        {
            occupants.Remove(occupant);
            occupant.currentRoom.RemoveCurrentOccupant(occupant);
            // TODO - make sure there aren't more memory leaks here
            GameObject.Destroy(occupant.gameObject);
        }

        public void FlagOccupantForRemoval(Occupant occupant)
        {
            occupantsToRemove.Add(occupant);
        }
    }
}
