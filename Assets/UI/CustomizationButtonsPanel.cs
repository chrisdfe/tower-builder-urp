using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class CustomizationButtonsPanel : MonoBehaviour
    {
        public GameObject uiButtonPrefab;

        WorldController worldController;

        Transform body;
        List<UIButton> variantButtons = new();
        Room inspectedRoom;

        void Awake()
        {
            worldController = WorldController.Get();

            body = transform.Find("Body");
        }

        void Update()
        {
            //
            if (worldController.toolsController.inspectTool.inspectTarget is Room)
            {
                inspectedRoom = worldController.toolsController.inspectTool.inspectTarget as Room;

                if (variantButtons.Count == 0)
                {
                    CreateVariantButtons();
                }
            }
            else if (variantButtons.Count > 0)
            {
                DestroyVariantButtons();
            }
        }

        void CreateVariantButtons()
        {
            foreach (var variant in RoomTileConstants.SEGMENT_DATA_MAP[RoomTileSegment.BackWall].variants)
            {
                var buttonGameObject = Instantiate(uiButtonPrefab, body);
                var uiButton = buttonGameObject.GetComponent<UIButton>();
                uiButton.SetText(variant);
                uiButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    inspectedRoom.SetRoomTileSegmentVariant(RoomTileSegment.BackWall, variant);
                });

                variantButtons.Add(uiButton);
            }
        }

        void DestroyVariantButtons()
        {
            foreach (var button in variantButtons)
            {
                Destroy(button.gameObject);
            }

            variantButtons = new();

        }
    }

}