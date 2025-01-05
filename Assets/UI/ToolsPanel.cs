using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class ToolsPanel : MonoBehaviour
    {
        Transform toolButtonsWrapper;
        ToolButton inspectButton;
        ToolButton buildButton;
        ToolButton destroyButton;

        ToolButton[] toolButtons;

        void Awake()
        {
            toolButtonsWrapper = transform.Find("ToolButtonsWrapper");
            inspectButton = toolButtonsWrapper.Find("InspectButton").GetComponent<ToolButton>();
            buildButton = toolButtonsWrapper.Find("BuildButton").GetComponent<ToolButton>();
            destroyButton = toolButtonsWrapper.Find("DestroyButton").GetComponent<ToolButton>();

            inspectButton.GetComponent<Button>().onClick.AddListener(OnInspectButtonClick);
            buildButton.GetComponent<Button>().onClick.AddListener(OnBuildButtonClick);
            destroyButton.GetComponent<Button>().onClick.AddListener(OnDestroyButtonClick);

            toolButtons = new ToolButton[3] {
                inspectButton,
                buildButton,
                destroyButton
            };

        }

        void Start()
        {
            HighlightToolButton(WorldController.Get().tool.current);
        }

        void OnInspectButtonClick()
        {
            SetActiveTool(Tool.Inspect);
            HighlightToolButton(Tool.Inspect);
        }

        void OnBuildButtonClick()
        {
            SetActiveTool(Tool.Build);
            HighlightToolButton(Tool.Build);
        }

        void OnDestroyButtonClick()
        {
            SetActiveTool(Tool.Destroy);
            HighlightToolButton(Tool.Destroy);
        }

        void SetActiveTool(Tool newTool)
        {
            WorldController.Get().SetTool(newTool);
        }

        void HighlightToolButton(Tool tool)
        {
            foreach (var toolButton in toolButtons)
            {
                toolButton.SetIsActive(toolButton.correspondingTool == tool);
            }
        }
    }
}