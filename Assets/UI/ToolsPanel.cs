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

        WorldController worldController;

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

            worldController = WorldController.Get();
            worldController.toolsController.onToolChanged += OnToolChanged;
        }

        void Start()
        {
            HighlightToolButton(worldController.toolsController.toolHandle.current);
        }

        void OnToolChanged()
        {

            HighlightToolButton(worldController.toolsController.toolHandle.current);
            ClearToolOptionsButtons();
            CreateToolOptionButtonsForCurrentTool();
        }

        void OnInspectButtonClick()
        {
            SetActiveTool(ToolHandle.Inspect);
        }

        void OnBuildButtonClick()
        {
            SetActiveTool(ToolHandle.Build);
        }

        void OnDestroyButtonClick()
        {
            SetActiveTool(ToolHandle.Destroy);
        }

        void SetActiveTool(ToolHandle newTool)
        {
            WorldController.Get().toolsController.SetTool(newTool);
        }

        void HighlightToolButton(ToolHandle tool)
        {
            foreach (var toolButton in toolButtons)
            {
                toolButton.SetIsActive(toolButton.correspondingTool == tool);
            }
        }

        void CreateToolOptionButtonsForCurrentTool()
        {
            //
            switch (WorldController.Get().toolsController.toolHandle.current)
            {
                case ToolHandle.Build:
                    //
                    foreach (var roomDefinition in RoomConstants.ALL_DEFINITIONS)
                    {
                        var toolOptionButton = Instantiate(toolButtonPrefab, toolOptionButtonsWrapper);
                        toolOptionButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = roomDefinition.title;
                        toolOptionButton.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            OnToolOptionButtonClick(roomDefinition.title);
                        });
                        toolOptionButtons.Add(toolOptionButton.GetComponent<ToolButton>());
                    }

                    HighlightActiveToolOptionButton();
                    break;
                default:
                    break;
            }
        }

        void OnToolOptionButtonClick(string title)
        {
            SetSelectedRoomDefinition(title);
            HighlightActiveToolOptionButton();
        }

        void SetSelectedRoomDefinition(string title)
        {
            WorldController.Get().toolsController.buildTool.SetSelectedRoomDefinition(title);
        }

        void HighlightActiveToolOptionButton()
        {
            var currentSelectedRoomDefinition = WorldController.Get().toolsController.buildTool.selectedRoomDefinition.current;

            if (currentSelectedRoomDefinition != null)
            {
                foreach (var button in toolOptionButtons)
                {
                    // not the best way to do this, but it's fine
                    var buttonLabel = button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
                    button.SetIsActive(buttonLabel == currentSelectedRoomDefinition.title);
                }
            }
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