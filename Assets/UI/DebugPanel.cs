using TMPro;
using TowerBuilder;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject bodyTextPrefab;

    TextMeshProUGUI allText;


    WorldController worldController;

    void Awake()
    {
        allText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();

        worldController = WorldController.Get();
    }

    // Update is called once per frame
    void Update()
    {
        allText.text = "";
        allText.text += "Tick: " + worldController.timeController.tick.current;
        allText.text += $"\nHovered tile: ({worldController.hoveredTile.current.x},{worldController.hoveredTile.current.y})";
        allText.text += "\nBuildings: " + worldController.buildingsController.buildings.Count;
        allText.text += "\nTotal rooms: " + worldController.buildingsController.RoomsCount();
        allText.text += "\nTotal room groups: " + worldController.buildingsController.RoomGroupCount();
        allText.text += "\nSelected tool: " + worldController.toolsController.toolHandle.current;

        allText.text += "\nTotal residents: " + worldController.buildingsController.ResidentCount();
        allText.text += "\nTotal workers: " + worldController.buildingsController.WorkerCount();

        // blueprint
        var blueprintRoom = worldController.toolsController.buildTool.blueprintRoom;
        if (blueprintRoom != null)
        {
            allText.text += "\nBlueprint: " + blueprintRoom.definition.title;
        }
        else
        {
            // blueprintDefinitionText.text = "";
        }

        var inspectTarget = worldController.toolsController.inspectTool.inspectTarget;
        if (inspectTarget != null)
        {
            allText.text += "\n";
            if (inspectTarget is Occupant)
            {
                var occupantInspectTarget = inspectTarget as Occupant;
                allText.text += "\nInspected occupant: " + occupantInspectTarget.title;
                allText.text += $"\nCurrent task: {occupantInspectTarget.currentTask.name}";
            }
            else if (inspectTarget is Room)
            {
                var inspectRoomTarget = inspectTarget as Room;
                var building = worldController.buildingsController.FindBuildingByRoom(inspectRoomTarget);
                var roomGroup = building.FindRoomGroupByRoom(inspectRoomTarget);

                allText.text += "\nInspected room: " + (inspectTarget as Room).title;
                allText.text += $"\nis in building: {building.title}";
                allText.text += $"\nis in room group: {roomGroup.title}";
            }
        }
        else
        {
            // allText.text = "\n\n";
        }
    }
}
