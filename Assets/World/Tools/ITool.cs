namespace TowerBuilder
{
    public interface ITool
    {
        //
        public void OnLeftMouseUp();
        public void OnRightMouseUp() { }
        public void Setup();
        public void Teardown();
        public void OnUpdate() { }
    }
}