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

            // TODO - offset to the left or right
            var x = Input.mousePosition.x;

            // TODO - above tile actually
            var y = Input.mousePosition.y + 100;
            tooltip.transform.position = new Vector3(x, y, 0);
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