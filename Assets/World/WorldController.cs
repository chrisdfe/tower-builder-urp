using System.Collections.Generic;
using TowerBuilder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{
    public static float TILE_SIZE { get; } = 1f;

    // Prefabs
    public GameObject roomTilePlaceholderPrefab;
    public GameObject buildingPrefab;
    public GameObject roomPrefab;
    public GameObject roomTilePrefab;

    public Material blueprintValidRoomTileMaterial;
    public Material blueprintInvalidRoomTileMaterial;

    // State
    public PrevAndCurrent<Tile> hoveredTile { get; private set; } = new PrevAndCurrent<Tile>(Tile.Zero(), Tile.Matches);
    public List<Building> buildings { get; private set; } = new();
    public PrevAndCurrent<Tool> tool { get; private set; } = new(Tool.Inspect);
    public PrevAndCurrent<RoomDefinition> selectedRoomDefinition { get; private set; } = new(RoomDefinition.ALL_DEFINITIONS[0]);

    // TODO - figure out why default implementation of this doen't work
    public PrevAndCurrent<bool> cursorIsOverUI { get; private set; } = new(false);

    // Other
    Transform buildingsContainer;
    Canvas canvas;
    EventSystem eventSystem;
    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;

    Room blueprintRoom;

    //
    // Lifecycle
    //
    void Awake()
    {
        // State
        if (tool.current == Tool.Build)
        {
            CreateAndInitializeBlueprintRoom();
        }

        // Other
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = canvas.GetComponent<EventSystem>();
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;
    }

    void Update()
    {
        CheckForCursorOverUI();
        HandleMouseInput();
        UpdateCurrentTilePosition();
        UpdateBlueprintRoom();
    }

    void CheckForCursorOverUI()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new();

        graphicRaycaster.Raycast(pointerEventData, results);

        cursorIsOverUI.Set(results.Count > 0);
    }

    void HandleMouseInput()
    {
        if (cursorIsOverUI.current)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (tool.current == Tool.Build)
            {
                AddRoomAtCurrentTileIfValid();
            }
        }
    }

    void UpdateCurrentTilePosition()
    {
        var tile = mousePositionToTile();
        hoveredTile.Set(tile);
    }

    void UpdateBlueprintRoom()
    {
        if (tool.current == Tool.Build)
        {
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
            // This seems to happen on the frame after Instantiating the blueprint room
            else if (blueprintRoom != null)
            {
                if (hoveredTile.HasChanged())
                {
                    blueprintRoom.SetOriginTile(hoveredTile.current);
                    ValidateBlueprintRoom();
                }
            }
        }
    }

    //
    // public interface
    //
    public uint RoomsCount()
    {
        uint result = 0;

        foreach (var building in buildings)
        {
            result += (uint)building.rooms.Count;
        }

        return result;
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
                !cursorIsOverUI.current
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
            if (!cursorIsOverUI.current)
            {
                RemoveBlueprintRoom();
                CreateAndInitializeBlueprintRoom();
            }
        }

        RoomDefinition FindDefinition()
        {
            foreach (var definition in RoomDefinition.ALL_DEFINITIONS)
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
    // private methods
    //
    void CreateAndInitializeBlueprintRoom()
    {
        blueprintRoom = CreateBlueprintRoom();
        ValidateBlueprintRoom();
    }

    void RemoveBlueprintRoom()
    {
        Destroy(blueprintRoom.gameObject);
        blueprintRoom = null;
    }

    void AddRoomAtCurrentTileIfValid()
    {
        if (!blueprintRoom.isValid)
        {
            return;
        }

        var tile = mousePositionToTile();
        var allAdjacentTiles = tile.GetAdjacentTilesIncludingSelf();

        // Search for a building adjacent
        // TODO - combine buildings?
        var building = buildings.Find(building => building.ContainsRoomAtTile(allAdjacentTiles));

        if (building == null)
        {
            building = AddBuilding();
        }

        building.AddRoom(tile, selectedRoomDefinition.current);
    }

    Room CreateBlueprintRoom()
    {
        var tile = mousePositionToTile();
        var position = tile.ToWorldPosition();

        var roomGameObject = Instantiate(roomPrefab, position, Quaternion.identity, transform);
        roomGameObject.name = "Blueprint Room";
        var blueprintRoom = roomGameObject.GetComponent<Room>();

        // Initialize room
        blueprintRoom.definition = selectedRoomDefinition.current;
        blueprintRoom.CalculateAndInstantiateTilesFromOriginTile(tile);

        blueprintRoom.SetBlueprintState(true);

        return blueprintRoom;
    }

    void ValidateBlueprintRoom()
    {
        var isValid = GetValid();
        blueprintRoom.SetValidState(isValid);

        bool GetValid()
        {
            // Validate overlap
            foreach (var building in buildings)
            {
                foreach (var otherRoom in building.rooms)
                {
                    if (otherRoom.ContainsTile(blueprintRoom.tiles.ToArray()))
                    {
                        return false;
                    }
                }
            }

            // 
            return true;
        }
    }

    Building AddBuilding()
    {
        var buildingGameObject = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity, buildingsContainer);
        var building = buildingGameObject.GetComponent<Building>();
        buildings.Add(building);
        return building;
    }

    Tile mousePositionToTile()
    {
        var mousePosition = Input.mousePosition;
        var screenPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var tile = new Tile(
            (int)(Mathf.Round(screenPosition.x) / TILE_SIZE * TILE_SIZE),
            (int)(Mathf.Round(screenPosition.y) / TILE_SIZE * TILE_SIZE)
        );

        return tile;
    }


    //
    // Static interface
    //

    // Since there is only ever 1 WorldController this is fine.
    static WorldController worldController;
    public static WorldController Get()
    {
        if (worldController == null)
        {
            worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
        }

        return worldController;
    }
}