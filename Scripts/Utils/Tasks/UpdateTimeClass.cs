using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnilTools.Update
{
    public class UpdateTimeTask : ITickable, IDisposable
    {
        public readonly Queue<ActionData> dataQueue;
        private event Action EndAction;
        public ActionData currentData;

        //for debug
        private readonly int CalledInstanceId;

        public UpdateTimeTask(Action updateAction, float endTime, Action endAction = null, int calledInstanceId = 0)
        {
            EndAction = endAction;
            dataQueue = new Queue<ActionData>();
            currentData = new ActionData(updateAction, endTime);
            this.CalledInstanceId = calledInstanceId;
        }

        public void Tick()
        {
            currentData.Invoke();

            if (currentData.CheckEnd())
            {
                if (dataQueue.Count == 0)
                {
                    EndAction?.Invoke();
                    Dispose();
                    return;
                }
                currentData = dataQueue.Dequeue();
            }
        }

        public int InstanceId() => CalledInstanceId;

        public void Dispose()
        {
            AnilUpdate.Remove(this);
            GC.SuppressFinalize(this);
        }

        public struct ActionData
        {
            public Action updateAction;
            public readonly float endTime;

            internal void Invoke()
            {
                updateAction.Invoke();
            }

            public bool CheckEnd()
            {
                return Time.time >= endTime;
            }

            public ActionData(Action updateAction, float endTime)
            {
                this.updateAction = updateAction;
                this.endTime = endTime;
            }
        }

    }
}
