using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class TimePanel : MonoBehaviour
    {
        TextMeshProUGUI debugText;
        TextMeshProUGUI hoursMinutesText;
        TextMeshProUGUI weeksSeasonsText;
        TextMeshProUGUI speedText;
        TextMeshProUGUI timeOfDayText;

        WorldController worldController;

        void Awake()
        {
            worldController = WorldController.Get();

            debugText = transform.Find("DebugText").GetComponent<TextMeshProUGUI>();
            hoursMinutesText = transform.Find("HoursMinutesText").GetComponent<TextMeshProUGUI>();
            weeksSeasonsText = transform.Find("WeeksSeasonsText").GetComponent<TextMeshProUGUI>();
            speedText = transform.Find("SpeedText").GetComponent<TextMeshProUGUI>();
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
            debugText.text = GetTimeValue().ToString();

            UpdateHoursMinutesText();
            UpdateWeeksSeasonsText();
            UpdateSpeedText();
            UpdateTimeOfDayText();
        }

        void UpdateHoursMinutesText()
        {
            hoursMinutesText.text = worldController.timeController.timeValue.ToString();
        }

        void UpdateWeeksSeasonsText()
        {
            var time = GetTimeValue();
            int day = time.day;
            int week = time.week;
            int season = time.season;
            int year = time.year;

            weeksSeasonsText.text = $"Day: {day}, Week: {week}, Season: {season}, Year: {year}";
        }

        void UpdateSpeedText()
        {
            speedText.text = $"Speed: {worldController.timeController.speed.current}";
        }

        void UpdateTimeOfDayText()
        {
            TimeOfDay currentTimeOfDay = GetTimeValue().GetTimeOfDay();
            timeOfDayText.text = currentTimeOfDay.name;
        }

        TimeValue GetTimeValue()
        {
            return worldController.timeController.timeValue;
        }
    }
}