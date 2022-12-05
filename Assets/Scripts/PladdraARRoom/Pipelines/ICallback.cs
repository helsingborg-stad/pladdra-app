using System;

namespace Pipelines
{
    public interface ICallback<T>
    {
        void OnSuccess(T result);
        void OnError(Exception exception);
    }
}