using System;

namespace Pipelines
{
    public class PipelineTaskEvents : IPipelineTaskEvents
    {
        private readonly Action taskStarted;
        private readonly Action<int> taskProgress;
        private readonly Action taskFinished;

        public PipelineTaskEvents(Action taskStarted, Action<int> taskProgress, Action taskFinished)
        {
            this.taskStarted = taskStarted;
            this.taskProgress = taskProgress;
            this.taskFinished = taskFinished;
        }

        public void TaskStarted() => taskStarted?.Invoke();

        public void TaskProgress(int step) => taskProgress?.Invoke(step);

        public void TaskFinished() => taskFinished?.Invoke();
    }
}