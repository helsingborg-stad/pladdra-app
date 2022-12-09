using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DefaultAbility
{
    public class WorkspaceController : SceneObjectController
    {
        [GrayOut] public string scaleText = "";
        public string ScaleText { get => scaleText; }
        public override void Scale(float scale)
        {
            currentScale = scale;
            float scaleFromCurve = uxManager.scaleCurve.Evaluate(scale);
            transform.localScale = new Vector3(scaleFromCurve, scaleFromCurve, scaleFromCurve);
            scaleText = $"1:{(1 / scale).ToString()}";
        }
    }
}