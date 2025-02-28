using System.Collections;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantAnimationWrapper : MonoBehaviour
    {
        Occupant occupant;

        Coroutine animationCoroutine;

        WorldController worldController;

        float originalY;

        void Awake()
        {
            // TODO - this doesn't seem great.
            worldController = WorldController.Get();
            originalY = transform.localPosition.y;
        }

        //
        // Public interface
        //
        public void ResetPosition()
        {
            transform.localPosition = Vector3.zero;
        }

        public void SetIsLyingDown(bool isLyingDown)
        {
            if (isLyingDown)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);
                transform.localPosition = new Vector3(0, originalY - 0.2f, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                transform.localPosition = new Vector3(0, originalY, 0);
            }
        }

        public void StartAnimatingTransitionBetweenTiles(Tile startTile, Tile destinationTile)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(Run());

            IEnumerator Run()
            {
                const float TRANSITION_LENGTH = TimeConstants.TICK_LENGTH_S;
                var tickInterval = worldController.timeController.GetTickInterval();

                var tileDiff = destinationTile.Subtract(startTile);
                var startPosition = Vector3.zero;
                var endPosition = tileDiff.ToWorldPosition();

                var timer = 0f;
                while (timer < TRANSITION_LENGTH)
                {
                    timer += Time.deltaTime / tickInterval;
                    var normalizedProgress = timer / TRANSITION_LENGTH;
                    var currentPostion = Vector3.Lerp(startPosition, endPosition, normalizedProgress);
                    transform.localPosition = currentPostion;

                    yield return null;
                }

                transform.localPosition = Vector3.zero;
                animationCoroutine = null;
            }
        }

        //
        // Static interface
        //
        public static OccupantAnimationWrapper FindFor(Occupant occupant)
        {
            var animationWrapper = occupant.transform.Find("OccupantAnimationWrapper").GetComponent<OccupantAnimationWrapper>();
            animationWrapper.occupant = occupant;
            return animationWrapper;
        }
    }
}