using System.Collections.Generic;
using UnityEngine;

namespace TowerBuilder
{
    public abstract class OccupantTaskBase
    {
        public enum Priority
        {
            High,
            Normal
        }

        public virtual string name { get; }

        public bool isComplete { get; protected set; } = false;
        public bool isCancelled { get; protected set; } = false;
        public Priority priority { get; set; } = Priority.Normal;

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
            if (currentSubTask != null)
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

        // TODO - this doesn't make sense for tasks without subtasks
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