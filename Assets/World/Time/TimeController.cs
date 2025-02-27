using UnityEngine;
using UnityEngine.Windows.Speech;

namespace TowerBuilder
{
    public class TimeController
    {
        public PrevAndCurrent<uint> tick { get; private set; } = new(0);
        public int minute { get; private set; } = 0;
        float tickTimerElapsed = 0f;

        public TimeValue timeValue { get; private set; } = TimeValue.Zero();
        public PrevAndCurrent<TimeSpeed> speed { get; private set; } = new(TimeSpeed.Normal);
        TimeSpeed speedBeforePause = TimeSpeed.Normal;

        WorldController worldController;

        public TimeController(WorldController worldController)
        {
            this.worldController = worldController;
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                speed.Set(TimeSpeed.Pause);
                tickTimerElapsed = float.PositiveInfinity;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                speed.Set(TimeSpeed.Normal);
                tickTimerElapsed = float.PositiveInfinity;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                speed.Set(TimeSpeed.Fast);
                tickTimerElapsed = float.PositiveInfinity;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                speed.Set(TimeSpeed.Fastest);
                tickTimerElapsed = float.PositiveInfinity;
            }
            else
            {
                speed.Set(speed.current);
                tickTimerElapsed += Time.deltaTime;
            }

            var tickInterval = GetTickInterval();

            if (tickTimerElapsed >= tickInterval)
            {
                tick.Set(tick.current + 1);
                minute = (int)tick.current * TimeConstants.MINUTES_ELAPSED_PER_TICK;
                timeValue = new TimeValue(minute);

                tickTimerElapsed = 0f;
            }
            else
            {
                // required for PrevAndCurrent.HasChanged() to actually work
                tick.Set(tick.current);
            }
        }

        public void Pause()
        {
            if (speed.current == TimeSpeed.Pause) return;

            speedBeforePause = speed.current;
            speed.Set(TimeSpeed.Pause);
        }

        public void UnPause()
        {
            speed.Set(speedBeforePause);
        }

        public float GetTickInterval()
        {
            return TimeConstants.TIME_SPEED_TICK_INTERVALS[speed.current];
        }
    }
}