using System.Collections;
using System.Collections.Generic;
using Pladdra.DialogueAbility.Data;
using UnityEngine;

namespace Pladdra.DialogueAbility
{
    [DisallowMultipleComponent]
    public class PivotController : SceneObjectController
    {
        public float pointScale = 0.1f;
        public GameObject pivotPoint;
        public Vector3 previousPosition = Vector3.zero;

        SphereCollider sphereCollider { get { return gameObject.GetComponentInChildren<SphereCollider>(); } }

        public override void Init(Project project)
        {
            base.Init(project);

            pivotPoint.transform.localScale = Vector3.one * pointScale;
            // sphereCollider.radius = (2 * pointScale) / 2f;
            ToggleVisibility(false);
        }

        public void UpdatePointScale()
        {
            pivotPoint.transform.localScale = Vector3.one * Mathf.Clamp(pointScale * (1f / transform.localScale.x), 1f, 15f);
            // sphereCollider.radius = Mathf.Clamp((2 * pointScale) * (1f / transform.localScale.x), 1f, 15f) / 2;
        }

        public void ToggleVisibility(bool show)
        {
            pivotPoint.SetActive(show);
        }

        public void ResetPivot()
        {
            Move(Vector3.zero);
        }

        // TODO clean these up
        public override void Move(Vector3 position)
        {
            Vector3 workspacePos = project.UXManager.Project.WorkspaceController.transform.position;
            transform.position = position;
            project.UXManager.Project.WorkspaceController.transform.position = workspacePos;
        }

        public void MoveWithWorkspace(Vector3 position)
        {
            Vector3 pivotWorkspaceOffset = transform.position - project.UXManager.Project.WorkspaceController.transform.position;
            transform.position = position + pivotWorkspaceOffset;
        }

        public void MoveWithoutOffset(Vector3 position)
        {
            transform.position = position;
        }

        public override void Select()
        {
            Debug.Log("Select Pivot");
            base.Select();
            gameObject.layer = LayerMask.NameToLayer("Selected");
        }

        public override void Deselect()
        {
            Debug.Log("Deselect Pivot");
            gameObject.layer = LayerMask.NameToLayer("ScalePivot");
            base.Deselect();
        }
    }
}