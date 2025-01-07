using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Room : MonoBehaviour
    {
        public RoomDefinition definition { get; set; }
        public List<Tile> tiles { get; private set; } = new List<Tile>() { Tile.Zero() };
        public bool isBlueprint { get; private set; } = false;
        // TODO - should be a list of validation errors
        public bool isValid { get; private set; } = true;

        List<GameObject> roomTiles = new();
        Tile originTile;

        // TODO - this isn't going to work for resizable rooms
        // TODO - make the originTile the center tile instead of bottom left
        public void CalculateAndInstantiateTilesFromOriginTile(Tile originTile)
        {
            this.originTile = originTile;

            // Calculate
            CalcluateTilesFromOrigin();

            // Instantiate tiles
            var roomTilePrefab = WorldController.Get().roomTilePrefab;
            foreach (var tile in tiles)
            {
                var roomTile = Instantiate(roomTilePrefab, tile.ToWorldPosition(), Quaternion.identity, transform);
                roomTiles.Add(roomTile);
            }
        }

        public void SetOriginTile(Tile originTile)
        {
            //
            this.originTile = originTile;

            CalcluateTilesFromOrigin();

            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                var roomTile = roomTiles[i];
                // 
                roomTile.transform.position = tile.ToWorldPosition();
            }
        }

        public void SetColor()
        {
            if (isBlueprint)
            {
                var blueprintMaterial = WorldController.Get().blueprintValidRoomTileMaterial;
                if (isBlueprint)
                {
                    foreach (var roomTile in roomTiles)
                    {
                        roomTile.GetComponent<MeshRenderer>().material = blueprintMaterial;
                    }
                }
            }
            else
            {
                // Use room definition color
                var color = RoomData.ROOM_TYPE_COLORS[definition.type];
                foreach (var roomTile in roomTiles)
                {
                    roomTile.GetComponent<MeshRenderer>().material.color = color;
                }
            }
        }

        public void SetZPosition()
        {
            Debug.Log("setting z position");

            float z;
            if (isBlueprint)
            {
                z = -RoomData.BLUEPRINT_Z_OFFSET;
            }
            else
            {
                z = RoomData.ROOM_LAYER_Z_OFFSETS[definition.layer] * -1;
            }

            foreach (var tile in roomTiles)
            {
                tile.transform.position = new Vector3(
                    tile.transform.position.x,
                    tile.transform.position.y,
                    z
                );
            }
        }

        // This assumes this room and its roomTiles have been instantiated
        public void SetBlueprintState(bool isBlueprint)
        {
            this.isBlueprint = isBlueprint;
            SetColor();
            SetZPosition();
        }

        public void SetValidState(bool isValid)
        {
            this.isValid = isValid;

            // TODO - can a room be both !isBlueprint and !isValid?
            if (isBlueprint)
            {
                UpdateBlueprintMaterial();
            }
        }

        public bool ContainsTile(Tile tile)
        {
            foreach (Tile t in tiles)
            {
                if (tile.Matches(t))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsTile(Tile[] targetTiles)
        {
            foreach (Tile tile in tiles)
            {
                foreach (Tile targetTile in targetTiles)
                {
                    if (tile.Matches(targetTile))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        void CalcluateTilesFromOrigin()
        {
            List<Tile> result = new();

            foreach (var tile in definition.shape)
            {
                var newTile = new Tile(originTile.x + tile.x, originTile.y + tile.y);
                result.Add(newTile);
            }

            tiles = result;
        }

        void UpdateBlueprintMaterial()
        {
            if (isBlueprint)
            {

                Material blueprintMaterial;
                if (isValid)
                {
                    blueprintMaterial = WorldController.Get().blueprintValidRoomTileMaterial;
                }
                else
                {
                    blueprintMaterial = WorldController.Get().blueprintInvalidRoomTileMaterial;
                }

                foreach (var roomTile in roomTiles)
                {
                    roomTile.GetComponent<MeshRenderer>().material = blueprintMaterial;
                }
            }
        }
    }
}