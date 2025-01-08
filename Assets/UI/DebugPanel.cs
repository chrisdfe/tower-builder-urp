using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject bodyTextPrefab;

    TextMeshProUGUI tickText;
    TextMeshProUGUI buildingsText;
    TextMeshProUGUI roomsText;
    TextMeshProUGUI selectedToolText;
    TextMeshProUGUI blueprintDefinitionText;
    TextMeshProUGUI inspectedRoomText;
    TextMeshProUGUI residentsText;
    TextMeshProUGUI workersText;

    WorldController worldController;

    void Awake()
    {
        tickText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        buildingsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        roomsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        selectedToolText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        blueprintDefinitionText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        inspectedRoomText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        residentsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        workersText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();

        worldController = WorldController.Get();
    }

    // Update is called once per frame
    void Update()
    {
        tickText.text = "Tick: " + worldController.tick.current;
        buildingsText.text = "Buildings: " + worldController.buildingsController.buildings.Count;
        roomsText.text = "Total rooms: " + worldController.buildingsController.RoomsCount();
        selectedToolText.text = "Selected tool: " + worldController.toolsController.tool.current;

        // blueprint
        var blueprintRoom = worldController.toolsController.blueprintRoom;
        if (blueprintRoom != null)
        {
            blueprintDefinitionText.text = "Blueprint: " + blueprintRoom.definition.title;
        }
        else
        {
            blueprintDefinitionText.text = "";
        }

        var inspectedRoom = worldController.toolsController.inspectedRoom;
        if (inspectedRoom != null)
        {
            inspectedRoomText.text = "Inspected room: " + inspectedRoom.name;
        }
        else
        {
            inspectedRoomText.text = "";
        }

        residentsText.text = "Total residents: " + worldController.buildingsController.ResidentsCount();
        workersText.text = "Total wokers: " + worldController.buildingsController.WorkerCount();
    }
}
