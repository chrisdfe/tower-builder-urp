using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerBuilder
{
    public class OccupantWanderingTask : IOccupantTask
    {
        enum WanderingState
        {
            Waiting,
            Traveling
        }

        public string name
        {
            get
            {
                if (currentState == WanderingState.Traveling && !currentSubTask.isComplete)
                {
                    return "Wandering";
                }

                return "Waiting around";
            }
        }

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        Occupant occupant;

        public IOccupantTask currentSubTask;
        Coroutine currentCoroutine;

        WanderingState currentState = WanderingState.Waiting;

        public OccupantWanderingTask(Occupant occupant)
        {
            this.occupant = occupant;
        }

        public void Setup()
        {
            StartWaiting();
        }

        public void Teardown()
        {
            if (currentCoroutine != null)
            {
                occupant.StopCoroutine(currentCoroutine);
            }

            // TODO - other teardown stuff
        }

        public void OnTick()
        {
            if (currentSubTask != null)
            {
                if (currentSubTask.isComplete)
                {
                    currentSubTask.Teardown();
                    TransitionToNextState();
                }

                currentSubTask.OnTick();
            }
        }

        void TransitionToNextState()
        {
            switch (currentState)
            {
                case WanderingState.Waiting:
                    StartWandering();
                    break;
                case WanderingState.Traveling:
                    StartWaiting();
                    break;
            }
        }

        void StartWaiting()
        {
            currentState = WanderingState.Waiting;
            currentSubTask = new OccupantWaitingTask(occupant, Random.Range(1f, 5f));
            currentSubTask.Setup();
        }

        void StartWandering()
        {
            currentState = WanderingState.Traveling;

            // Find a random tile in the occupant's current room
            var nextTile = occupant.currentRoom.GetRandomTile();

            // find a route to that tile
            var routeFinder = new OccupantRouteFinder(occupant.currentRoom.building, occupant.tile, nextTile);
            var route = routeFinder.FindRoute();
            Assert.IsNotNull(route);

            // create subtask
            currentSubTask = new OccupantTravelingToDestinationTask(occupant, route);
            currentSubTask.Setup();
        }
    }
}
