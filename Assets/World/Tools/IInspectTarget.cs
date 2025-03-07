using UnityEngine;

namespace TowerBuilder
{
    public interface IInspectTarget
    {
        public string title { get; }
        public void SetInspectedState(bool inspected);
        public void SetInspectionHoveredState(bool inspected);
        public Vector2 GetInspectFocalPoint();
        public Vector2 GetScreenPosition();
    }
}
