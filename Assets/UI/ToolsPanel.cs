using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class ToolsPanel : MonoBehaviour
    {
        public GameObject toolButtonPrefab;

        Transform toolButtonsWrapper;
        Transform toolOptionButtonsWrapper;

        ToolButton inspectButton;
        ToolButton buildButton;
        ToolButton destroyButton;

        List<ToolButton> toolButtons = new();
        List<ToolButton> toolOptionButtons = new();

        void Awake()
        {
            // Tool buttons
            toolButtonsWrapper = transform.Find("ToolButtonsWrapper");
            inspectButton = toolButtonsWrapper.Find("InspectButton").GetComponent<ToolButton>();
            buildButton = toolButtonsWrapper.Find("BuildButton").GetComponent<ToolButton>();
            destroyButton = toolButtonsWrapper.Find("DestroyButton").GetComponent<ToolButton>();

            inspectButton.GetComponent<Button>().onClick.AddListener(OnInspectButtonClick);
            buildButton.GetComponent<Button>().onClick.AddListener(OnBuildButtonClick);
            destroyButton.GetComponent<Button>().onClick.AddListener(OnDestroyButtonClick);

            toolButtons = new List<ToolButton> {
                inspectButton,
                buildButton,
                destroyButton
            };

            // Tool options buttons
            toolOptionButtonsWrapper = transform.Find("ToolOptionsButtonsWrapper");
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
            ClearToolOptionsButtons();
            CreateToolOptionButtonsForCurrentTool();
        }

        void HighlightToolButton(Tool tool)
        {
            foreach (var toolButton in toolButtons)
            {
                toolButton.SetIsActive(toolButton.correspondingTool == tool);
            }
        }

        void CreateToolOptionButtonsForCurrentTool()
        {
            //
            switch (WorldController.Get().tool.current)
            {
                case Tool.Build:
                    //
                    foreach (var roomDefinition in RoomDefinition.ALL_DEFINITIONS)
                    {
                        var toolOptionButton = Instantiate(toolButtonPrefab, toolOptionButtonsWrapper);
                        toolOptionButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = roomDefinition.title;
                        toolOptionButton.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            SetSelectedRoomDefinition(roomDefinition.title);
                        });
                        toolOptionButtons.Add(toolOptionButton.GetComponent<ToolButton>());
                    }
                    break;
                default:
                    break;
            }
        }

        void SetSelectedRoomDefinition(string title)
        {
            WorldController.Get().SetSelectedRoomDefinition(title);
        }

        void ClearToolOptionsButtons()
        {
            foreach (var toolOptionButton in toolOptionButtons)
            {
                Destroy(toolOptionButton.gameObject);
            }

            toolOptionButtons = new();
        }
    }
}