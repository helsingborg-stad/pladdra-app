using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using Pladdra.DefaultAbility.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden.Utils;
using UntoldGarden;
using UnityEngine.Rendering.Universal;

namespace Pladdra.DefaultAbility.UX
{
    public class AllowUserToInspectModel : UXHandler
    {
        PladdraResource resource;
        GameObject preview;
        Transform container;
        public AllowUserToInspectModel(InteractionManager interactionManager, PladdraResource resource)
        {
            this.interactionManager = interactionManager;
            this.project = interactionManager.Project;
            this.resource = resource;
        }
        public override void Activate()
        {
            interactionManager.RenderManager.GetComponent<RenderManager>().ToggleFeature(true, "BlurRenderFeature");
            Debug.Log($"Inspecting {resource.Name}");

            container = new GameObject("PreviewContainer").transform;
            container.SetParent(interactionManager.PreviewObjectHolder.transform);
            container.localPosition = Vector3.zero;
            
            preview = Object.Instantiate(resource.Model, container);
            preview.SetAllChildLayers("ModelPreview");
            preview.MoveToBoundsCenter();

            // Bounds b = preview.GetBounds();
            float optimalDimension = 0.5f;
            Vector2 screenBounds = preview.GetBounds().CalculateScreenBounds();
            float largestDimension = screenBounds.x > screenBounds.y ? screenBounds.x : screenBounds.y;
            float scale = optimalDimension / largestDimension;
            container.transform.localScale = new Vector3(scale, scale, scale);

            preview.SetActive(true);

            interactionManager.UIManager.ShowUI("inspect-resource", root =>
            {
                root.Q<Button>("close").clicked += () =>
                {
                    UXHandler ux = new AllowUserToViewResourceLibrary(interactionManager);
                    interactionManager.UseUxHandler(ux);
                };
                root.Q<Label>("name").text = resource.Name;
                root.Q<Button>("place").clicked += () =>
                {
                    // TODO Get drawable layers
                    Vector3 position = interactionManager.User.RelativeToObject(new Vector3(0, 2f, 2f), VectorExtensions.RelativeToObjectOptions.OnGroundLayers, new string[] { "Default" });
                    interactionManager.ProposalManager.AddObject(resource, position, out PlacedObjectController controller);
                    UXHandler ux = new AllowUserToManipulateSelectedModel(interactionManager, controller, false);
                    interactionManager.UseUxHandler(ux);
                    // Debug.Log("Placing model");
                };
            });

            // TODO Darken screen
        }
        public override void Deactivate()
        {
            interactionManager.PreviewObjectHolder.transform.rotation = Quaternion.identity;
            Object.Destroy(preview);
            Object.Destroy(container.gameObject);
            preview = null;
            container = null;
            interactionManager.RenderManager.GetComponent<RenderManager>().ToggleFeature(false, "BlurRenderFeature");

            // TODO Restore screen
        }
    }
}