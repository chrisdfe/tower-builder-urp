using UnityEngine;

public class TilesController : MonoBehaviour
{
    static float TILE_SIZE = 1f;

    Transform placeholderTile;

    void Awake()
    {
        placeholderTile = GameObject.Find("PlaceholderTile").transform;
    }

    void Update()
    {
        var mousePosition = Input.mousePosition;
        var screenPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        var tilePosition = new Vector3(
            Mathf.Round(screenPosition.x) / TILE_SIZE * TILE_SIZE,
            Mathf.Round(screenPosition.y) / TILE_SIZE * TILE_SIZE,
            0
        );


        placeholderTile.position = tilePosition;
    }

    public static TilesController Get()
    {
        return GameObject.Find("TilesController").GetComponent<TilesController>();
    }
}
