using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class ToolsController
    {
        public PrevAndCurrent<Tool> tool { get; private set; } = new(Tool.Inspect);
        public PrevAndCurrent<RoomDefinition> selectedRoomDefinition { get; private set; } = new(RoomConstants.ALL_DEFINITIONS[0]);
        public Room blueprintRoom { get; private set; }
        public Room inspectedRoom { get; private set; }
        public Resident blueprintResident { get; private set; }

        public InspectTarget hoveredInspectTarget { get; private set; }
        public InspectTarget inspectTarget { get; private set; }

        WorldController worldController;

        int worldEntityLayerMask;

        public ToolsController(WorldController worldController)
        {
            // TODO - Probably should go somewhere else, since this is game-wide
            int worldEntityMaskLayerIndex = LayerMask.NameToLayer("World Entity");
            worldEntityLayerMask = 1 << worldEntityMaskLayerIndex;

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
            var hoveredTile = worldController.hoveredTile;
            var cursorIsOverUI = worldController.cursorIsOverUI;

            // TODO - Split into 'handle cursorIsOverUIChanged' and 'handleTileChanged'
            switch (tool.current)
            {
                case Tool.Build:
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
                    break;
                case Tool.Inspect:
                    CalculateInspectionHoverState();
                    break;
                case Tool.Destroy:
                    if (hoveredTile.HasChanged())
                    {
                        if (hoveredTile.prev != null)
                        {
                            // un-mark for deletion previous room
                            var room = worldController.buildingsController.FindFrontmostRoomAtTile(hoveredTile.prev);
                            room?.SetMarkedForDeletionState(false);
                        }

                        if (hoveredTile.current != null)
                        {
                            var room = worldController.buildingsController.FindFrontmostRoomAtTile(hoveredTile.current);
                            room?.SetMarkedForDeletionState(true);
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
                case Tool.Inspect:

                    // inspect current hovered target
                    if (hoveredInspectTarget != null)
                    {
                        // uninspect current inspected target, (if it exists)
                        TeardownInspectTarget();

                        // inspect hovered inspect target
                        inspectTarget = hoveredInspectTarget;

                        if (hoveredInspectTarget is ResidentInspectTarget)
                        {
                            (inspectTarget as ResidentInspectTarget).resident.SetInspectionHoveredState(false);
                            (inspectTarget as ResidentInspectTarget).resident.SetInspectedState(true);
                        }
                        else if (hoveredInspectTarget is RoomInspectTarget)
                        {
                            (inspectTarget as RoomInspectTarget).room.SetInspectionHoveredState(false);
                            (inspectTarget as RoomInspectTarget).room.SetInspectedState(true);
                        }

                        // TODO - building
                    }

                    break;
                case Tool.Destroy:
                    worldController.buildingsController.RemoveFrontmostRoomAtCurrentTile();
                    break;
                default:
                    break;
            }
        }

        public void SetTool(Tool newTool)
        {
            tool.Set(newTool);

            // Transition states
            if (tool.HasChanged())
            {
                // tear down previous tool
                switch (tool.prev)
                {
                    case Tool.Build:
                        // blueprintRoom will be null when the player hovers over the UI
                        if (blueprintRoom != null)
                        {
                            RemoveBlueprintRoom();
                        }
                        break;
                    case Tool.Inspect:
                        {
                            // teardown inspectedHoveredTarget
                            if (hoveredInspectTarget != null)
                            {
                                if (hoveredInspectTarget is ResidentInspectTarget)
                                {
                                    (hoveredInspectTarget as ResidentInspectTarget).resident.SetInspectionHoveredState(false);
                                }

                                hoveredInspectTarget = null;
                            }

                            TeardownInspectTarget();

                            break;
                        }
                    case Tool.Destroy:
                        {
                            var room = worldController.buildingsController.FindFrontmostRoomAtTile(worldController.hoveredTile.current);
                            room?.SetMarkedForDeletionState(false);
                            break;
                        }
                    default:
                        break;
                }

                // set up new tool
                switch (tool.current)
                {
                    case Tool.Build:
                        // avoid creating duplicate blueprint rooms
                        // a blueprint room will be created when the cursor leaves the UI so don't do it here
                        if (!worldController.cursorIsOverUI.current)
                        {
                            CreateAndInitializeBlueprintRoom();
                        }
                        break;
                    case Tool.Inspect:
                        {
                            CalculateInspectionHoverState();
                            break;
                        }
                    case Tool.Destroy:
                        {
                            var room = worldController.buildingsController.FindFrontmostRoomAtTile(worldController.hoveredTile.current);
                            room?.SetMarkedForDeletionState(true);
                            break;
                        }
                    default:
                        break;
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

        void CalculateInspectionHoverState()
        {
            // Determine what "world entity" is being hovered over currently
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, worldEntityLayerMask))
            {
                var tag = hit.transform.tag;

                switch (tag)
                {
                    case "Resident":
                        {
                            var resident = hit.transform.GetComponent<Resident>();

                            // Don't do anything if this resident is the current inspect hover target
                            if (
                                !(hoveredInspectTarget is ResidentInspectTarget && (hoveredInspectTarget as ResidentInspectTarget).resident == resident)
                            )
                            {
                                TeardownHoveredInspectTarget();

                                hoveredInspectTarget = new ResidentInspectTarget(resident);
                                resident.SetInspectionHoveredState(true);
                            }

                            break;
                        }
                    case "RoomTile":
                        {
                            // Debug.Log("it is a room tile");
                            var roomTile = hit.transform.GetComponent<RoomTile>();
                            var room = roomTile.room;

                            // Don't do anything if this room is currently being hovered over
                            if (
                                hoveredInspectTarget == null ||
                                !(hoveredInspectTarget is RoomInspectTarget && (hoveredInspectTarget as RoomInspectTarget).room == room)
                            )
                            {
                                TeardownHoveredInspectTarget();

                                hoveredInspectTarget = new RoomInspectTarget(room);
                                room.SetInspectionHoveredState(true);
                            }

                            break;
                        }
                    default:
                        break;
                }
            }
            else
            {
                // nothing is being hovered over - unset hoveredInspectTarget if it is not null
                TeardownHoveredInspectTarget();
            }
        }

        void TeardownHoveredInspectTarget()
        {
            //
            if (hoveredInspectTarget != null)
            {
                if (hoveredInspectTarget is ResidentInspectTarget)
                {
                    (hoveredInspectTarget as ResidentInspectTarget).resident.SetInspectionHoveredState(false);
                }
                else if (hoveredInspectTarget is RoomInspectTarget)
                {
                    (hoveredInspectTarget as RoomInspectTarget).room.SetInspectionHoveredState(false);
                }

                hoveredInspectTarget = null;
            }
        }

        void TeardownInspectTarget()
        {
            //
            if (inspectTarget != null)
            {
                if (inspectTarget is ResidentInspectTarget)
                {
                    (inspectTarget as ResidentInspectTarget).resident.SetInspectedState(false);
                }
                else if (inspectTarget is RoomInspectTarget)
                {
                    (inspectTarget as RoomInspectTarget).room.SetInspectedState(false);
                }

                inspectTarget = null;
            }
        }
    }
}