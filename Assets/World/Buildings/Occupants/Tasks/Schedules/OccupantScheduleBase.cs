namespace TowerBuilder
{
    public abstract class OccupantScheduleBase
    {
        protected Occupant occupant;

        public OccupantTaskBase currentTask { get; protected set; }

        public OccupantScheduleBase(Occupant occupant)
        {
            this.occupant = occupant;
        }

        public virtual void OnTick() { }

        public virtual void Regenerate() { }
    }
}