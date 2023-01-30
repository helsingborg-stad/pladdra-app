using Pladdra.DialogueAbility.Data;
using Pladdra.UX;
using UnityEngine;
using UnityEngine.UIElements;
using UntoldGarden.Utils;

namespace Pladdra.DialogueAbility.UX
{
    public class AllowUserToInspectModel : UXHandler
    {
        DialogueResource resource;
        GameObject preview;
        Transform container;
        public AllowUserToInspectModel(UXManager uxManager, DialogueResource resource)
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

            preview = Object.Instantiate(resource.gameObject, container);
            preview.SetAllChildLayers("ModelPreview");
            preview.MoveToBoundsCenter();

            // Bounds b = preview.GetBounds();
            float optimalDimension = 0.5f;
            Vector2 screenBounds = preview.GetBounds().CalculateScreenBounds();
            float largestDimension = screenBounds.x > screenBounds.y ? screenBounds.x : screenBounds.y;
            float scale = optimalDimension / largestDimension;
            container.transform.localScale = new Vector3(scale, scale, scale);

            preview.SetActive(true);

            uxManager.UIManager.DisplayUI("inspect-resource", root =>
            {
                root.Q<Button>("close").clicked += () =>
                {
                    UXHandler ux = new AllowUserToViewResourceLibrary(uxManager);
                    uxManager.UseUxHandler(ux);
                };
                root.Q<Label>("name").text = resource.name;
                root.Q<Button>("place").clicked += () =>
                {
                    uxManager.Project.Show();
                    Vector3 position = uxManager.User.RelativeToObjectOnGround(new Vector3(0, 2f, 2f), VectorExtensions.RelativeToObjectOptions.OnGroundLayers, uxManager.RaycastManager.GetLayerMask("placeableLayers"));
                    Debug.Log("Placing model at " + position);

                    uxManager.Project.ProposalHandler.AddObject(resource, position, out PlacedObjectController controller);
                    UXHandler ux = new AllowUserToManipulateSelectedModel(uxManager, controller, false);
                    uxManager.UseUxHandler(ux);
                };
            });
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
        }
    }
}