using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimePanelClock : MonoBehaviour
{
    TextMeshProUGUI hoursMinutesText;

    Transform clockFace;
    Transform hourHand;
    Transform minuteHand;

    void Awake()
    {
        hoursMinutesText = transform.Find("HoursMinutesText").GetComponent<TextMeshProUGUI>();
        clockFace = transform.Find("ClockFace");
        hourHand = clockFace.Find("HourHandWrapper");
        minuteHand = clockFace.Find("MinuteHandWrapper");
    }

    void Update()
    {
        var currentTime = WorldController.Get().timeController.timeValue;
        hoursMinutesText.text = currentTime.ToString();

        var normalizedHours = currentTime.hour / 12f;
        var hourRotation = 360f * normalizedHours * -1;
        hourHand.rotation = Quaternion.Euler(0, 0, hourRotation);

        var normalizedMinutes = currentTime.minute / 60f;
        var minuteRotation = 360f * normalizedMinutes * -1;
        minuteHand.rotation = Quaternion.Euler(0, 0, minuteRotation);
    }
}
