using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class RoomInspectTooltip : MonoBehaviour
    {
        // The distance between the edge of the tooltip and the Occupant
        const int MARGIN = 100;

        GameObject panel;
        TextMeshProUGUI nameText;
        TextMeshProUGUI currentOccupantsText;
        TextMeshProUGUI residentsText;
        TextMeshProUGUI workersText;
        TextMeshProUGUI hotelGuestsText;

        RectTransform rectTransform;

        Room room;

        bool isVisible = false;

        //
        // Lifecycle
        //
        void Awake()
        {
            panel = transform.Find("Panel").gameObject;
            nameText = panel.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            currentOccupantsText = panel.transform.Find("CurrentOccupantsText").GetComponent<TextMeshProUGUI>();
            residentsText = panel.transform.Find("ResidentsText").GetComponent<TextMeshProUGUI>();
            workersText = panel.transform.Find("WorkersText").GetComponent<TextMeshProUGUI>();
            hotelGuestsText = panel.transform.Find("HotelGuestsText").GetComponent<TextMeshProUGUI>();

            rectTransform = GetComponent<RectTransform>();

            ToggleVisibility(false);
        }

        void Update()
        {
            if (!isVisible) return;

            UpdatePosition();
            UpdateText();
        }

        //
        // Public interface
        //
        void ToggleVisibility(bool isVisible)
        {
            this.isVisible = isVisible;
            panel.SetActive(isVisible);
        }

        void UpdatePosition()
        {
            if (!room) return;

            var occupantScreenPosition = room.GetScreenPosition();

            // TODO - determine whether to show on the left or right
            //        for now, always show on the left of the occupant
            var x =
                occupantScreenPosition.x - (rectTransform.rect.width / 2) - MARGIN;

            var y = occupantScreenPosition.y;

            transform.position = new Vector3(x, y, 0);
        }

        void UpdateText()
        {
            if (room != null)
            {
                nameText.text = room.name;
                // TODO - include the total capacity
                //        include how many/who is in the room currently
                currentOccupantsText.text = $"Current: {room.currentOccupants.Count}";
                // TODO - don't show this if this is not a residents/office etc
                residentsText.text = $"Residents: {room.residents.Count}";
                workersText.text = $"Workers: {room.workers.Count}";
                // TODO - don't show this if this is not a hotel room
                hotelGuestsText.text = $"Hotel guests: {room.hotelGuests.Count}";
            }
            else
            {
                nameText.text = "";
            }
        }

        //
        // Static interface
        //
        static RoomInspectTooltip _roomInspectTooltip;
        public static RoomInspectTooltip Get()
        {
            if (_roomInspectTooltip == null)
            {
                _roomInspectTooltip = GameObject.Find("RoomInspectTooltip").GetComponent<RoomInspectTooltip>();
            }

            return _roomInspectTooltip;
        }

        public static void SetRoom(Room room)
        {
            var tooltip = Get();
            tooltip.room = room;
            tooltip.UpdateText();
            tooltip.ToggleVisibility(room != null);
        }
    }
}
