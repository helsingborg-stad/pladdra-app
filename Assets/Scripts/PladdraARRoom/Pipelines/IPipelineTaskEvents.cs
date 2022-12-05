namespace Pipelines
{
    public interface IPipelineTaskEvents
    {
        void TaskStarted();
        void TaskProgress(int i);
        void TaskFinished();
    }
}