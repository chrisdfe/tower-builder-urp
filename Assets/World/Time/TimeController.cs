namespace TowerBuilder
{
    public class TimeController
    {
        public int tick { get; private set; } = 0;

        WorldController worldController;

        public TimeController(WorldController worldController)
        {
            this.worldController = worldController;
        }

        public void OnUpdate()
        {
            // TODO- increment tick
        }
    }
}