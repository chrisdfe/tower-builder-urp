using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class ToolsPanel : MonoBehaviour
    {
        public GameObject toolButtonPrefab;
        public GameObject roomDefinitionButtonPrefab;

        Transform toolButtonsWrapper;
        Transform toolOptionButtonsWrapper;

        ToolButton inspectButton;
        ToolButton buildButton;
        ToolButton destroyButton;

        List<ToolButton> toolButtons = new();
        List<ToolButton> toolOptionButtons = new();

        WorldController worldController;

        RoomDefinitionButton currentHoveredRoomDefinitionButton;
        RoomDefinitionTooltip roomDefinitionTooltip;
        Canvas canvas;
        GraphicRaycaster graphicRaycaster;
        EventSystem eventSystem;

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

            roomDefinitionTooltip = RoomDefinitionTooltip.Get();

            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            eventSystem = canvas.GetComponent<EventSystem>();
        }

        void Start()
        {
            HighlightToolButton(worldController.toolsController.toolHandle.current);
        }

        void Update()
        {
            CheckForRoomDefinitionButtonMouseover();
        }

        //
        // Private interface
        //
        void CheckForRoomDefinitionButtonMouseover()
        {
            var pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();

            graphicRaycaster.Raycast(pointerEventData, results);

            var isHovering = false;
            foreach (var result in results)
            {
                if (result.gameObject.tag == "RoomDefinitionButton")
                {
                    var button = RoomDefinitionButton.FromRaycastHit(result.gameObject);
                    isHovering = true;
                    OnRoomDefinitionButtonMouseOver(button);
                    break;
                }
                else if (result.gameObject.name == "ToolOptionsButtonsWrapper")
                // Note - a bit fragile. Don't change the name of the game object!
                // Avoid tooltip hiding/showing as player moves the cursor through the gap between buttons
                {
                    isHovering = true;
                }
            }

            if (!isHovering)
            {
                OnRoomDefinitionButtonMouseOut();
            }
        }

        void OnToolChanged()
        {
            HighlightToolButton(worldController.toolsController.toolHandle.current);
            ClearToolOptionsButtons();
            CreateToolOptionButtonsForCurrentTool();

            // Some one-off setup/teardown things for now
            if (worldController.toolsController.toolHandle.prev == ToolHandle.Build)
            {
                roomDefinitionTooltip.Hide();
            }
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
                        var roomDefinitionButtonGameObject = Instantiate(roomDefinitionButtonPrefab, toolOptionButtonsWrapper);
                        var roomDefinitionButton = roomDefinitionButtonGameObject.GetComponent<RoomDefinitionButton>();
                        roomDefinitionButton.SetDefinition(roomDefinition);
                        roomDefinitionButton.onClick += (RoomDefinition roomDefinition) =>
                        {
                            OnToolOptionButtonClick(roomDefinition.title);
                        };

                        toolOptionButtons.Add(roomDefinitionButton.GetComponent<ToolButton>());
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

        void OnRoomDefinitionButtonMouseOver(RoomDefinitionButton button)
        {
            if (button == currentHoveredRoomDefinitionButton) return;

            currentHoveredRoomDefinitionButton = button;
            roomDefinitionTooltip.Show();
            roomDefinitionTooltip.SetRoomDefinition(button.roomDefinition);
        }

        void OnRoomDefinitionButtonMouseOut()
        {
            roomDefinitionTooltip.Hide();
            currentHoveredRoomDefinitionButton = null;
        }
    }
}