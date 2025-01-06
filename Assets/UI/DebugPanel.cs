using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public GameObject bodyTextPrefab;

    TextMeshProUGUI buildingsText;
    TextMeshProUGUI roomsText;
    TextMeshProUGUI selectedToolText;

    void Awake()
    {
        buildingsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        roomsText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        selectedToolText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        buildingsText.text = "Buildings: " + WorldController.Get().buildingsController.buildings.Count;
        roomsText.text = "Total rooms: " + WorldController.Get().buildingsController.RoomsCount();
        selectedToolText.text = "Selected tool: " + WorldController.Get().toolsController.tool.current;
    }
}
