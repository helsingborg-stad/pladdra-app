using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DefaultAbility
{
    public class WorkspaceController : SceneObjectController
    {
        [GrayOut] public string scaleText = "";
        public string ScaleText { get => scaleText; }
        public List<Texture2D> thumbnails = new List<Texture2D>();
        public override void Scale(float scale)
        {
            currentScale = scale;
            float scaleFromCurve = interactionManager.scaleCurve.Evaluate(scale);
            transform.localScale = new Vector3(scaleFromCurve, scaleFromCurve, scaleFromCurve);
            scaleText = $"1:{(1 / scale).ToString()}";
        }

        public void SetThumbnail(Texture2D texture)
        {
            thumbnails.Add(texture);
        }

    }
}