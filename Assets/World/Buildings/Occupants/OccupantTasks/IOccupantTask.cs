namespace TowerBuilder
{
    public interface IOccupantTask
    {
        public bool isComplete { get; }
        public void Setup();
        public void Teardown();
        public void OnTick();
    }
}