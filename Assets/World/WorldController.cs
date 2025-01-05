using System.Collections.Generic;
using TowerBuilder;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static float TILE_SIZE { get; } = 1f;

    // Prefabs
    public GameObject roomTilePlaceholderPrefab;
    public GameObject buildingPrefab;
    public GameObject roomPrefab;
    public GameObject roomTilePrefab;

    Transform buildingsContainer;
    GameObject blueprintTile;

    // State
    PrevAndCurrent<Tile> hoveredTile;
    public List<Building> buildings { get; private set; }

    void Awake()
    {
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;

        blueprintTile = Instantiate(roomTilePlaceholderPrefab, Vector3.zero, Quaternion.identity, buildingsContainer);

        // State
        hoveredTile = new PrevAndCurrent<Tile>(Tile.Zero(), Tile.Matches);
        buildings = new();
    }


    void Update()
    {
        UpdateCurrentTilePosition();
        HandleMouseInput();
    }

    void UpdateCurrentTilePosition()
    {
        var tile = mousePositionToTile();
        hoveredTile.Set(tile);

        if (hoveredTile.HasChanged())
        {
            blueprintTile.transform.position = tile.ToWorldPosition();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            AddRoomAtCurrentTile();
        }
    }

    void AddRoomAtCurrentTile()
    {
        var tile = mousePositionToTile();
        var allAdjacentTiles = tile.GetAdjacentTilesIncludingSelf();

        // Search for a building adjacent
        // TODO - combine buildings?
        var building = buildings.Find(building => building.ContainsRoomAtTile(allAdjacentTiles));

        if (building == null)
        {
            building = AddBuilding();
        }

        building.AddRoom(tile);
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

    public static WorldController Get()
    {
        return GameObject.Find("WorldController").GetComponent<WorldController>();
    }
}