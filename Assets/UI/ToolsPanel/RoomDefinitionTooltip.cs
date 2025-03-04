using TMPro;
using UnityEngine;

namespace TowerBuilder
{
    public class RoomDefinitionTooltip : MonoBehaviour
    {
        GameObject panel;
        TextMeshProUGUI nameText;
        TextMeshProUGUI descriptionText;
        TextMeshProUGUI roomTypeText;

        void Awake()
        {
            panel = transform.Find("Panel").gameObject;
            nameText = panel.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            descriptionText = panel.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            roomTypeText = panel.transform.Find("RoomTypeText").GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            Hide();
        }

        //
        // Public interface
        //
        public void SetRoomDefinition(RoomDefinition roomDefinition)
        {
            nameText.text = roomDefinition.title;
            descriptionText.text = roomDefinition.description;
            roomTypeText.text = "Type: " + RoomDefinition.GetRoomTypeName(roomDefinition.type);
        }

        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }

        //
        // Static interface
        //
        public static RoomDefinitionTooltip Get() =>
            GameObject.Find("RoomDefinitionTooltip").GetComponent<RoomDefinitionTooltip>();
    }
}
