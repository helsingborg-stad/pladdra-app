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
        public AllowUserToInspectModel(UXManager uxManager, PladdraResource resource)
        {
            this.uxManager = uxManager;
            this.project = uxManager.Project;
            this.resource = resource;
        }
        public override void Activate()
        {
            uxManager.RenderManager.GetComponent<RenderManager>().ToggleFeature(true, "BlurRenderFeature");
            uxManager.Project.Hide();

            container = new GameObject("PreviewContainer").transform;
            container.SetParent(uxManager.PreviewObjectHolder.transform);
            container.localPosition = Vector3.zero;
            
            preview = Object.Instantiate(resource.model, container);
            preview.SetAllChildLayers("ModelPreview");
            preview.MoveToBoundsCenter();

            // Bounds b = preview.GetBounds();
            float optimalDimension = 0.5f;
            Vector2 screenBounds = preview.GetBounds().CalculateScreenBounds();
            float largestDimension = screenBounds.x > screenBounds.y ? screenBounds.x : screenBounds.y;
            float scale = optimalDimension / largestDimension;
            container.transform.localScale = new Vector3(scale, scale, scale);

            preview.SetActive(true);

            uxManager.UIManager.ShowUI("inspect-resource", root =>
            {
                root.Q<Button>("close").clicked += () =>
                {
                    UXHandler ux = new AllowUserToViewResourceLibrary(uxManager);
                    uxManager.UseUxHandler(ux);
                };
                root.Q<Label>("name").text = resource.name;
                root.Q<Button>("place").clicked += () =>
                {
                    // TODO Get drawable layers
                    Vector3 position = uxManager.User.RelativeToObject(new Vector3(0, 2f, 2f), VectorExtensions.RelativeToObjectOptions.OnGroundLayers, new string[] { "Default" });
                    uxManager.ProposalManager.AddObject(resource, position, out PlacedObjectController controller);
                    UXHandler ux = new AllowUserToManipulateSelectedModel(uxManager, controller, false);
                    uxManager.UseUxHandler(ux);
                    // Debug.Log("Placing model");
                };
            });

            // TODO Darken screen
        }
        public override void Deactivate()
        {
            uxManager.PreviewObjectHolder.transform.rotation = Quaternion.identity;
            Object.Destroy(preview);
            Object.Destroy(container.gameObject);
            preview = null;
            container = null;
            uxManager.RenderManager.GetComponent<RenderManager>().ToggleFeature(false, "BlurRenderFeature");
            uxManager.Project.Show();

            // TODO Restore screen
        }
    }
}