using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.ARSandbox.Dialogues.Data;
using UnityEngine;
using UntoldGarden.Utils;
using System.Threading.Tasks;
using GLTFast;

namespace Pladdra.ARSandbox.Dialogues
{
    /// <summary>
    /// Controller for interactive objects in the scene.
    /// </summary>
    [DisallowMultipleComponent]
    public class InteractiveObjectController : SceneObjectController
    {
        public DialogueResource resource;
        protected string id;
        public string Id { get => id; }
        bool isSelected = false;

        public virtual void Init(Project project, DialogueResource resource)
        {
            this.project = project;
            this.resource = resource;
            id = Guid.NewGuid().ToString();

            try
            {
                CreateModel();
            }
            catch (Exception e)
            {
                Debug.Log("Eror creating model:" + e);
            }
        }
        async void CreateModel()
        {
            GameObject model = new GameObject("Model");
            model.transform.SetParent(transform);
            model.transform.localPosition = Vector3.zero;
            model.AddComponent<GltfAsset>().Url = resource.path;

            while (model.GetComponent<GltfAsset>().SceneInstance == null)
            {
                await Task.Yield();
            }

            if (resource.scale != 0) model.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

            // TODO Move to collider extensions
            Bounds bounds = gameObject.GetBounds();
            //increase bounds scale to counter the workplace scale
            float scaleFactor = 1.2f;
            bounds.size *= 1f / project.UXManager.Project.scalePivot.transform.localScale.x;
            bounds.size = Vector3.Scale(bounds.size, new Vector3(scaleFactor, scaleFactor, scaleFactor));
            bounds.center = bounds.center - transform.position;
            boxCollider.size = bounds.size;
            boxCollider.center = bounds.center;

            if (!isSelected)
                gameObject.SetAllChildLayers("Object");
        }

        public override void Select()
        {
            isSelected = true;
            Debug.Log("Selecting " + gameObject.name);
            base.Select();
            gameObject.SetAllChildLayers("Selected");
        }

        public override void Deselect()
        {
            isSelected = false;
            Debug.Log("Deselecting " + gameObject.name);
            base.Deselect();
            gameObject.SetAllChildLayers("Object");
        }

    }
}