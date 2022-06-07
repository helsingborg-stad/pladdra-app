using System;
using UnityEngine;

namespace Pipelines
{
    public class Load3dModel: CustomYieldInstruction {
        private Piglet.GltfImportTask Task { get; }
        public Load3dModel(string path, Action<GameObject> callback): this(path, new Callback<GameObject>(callback)) {}
        public Load3dModel(string path, ICallback<GameObject> callback) {
            
            var options = new Piglet.GltfImportOptions(){
                ImportAnimations = true,
                ShowModelAfterImport = false
            };
            Task = Piglet.RuntimeGltfImporter.GetImportTask(path, options);
            Task.OnAborted = () => callback.OnSuccess(null);
            Task.OnCompleted = callback.OnSuccess;
            Task.OnException = callback.OnError;
            Task.OnProgress = (step, completed, total) => { };
        }

        public override bool keepWaiting
        {
            get
            {
                try
                {
                    return Task.MoveNext();
                }
                catch (Exception e)
                {
                    // Errors will be propagated through Task.OnException -> callback.OnError
                    // Still important that we suppress them here
                    return false;
                }
            }
        }
    }
}
