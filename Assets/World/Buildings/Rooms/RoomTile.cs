using System.Collections.Generic;
using TowerBuilder;
using UnityEngine;
using UnityEngine.UI;

public class RoomTile : MonoBehaviour
{
    const string CEILING_NODE_NAME = "Ceiling";
    const string LEFT_WALL_NODE_NAME_BASE = "LeftWall";
    const string RIGHT_WALL_NODE_NAME_BASE = "RightWall";
    const string BACK_WALL_NODE_NAME_BASE = "BackWall";
    const string FLOOR_NODE_NAME_BASE = "Floor";

    enum Segment
    {
        Ceiling,
        LeftWall,
        RightWall,
        BackWall,
        Floor
    }

    static Dictionary<TilePosition, Segment[]> SEGMENTS_FOR_TILE_POSITION =
        new Dictionary<TilePosition, Segment[]>() {
            {
                TilePosition.None,
                new Segment[] { }
            },

            // single (isolated)
            {
                TilePosition.Single,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall,
                    Segment.RightWall,
                    Segment.Floor
                }
            },

            // middle (non-corner) edge tiles
            {
                TilePosition.Top,
                new Segment[] {
                    Segment.Ceiling
                }
            },
            {
                TilePosition.Right,
                new Segment[] {
                    Segment.RightWall
                }
            },
            {
                TilePosition.Bottom,
                new Segment[] {
                    Segment.Floor
                }
            },
            {
                TilePosition.Left,
                new Segment[] {
                    Segment.LeftWall
                }
            },

            // centers
            {
                TilePosition.Center,
                new Segment[] { }
            },
            {
                TilePosition.HorizontalCenter,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.Floor
                }
            },
            {
                TilePosition.VerticalCenter,
                new Segment[] {
                    Segment.LeftWall,
                    Segment.RightWall
                }
            },
            {
                TilePosition.LowToHighDiagonalCenter,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall,
                    Segment.RightWall,
                    Segment.Floor
                }
            },
            {
                TilePosition.HighToLowDiagonalCenter,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall,
                    Segment.RightWall,
                    Segment.Floor
                }
            },

            // corners
            {
                TilePosition.TopLeft,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall
                }
            },
            {
                TilePosition.TopRight,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.RightWall
                }
            },
            {
                TilePosition.BottomRight,
                new Segment[] {
                    Segment.RightWall,
                    Segment.Floor
                }
            },
            {
                TilePosition.BottomLeft,
                new Segment[] {
                    Segment.LeftWall,
                    Segment.Floor
                }
            },

            // isolated cells
            {
                TilePosition.TopIsolated,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall,
                    Segment.RightWall,
                }
            },
            {
                TilePosition.RightIsolated,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.RightWall,
                    Segment.Floor
                }
            },
            {
                TilePosition.BottomIsolated,
                new Segment[] {
                    Segment.LeftWall,
                    Segment.RightWall,
                    Segment.Floor
                }
            },
            {
                TilePosition.LeftIsolated,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall,
                    Segment.Floor
                }
            },

            {
                TilePosition.TopRightIsolated,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.RightWall,
                }
            },
            {
                TilePosition.BottomRightIsolated,
                new Segment[] {
                    Segment.RightWall,
                    Segment.Floor
                }
            },
            {
                TilePosition.BottomLeftIsolated,
                new Segment[] {
                    Segment.LeftWall,
                    Segment.Floor
                }
            },
            {
                TilePosition.TopLeftIsolated,
                new Segment[] {
                    Segment.Ceiling,
                    Segment.LeftWall
                }
            },
        };

    Transform meshRoot;

    Transform ceilingWrapper;
    GameObject ceilingFull;

    Transform leftWallWrapper;
    GameObject leftWallFull;

    Transform rightWallWrapper;
    GameObject rightWallFull;

    Transform backWallWrapper;
    GameObject backWallWindow;

    Transform floorWrapper;
    GameObject floorFull;

    [HideInInspector]
    public Room room;

    [HideInInspector]
    public Tile tile { get; private set; }

    MeshRenderer[] segmentMeshRenderers;
    Dictionary<Segment, Transform> segmentTransformMap;

    void Awake()
    {
        meshRoot = transform.Find("RoomTileMesh");

        backWallWrapper = meshRoot.Find(BACK_WALL_NODE_NAME_BASE);
        backWallWindow = backWallWrapper.Find($"{BACK_WALL_NODE_NAME_BASE}_Window").gameObject;

        ceilingWrapper = meshRoot.Find(CEILING_NODE_NAME);
        ceilingFull = ceilingWrapper.Find($"{CEILING_NODE_NAME}_Full").gameObject;

        leftWallWrapper = meshRoot.Find(LEFT_WALL_NODE_NAME_BASE);
        leftWallFull = leftWallWrapper.Find($"{LEFT_WALL_NODE_NAME_BASE}_Full").gameObject;

        rightWallWrapper = meshRoot.Find(RIGHT_WALL_NODE_NAME_BASE);
        rightWallFull = rightWallWrapper.Find($"{RIGHT_WALL_NODE_NAME_BASE}_Full").gameObject;

        floorWrapper = meshRoot.Find(FLOOR_NODE_NAME_BASE);
        floorFull = floorWrapper.Find($"{FLOOR_NODE_NAME_BASE}_Full").gameObject;

        segmentMeshRenderers = new MeshRenderer[] {
            backWallWindow.GetComponent<MeshRenderer>(),
            ceilingFull.GetComponent<MeshRenderer>(),
            leftWallFull.GetComponent<MeshRenderer>(),
            rightWallFull.GetComponent<MeshRenderer>(),
            floorFull.GetComponent<MeshRenderer>(),
        };

        segmentTransformMap = new() {
            { Segment.BackWall, backWallWrapper },
            { Segment.Ceiling, ceilingWrapper },
            { Segment.LeftWall, leftWallWrapper },
            { Segment.RightWall, rightWallWrapper },
            { Segment.Floor, floorWrapper },
        };
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
        foreach (var meshRenderer in segmentMeshRenderers)
        {
            meshRenderer.material = material;
        }
    }

    public void SetColor(Color color)
    {
        foreach (var meshRenderer in segmentMeshRenderers)
        {
            meshRenderer.material.color = color;
        }
    }

    public void ToggleSegmentsForTilePosition()
    {
        var currentSegments = SEGMENTS_FOR_TILE_POSITION[tile.orthogonalPosition];

        foreach (var item in segmentTransformMap)
        {
            var segment = item.Key;
            var segmentTransform = item.Value;

            // Always show back wall
            if (segment == Segment.BackWall)
            {
                segmentTransform.gameObject.SetActive(true);
            }
            else
            {
                segmentTransform.gameObject.SetActive(ContainsSegment(currentSegments, segment));
            }
        }
    }

    //
    // private interface
    //
    bool ContainsSegment(Segment[] segments, Segment segment)
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
}

