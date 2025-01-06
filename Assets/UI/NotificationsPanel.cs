using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationsPanel : MonoBehaviour
{
    public GameObject bodyTextPrefab;

    TextMeshProUGUI bodyText;

    void Awake()
    {
        bodyText = Instantiate(bodyTextPrefab, transform).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        var text = "";

        foreach (var notification in WorldController.Get().notifications)
        {
            text += notification.message;
            text += "\n";
        }

        bodyText.text = text;
    }
}
