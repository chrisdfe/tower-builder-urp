using TowerBuilder;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
    public Room room;

    MeshRenderer bodMeshRenderer;

    void Awake()
    {
        bodMeshRenderer = transform.Find("Bod").GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material)
    {
        bodMeshRenderer.material = material;
    }

    public void SetColor(Color color)
    {
        bodMeshRenderer.material.color = color;
    }
}

