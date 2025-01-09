using UnityEngine;

namespace TowerBuilder
{
    public class BuildTool : ITool
    {
        public Room blueprintRoom { get; private set; } = null;
        public PrevAndCurrent<RoomDefinition> selectedRoomDefinition { get; private set; } = new(RoomConstants.ALL_DEFINITIONS[0]);

        WorldController worldController;

        public BuildTool(WorldController worldController)
        {
            this.worldController = worldController;
        }

        //
        // Lifecycle/handlers
        //
        public void OnMouseUp()
        {
            //
            worldController.buildingsController.AddRoomAtCurrentTileIfValid();
        }

        public void Teardown()
        {
            //
            // blueprintRoom will be null when the player hovers over the UI
            if (blueprintRoom != null)
            {
                RemoveBlueprintRoom();
            }
        }

        public void Setup()
        {
            // avoid creating duplicate blueprint rooms
            // a blueprint room will be created when the cursor leaves the UI so don't do it here
            if (!worldController.cursorIsOverUI.current)
            {
                CreateAndInitializeBlueprintRoom();
            }
        }

        public void OnUpdate()
        {
            var hoveredTile = worldController.hoveredTile;
            var cursorIsOverUI = worldController.cursorIsOverUI;

            if (cursorIsOverUI.HasChanged())
            {
                if (cursorIsOverUI.current)
                {
                    RemoveBlueprintRoom();
                }
                else
                {
                    CreateAndInitializeBlueprintRoom();
                }
            }
            else if (hoveredTile.HasChanged())
            {
                // This seems to happen on the frame after Instantiating the blueprint room
                if (blueprintRoom != null)
                {
                    blueprintRoom.SetOriginTile(worldController.hoveredTile.current);
                    blueprintRoom.SetZPosition();
                    ValidateBlueprintRoom();
                }
            }
        }

        //
        // Public interface
        //
        public void SetSelectedRoomDefinition(string title)
        {
            var newRoomDefinition = FindDefinition();
            selectedRoomDefinition.Set(newRoomDefinition);

            // update blueprint to use new room definition - just delete/create a new one for now
            if (selectedRoomDefinition.HasChanged())
            {
                if (!worldController.cursorIsOverUI.current)
                {
                    RemoveBlueprintRoom();
                    CreateAndInitializeBlueprintRoom();
                }
            }

            RoomDefinition FindDefinition()
            {
                foreach (var definition in RoomConstants.ALL_DEFINITIONS)
                {
                    if (definition.title == title)
                    {
                        return definition;
                    }
                }
                return null;
            }
        }



        //
        // Private interface
        //
        void CreateAndInitializeBlueprintRoom()
        {
            blueprintRoom = CreateBlueprintRoom();
            ValidateBlueprintRoom();
        }

        Room CreateBlueprintRoom()
        {
            var tile = worldController.mousePositionToTile();
            var position = tile.ToWorldPosition();

            var roomGameObject = GameObject.Instantiate(worldController.roomPrefab, position, Quaternion.identity, worldController.transform);
            roomGameObject.name = "Blueprint Room";
            var blueprintRoom = roomGameObject.GetComponent<Room>();

            // Initialize room
            blueprintRoom.definition = selectedRoomDefinition.current;
            blueprintRoom.CalculateAndInstantiateTilesFromOriginTile(tile);
            blueprintRoom.SetBlueprintState(true);

            return blueprintRoom;
        }

        void RemoveBlueprintRoom()
        {
            GameObject.Destroy(blueprintRoom.gameObject);
            blueprintRoom = null;
        }

        void ValidateBlueprintRoom()
        {
            var isValid = GetValid();
            blueprintRoom.SetValidState(isValid);

            bool GetValid()
            {
                // Validate overlap
                foreach (var building in worldController.buildingsController.buildings)
                {
                    foreach (var otherRoom in building.rooms)
                    {
                        if (
                            otherRoom.ContainsTile(blueprintRoom.tiles.ToArray()) &&
                            // rooms of different layers can be built on top of each other obviously
                            blueprintRoom.definition.layer == otherRoom.definition.layer
                        )
                        {
                            return false;
                        }
                    }
                }

                // 
                return true;
            }
        }


    }
}