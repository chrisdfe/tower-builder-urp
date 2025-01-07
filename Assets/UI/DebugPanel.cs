using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject bodyTextPrefab;

    TextMeshProUGUI buildingsText;
    TextMeshProUGUI roomsText;
    TextMeshProUGUI selectedToolText;
    TextMeshProUGUI blueprintDefinitionText;
    TextMeshProUGUI inspectedRoomText;

    void Awake()
    {
        buildingsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        roomsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        selectedToolText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        blueprintDefinitionText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        inspectedRoomText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        buildingsText.text = "Buildings: " + WorldController.Get().buildingsController.buildings.Count;
        roomsText.text = "Total rooms: " + WorldController.Get().buildingsController.RoomsCount();
        selectedToolText.text = "Selected tool: " + WorldController.Get().toolsController.tool.current;

        // blueprint
        var blueprintRoom = WorldController.Get().toolsController.blueprintRoom;
        if (blueprintRoom != null)
        {
            blueprintDefinitionText.text = "Blueprint: " + blueprintRoom.definition.title;
        }
        else
        {
            blueprintDefinitionText.text = "";
        }

        var inspectedRoom = WorldController.Get().toolsController.inspectedRoom;
        if (inspectedRoom != null)
        {
            inspectedRoomText.text = "Inspected room: " + inspectedRoom.name;
        }
        else
        {
            inspectedRoomText.text = "";
        }
    }
}
