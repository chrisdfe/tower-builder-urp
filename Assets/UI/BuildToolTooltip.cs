using TMPro;
using UnityEngine;

namespace TowerBuilder
{
    public class BuildToolTooltip : MonoBehaviour
    {
        TextMeshProUGUI text;

        public enum State
        {
            Valid,
            Invalid
        }

        void Awake()
        {
            text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.Valid:
                    text.color = Color.white;
                    break;
                case State.Invalid:
                    text.color = Color.red;
                    break;
                default:
                    break;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}