using UnityEngine;
using UnityEngine.UIElements;
using Pladdra.DialogueAbility.UX;
using Pladdra;
using Pladdra.UI;
using Pladdra.DialogueAbility;
using Pladdra.DialogueAbility.Data;

namespace Pladdra.UX
{
    /// <summary>
    /// Manages UXHandlers.
    /// </summary>
    // TODO Move to interface based structure
    public class UXManager : MonoBehaviour
    {
        #region Private
        protected UXHandler uxHandler;
        protected UXHandler pastUXHandler;
        [HideInInspector] public GameObject User { get; set; }
        UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        public UIManager UIManager { get { return uiManager; } }
        #endregion Private

        //TODO Move below to inherited class
        #region Exposed
        public PladdraDefaultSettings settings;
        #endregion Exposed

        #region Private
        Project project;
        public Project Project { get { return project; } set { project = value; } }
        [HideInInspector] public GameObject PreviewObjectHolder { get { return User.transform.Find("PreviewObjectHolder").gameObject; } }
        RaycastManager raycastManager { get { return transform.parent.gameObject.GetComponentInChildren<RaycastManager>(); } }
        public RaycastManager RaycastManager { get { return raycastManager; } }
        RenderManager renderManager { get { return transform.parent.gameObject.GetComponentInChildren<RenderManager>(); } }
        public RenderManager RenderManager { get { return renderManager; } }
        GeospatialManager geospatialManager { get { return transform.parent.gameObject.GetComponentInChildren<GeospatialManager>(); } }
        public GeospatialManager GeospatialManager { get { return geospatialManager; } }
        AppManager_Dialogues appManager { get { return transform.parent.gameObject.GetComponentInChildren<AppManager_Dialogues>(); } }
        public AppManager_Dialogues AppManager { get { return appManager; } }
        protected ARReferenceImageHandler arReferenceImageHandler { get { return transform.parent.gameObject.GetComponentInChildren<ARReferenceImageHandler>(); } }
        public ARReferenceImageHandler ARReferenceImageHandler { get { return arReferenceImageHandler; } }


        public float ObjectRotationSpeed { get { return settings.rotationSpeed; } } // TODO Move to global config
        #endregion Private

        protected virtual void Start()
        {
            User = AppManager.ARSessionManager.GetUser().gameObject; // TODO Set this from ARSessionOrigin
            // PreviewObjectHolder = User.transform.Find("PreviewObjectHolder").gameObject; //TODO Move to inherited class
        }

        /// <summary>
        /// Sets our UXHandler, that deals with all currently displayed UI and user-accessible functionality
        /// There can only be one UXHandler at a time, this is a simple state machine
        /// </summary>
        /// <param name="uxHandler"></param>
        public void UseUxHandler(UXHandler uxHandler)
        {
            if (this.uxHandler != null)
            {
                pastUXHandler = uxHandler;
                this.uxHandler.Deactivate();
            }
            this.uxHandler = uxHandler;
            uxHandler.Activate();
        }

        /// <summary>
        /// Nulls the UXHandler
        /// </summary>
        public void SetUXToNull()
        {
            if (uxHandler != null)
                uxHandler.Deactivate();
            uxHandler = null;
        }

        /// <summary>
        /// Uses the last used UXHandler
        /// </summary>
        public void UseLastUX()
        {
            if (pastUXHandler != null)
            {
                Debug.Log("UXManager: Using last UX: " + pastUXHandler.GetType().ToString());
                UseUxHandler(pastUXHandler);
            }
        }

        /// <summary>
        /// Shows the default workspace UI, with object library, site model gizmo and so on
        /// </summary>
        public void ShowWorkspaceDefault()
        {
            UXHandler ux = new AllowUserToViewWorkspace(this);
            UseUxHandler(ux);
        }

        /// <summary>
        /// Checks if a selected object can be interacted with and creates relevant UXHandler.
        /// </summary>
        /// <param name="obj">The object to check</param>
        public void SelectObject(GameObject obj)
        {
            switch (LayerMask.LayerToName(obj.layer))
            {
                case "Object":
                    UseUxHandler(new AllowUserToManipulateSelectedModel(this, obj.GetComponent<InteractiveObjectController>()));
                    break;
                case "ScalePivot":
                    UseUxHandler(new AllowUserToManipulatePivot(this, obj.GetComponentInParent<PivotController>()));
                    break;
            }
        }
    }
}