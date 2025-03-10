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

        void Start()
        {
            tooltip = CreateTooltip();
        }

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
            tooltip.SetActive(true);
        }

        public void HideTooltip()
        {
            //
            if (tooltip != null)
            {
                tooltip.SetActive(false);
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
            var currentTile = WorldController.Get().hoveredTile.current ?? Tile.zero;
            var blueprintRoom = WorldController.Get().toolsController.buildTool.blueprintRoom;
            if (blueprintRoom == null) return;

            var screenRect = TileList.GetScreenRect(blueprintRoom.tiles);

            // TODO - offset to the left or right
            // var x = Input.mousePosition.x;

            // TODO - snap to tile
            // TODO - use blueprintRoom.GetScreenRect instead
            // var y = ((Input.mousePosition.y + 100) / currentTile.y) * currentTile.y;
            // tooltip.transform.position = new Vector3(x, y, 0);

            // DEBUG
            var x = screenRect.x + (screenRect.width / 4);
            // var y = screenRect.y + (screenRect.height / 2);
            var y = screenRect.y;
            tooltip.transform.position = new Vector3(x, y, 0);
            var rectTransform = tooltip.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new(screenRect.width, screenRect.height);
            var imageRectTransform = rectTransform.transform.Find("Image").GetComponent<RectTransform>();
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