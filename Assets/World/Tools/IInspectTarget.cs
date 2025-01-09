
namespace TowerBuilder
{
    public interface IInspectTarget
    {
        public string title { get; }
        public void SetInspectedState(bool inspected);
        public void SetInspectionHoveredState(bool inspected);
    }
}
