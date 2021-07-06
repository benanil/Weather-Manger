

using System;
using System.Collections.Generic;

namespace AnilTools.Update
{
    public struct WaitUntilTask : ITickable,IDisposable
    {
        private readonly Queue<WaitTaskData> tasks;
        private WaitTaskData currentTask;

        private readonly int calledInstanceId;

        public WaitUntilTask(Func<bool> endCondition, Action endAction , int calledInstanceId = 0)
        {
            this.calledInstanceId = calledInstanceId;
            tasks = new Queue<WaitTaskData>();
            currentTask = new WaitTaskData(endCondition,endAction);
        }

        public void Tick()
        {
            if (currentTask.endCondition.Invoke()){
                currentTask.EndAction.Invoke();
                if (tasks.Count == 0){
                    Dispose();
                    return;
                }
                currentTask = tasks.Dequeue();
            }
        }

        public void Join(Func<bool> endCondition, Action endAction)
        {
            tasks.Enqueue(new WaitTaskData(endCondition, endAction));
        }
        
        public int InstanceId() => calledInstanceId;

        public void Dispose()
        {
            AnilUpdate.Remove(this);
            GC.SuppressFinalize(this);
        }

        private readonly struct WaitTaskData
        { 
            public readonly Func<bool> endCondition;
            public readonly Action EndAction;

            internal WaitTaskData(Func<bool> endCondition, Action endAction)
            {
                this.endCondition = endCondition;
                EndAction = endAction;
            }
        }

    }
}
