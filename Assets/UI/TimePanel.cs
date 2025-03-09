using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class TimePanel : MonoBehaviour
    {
        TextMeshProUGUI hoursMinutesText;
        TextMeshProUGUI weeksSeasonsText;
        TextMeshProUGUI speedText;
        TextMeshProUGUI timeOfDayText;

        WorldController worldController;

        Transform playPauseButtonsWrapper;
        Dictionary<string, TimeSpeed> buttonNameTimeSpeedMap = new() {
            { "PauseButton", TimeSpeed.Pause },
            { "PlayButton", TimeSpeed.Normal },
            { "FastButton", TimeSpeed.Fast },
            { "FastestButton", TimeSpeed.Fastest },
        };
        Dictionary<string, Button> timeSpeedButtonMap = new();

        void Awake()
        {
            worldController = WorldController.Get();

            hoursMinutesText = transform.Find("HoursMinutesText").GetComponent<TextMeshProUGUI>();
            weeksSeasonsText = transform.Find("WeeksSeasonsText").GetComponent<TextMeshProUGUI>();
            speedText = transform.Find("SpeedText").GetComponent<TextMeshProUGUI>();
            timeOfDayText = transform.Find("TimeOfDayText").GetComponent<TextMeshProUGUI>();

            playPauseButtonsWrapper = transform.Find("PlayPauseButtonsWrapper");

            timeSpeedButtonMap = new();
            foreach (var entry in buttonNameTimeSpeedMap)
            {
                var (buttonName, buttonSpeed) = entry;
                var button = playPauseButtonsWrapper.Find(buttonName).GetComponent<Button>();
                timeSpeedButtonMap[buttonName] = button;
                button.onClick.AddListener(() =>
                {
                    worldController.timeController.SetSpeed(buttonSpeed);
                    SetActiveButton(buttonSpeed);
                });
            }

            SetActiveButton(worldController.timeController.speed.current);
        }

        void Start()
        {
            UpdateText();
        }

        void Update()
        {
            UpdateText();

            if (worldController.timeController.speed.HasChanged())
            {
                SetActiveButton(worldController.timeController.speed.current);
            }
        }

        void UpdateText()
        {
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
            // int season = time.season;
            int year = time.year;

            weeksSeasonsText.text = $"Day {day}, Week {week}, {time.GetSeasonLabel()} Year {year}";
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

        void SetActiveButton(TimeSpeed speed)
        {
            foreach (var entry in timeSpeedButtonMap)
            {
                var (buttonName, button) = entry;
                var timeSpeed = buttonNameTimeSpeedMap[buttonName];
                button.image.color = timeSpeed == speed ? Color.red : Color.white;
            }
        }

        TimeValue GetTimeValue()
        {
            return worldController.timeController.timeValue;
        }
    }
}