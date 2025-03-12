using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class BuildTool : ITool
    {
        public Room blueprintRoom { get; private set; } = null;
        public PrevAndCurrent<RoomDefinition> selectedRoomDefinition { get; private set; }

        WorldController worldController;
        BuildToolTooltipManager buildToolTooltipManager;

        public List<RoomDefinition> roomDefinitions { get; private set; }

        // After building a room, pause briefly allowing player to build again
        const float POST_BUILD_LOCK_LENGTH_S = 0.25f;
        bool isPostBuildLocked = false;

        public BuildTool(WorldController worldController)
        {
            this.worldController = worldController;

            SetupRoomDefinitions();
            selectedRoomDefinition = new(roomDefinitions[0]);
        }

        //
        // Lifecycle/handlers
        //
        public void OnLeftMouseUp()
        {
            if (isPostBuildLocked) return;
            //
            BuildRoom();
        }

        public void Setup()
        {
            worldController.timeController.Pause();
            SetExtraMouseOffset();

            if (buildToolTooltipManager == null)
            {
                buildToolTooltipManager = BuildToolTooltipManager.Get();
            }

            buildToolTooltipManager.ShowTooltip();

            // avoid creating duplicate blueprint rooms
            // a blueprint room will be created when the cursor leaves the UI so don't do it here
            if (!worldController.cursorIsOverUI.current)
            {
                CreateAndInitializeBlueprintRoom();
            }
        }

        public void Teardown()
        {
            worldController.timeController.UnPause();
            worldController.mousePositionExtraOffset = Vector2.zero;

            buildToolTooltipManager.HideTooltip();

            // blueprintRoom will be null when the player hovers over the UI
            if (blueprintRoom != null)
            {
                RemoveBlueprintRoom();
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
                    buildToolTooltipManager.HideTooltip();
                }
                else
                {
                    CreateAndInitializeBlueprintRoom();
                    buildToolTooltipManager.ShowTooltip();
                }
            }
            else if (hoveredTile.HasChanged())
            {
                // This seems to happen on the frame after Instantiating the blueprint room
                if (blueprintRoom != null)
                {
                    blueprintRoom.SetOriginTile(worldController.hoveredTile.current);
                    blueprintRoom.SetZPosition();
                    blueprintRoom.Validate(worldController);

                    UpdateTooltip();
                }
            }

            void UpdateTooltip()
            {
                if (blueprintRoom == null) return;

                if (blueprintRoom.isValid)
                {
                    buildToolTooltipManager.SetTooltipState(BuildToolTooltip.State.Valid);
                    buildToolTooltipManager.SetTooltipText(Money.Format(blueprintRoom.definition.price));
                }
                else
                {
                    buildToolTooltipManager.SetTooltipState(BuildToolTooltip.State.Invalid);
                    buildToolTooltipManager.SetTooltipText(blueprintRoom.buildValidationErrors[0].message);
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
                SetExtraMouseOffset();

                if (!worldController.cursorIsOverUI.current)
                {
                    RemoveBlueprintRoom();
                    CreateAndInitializeBlueprintRoom();
                }
            }

            RoomDefinition FindDefinition()
            {
                foreach (var definition in roomDefinitions)
                {
                    if (definition.title == title)
                    {
                        return definition;
                    }
                }

                return null;
            }
        }

        public List<RoomDefinition> SetupRoomDefinitions()
        {
            if (roomDefinitions == null)
            {
                roomDefinitions = new List<RoomDefinition>(RoomConstants.ALL_DEFINITIONS);
                // TODO - if debug mode
                roomDefinitions.AddRange(RoomConstants.DEBUG_ROOM_DEFINITIONS);
            }

            return roomDefinitions;
        }

        //
        // Private interface
        //
        void BuildRoom()
        {
            var roomWasBuilt = worldController.buildingsController.AddRoomAtCurrentTileIfValid();

            if (roomWasBuilt)
            {
                // Lock 
                worldController.StartCoroutine(AnimateAndWait());
            }
            else
            {
                buildToolTooltipManager.PlayInvalidRoomAnimation();
            }

            IEnumerator AnimateAndWait()
            {
                buildToolTooltipManager.PlayFloatingAnimationThenDestroy();
                isPostBuildLocked = true;
                // TODO - just hide it
                RemoveBlueprintRoom();

                yield return new WaitForSeconds(POST_BUILD_LOCK_LENGTH_S);

                buildToolTooltipManager.ShowTooltip();
                isPostBuildLocked = false;
                CreateAndInitializeBlueprintRoom();
            }
        }

        void CreateAndInitializeBlueprintRoom()
        {
            blueprintRoom = CreateBlueprintRoom();
            blueprintRoom.Validate(worldController);
        }

        Room CreateBlueprintRoom()
        {
            var tile = worldController.GetMousePositionToTile();
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

        void SetExtraMouseOffset()
        {
            worldController.mousePositionExtraOffset = -1 * TileList.GetRelativeCenterPoint(selectedRoomDefinition.current.shape);
        }
    }
}