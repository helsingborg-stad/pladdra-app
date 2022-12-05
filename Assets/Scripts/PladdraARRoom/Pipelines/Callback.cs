using System;
using UnityEngine;

namespace Pipelines
{
    public class Callback<T> : ICallback<T>
    {
        public Callback(Action<T> success): this(success, e => throw e)
        {
        }
        public Callback(Action<T> success, Action<Exception> error)
        {
            Success = success;
            Error = error;
        }

        private Action<Exception> Error { get; set; }

        private Action<T> Success { get; set; }
        public void OnSuccess(T result)
        {
            Success(result);
        }

        public void OnError(Exception exception)
        {
            Debug.LogError(exception);
            Error(exception);
        }
    }
}