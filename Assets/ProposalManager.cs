using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Data;
using Pladdra.UI;
using Pladdra.Workspace.EditHistory;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UXHandlers; //TODO add to Pladdra namespace
using Abilities.ARRoomAbility.UxHandlers;

namespace Pladdra
{
    public class ProposalManager : MonoBehaviour
    {
        #region Public
        [SerializeField] GameObject item_Prefab;
        #endregion Public

        #region Private
        Project project;
        ProjectManager projectManager { get { return transform.parent.gameObject.GetComponentInChildren<ProjectManager>(); } }

        #endregion Private
        // TODO Make auto name 
        // TODO Prompt name on exit
        // TODO Save proposal
        // TODO also save proposal to local storage

        public void Activate(Project project)
        {
            this.project = project;
        }

        public void AddItem(PladdraResource resource)
        {
            //TODO Move to project
            Debug.Log($"Adding item {resource.Name}");
            GameObject item = Instantiate(item_Prefab, projectManager.Origin().transform);
            GameObject model = Instantiate(resource.Model, item.transform);
            if(resource.Scale != Vector3.zero) model.transform.localScale = resource.Scale;
            model.SetActive(true); 
        }

    }
}