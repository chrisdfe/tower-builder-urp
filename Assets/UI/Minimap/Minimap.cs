using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class Minimap : MonoBehaviour
    {
        class GridCoordinates
        {
            public int x;
            public int y;

            public GridCoordinates(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString() => $"{x}, {y}";

            public (int, int) AsTuple() => (x, y);

            public static GridCoordinates zero => new(0, 0);
        }

        class GridDimensions : Dimensions<int>
        {
            public GridDimensions(int width, int height) : base(width, height) { }
            public GridDimensions(int size) : base(size) { }
        }

        public Color skyColor;
        public Color groundColor;
        public Color roomColor;

        Image image;

        Texture2D texture;
        // TODO - right now textureDimensions width/height need to be divisible by tileSize 
        Dimensions<int> textureDimensions = new(512);
        int tileSize = 16;

        GridDimensions gridDimensions;
        // the position on the minimap of (0, 0)
        GridCoordinates originGridCoordinates;

        void Awake()
        {
            image = GetComponent<Image>();

            gridDimensions = new(textureDimensions.width / tileSize, textureDimensions.height / tileSize);
            originGridCoordinates = new(gridDimensions.width / 2, gridDimensions.height / 2);

        }

        void Start()
        {
            DrawTextureAndSetSprite();

            WorldController.Get().buildingsController.onRoomBuilt += OnRoomBuilt;
        }

        void OnDestroy()
        {
            WorldController.Get().buildingsController.onRoomBuilt -= OnRoomBuilt;
        }


        //
        // Event handlers
        //
        void OnRoomBuilt(Room room)
        {
            DrawRoom(room);
            texture.Apply();
        }

        //
        // Texture/tile drawing
        //
        void DrawTextureAndSetSprite()
        {
            DrawTexture();

            var sprite = Sprite.Create(
                texture,
                new(0, 0, textureDimensions.width, textureDimensions.height),
                new(textureDimensions.width / 2, textureDimensions.height / 2)
            );
            sprite.name = "Mipmap Sprite";
            image.sprite = sprite;
        }

        void DrawTexture()
        {
            texture = new Texture2D(textureDimensions.width, textureDimensions.height);

            DrawBackground();
            DrawGround();
            DrawRooms();
            // DrawDebugTiles();
            // FillTile(0, 0, Color.red);

            texture.Apply();
        }

        void DrawBackground()
        {
            // TODO - get actual color
            FillTiles(GridCoordinates.zero, gridDimensions, skyColor);
        }

        void DrawGround()
        {
            var groundCoordinates = new GridCoordinates(0, 0);
            var groundDimensions = new GridDimensions(gridDimensions.width, gridDimensions.height - originGridCoordinates.y);

            // texture.SetPixels(0, groundCoordinates.y, groundDimensions.width, groundDimensions.height, colors);
            FillTiles(groundCoordinates, groundDimensions, groundColor);
        }

        void DrawRooms()
        {
            foreach (var building in WorldController.Get().buildingsController.buildings)
            {
                foreach (var room in building.rooms)
                {
                    DrawRoom(room);
                }
            }
        }

        void DrawRoom(Room room)
        {
            foreach (var tile in room.tiles)
            {
                var coordinates = TileToGridCoordinates(tile);
                // TODO - custom color
                FillTile(coordinates, roomColor);
            }
        }

        GridCoordinates TileToGridCoordinates(Tile tile) =>
            new(
                originGridCoordinates.x + tile.x,
                originGridCoordinates.y + tile.y
            );

        void DrawDebugTiles()
        {
            for (var x = 0; x < gridDimensions.width; x++)
            {
                for (var y = 0; y < gridDimensions.height; y++)
                {
                    if (x % 4 == 0 || y % 4 == 0)
                    {
                        FillTile(new(x, y), Color.cyan);
                    }
                }
            }
        }

        void FillTile(GridCoordinates coordinates, Color color)
        {
            var (x, y) = coordinates.AsTuple();
            var colors = Enumerable.Repeat(0, tileSize * tileSize).Select((_) => color).ToArray();
            texture.SetPixels(x * tileSize, y * tileSize, tileSize, tileSize, colors);
        }

        // Note - parameters are in grid coordinates/dimensions not texture
        void FillTiles(GridCoordinates coordinates, GridDimensions dimensions, Color color)
        {
            var (x, y) = coordinates.AsTuple();
            var (width, height) = dimensions.AsTuple();

            var colors = CreateColorArray(color, width * tileSize * height * tileSize);

            texture.SetPixels(
                x * tileSize,
                y * tileSize,
                width * tileSize,
                height * tileSize,
                colors
            );
        }

        Color[] CreateColorArray(Color color, int size) => Enumerable.Repeat(0, size).Select((_) => color).ToArray();
    }
}