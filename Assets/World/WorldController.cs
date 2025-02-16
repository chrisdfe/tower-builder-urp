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
    public GameObject residentPrefab;

    public Material blueprintValidRoomTileMaterial;
    public Material blueprintInvalidRoomTileMaterial;

    // State
    public PrevAndCurrent<Tile> hoveredTile { get; private set; } = new PrevAndCurrent<Tile>(Tile.zero, Tile.Matches);

    public ToolsController toolsController { get; private set; }
    public BuildingsController buildingsController { get; private set; }
    public TimeController timeController { get; private set; }

    public PrevAndCurrent<bool> cursorIsOverUI { get; private set; } = new(false);

    public List<Notification> notifications { get; private set; } = new();

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

        if (Input.GetMouseButtonUp(0))
        {
            toolsController.OnMouseUp();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (toolsController.toolHandle.current == ToolHandle.Inspect && toolsController.inspectTool.inspectTarget != null)
            {
                toolsController.inspectTool.SetInspectTarget(null);
            }
            else if (toolsController.toolHandle.current != ToolHandle.Inspect)
            {
                toolsController.SetTool(ToolHandle.Inspect);
            }
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
    }

    void UpdateCurrentTilePosition()
    {
        var tile = mousePositionToTile();
        hoveredTile.Set(tile);
    }

    void OnTick()
    {
        buildingsController.OnTick();
    }

    //
    // Public interface
    //
    public void AddNotification(string message)
    {
        notifications.Add(new Notification(message));
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