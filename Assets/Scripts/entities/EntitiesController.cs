using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerBuilder
{
    public class EntitiesController : MonoBehaviour
    {
        public GameObject entityPlaceholderPrefab;

        static float TILE_SIZE = 1f;

        Transform entitiesContainer;
        Transform placeholderTile;

        public PrevAndCurrent<Tile> hoveredTile;

        // TODO - List<Entity>

        void Awake()
        {
            entitiesContainer = GameObject.Find("EntitiesContainer").transform;
            placeholderTile = entitiesContainer.Find("PlaceholderTile").transform;

            hoveredTile = new PrevAndCurrent<Tile>(Tile.Zero());
        }

        void Update()
        {
            UpdateCurrentTilePosition();
            HandleClick();
        }

        void UpdateCurrentTilePosition()
        {
            var tile = mousePositionToTile();
            hoveredTile.Set(tile);

            if (hoveredTile.HasChanged())
            {
                placeholderTile.position = tile.ToVector();
            }
        }

        void HandleClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                AddEntityAtCurrentTile();
            }
        }

        void AddEntityAtCurrentTile()
        {
            Instantiate(entityPlaceholderPrefab);
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
    }

    // TODO - implement IEnumerale?
    public class TileGroup
    {
        public List<Tile> tiles { get; }
        // TODO - add HashMap keyed by coordinates

        public TileGroup()
        {
            tiles = new List<Tile>();
        }

        public bool Add(Tile tile)
        {
            // TODO - validation
            tiles.Add(tile);
            return true;
        }
    }

    public struct Tile
    {
        public int x;
        public int y;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Tile other)
        {
            return x == other.x && y == other.y;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, 0);
        }

        public static Tile Zero()
        {
            return new Tile(0, 0);
        }
    }

    public class Entity
    {
        int layer;
        TileGroup tiles;
    }

    public class PrevAndCurrent<T>
    {

        T prev;
        T current;

        public PrevAndCurrent(T initialValue)
        {
            prev = initialValue;
            current = initialValue;
        }

        public void Set(T newValue)
        {
            prev = current;
            current = newValue;
        }

        public bool HasChanged()
        {
            return EqualityComparer<T>.Default.Equals(prev, current);
        }
    }
}