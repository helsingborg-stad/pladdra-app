using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using UnityEngine;
using Pladdra.DefaultAbility.UI;
using UXHandlers;
using UnityEngine.UIElements;
using Pladdra.Workspace.EditHistory;
using Pladdra.DefaultAbility.UX;

namespace Pladdra.DefaultAbility
{
    /// <summary>
    /// Manages UXHandlers.
    /// </summary>
    public class UXManager : MonoBehaviour
    {
        #region Exposed
        [SerializeField] float previewObjectDistance;
        [SerializeField] GameObject objectPrefab;
        public GameObject ObjectPrefab { get { return objectPrefab; } }
        public float PreviewObjectDistance { get { return previewObjectDistance; } }
        public AnimationCurve scaleCurve;
        #endregion Exposed

        #region Private
        UXHandler uxHandler;
        Project project;
        public Project Project { get { return project; } set { project = value; } }
        [HideInInspector] public GameObject User { get; set; }
        [HideInInspector] public GameObject PreviewObjectHolder { get; set; }
        UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        public UIManager UIManager { get { return uiManager; } }
        ProposalManager proposalManager { get { return transform.parent.gameObject.GetComponentInChildren<ProposalManager>(); } }
        public ProposalManager ProposalManager { get { return proposalManager; } }
        RaycastManager raycastManager { get { return transform.parent.gameObject.GetComponentInChildren<RaycastManager>(); } }
        public RaycastManager RaycastManager { get { return raycastManager; } }
        AppManager appManager { get { return transform.parent.gameObject.GetComponentInChildren<AppManager>(); } }
        RenderManager renderManager { get { return transform.parent.gameObject.GetComponentInChildren<RenderManager>(); } }
        public RenderManager RenderManager { get { return renderManager; } }
        public float ObjectRotationSpeed { get { return appManager.settings.rotationSpeed; } } // TODO Move to global config
        #endregion Private

        void Start()
        {
            User = Camera.main.transform.gameObject; // TODO Set this from ARSessionOrigin
            PreviewObjectHolder = User.transform.Find("PreviewObjectHolder").gameObject;
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
        /// Sets our UXHandler, that deals with all currently displayed UI and user-accessible functionality
        /// There can only be one UXHandler at a time, this is a simple state machine
        /// </summary>
        /// <param name="uxHandler"></param>
        public void UseUxHandler(UXHandler uxHandler)
        {
            if (this.uxHandler != null) this.uxHandler.Deactivate();
            this.uxHandler = uxHandler;
            uxHandler.Activate();
        }

        /// <summary>
        /// Nulls the UXHandler
        /// </summary>
        public void SetUXToNull()
        {
            uxHandler = null;
        }

        /// <summary>
        /// Checks if an object hit from a raycast from raycastManager is an object placed in the proposal
        /// </summary>
        /// <param name="obj">The object to check</param>
        public void SelectObject(GameObject obj)
        {

            if (proposalManager.IsProposalObject(obj, out PlacedObjectController controller))
            {
                UXHandler ux = new AllowUserToManipulateSelectedModel(this, controller);
                UseUxHandler(ux);
            }
        }
    }
}