using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerBuilder
{
    public class Room : MonoBehaviour, IInspectTarget
    {
        public RoomDefinition definition;
        public string title { get; private set; } = "Room";

        public List<Tile> tiles { get; private set; } = new() { Tile.zero };

        // The building this room belongs to
        public Building building;

        // Occupants that live in this room
        public List<Occupant> residents { get; private set; } = new();

        // Occupants that work in this room
        public List<Occupant> workers { get; private set; } = new();

        // Occupants currently in this room
        public HashSet<Occupant> currentOccupants { get; private set; } = new();

        // Occupant currently sleeping in this room
        public List<Occupant> asleepOccupants { get; private set; } = new();

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

        public RoomBehaviorBase behavior;

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

        public void SetTitle(string title)
        {
            this.title = title;
            gameObject.name = title;
        }

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

        public void AddCurrentOccupant(Occupant occupant)
        {
            currentOccupants.Add(occupant);
            UpdateColor();
        }

        public void RemoveCurrentOccupant(Occupant occupant)
        {
            currentOccupants.Remove(occupant);
            UpdateColor();
        }

        public void UpdateColor()
        {
            var highlightIntensity = 0f;
            var validBlueprintColorAmount = 0f;
            var invalidBlueprintColorAmount = 0f;
            var markedForDeletionAmount = 0f;

            if (isBlueprint)
            {
                if (isValid)
                {
                    validBlueprintColorAmount = 1f;
                }
                else
                {
                    invalidBlueprintColorAmount = 1f;
                }
            }
            else
            {
                if (isMarkedForDeletion)
                {
                    markedForDeletionAmount = 1f;
                }
                else if (isInspected)
                {
                    highlightIntensity = 0.4f;
                }
                else if (isInspectionHovered)
                {
                    highlightIntensity = 0.2f;
                }
            }

            foreach (var roomTile in roomTiles)
            {
                roomTile.SetHighlightIntensity(highlightIntensity);
                roomTile.SetValidBlueprintColorAmount(validBlueprintColorAmount);
                roomTile.SetInvalidBlueprintColorAmount(invalidBlueprintColorAmount);
                roomTile.SetMarkedForDeletionColorAmount(markedForDeletionAmount);
                roomTile.SetWallColor(RoomTypeColorMap.GetForType(definition.type));

                roomTile.SetLightsOn(asleepOccupants.Count < currentOccupants.Count);
            }
        }

        public void SetZPosition()
        {
            var z = isBlueprint
                ? -RoomConstants.BLUEPRINT_ROOM_Z_OFFSET
                : RoomConstants.NORMAL_ROOM_Z_OFFSET;

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

        public void AddAsleepOccupant(Occupant occupant)
        {
            asleepOccupants.Add(occupant);
            UpdateColor();
        }

        public void RemoveAsleepOccupant(Occupant occupant)
        {
            asleepOccupants.Remove(occupant);
            UpdateColor();
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

        public Tile GetRandomTile(List<Tile> excludedTiles)
        {
            var tilesToSelectFrom = new List<Tile>();

            foreach (var tile in tiles)
            {
                var excludedTile = excludedTiles.Find(excludedTile => excludedTile.Matches(tile));

                if (excludedTile == null)
                {
                    tilesToSelectFrom.Add(tile);
                }
            }

            var index = Random.Range(0, tilesToSelectFrom.Count);
            return tilesToSelectFrom[index];
        }

        public Tile GetRandomTile() => GetRandomTile(new());

        public Color GetInteriorLightsColor() => RoomTypeColorMap.GetForType(definition.type);

        public int GetResidentialVacancies()
        {
            if (definition.type != RoomType.Residential)
            {
                return 0;
            }

            return (int)definition.capacity - residents.Count;
        }

        public bool HasResidentialVacancies() =>
            definition.type == RoomType.Residential &&
            residents.Count < definition.capacity;

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