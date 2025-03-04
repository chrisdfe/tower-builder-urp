using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class RoomDefinitionButton : MonoBehaviour
    {
        public RoomDefinition roomDefinition { get; private set; }

        public delegate void RoomDefinitionButtonClickEvent(RoomDefinition roomDefinition);
        public RoomDefinitionButtonClickEvent onClick;

        Button button;
        TextMeshProUGUI text;

        void Awake()
        {
            button = GetComponent<Button>();
            text = button.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            button.onClick.AddListener(() =>
            {
                onClick?.Invoke(roomDefinition);
            });
        }

        public void SetDefinition(RoomDefinition roomDefinition)
        {
            this.roomDefinition = roomDefinition;
            text.text = roomDefinition.title;
        }

        public static RoomDefinitionButton FromRaycastHit(GameObject gameObject)
        {
            var roomDefinitionButton = gameObject.GetComponent<RoomDefinitionButton>();

            if (roomDefinitionButton != null)
            {
                return roomDefinitionButton;
            }

            // we assume the gameObject is text then
            roomDefinitionButton = gameObject.transform.parent.gameObject.GetComponent<RoomDefinitionButton>();
            Assert.IsNotNull(roomDefinitionButton);
            return roomDefinitionButton;
        }
    }
}