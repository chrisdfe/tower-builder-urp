namespace TowerBuilder
{
    public interface ITool
    {
        //
        public void OnMouseUp();
        public void Teardown();
        public void Setup();
        public void OnUpdate();
    }
}