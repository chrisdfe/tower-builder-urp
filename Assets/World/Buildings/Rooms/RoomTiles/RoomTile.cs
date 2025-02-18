using System.Collections.Generic;
using System.Linq;
using TowerBuilder;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class RoomTile : MonoBehaviour
{
    Transform meshRoot;

    List<RoomTileSegmentTransformWrapper> segmentTransformWrappers;

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
        foreach (var segmentTransformWrapper in segmentTransformWrappers)
        {
            segmentTransformWrapper.SetMaterial(material);
        }
    }

    public void SetColor(Color color)
    {
        foreach (var segmentTransformWrapper in segmentTransformWrappers)
        {
            segmentTransformWrapper.SetColor(color);
        }
    }

    public void ToggleSegmentsForTilePosition()
    {
        var currentSegments = RoomTileConstants.SEGMENTS_FOR_TILE_POSITION[tile.orthogonalPosition];

        foreach (var segmentTransformWrapper in segmentTransformWrappers)
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
        var transformWrapper = segmentTransformWrappers.Find(wrapper => wrapper.segment == segment);
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
        var result = new List<RoomTileSegmentTransformWrapper>();

        foreach (var segment in RoomTileConstants.SEGMENT_DATA_MAP.Keys)
        {
            var segmentTransformData = new RoomTileSegmentTransformWrapper(segment, meshRoot);
            result.Add(segmentTransformData);
        }

        segmentTransformWrappers = result;
    }
}

