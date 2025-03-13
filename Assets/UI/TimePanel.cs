using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class TimePanel : MonoBehaviour
    {
        TextMeshProUGUI weeksSeasonsText;
        TextMeshProUGUI timeOfDayText;

        WorldController worldController;

        void Awake()
        {
            worldController = WorldController.Get();

            weeksSeasonsText = transform.Find("WeeksSeasonsText").GetComponent<TextMeshProUGUI>();
            timeOfDayText = transform.Find("TimeOfDayText").GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            UpdateText();
        }

        void Update()
        {
            UpdateText();
        }

        void UpdateText()
        {
            UpdateWeeksSeasonsText();
            UpdateTimeOfDayText();
        }

        void UpdateWeeksSeasonsText()
        {
            var time = GetTimeValue();
            int day = time.day;
            int week = time.week;
            // int season = time.season;
            int year = time.year;

            weeksSeasonsText.text = $"Day {day}, Week {week}, {time.GetSeasonLabel()} Year {year}";
        }

        void UpdateTimeOfDayText()
        {
            TimeOfDay currentTimeOfDay = GetTimeValue().GetTimeOfDay();
            timeOfDayText.text = currentTimeOfDay.name;
        }

        TimeValue GetTimeValue() => worldController.timeController.timeValue;
    }
}