using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject debugTextPrefab;

    TextMeshProUGUI buildingsText;
    TextMeshProUGUI roomsText;
    TextMeshProUGUI selectedToolText;

    void Awake()
    {
        buildingsText = Instantiate(debugTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        roomsText = Instantiate(debugTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        selectedToolText = Instantiate(debugTextPrefab, transform).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        buildingsText.text = "Buildings: " + WorldController.Get().buildingsController.buildings.Count;
        roomsText.text = "Total rooms: " + WorldController.Get().buildingsController.RoomsCount();
        selectedToolText.text = "Selected tool: " + WorldController.Get().toolsController.tool.current;
    }
}
