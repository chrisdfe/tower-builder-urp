namespace TowerBuidler
{
    public interface IOccupantSubTask
    {
        public bool isComplete { get; }
        public void Setup();
        public void Teardown();
        public void OnTick();
    }

    public class OccupantNoneSubTask : IOccupantSubTask
    {
        public bool isComplete => true;

        public void Setup() { }
        public void Teardown() { }
        public void OnTick() { }
    }

    public class OccupantWalkingSubTask : IOccupantSubTask
    {
        public bool _isComplete = false;

        public bool isComplete => _isComplete;

        public void Setup() { }
        public void Teardown() { }
        public void OnTick()
        {
            throw new System.NotImplementedException("TODO");
        }
    }
}