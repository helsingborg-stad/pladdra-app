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
            Debug.Log("Init interactive object " + resource.name);
            this.project = project;
            this.resource = resource;
            id = Guid.NewGuid().ToString();

            try
            {
                CreateModel();
            }
            catch (Exception e)
            {
                Debug.Log("Error creating model:" + e);
            }
        }
        async void CreateModel()
        {

            GameObject model = new GameObject("Model");
            model.transform.SetParent(transform);
            model.transform.localPosition = Vector3.zero;
            model.AddComponent<GltfAsset>().Url = resource.path;

            float f = 0;
            bool breakFlag = false;
            while (model.GetComponent<GltfAsset>().SceneInstance == null && !breakFlag)
            {
                if (f > 30)
                {
                    Debug.Log("Loading timeout for " + resource.name);
                    project.UXManager.UIManager.ShowTimedPrompt("Misslyckades med att ladda interaktiv model " + resource.name, 3f);
                    breakFlag = true;
                }
                else
                {
                    f += Time.deltaTime;
                }
                await Task.Yield();
            }
            Vector3 pos = gameObject.transform.position;
            gameObject.transform.position = Vector3.zero;

            MeshFilter mf = model.AddComponent<MeshFilter>();
            Mesh mesh = gameObject.CombineMeshesInChildren();

            if (mesh != null)
            {
                mf.mesh = mesh;
                MeshCollider meshCollider = model.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mf.mesh;
            }
            else
            {
                Debug.Log("No mesh found for interactive resource " + resource.name);
            }

            gameObject.transform.position = pos;

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
            {
                gameObject.SetAllChildLayers("StaticResources", false);
                gameObject.layer = LayerMask.NameToLayer("Object");
            }

            if (resource.scale != 0) gameObject.transform.localScale = new Vector3(resource.scale, resource.scale, resource.scale);
            if (resource.position != Vector3.zero) gameObject.transform.localPosition = resource.position;
            if (resource.position != Vector3.zero) gameObject.transform.localRotation = Quaternion.Euler(resource.rotation);
        }

        public override void Select()
        {
            isSelected = true;
            Debug.Log("Selecting " + gameObject.name);
            base.Select();
            gameObject.SetAllChildLayers("Selected");
            // gameObject.layer = LayerMask.NameToLayer("Selected");
        }

        public override void Deselect()
        {
            isSelected = false;
            Debug.Log("Deselecting " + gameObject.name);
            base.Deselect();
            // gameObject.SetAllChildLayers("Object");
            gameObject.layer = LayerMask.NameToLayer("Object");
            gameObject.SetAllChildLayers("StaticResources", false);
        }

    }
}