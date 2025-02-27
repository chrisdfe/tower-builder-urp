namespace TowerBuilder
{
    // TODO - ultimately a way of canceling a task - may or may not need to wrap up over the next few ticks
    public interface IOccupantTask
    {
        public string name { get; }
        public bool isComplete { get; }
        public void Setup();
        public void Teardown();
        public void OnTick();
        public void Cancel();
    }
}