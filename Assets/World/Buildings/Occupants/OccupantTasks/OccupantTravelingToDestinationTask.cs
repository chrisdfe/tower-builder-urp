using System.Collections;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantTravelingToDestinationTask : IOccupantTask
    {
        public string name => $"Traveling to tile {route.GetLastNode().tile}";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        Occupant occupant;
        OccupantRoute route;
        int currentIdx = 0;

        public OccupantTravelingToDestinationTask(Occupant occupant, OccupantRoute route)
        {
            this.occupant = occupant;
            this.route = route;
        }

        public void Setup() { }

        public void Teardown() { }

        public void OnTick()
        {
            // Walk to destination until we have reached the destination
            var currentNode = GetCurrentNode();

            occupant.SetTile(currentNode.tile);
            occupant.movementAnimationWrapper.transform.localPosition = Vector3.zero;

            // set random subtile offset too?

            if (GetNextIdx() == -1)
            {
                _isComplete = true;

                // for now, manually set occupant's new state
                occupant.TransitionToTask(new OccupantIdleTask());
            }
            else
            {
                StartAnimatingTransitionBetweenTiles();
                currentIdx++;
            }
        }

        void StartAnimatingTransitionBetweenTiles()
        {
            occupant.StartCoroutine(Run());

            IEnumerator Run()
            {
                const float TRANSITION_LENGTH = TimeConstants.TICK_LENGTH_S;

                var startNode = GetCurrentNode();
                var nextNode = GetNextNode();

                // TODO - this causes a null reference exception on the final tile
                var tileDiff = nextNode.tile.Subtract(startNode.tile);
                var startPosition = Vector3.zero;
                var endPosition = tileDiff.ToWorldPosition();

                var timer = 0f;
                while (timer < TRANSITION_LENGTH)
                {
                    timer += Time.deltaTime;
                    var normalizedProgress = timer / TRANSITION_LENGTH;
                    var currentPostion = Vector3.Lerp(startPosition, endPosition, normalizedProgress);
                    occupant.movementAnimationWrapper.transform.localPosition = currentPostion;

                    yield return null;
                }

                occupant.movementAnimationWrapper.transform.localPosition = Vector3.zero;
            }
        }

        OccupantRouteNode GetCurrentNode() => route.path[currentIdx];

        int GetNextIdx()
        {
            var nextIdx = currentIdx + 1;

            if (nextIdx > route.path.Count - 1)
            {
                return -1;
            }

            return nextIdx;
        }

        OccupantRouteNode GetNextNode()
        {
            var nextIdx = GetNextIdx();

            if (nextIdx == -1)
            {
                return null;
            }

            return route.path[nextIdx];
        }
    }
}