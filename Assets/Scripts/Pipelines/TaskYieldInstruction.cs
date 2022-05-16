using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Pipelines
{
    public class TaskYieldInstruction<T> : CustomYieldInstruction
    {
        public override bool keepWaiting {
            get {
                if (Task.IsFaulted) {
                    throw Task.Exception;
                }
                if (Task.IsCompleted) {
                    Callback(Task.Result);
                    return false;
                }
                return true;
            }
        }

        private Func<Task<T>> Action { get; }
        private Action<T> Callback { get; }
        private Task<T> Task { get; }

        protected TaskYieldInstruction(Func<Task<T>> action, Action<T> callback)
        {
            Action = action;
            Callback = callback;
            Task = System.Threading.Tasks.Task.Run(action);
        }
    }
}
