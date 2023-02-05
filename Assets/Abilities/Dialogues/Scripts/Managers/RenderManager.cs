using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Pladdra.ARSandbox.Dialogues
{
    /// <summary>
    /// Manages render features in the render settings.
    /// </summary>
    public class RenderManager : MonoBehaviour
    {
        [SerializeField] private UniversalRendererData rendererData = null;

        /// <summary>
        /// Toggle a render feature
        /// </summary>
        /// <param name="enable">Toggle bool</param>
        /// <param name="featureName">Name of feature</param>
        public void ToggleFeature(bool enable, string featureName)
        {
            if (TryGetFeature(featureName, out var feature))
            {
                feature.SetActive(enable);
                rendererData.SetDirty();
            }
        }

        /// <summary>
        /// Return all toggled features on to their initial state on application quit.
        /// </summary>
        void OnApplicationQuit()
        {
            //TODO Make generic, store all changed render features
            ToggleFeature(false, "BlurRenderFeature");
        }

        /// <summary>
        /// Try to get a render feature by name.
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="feature">Returns the render feature</param>
        /// <returns>Render feature</returns>
        private bool TryGetFeature(string featureName, out ScriptableRendererFeature feature)
        {
            foreach (var f in rendererData.rendererFeatures)
            {
                if (f.name == featureName)
                {
                    feature = f;
                    return true;
                }
            }
            feature = null;
            return false;
        }

    }
}