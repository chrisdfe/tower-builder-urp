using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TowerBuilder
{
    public class BuildToolTooltip : MonoBehaviour
    {
        TextMeshProUGUI text;
        Image backgroundImage;

        public enum State
        {
            Valid,
            Invalid,
        }

        void Awake()
        {
            text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            backgroundImage = transform.Find("Background").GetComponent<Image>();
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

        public void PlayInvalidRoomAnimation()
        {
            StartCoroutine(Run());

            IEnumerator Run()
            {
                const float ANIMATION_LENGTH = .2f;
                const float WIGGLE_FREQUENCY = 15f;
                const float WIGGLE_INTENSITY = 3f;

                var timer = 0f;
                var originalPosition = text.transform.localPosition;

                while (timer < ANIMATION_LENGTH)
                {
                    timer += Time.deltaTime;

                    var normalizedProgress = timer / ANIMATION_LENGTH;
                    var x = originalPosition.x + (Mathf.Sin(normalizedProgress * WIGGLE_FREQUENCY) * WIGGLE_INTENSITY);

                    text.transform.localPosition = new Vector3(
                        x,
                        originalPosition.y,
                        originalPosition.z
                    );

                    yield return null;
                }
            }
        }

        public void PlayFloatingAnimationThenDestroy()
        {
            StartCoroutine(Run());

            IEnumerator Run()
            {
                const float ANIMATION_LENGTH = .8f;
                const float FLOAT_HEIGHT = 70f;
                const float WIGGLE_FREQUENCY = 10f;
                const float WIGGLE_INTENSITY = 4f;
                var timer = 0f;

                var originalPosition = transform.position;
                var originalBackgroundColor = backgroundImage.color;
                var originalTextColor = text.color;

                while (timer < ANIMATION_LENGTH)
                {
                    timer += Time.deltaTime;
                    var normalizedProgress = timer / ANIMATION_LENGTH;
                    var currentFloatHeight = Mathf.Lerp(0, FLOAT_HEIGHT, normalizedProgress);
                    var x = originalPosition.x + (Mathf.Sin(normalizedProgress * WIGGLE_FREQUENCY) * WIGGLE_INTENSITY);
                    var y = originalPosition.y + currentFloatHeight;

                    transform.position = new Vector3(
                        x,
                        y,
                        originalPosition.z
                    );

                    text.color = new Color(
                        originalTextColor.r,
                        originalTextColor.g,
                        originalTextColor.b,
                        Mathf.Lerp(originalTextColor.a, 0, normalizedProgress)
                    );

                    backgroundImage.color = new Color(
                        originalBackgroundColor.r,
                        originalBackgroundColor.g,
                        originalBackgroundColor.b,
                        Mathf.Lerp(originalBackgroundColor.a, 0, normalizedProgress)
                    );

                    yield return null;
                }

                Destroy(gameObject);
            }
        }
    }
}