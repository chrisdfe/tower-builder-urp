using System.Collections;
using UnityEngine;

namespace TowerBuilder
{
    public class OccupantWaitingTask : OccupantTaskBase
    {
        public override string name => "Waiting";

        Coroutine waitCoroutine;

        float waitTime = 1f;

        public OccupantWaitingTask(Occupant occupant, float waitTime) : base(occupant)
        {
            this.waitTime = waitTime;
        }

        public override void Setup()
        {
            StartWaitTimer();
        }

        public override void OnTick() { }

        public override void Cancel()
        {
            occupant.StopCoroutine(waitCoroutine);

            base.Cancel();
        }

        void StartWaitTimer()
        {
            waitCoroutine = occupant.StartCoroutine(Run());

            IEnumerator Run()
            {
                var timer = 0f;

                while (timer < waitTime && !isCancelled)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }

                waitCoroutine = null;
                isComplete = true;
            }
        }
    }
}