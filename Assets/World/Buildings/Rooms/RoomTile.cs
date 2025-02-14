using TowerBuilder;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
    [HideInInspector]
    public Room room;

    Transform meshRoot;

    Transform backWallWrapper;
    GameObject backWallWindow;

    Transform ceilingWrapper;
    GameObject ceilingFull;

    Transform leftWallWrapper;
    GameObject leftWallFull;

    Transform rightWallWrapper;
    GameObject rightWallFull;

    Transform floorWrapper;
    GameObject floorFull;

    MeshRenderer[] fragmentMeshRenderers;

    void Awake()
    {
        meshRoot = transform.Find("RoomTileMesh");

        backWallWrapper = meshRoot.Find("BackWall");
        backWallWindow = backWallWrapper.Find("BackWall_Window").gameObject;

        ceilingWrapper = meshRoot.Find("Ceiling");
        ceilingFull = ceilingWrapper.Find("Ceiling_Full").gameObject;

        leftWallWrapper = meshRoot.Find("LeftWall");
        leftWallFull = leftWallWrapper.Find("LeftWall_Full").gameObject;

        rightWallWrapper = meshRoot.Find("RightWall");
        rightWallFull = rightWallWrapper.Find("RightWall_Full").gameObject;

        floorWrapper = meshRoot.Find("Floor");
        floorFull = floorWrapper.Find("Floor_Full").gameObject;

        fragmentMeshRenderers = new MeshRenderer[] {
            backWallWindow.GetComponent<MeshRenderer>(),
            ceilingFull.GetComponent<MeshRenderer>(),
            leftWallFull.GetComponent<MeshRenderer>(),
            rightWallFull.GetComponent<MeshRenderer>(),
            floorFull.GetComponent<MeshRenderer>(),
        };
    }

    public void SetMaterial(Material material)
    {
        foreach (var meshRenderer in fragmentMeshRenderers)
        {
            meshRenderer.material = material;
        }
    }

    public void SetColor(Color color)
    {
        foreach (var meshRenderer in fragmentMeshRenderers)
        {
            meshRenderer.material.color = color;
        }
    }

    public void SetBackWallVisibility(bool visible) { }

    public void SetCeilingVisibility(bool visible) { }

    public void SetLeftWallVisibility(bool visible) { }

    public void SetRightWallVisibility(bool visible) { }

    public void SetFloorVisibility(bool visible) { }
}

