using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class TimeSpeedPanel : MonoBehaviour
    {
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
            playPauseButtonsWrapper = transform.Find("PlayPauseButtonsWrapper");

            timeSpeedButtonMap = new();
            foreach (var entry in buttonNameTimeSpeedMap)
            {
                var (buttonName, buttonSpeed) = entry;
                var button = playPauseButtonsWrapper.Find(buttonName).GetComponent<Button>();
                timeSpeedButtonMap[buttonName] = button;
                button.onClick.AddListener(() =>
                {
                    WorldController.Get().timeController.SetSpeed(buttonSpeed);
                    SetActiveButton(buttonSpeed);
                });
            }
        }

        void Start()
        {
            SetActiveButton(WorldController.Get().timeController.speed.current);
        }

        void Update()
        {
            if (WorldController.Get().timeController.speed.HasChanged())
            {
                SetActiveButton(WorldController.Get().timeController.speed.current);
            }
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
    }
}
