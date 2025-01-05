using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    TextMeshProUGUI buildingsText;
    TextMeshProUGUI roomsText;

    void Awake()
    {
        buildingsText = transform.Find("BuildingsText").GetComponent<TextMeshProUGUI>();
        roomsText = transform.Find("RoomsText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        buildingsText.text = "buildings: " + WorldController.Get().buildings.Count;
        roomsText.text = "total rooms: " + WorldController.Get().RoomsCount();
    }
}
