using System;
using System.IO;
using UnityEngine;

namespace Pipelines
{
    public class Load2dTexture: CustomYieldInstruction {
        private Action<Texture2D> Callback { get; }

        private string Path { get; }

        public Load2dTexture(string path, Action<Texture2D> callback)
        {
            Path = path;
            Callback = callback;
        }
        
        public override bool keepWaiting {
            get {
                var fileData = File.ReadAllBytes(Path);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                Callback(texture);
                return false;
            }
        }
    }
}
