using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DialogueAbility
{
        [DisallowMultipleComponent]
    public class WorkspaceController : SceneObjectController
    {
        [GrayOut] public string scaleText = "";
        public bool scalingDown = false;
        public string ScaleText { get => scaleText; }
        protected bool normalMove;
        protected bool initMove = true;
        //TODO: don't really like putting this here...
        // protected LayerMask workspaceRaycastMask;
        protected Vector3 originPosition = Vector3.zero;
        protected Vector3 pivotOffset = Vector3.zero;
        protected Vector3 initPos;
        protected Vector2 initTouchPos;

        public override void Scale(float scale)
        {
            currentScale = scale;
            float scaleFromCurve = project.UXManager.settings.scaleCurve.Evaluate(scale);
            project.UXManager.Project.scalePivot.transform.localScale = new Vector3(scaleFromCurve, scaleFromCurve, scaleFromCurve);
            project.UXManager.Project.scalePivot.GetComponent<PivotController>().UpdatePointScale();
            scaleText = $"1:{(1 / scale).ToString()}";
        }

        public void SelectObject(Touch[] touches, RaycastHit[] hits, RaycastHit firstHit)
        {
            bool hitScalePivot = false;
            bool hitWorkspace = false;
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ScalePivot"))
                    {
                        hitScalePivot = true;
                        project.UXManager.SelectObject(hit.transform.gameObject);
                        project.UXManager.Project.scalePivot.GetComponent<PivotController>().Select();
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StaticResources"))
                    {
                        hitWorkspace = true;
                    }
                }
            }

            if (!hitScalePivot && firstHit.transform != null)
            {
                ;
                MoveWorkspace(touches, firstHit.point, firstHit.transform.gameObject, hitWorkspace);
            }

        }

        public override void Rotate(Touch touch1, Touch touch2)
        {
            // base.Rotate(touch1, touch2);
            return;
        }

        public override void SetRotation(float f)
        {
            project.UXManager.Project.scalePivot.rotation = Quaternion.Euler(0, f, 0);
        }

        public void MoveWorkspace(Touch[] touches, Vector3 position, GameObject obj, bool hitWorkspace)
        {
            //Triggers once when a touch event is activated. initMove value gets reset when a touch is released
            if (initMove == true)
            {
                if (obj == null)
                {
                    transform.position = position;
                }
                else if (!hitWorkspace)
                {
                    Debug.Log("Normal move");
                    originPosition = new Vector3(position.x, position.y + 0.01f, position.z);
                    //update pivot position
                    project.UXManager.Project.scalePivot.GetComponent<PivotController>().MoveWithWorkspace(originPosition);
                    //update workspace position
                    transform.position = originPosition;
                    initPos = transform.position;
                    normalMove = true;
                }
                else
                {
                    Debug.Log("Pivot move active");
                    normalMove = false;
                    initPos = transform.position;
                    initTouchPos = touches[0].position;
                }
                initMove = false;
                //After defining move type, change the layermask to only hit the the ground plane to avoid jitter
                // workspaceRaycastMask = project.UXManager.RaycastManager.LayerMask;
                // project.UXManager.RaycastManager.LayerMask = LayerMask.GetMask("ARMesh");

                project.UXManager.RaycastManager.SetLayerMask("ARMesh");
            }
            //Triggers each frame after an initial touch continues to be held down
            else
            {
                if (normalMove == false)
                {
                    Vector3 camForward = transform.InverseTransformDirection(Camera.main.transform.forward);
                    camForward.y = 0f;
                    camForward.Normalize();
                    Vector3 camRight = transform.InverseTransformDirection(Camera.main.transform.right);
                    camRight.y = 0f;
                    camRight.Normalize();
                    Vector3 touchMoveDir = new Vector3(touches[0].position.x, 0f, touches[0].position.y) - new Vector3(initTouchPos.x, 0f, initTouchPos.y);
                    Vector3 moveDirection = transform.TransformDirection((camRight * touchMoveDir.x) + (camForward * touchMoveDir.z));
                    originPosition = initPos + (moveDirection * (0.02f * project.UXManager.Project.scalePivot.localScale.x));
                    //update pivot position
                    project.UXManager.Project.scalePivot.GetComponent<PivotController>().MoveWithWorkspace(originPosition);
                    //update workspace position
                    transform.position = originPosition;
                    // Debug.Log("Setting Pivot move: " + originPosition);
                }
            }
        }

        /// <summary>
        /// Finalizes a move
        /// </summary>
        public virtual void FinalizeMove()
        {
            Debug.Log("Finalize move");
            initMove = true;
            normalMove = false;
            // project.UXManager.RaycastManager.LayerMask = workspaceRaycastMask;
            project.UXManager.RaycastManager.SetLayerMask("allowUserToManipulateWorkspace");
        }
    }
}