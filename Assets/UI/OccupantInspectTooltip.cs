using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class OccupantInspectTooltip : MonoBehaviour
    {
        // The distance between the edge of the tooltip and the Occupant
        const int MARGIN = 100;

        GameObject panel;
        TextMeshProUGUI nameText;
        TextMeshProUGUI taskText;
        TextMeshProUGUI residenceText;
        TextMeshProUGUI officeText;
        TextMeshProUGUI hotelRoomText;
        RectTransform rectTransform;

        Occupant occupant;

        bool isVisible = false;

        //
        // Lifecycle
        //
        void Awake()
        {
            panel = transform.Find("Panel").gameObject;
            nameText = panel.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            taskText = panel.transform.Find("TaskText").GetComponent<TextMeshProUGUI>();
            residenceText = panel.transform.Find("ResidenceText").GetComponent<TextMeshProUGUI>();
            officeText = panel.transform.Find("OfficeText").GetComponent<TextMeshProUGUI>();
            hotelRoomText = panel.transform.Find("HotelRoomText").GetComponent<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();

            ToggleVisibility(false);
        }

        void Update()
        {
            if (!isVisible) return;

            UpdatePosition();
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
            if (!occupant) return;

            var occupantScreenPosition = occupant.GetScreenPosition();

            // TODO - determine whether to show on the left or right
            //        for now, always show on the left of the occupant
            var x =
                occupantScreenPosition.x - (rectTransform.rect.width / 2) - MARGIN;

            var y = occupantScreenPosition.y;

            transform.position = new Vector3(x, y, 0);
        }

        void UpdateText()
        {
            // TODO 
            if (occupant != null)
            {
                nameText.text = occupant.name;

                // TODO - hide these if they are not relevant
                taskText.text = "Current task: " + GetCurrentTaskText(occupant);
                residenceText.text = "Residence: " + GetResidenceText(occupant);
                officeText.text = "Workplace: " + GetOfficeText(occupant);
                hotelRoomText.text = "Hotel room: " + GetHotelRoomText(occupant);
            }
            else
            {
                nameText.text = "";
                taskText.text = "";
                residenceText.text = "";
                officeText.text = "";
            }
        }

        string GetCurrentTaskText(Occupant occupant)
        {
            if (occupant.immediateTask != null)
            {
                return occupant.immediateTask.name;
            }

            if (occupant.schedule?.currentTask != null)
            {
                return occupant.schedule.currentTask.name;
            }

            return "Nothing";
        }

        string GetResidenceText(Occupant occupant)
        {
            if (occupant.residence != null)
            {
                return occupant.residence.name;
            }

            return "Nowhere";
        }

        string GetOfficeText(Occupant occupant)
        {
            if (occupant.office != null)
            {
                return occupant.office.name;
            }

            return "Nowhere";
        }

        string GetHotelRoomText(Occupant occupant)
        {
            if (occupant.hotelRoom != null)
            {
                return occupant.hotelRoom.name;
            }

            return "Nowhere";
        }

        //
        // Static interface
        //
        static OccupantInspectTooltip _occupantInspectTooltip;
        public static OccupantInspectTooltip Get()
        {
            if (_occupantInspectTooltip == null)
            {
                _occupantInspectTooltip = GameObject.Find("OccupantInspectTooltip").GetComponent<OccupantInspectTooltip>();
            }

            return _occupantInspectTooltip;
        }

        public static void SetOccupant(Occupant occupant)
        {
            var tooltip = Get();
            tooltip.occupant = occupant;
            tooltip.UpdateText();
            tooltip.ToggleVisibility(occupant != null);
        }
    }
}
