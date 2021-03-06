using System;
using System.IO;
using UnityEngine;

namespace Pipelines
{
    public class LoadPreview: CustomYieldInstruction {
        private Action<Texture2D> Callback { get; }

        private GameObject Go { get; }

        public LoadPreview(GameObject go, Action<Texture2D> callback)
        {
            Go = go;
            Callback = text =>
            {
                Debug.Log(text);
                callback(text);
            };
        }
        
        public override bool keepWaiting {
            get {
                RuntimePreviewGenerator.BackgroundColor = Color.clear;
                RuntimePreviewGenerator.MarkTextureNonReadable = false;
                Callback(RuntimePreviewGenerator.GenerateModelPreview(Go.transform, 512, 512));
                return false;
            }
        }
    }
}
