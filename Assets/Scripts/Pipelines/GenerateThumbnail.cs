using System;
using System.IO;
using UnityEngine;

namespace Pipelines
{
    public class GenerateThumbnail: CustomYieldInstruction {
        private Action<Texture2D> Callback { get; }

        private GameObject Go { get; }

        public GenerateThumbnail(GameObject go, Action<Texture2D> callback)
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
                Callback(RuntimePreviewGenerator.GenerateModelPreview(Go.transform));
                return false;
            }
        }
    }
}
