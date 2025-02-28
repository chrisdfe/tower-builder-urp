using System.Collections.Generic;
using System.Linq;
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
    public GameObject occupantPrefab;
    public GameObject debugTileSquarePrefab;

    // State

    public PrevAndCurrent<Tile> hoveredTile { get; private set; } = new PrevAndCurrent<Tile>(Tile.zero, Tile.Matches);

    // affects the mouse position used to determine hoveredTile
    // used for centering blueprint room etc
    // TODO - could have a better name
    public Vector2 mousePositionExtraOffset = Vector2.zero;

    public ToolsController toolsController { get; private set; }
    public BuildingsController buildingsController { get; private set; }
    public TimeController timeController { get; private set; }

    public PrevAndCurrent<bool> cursorIsOverUI { get; private set; } = new(false);

    public List<Notification> notifications { get; private set; } = new();

    // Debug state
    public List<GameObject> debugTileSquares = new();

    // Other
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
        buildingsController = new BuildingsController(this);
        timeController = new TimeController(this);

        // Other
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = canvas.GetComponent<EventSystem>();
    }

    void Start()
    {
        // this won't ultimately be here - just for debug reasons
        var entranceExitRoomDefinition = RoomConstants.ALL_DEFINITIONS.ToList().Find(definition => definition.title == "Entrance/Exit");
        buildingsController.AddRoomAtTile(entranceExitRoomDefinition, Tile.zero);
    }

    void Update()
    {
        UpdateTime();
        CheckForCursorOverUI();
        HandleMouseInput();
        HandleKeyboardInput();
        UpdateCurrentTilePosition();

        toolsController.OnUpdate();
    }

    void UpdateTime()
    {
        timeController.OnUpdate();

        if (timeController.tick.HasChanged())
        {
            OnTick();
        }
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

        // left mouse button
        if (Input.GetMouseButtonUp(0))
        {
            toolsController.OnLeftMouseUp();
        }

        // left mouse up
        if (Input.GetMouseButtonUp(1))
        {
            toolsController.OnRightMouseUp();
        }
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            toolsController.SetTool(ToolHandle.Inspect);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            toolsController.SetTool(ToolHandle.Build);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            toolsController.SetTool(ToolHandle.Destroy);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (toolsController.toolHandle.current == ToolHandle.Inspect)
            {
                if (toolsController.inspectTool.inspectTarget != null)
                {
                    toolsController.inspectTool.SetInspectTarget(null);
                }
            }
            else
            {
                toolsController.SetTool(ToolHandle.Inspect);
            }
        }

        // DEBUG
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (debugTileSquares.Count == 0)
            {
                if (toolsController.toolHandle.current == ToolHandle.Inspect && toolsController.inspectTool.inspectTarget is Room)
                {
                    var roomInspectTarget = toolsController.inspectTool.inspectTarget as Room;
                    var building = buildingsController.FindBuildingByRoom(roomInspectTarget);
                    var roomGroup = building.FindRoomGroupByRoom(roomInspectTarget);

                    if (roomGroup != null)
                    {
                        var tiles = roomGroup.Aggregate(new List<Tile>(), (result, room) =>
                        {
                            foreach (var tile in room.tiles)
                            {
                                result.Add(tile);
                            }

                            return result;
                        });

                        CreateDebugTiles(tiles);
                    }
                    else
                    {
                        Debug.Log($"{roomInspectTarget} is not in a roomGroup");
                    }
                }
            }
            else
            {
                DestroyDebugTiles();
            }
        }

        // DEBUG
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (toolsController.toolHandle.current == ToolHandle.Inspect && toolsController.inspectTool.inspectTarget is Occupant)
            {
                var occupantInspectTarget = toolsController.inspectTool.inspectTarget as Occupant;

                occupantInspectTarget.CancelCurrentTask();
            }
        }
    }

    void UpdateCurrentTilePosition()
    {
        var tile = GetMousePositionToTile();
        hoveredTile.Set(tile);
    }

    void OnTick()
    {
        buildingsController.OnTick();
    }

    void CreateDebugTiles(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            var tileGo = Instantiate(debugTileSquarePrefab, tile.ToWorldPosition(), Quaternion.identity);
            debugTileSquares.Add(tileGo);
        }
    }

    void DestroyDebugTiles()
    {
        foreach (var go in debugTileSquares)
        {
            Destroy(go);
        }

        debugTileSquares = new();

    }

    //
    // Public interface
    //
    public void AddNotification(string message)
    {
        notifications.Add(new Notification(message));
    }

    public Tile GetMousePositionToTile()
    {
        var mousePosition = Input.mousePosition;
        var screenPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var screenPositionWithExtraOffset = new Vector2(
            screenPosition.x + mousePositionExtraOffset.x,
            screenPosition.y + mousePositionExtraOffset.y
        );

        var tile = new Tile(
            (int)(Mathf.Round(screenPositionWithExtraOffset.x) / TILE_SIZE * TILE_SIZE),
            (int)(Mathf.Round(screenPositionWithExtraOffset.y) / TILE_SIZE * TILE_SIZE)
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