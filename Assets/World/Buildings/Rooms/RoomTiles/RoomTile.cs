using System.Collections.Generic;
using System.Linq;
using TowerBuilder;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class RoomTile : MonoBehaviour
{
    Transform meshRoot;

    Dictionary<RoomTileSegment, RoomTileSegmentTransformWrapper> segmentTransformWrapperMap;

    [HideInInspector]
    public Room room;

    [HideInInspector]
    public Tile tile { get; private set; }

    void Awake()
    {
        meshRoot = transform.Find("RoomTileMesh");

        BuildSegmentTransformDataMap();
    }

    //
    // public interface
    //
    public void SetTile(Tile tile)
    {
        this.tile = tile;
    }

    public void CalculateSegmentsFromTileList(List<Tile> roomTiles)
    {
        tile.CalculatePositionFromTileList(roomTiles);
        ToggleSegmentsForTilePosition();
    }

    public void SetMaterial(Material material)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            segmentTransformWrapper.SetMaterial(material);
        }
    }

    public void SetHighlightIntensity(float intensity)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            segmentTransformWrapper.SetHighlightIntensity(intensity);
        }
    }

    public void SetValidBlueprintAmount(float amount)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            segmentTransformWrapper.SetValidBlueprintAmount(amount);
        }
    }

    public void SetInvalidBlueprintAmount(float amount)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            segmentTransformWrapper.SetInvalidBlueprintAmount(amount);
        }
    }

    public void SetMarkedForDeletionColorAmount(float amount)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            segmentTransformWrapper.SetMarkedForDeletionColorAmount(amount);
        }
    }

    public void SetColor(Color color)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            segmentTransformWrapper.SetColor(color);
        }
    }

    public void SetWallColor(Color color)
    {
        var backWallWrapper = segmentTransformWrapperMap[RoomTileSegment.BackWall];
        backWallWrapper.SetColor(color);
    }

    public void SetLightsOn(bool lightsAreOn)
    {
        var backWallWrapper = segmentTransformWrapperMap[RoomTileSegment.BackWall];
        backWallWrapper.SetInteriorLightsIntensity(lightsAreOn ? 1f : 0f);
    }

    public void ToggleSegmentsForTilePosition()
    {
        var currentSegments = RoomTileConstants.SEGMENTS_FOR_TILE_POSITION[tile.orthogonalPosition];

        foreach (var segmentTransformWrapper in segmentTransformWrapperMap.Values)
        {
            var segment = segmentTransformWrapper.segment;

            // Always show back wall
            if (segment == RoomTileSegment.BackWall)
            {
                segmentTransformWrapper.SetActive(true);
            }
            else
            {
                segmentTransformWrapper.SetActive(ContainsSegment(currentSegments, segment));
            }
        }
    }

    public void SetSegmentVariant(RoomTileSegment segment, string variant)
    {
        var transformWrapper = segmentTransformWrapperMap[segment];
        transformWrapper.SetActiveVariant(variant);
    }

    //
    // private interface
    //
    bool ContainsSegment(RoomTileSegment[] segments, RoomTileSegment segment)
    {
        foreach (var s in segments)
        {
            if (s == segment)
            {
                return true;
            }
        }

        return false;
    }

    void BuildSegmentTransformDataMap()
    {
        var result = new Dictionary<RoomTileSegment, RoomTileSegmentTransformWrapper>();

        foreach (var segment in RoomTileConstants.SEGMENT_DEFINITIONS.Keys)
        {
            var segmentTransformData = new RoomTileSegmentTransformWrapper(segment, meshRoot);
            result.Add(segment, segmentTransformData);
        }

        segmentTransformWrapperMap = result;
    }

    //
    // Static interface
    //
    public static RoomTile GetRoomTileFromCollider(Transform colliderTransform)
    {
        // collider is attached to a child of the main room tile gameobject
        return colliderTransform.parent.GetComponent<RoomTile>();
    }
}

