using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace TowerBuilder
{
    public class Room : MonoBehaviour, IInspectTarget
    {

        public RoomDefinition definition;
        public string title { get; set; } = "Room";

        public List<Tile> tiles { get; private set; } = new List<Tile>() { Tile.zero };

        // Occupants that live in this room
        public List<Occupant> residents { get; private set; } = new List<Occupant>();

        // Occupants that work in this room
        public List<Occupant> workers { get; private set; } = new List<Occupant>();

        // TODO - should be a list of validation errors
        public bool isValid { get; private set; } = true;

        public bool isBlueprint { get; private set; } = false;

        // player is hovering over the room with the destoy tool
        public bool isMarkedForDeletion { get; private set; } = false;

        // player is hovering over the room with the inspect tool
        public bool isInspectionHovered { get; private set; } = false;

        // this is the room that is being inspected with the inspect tool
        public bool isInspected { get; private set; } = false;

        public List<RoomValidationError> buildValidationErrors { get; private set; } = new();

        // Prefabs
        public GameObject roomTilePrefab;

        List<RoomTile> roomTiles = new();
        Tile originTile;

        //
        // Public interface
        //

        //
        // Write methods
        //

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
                roomTile.SetTile(tile);
                roomTile.CalculateSegmentsFromTileList(tiles);
                roomTiles.Add(roomTile);
            }
        }

        public void CalculateSegmentsFromTileList(List<Tile> tiles)
        {
            foreach (var roomTile in roomTiles)
            {
                roomTile.CalculateSegmentsFromTileList(tiles);
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

                // TODO - something about this function seems incomplete
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
                else if (isInspected)
                {
                    color = RoomConstants.ROOM_INSPECTED_COLOR;
                }
                else if (isInspectionHovered)
                {
                    color = RoomConstants.ROOM_INSPECTION_HOVERED_COLOR;
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

        public void Validate(WorldController worldController)
        {
            List<RoomValidationError> errors = new();

            foreach (var buildValidator in definition.buildValidators)
            {
                //
                var error = buildValidator.Validate(this, worldController);

                if (error != null)
                {
                    errors.Add(error);
                }
            }

            this.buildValidationErrors = errors;
            this.isValid = errors.Count == 0;

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

        public void SetRoomTileSegmentVariant(RoomTileSegment roomTileSegment, string variant)
        {
            foreach (var roomTile in roomTiles)
            {
                roomTile.SetSegmentVariant(roomTileSegment, variant);
            }
        }

        //
        // Read methods
        // 
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

        public bool ContainsTiles(List<Tile> targetTiles)
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

        public Vector2 GetInspectFocalPoint() => GetCenterPoint();

        public Vector2 GetCenterPoint()
        {
            var ((highestX, lowestX), (highestY, lowestY)) = TileList.GetHighestAndLowestValues(tiles);

            var x = lowestX + (highestX - lowestX);
            var y = lowestY + (highestY - lowestY);

            var resultTile = new Tile(x, y);
            var resultVector3 = resultTile.ToWorldPosition();
            return new Vector2(resultVector3.x, resultVector3.y);
        }

        public List<Tile> GetAdjacentTiles()
        {
            // TODO - could probably use a HashSet or something
            var result = new List<Tile>();

            foreach (var tile in tiles)
            {
                var adjacentTiles = tile.GetOrthagonalAdjacentTiles();

                foreach (var adjacentTile in adjacentTiles)
                {
                    if (
                        // Don't include tiles that are part of this room
                        !ContainsTile(adjacentTile) &&
                        // Don't add the same tile twice
                        !isAlreadyPresentInResults(adjacentTile))
                    {
                        result.Add(adjacentTile);
                    }
                }
            }

            return result;

            bool isAlreadyPresentInResults(Tile adjacentTile)
            {
                foreach (var resultTile in result)
                {
                    if (resultTile.Matches(adjacentTile))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public List<int> GetFloors()
        {
            HashSet<int> results = new();

            foreach (var tile in tiles)
            {
                results.Add(tile.y);
            }

            return results.ToList();
        }

        public bool ContainsFloor(int floor) => GetFloors().Contains(floor);

        public int GetLowestFloor()
        {
            var result = int.MaxValue;

            foreach (var tile in tiles)
            {
                if (tile.y < result)
                {
                    result = tile.y;
                }
            }

            return result;
        }

        public Tile GetBottomLeftTile()
        {
            Tile result = null;

            var x = int.MaxValue;
            var y = int.MinValue;
            foreach (var tile in tiles)
            {
                if (tile.x < x || tile.y < y)
                {
                    result = tile;
                    x = tile.x;
                    y = tile.y;
                }
            }

            return result;
        }

        public Tile GetRandomTile()
        {
            var index = Random.Range(0, tiles.Count);
            return tiles[index];
        }

        //
        // Private interface
        //
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