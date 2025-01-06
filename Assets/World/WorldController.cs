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
    public ToolsController toolsController { get; private set; }

    // TODO - figure out why default implementation of this doen't work
    public PrevAndCurrent<bool> cursorIsOverUI { get; private set; } = new(false);

    // Other
    Transform buildingsContainer;
    Canvas canvas;
    EventSystem eventSystem;
    GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;


    //
    // Lifecycle
    //
    void Awake()
    {
        toolsController = new ToolsController(this);

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

        toolsController.OnUpdate();
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
            if (toolsController.tool.current == Tool.Build)
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

    public Tile mousePositionToTile()
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
    // Private interface 
    //
    void AddRoomAtCurrentTileIfValid()
    {
        if (!toolsController.blueprintRoom.isValid)
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

        building.AddRoom(tile, toolsController.selectedRoomDefinition.current);
    }

    Building AddBuilding()
    {
        var buildingGameObject = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity, buildingsContainer);
        var building = buildingGameObject.GetComponent<Building>();
        buildings.Add(building);
        return building;
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