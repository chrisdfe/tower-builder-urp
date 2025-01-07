using Unity.VisualScripting;
using UnityEngine;

namespace TowerBuilder
{
    public class ToolsController
    {
        public PrevAndCurrent<Tool> tool { get; private set; } = new(Tool.Inspect);
        public PrevAndCurrent<RoomDefinition> selectedRoomDefinition { get; private set; } = new(RoomData.ALL_DEFINITIONS[0]);
        public Room blueprintRoom { get; private set; }

        WorldController worldController;

        public ToolsController(WorldController worldController)
        {
            this.worldController = worldController;

            // State
            if (tool.current == Tool.Build)
            {
                CreateAndInitializeBlueprintRoom();
            }
        }

        //
        // Lifecycle
        //
        public void OnUpdate()
        {
            switch (tool.current)
            {
                case Tool.Build:
                    if (worldController.cursorIsOverUI.HasChanged())
                    {
                        if (worldController.cursorIsOverUI.current)
                        {
                            RemoveBlueprintRoom();
                        }
                        else
                        {
                            CreateAndInitializeBlueprintRoom();
                        }
                    }
                    // This seems to happen on the frame after Instantiating the blueprint room
                    else if (blueprintRoom != null)
                    {
                        if (worldController.hoveredTile.HasChanged())
                        {
                            blueprintRoom.SetOriginTile(worldController.hoveredTile.current);
                            blueprintRoom.SetZPosition();
                            ValidateBlueprintRoom();
                        }
                    }
                    break;
                case Tool.Inspect:
                    break;
                case Tool.Destroy:
                    var hoveredTile = worldController.hoveredTile;
                    if (hoveredTile.HasChanged())
                    {
                        if (hoveredTile.prev != null)
                        {
                            // un-mark for deletion previous room
                            var room = worldController.buildingsController.FindFrontmostRoomAtTile(hoveredTile.prev);
                            if (room != null)
                            {
                                room.SetMarkedForDeletionState(false);
                            }
                        }

                        if (hoveredTile.current != null)
                        {
                            var room = worldController.buildingsController.FindFrontmostRoomAtTile(hoveredTile.current);
                            if (room != null)
                            {
                                room.SetMarkedForDeletionState(true);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        //
        // Public interface
        // 
        public void OnMouseUp()
        {
            switch (tool.current)
            {
                case Tool.Build:
                    worldController.buildingsController.AddRoomAtCurrentTileIfValid();
                    break;
                default:
                    worldController.buildingsController.RemoveFrontmostRoomAtCurrentTile();
                    break;
            }
        }

        public void SetTool(Tool newTool)
        {
            tool.Set(newTool);

            // Transition states
            if (tool.HasChanged())
            {
                if (
                    tool.prev == Tool.Build &&
                    // blueprintRoom will be null when the player hovers over the UI
                    blueprintRoom != null
                )
                {
                    RemoveBlueprintRoom();
                }
                else if (
                    tool.current == Tool.Build &&
                    // avoid creating duplicate blueprint rooms
                    // a blueprint room will be created when the cursor leaves the UI so don't do it here
                    !worldController.cursorIsOverUI.current
                )
                {
                    CreateAndInitializeBlueprintRoom();
                }
            }
        }

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
                foreach (var definition in RoomData.ALL_DEFINITIONS)
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