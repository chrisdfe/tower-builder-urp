using UnityEngine;

namespace TowerBuilder
{
    public class BuildToolTooltipManager : MonoBehaviour
    {
        public GameObject tooltipPrefab;

        BuildToolTooltip tooltip;

        void Awake()
        {

        }

        void Start() { }

        void Update()
        {
            if (tooltip != null)
            {
                //
                PositionTooltip();
            }
        }

        //
        // Public
        //
        public void ShowTooltip()
        {
            //
            if (tooltip == null)
            {
                tooltip = CreateTooltip();
            }
        }

        public void HideTooltip()
        {
            //
            if (tooltip != null)
            {
                Destroy(tooltip.gameObject);
                tooltip = null;
            }
        }

        public void SetTooltipText(string text)
        {
            if (tooltip == null) return;
            tooltip.SetText(text);
        }

        public void SetTooltipState(BuildToolTooltip.State state)
        {
            if (tooltip == null) return;
            tooltip.SetState(state);
        }

        //
        // Private
        //
        BuildToolTooltip CreateTooltip()
        {
            var gameObject = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity, transform);
            return gameObject.GetComponent<BuildToolTooltip>();
        }

        void PositionTooltip()
        {
            if (tooltip == null) return;

            var blueprintRoom = WorldController.Get().toolsController.buildTool.blueprintRoom;
            if (blueprintRoom == null) return;

            var screenRect = TileList.GetScreenRect(blueprintRoom.tiles);

            var x = screenRect.x;
            var y = screenRect.y;

            // Render above blueprint
            y += screenRect.height / 2;
            // some extra margin
            y += 30;

            tooltip.transform.position = new Vector3(x, y, 0);

            var rectTransform = tooltip.GetComponent<RectTransform>();
            rectTransform.transform.position = new Vector3(x, y, 0);
            rectTransform.sizeDelta = new(screenRect.width, screenRect.height);

            var imageRectTransform = rectTransform.transform.Find("Background").GetComponent<RectTransform>();
            imageRectTransform.position = new Vector3(x, y, 0);
            imageRectTransform.sizeDelta = new(screenRect.width, screenRect.height);
        }

        //
        // Static
        //
        static BuildToolTooltipManager _buildToolTooltipManager;
        public static BuildToolTooltipManager Get()
        {
            if (_buildToolTooltipManager == null)
            {
                _buildToolTooltipManager = GameObject.Find("BuildToolTooltipManager").GetComponent<BuildToolTooltipManager>();
            }

            return _buildToolTooltipManager;
        }
    }
}