using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class BuildingsController
    {
        public List<Building> buildings { get; private set; } = new();

        WorldController worldController;

        Transform buildingsContainer;

        public BuildingsController(WorldController worldController)
        {
            this.worldController = worldController;

            buildingsContainer = GameObject.Find("BuildingsContainer").transform;
        }

        //
        // Public interface
        //
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
                building = AddBuilding();
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
        Building AddBuilding()
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
    }
}
