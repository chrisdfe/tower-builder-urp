
namespace TowerBuilder
{
    public interface IOccupantTask
    {
        public bool isComplete { get; }
        public void Setup();
        public void Teardown();
        public void OnTick();
    }

    public class OccupantIdleTask : IOccupantTask
    {
        public bool isComplete => true;

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            // Nothing for now
        }
    }

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
        }
    }

    public class OccupantTravelingToDestinationTask : IOccupantTask
    {
        bool _isComplete = false;
        public bool isComplete => _isComplete;

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            // Walk to destination until we have reached the destination
        }
    }
}