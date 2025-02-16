using TMPro;
using TowerBuilder;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject bodyTextPrefab;

    TextMeshProUGUI tickText;
    TextMeshProUGUI hoveredTileText;
    TextMeshProUGUI buildingsText;
    TextMeshProUGUI roomsText;
    TextMeshProUGUI selectedToolText;
    TextMeshProUGUI blueprintDefinitionText;
    TextMeshProUGUI inspectTargetText;
    // I can't figure out how to get the panel to resize when inspect text wraps onto 2 lines
    // so it's this for now
    TextMeshProUGUI paddingText1;
    TextMeshProUGUI paddingText2;
    TextMeshProUGUI occupantsText;
    TextMeshProUGUI workersText;

    WorldController worldController;

    void Awake()
    {
        tickText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        hoveredTileText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        buildingsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        roomsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        selectedToolText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        blueprintDefinitionText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        inspectTargetText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        paddingText1 = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        paddingText1.text = "";
        paddingText2 = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        paddingText2.text = "";
        occupantsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        workersText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();

        worldController = WorldController.Get();
    }

    // Update is called once per frame
    void Update()
    {
        tickText.text = "Tick: " + worldController.timeController.tick.current;
        hoveredTileText.text = $"Hovered tile: ({worldController.hoveredTile.current.x},{worldController.hoveredTile.current.y})";
        buildingsText.text = "Buildings: " + worldController.buildingsController.buildings.Count;
        roomsText.text = "Total rooms: " + worldController.buildingsController.RoomsCount();
        selectedToolText.text = "Selected tool: " + worldController.toolsController.toolHandle.current;

        // blueprint
        var blueprintRoom = worldController.toolsController.buildTool.blueprintRoom;
        if (blueprintRoom != null)
        {
            blueprintDefinitionText.text = "Blueprint: " + blueprintRoom.definition.title;
        }
        else
        {
            blueprintDefinitionText.text = "";
        }

        var inspectTarget = worldController.toolsController.inspectTool.inspectTarget;
        if (inspectTarget != null)
        {
            if (inspectTarget is Occupant)
            {
                inspectTargetText.text = "Inspected occupant: " + (inspectTarget as Occupant).title;
            }
            else if (inspectTarget is Room)
            {
                inspectTargetText.text = "Inspected room: " + (inspectTarget as Room).title;
            }
        }
        else
        {
            inspectTargetText.text = "\n\n";
        }

        occupantsText.text = "Total occupants: " + worldController.buildingsController.OccupantsCount();
        workersText.text = "Total wokers: " + worldController.buildingsController.WorkerCount();
    }
}
