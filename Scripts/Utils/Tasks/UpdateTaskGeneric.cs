using System;
using System.Collections.Generic;

namespace AnilTools.Update
{
    public class UpdateTaskGeneric <T>: ITickable, IDisposable
    {
        public readonly Queue<ActionGenericData> dataQueue;
        private event Action<T> EndAction;
        public ActionGenericData currentData;

        protected readonly T sender;

        //for debug
        private readonly int CalledInstanceId = 0;

        public UpdateTaskGeneric(Action<T> updateAction, Func<bool> endCondition, T sender, Action<T> endAction = null)
        {
            EndAction = endAction;
            dataQueue = new Queue<ActionGenericData>();
            currentData = new ActionGenericData(updateAction, endCondition, sender);

            this.sender = sender;

            if (sender is UnityEngine.Object obj){
                CalledInstanceId = obj.GetInstanceID();
            }
        }

        public UpdateTaskGeneric<T> Join(ActionGenericData actionData)
        {
            dataQueue.Enqueue(actionData);
            return this;
        }

        public UpdateTaskGeneric<T> Join(Action<T> action, Func<bool> endCondition, T sender)
        {
            dataQueue.Enqueue(new ActionGenericData(action, endCondition, sender));
            return this;
        }

        public void Tick()
        {
            currentData.Invoke();

            if (!currentData.CheckEnd())
            {
                if (dataQueue.Count == 0)
                {
                    EndAction?.Invoke(currentData.sender);
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

        public struct ActionGenericData
        {
            public Action<T> updateAction;
            public readonly Func<bool> endCondition;
            public readonly T sender;

            internal void Invoke()
            {
                updateAction.Invoke(sender);
            }

            internal bool CheckEnd()
            {
                return endCondition.Invoke();
            }

            public ActionGenericData(Action<T> updateAction, Func<bool> endCondition,T sender)
            {
                this.updateAction = updateAction;
                this.endCondition = endCondition;
                this.sender = sender;
            }
        }

    }
}
