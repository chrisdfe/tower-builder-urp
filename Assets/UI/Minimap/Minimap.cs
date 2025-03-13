using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Material material;

    Image image;
    Material imageMaterial;

    Texture2D texture;
    int width = 512;
    int height = 512;
    int tileSize = 8;

    void Awake()
    {
        image = GetComponent<Image>();
        imageMaterial = image.material;

        texture = new Texture2D(width, height);
    }

    void Start()
    {
        DrawTexture();

        var sprite = Sprite.Create(
            texture,
            new(0, 0, width, height),
            new(width / 2, height / 2)
        );

        Debug.Log(sprite.bounds);
        image.sprite = sprite;
    }

    void DrawTexture()
    {
        for (var x = 0; x < width; x += tileSize)
        {
            for (var y = 0; y < height; y += tileSize)
            {
                Color color;
                if (x / tileSize % 2 == 0 && y / tileSize % 2 == 0)
                {
                    color = Color.cyan;
                }
                else
                {
                    color = Color.magenta;
                }


                var colors = Enumerable.Repeat(0, tileSize * tileSize).Select((_) => color).ToArray();
                texture.SetPixels(x, y, tileSize, tileSize, colors);
            }
        }

        texture.Apply();
    }
}
