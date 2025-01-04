using System.Collections.Generic;
using TowerBuilder;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    static float TILE_SIZE = 1f;

    public GameObject roomTilePlaceholderPrefab;
    public GameObject buildingPrefab;
    public GameObject roomPrefab;

    Transform buildingsContainer;
    GameObject blueprintTile;

    // State
    PrevAndCurrent<Tile> hoveredTile;
    public List<Building> buildings { get; private set; }

    void Awake()
    {
        buildingsContainer = GameObject.Find("BuildingsContainer").transform;

        hoveredTile = new PrevAndCurrent<Tile>(Tile.Zero());
        blueprintTile = GameObject.Instantiate(roomTilePlaceholderPrefab, Vector3.zero, Quaternion.identity, buildingsContainer);
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
            blueprintTile.transform.position = tile.ToVector();
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

        // Search for a building adjacent

        // if there is no building, then add one

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