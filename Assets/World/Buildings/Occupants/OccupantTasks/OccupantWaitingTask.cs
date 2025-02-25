using System.Collections;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantWaitingTask : IOccupantTask
    {
        public string name => $"Waiting";

        bool _isComplete = false;
        public bool isComplete => _isComplete;

        Occupant occupant;
        Coroutine waitCoroutine;

        float waitTime = 1f;

        public OccupantWaitingTask(Occupant occupant, float waitTime)
        {
            this.occupant = occupant;
            this.waitTime = waitTime;
        }

        public void Setup()
        {
            StartWaitTimer();
        }

        public void Teardown() { }

        public void OnTick() { }

        void StartWaitTimer()
        {
            waitCoroutine = occupant.StartCoroutine(Run());

            IEnumerator Run()
            {
                var timer = 0f;

                while (timer < waitTime)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

                waitCoroutine = null;
                _isComplete = true;
            }
        }
    }
}