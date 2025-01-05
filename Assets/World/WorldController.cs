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

    public Material blueprintValidRoomTileMaterial;
    public Material blueprintInvalidRoomTileMaterial;

    // Cached stuff from game world
    Transform buildingsContainer;

    // State
    PrevAndCurrent<Tile> hoveredTile;
    public List<Building> buildings { get; private set; }
    Room blueprintRoom;

    // Lifecycle
    void Awake()
    {
        // Cached stuff from game world
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;

        // State
        hoveredTile = new PrevAndCurrent<Tile>(Tile.Zero(), Tile.Matches);
        buildings = new();

        blueprintRoom = AddRoomAtCurrentTile();
        blueprintRoom.SetBlueprintState(true);
        ValidateRoom(blueprintRoom);
    }

    void Update()
    {
        UpdateCurrentTilePosition();
        HandleMouseInput();
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

    //
    // private methods
    //
    void UpdateCurrentTilePosition()
    {
        var tile = mousePositionToTile();
        hoveredTile.Set(tile);

        if (hoveredTile.HasChanged())
        {
            blueprintRoom.SetOriginTile(tile);
            ValidateRoom(blueprintRoom);
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            AddRoomAtCurrentTileIfValid();
        }
    }

    void AddRoomAtCurrentTileIfValid()
    {
        if (blueprintRoom.isValid)
        {
            AddRoomAtCurrentTile();
        }
    }

    Room AddRoomAtCurrentTile()
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

        var room = building.AddRoom(tile);
        return room;
    }

    Building AddBuilding()
    {
        var buildingGameObject = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity, buildingsContainer);
        var building = buildingGameObject.GetComponent<Building>();
        buildings.Add(building);
        return building;
    }

    void ValidateRoom(Room room)
    {
        var isValid = GetValid();
        room.SetValidState(isValid);

        bool GetValid()
        {
            // Validate overlap
            foreach (var building in buildings)
            {
                foreach (var otherRoom in building.rooms)
                {
                    if (room != otherRoom)
                    {
                        if (otherRoom.ContainsTile(room.tiles.ToArray()))
                        {
                            return false;
                        }
                    }
                }
            }

            // 
            return true;
        }
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