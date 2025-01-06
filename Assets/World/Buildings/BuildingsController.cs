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
