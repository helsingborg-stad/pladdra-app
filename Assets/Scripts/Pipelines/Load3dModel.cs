using System;
using UnityEngine;

namespace Pipelines
{
    public class Load3dModel: CustomYieldInstruction {
        private Piglet.GltfImportTask Task { get; }
        private Tuple<GameObject, Exception> Result {get; set; }

        public Load3dModel(string path, Action<GameObject> callback) {
            Callback = callback;
            var options = new Piglet.GltfImportOptions(){
                ShowModelAfterImport = false
            };
            Task = Piglet.RuntimeGltfImporter.GetImportTask(path, options);
            Task.OnAborted = () => Result = new Tuple<GameObject, Exception>(null, null);
            Task.OnCompleted = go => Result = new Tuple<GameObject, Exception>(go, null);
            Task.OnException = err => Result = new Tuple<GameObject, Exception>(null, err);
            Task.OnProgress = (step, completed, total) => { };
        }

        public override bool keepWaiting {
            get {
                if (Result == null) {
                    Task.MoveNext();
                    return true;
                }
                if (Result.Item2 != null) {
                    throw Result.Item2;
                }
                Callback(Result.Item1);
                return false;
            }
        }

        private Action<GameObject> Callback { get; }
    }
}
