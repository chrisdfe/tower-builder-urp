namespace TowerBuilder
{
    public class OccupantWanderingTask : IOccupantTask
    {
        bool _isComplete = false;
        public bool isComplete => _isComplete;

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            // wait around for a while
            // find another place nearby
            // go stand there
            throw new System.NotImplementedException("TODO");
        }
    }
}
