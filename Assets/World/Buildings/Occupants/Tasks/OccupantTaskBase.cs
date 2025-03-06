using System.Collections.Generic;

namespace TowerBuilder
{
    public abstract class OccupantTaskBase
    {
        public virtual string name { get; }

        public bool isComplete { get; protected set; } = false;
        public bool isCancelled { get; protected set; } = false;

        public virtual bool isImmediatelyCancellable => false;

        protected OccupantTaskBase currentSubTask;

        protected Occupant occupant;

        public OccupantTaskBase(Occupant occupant)
        {
            this.occupant = occupant;
        }

        public virtual void Setup()
        {
            TransitionToNextSubTask();
        }

        public virtual void Teardown()
        {
            currentSubTask?.Teardown();
        }

        public virtual void OnTick()
        {
            currentSubTask.OnTick();

            if (currentSubTask.isComplete)
            {
                if (isCancelled)
                {
                    isComplete = true;
                }
                else
                {
                    TransitionToNextSubTask();
                }
            }
        }

        public virtual void Cancel()
        {
            isCancelled = true;

            // TODO - there might be some weirdness if a task is immediately cancelable but
            //        has an active subtask that is still not complete

            if (isImmediatelyCancellable)
            {
                isComplete = true;
            }
            else if (currentSubTask != null)
            {
                currentSubTask.Cancel();

                if (currentSubTask.isComplete)
                {
                    isComplete = true;
                }
            }
        }

        protected virtual void TransitionToNextSubTask()
        {
            currentSubTask?.Teardown();
            currentSubTask = GetNextSubTask();

            if (currentSubTask == null)
            {
                isComplete = true;
            }
            else
            {
                currentSubTask.Setup();
            }
        }

        protected virtual OccupantTaskBase GetNextSubTask() => null;
    }
}