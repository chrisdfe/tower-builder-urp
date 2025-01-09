using System.Collections;
using System.Collections.Generic;
using TowerBuilder;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    public Color activeColor = Color.red;
    public ToolHandle correspondingTool;

    Image buttonImage;
    Button button;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void SetIsActive(bool isActive)
    {
        Color newColor;
        if (isActive)
        {
            newColor = activeColor;
        }
        else
        {
            // this seems like it might not always be the right color but probably fine for now
            newColor = button.colors.normalColor;
        }

        buttonImage.color = newColor;
    }
}
