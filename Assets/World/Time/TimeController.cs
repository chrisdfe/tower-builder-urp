using UnityEngine;

namespace TowerBuilder
{
    public class TimeController
    {
        public const float TICK_LENGTH_S = 1f;

        public PrevAndCurrent<uint> tick { get; private set; } = new(0);
        float tickTimerElapsed = 0f;

        WorldController worldController;

        public TimeController(WorldController worldController)
        {
            this.worldController = worldController;
        }

        public void OnUpdate()
        {
            tickTimerElapsed += Time.deltaTime;
            if (tickTimerElapsed >= TICK_LENGTH_S)
            {
                tick.Set(tick.current + 1);
                tickTimerElapsed = 0f;
            }
            else
            {
                // required for PrevAndCurrent.HasChanged() to actually work
                tick.Set(tick.current);
            }
        }
    }
}