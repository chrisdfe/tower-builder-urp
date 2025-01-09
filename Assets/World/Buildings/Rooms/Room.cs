using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public class Room : MonoBehaviour
    {

        public RoomDefinition definition;
        public string title;
        public List<Tile> tiles { get; private set; } = new List<Tile>() { Tile.Zero() };

        // Residents that live in this room
        public List<Resident> residents { get; private set; } = new List<Resident>();

        // Residents that work in this room
        public List<Resident> workers { get; private set; } = new List<Resident>();

        // TODO - should be a list of validation errors
        public bool isValid { get; private set; } = true;

        public bool isBlueprint { get; private set; } = false;

        // player is hovering over the room with the destoy tool
        public bool isMarkedForDeletion { get; private set; } = false;

        // player is hovering over the room with the inspect tool
        public bool isInspectionHovered { get; private set; } = false;

        // this is the room that is being inspected with the inspect tool
        public bool isInspected { get; private set; } = false;

        // Prefabs
        public GameObject roomTilePrefab;

        List<RoomTile> roomTiles = new();
        Tile originTile;

        // TODO - this isn't going to work for resizable rooms
        // TODO - make the originTile the center tile instead of bottom left
        public void CalculateAndInstantiateTilesFromOriginTile(Tile originTile)
        {
            this.originTile = originTile;

            // Calculate
            CalcluateTilesFromOrigin();

            // Instantiate tiles
            foreach (var tile in tiles)
            {
                var roomTileGameObject = Instantiate(roomTilePrefab, tile.ToWorldPosition(), Quaternion.identity, transform);
                var roomTile = roomTileGameObject.GetComponent<RoomTile>();
                roomTile.room = this;
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

        public void UpdateColor()
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
                    roomTile.SetMaterial(blueprintMaterial);
                }
            }
            else
            {
                Color color;
                if (isMarkedForDeletion)
                {
                    color = RoomConstants.ROOM_MARKED_FOR_DELETION_COLOR;
                }
                else if (isInspectionHovered)
                {
                    color = RoomConstants.ROOM_INSPECTION_HOVERED_COLOR;
                }
                else if (isInspected)
                {
                    color = RoomConstants.ROOM_INSPECTED_COLOR;
                }
                else
                {
                    // Default to room definition color
                    color = RoomConstants.ROOM_TYPE_COLORS[definition.type];
                }

                foreach (var roomTile in roomTiles)
                {
                    roomTile.SetColor(color);
                }
            }
        }

        public void SetZPosition()
        {
            float z;
            if (isBlueprint)
            {
                z = -RoomConstants.BLUEPRINT_Z_OFFSET;
            }
            else
            {
                z = RoomConstants.ROOM_LAYER_Z_OFFSETS[definition.layer] * -1;
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
            UpdateColor();
            SetZPosition();
        }

        public void SetValidState(bool isValid)
        {
            this.isValid = isValid;
            UpdateColor();
        }

        public void SetMarkedForDeletionState(bool isMarkedForDeletion)
        {
            this.isMarkedForDeletion = isMarkedForDeletion;
            UpdateColor();
        }

        public void SetInspectionHoveredState(bool isInspectionHovered)
        {
            this.isInspectionHovered = isInspectionHovered;
            UpdateColor();
        }

        public void SetInspectedState(bool isInspected)
        {
            this.isInspected = isInspected;
            UpdateColor();
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
    }
}