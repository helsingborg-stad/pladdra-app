using System.Collections;
using System.Collections.Generic;
using Pladdra.ARSandbox.Dialogues.Data;
using UnityEngine;


namespace Pladdra.ARSandbox.Dialogues
{
    [DisallowMultipleComponent]
    public class SceneObjectController : MonoBehaviour
    {
        protected Project project;
        protected float initRotY;
        protected float rotation;
        protected bool initRotation;
        protected Vector2 initVector;
        protected float currentScale;
        public float CurrentScale { get => currentScale; }

        /// <summary>
        /// Initializes the controller
        /// </summary>
        /// <param name="proposalManager">Reference to proposalManager</param>
        /// <param name="project.UXManager">Reference to UIManager</param>
        public virtual void Init(Project project)
        {
            this.project = project;
        }
        //TODO Move up a level
        public virtual void Select()
        {

        }

        /// <summary>
        /// Moves the object to a new position
        /// </summary>
        /// <param name="position">The position to move the object to</param>
        public virtual void Move(Vector3 position, GameObject hit)
        {
            // if (IsDescendant(hit.transform, transform))
            // {
            //     return;
            // }
            transform.position = position;
        }

        private bool IsDescendant(Transform child, Transform parent)
        {
            if (child == parent) return true;
            if (child.parent == null) return false;
            return IsDescendant(child.parent, parent);
        }


        /// <summary>
        /// Rotate the object by touches
        /// </summary>
        /// <param name="touch1">Touch to calculate rotation from</param>
        /// <param name="touch2">Touch to calculate rotation from</param>
        public virtual void Rotate(Touch touch1, Touch touch2)
        {
            if (initRotation == true)
            {
                // Debug.Log("Init rotation");
                initRotY = transform.rotation.eulerAngles.y;
                initVector = touch2.position - touch1.position;
                initRotation = false;
            }
            else
            {
                rotation = Vector2.SignedAngle(initVector, touch2.position - touch1.position) - (project.UXManager.ObjectRotationSpeed);
                transform.rotation = Quaternion.Euler(0, initRotY - rotation, 0);
            }
        }

        /// <summary>
        /// Sets rotation for Y axis
        /// </summary>
        /// <param name="f">Float to set y to</param>
        public virtual void SetRotation(float f)
        {
            // Debug.Log("Set rotation to " + f + " degrees on " + gameObject.name);
            transform.rotation = Quaternion.Euler(0, f, 0);
        }

        /// <summary>
        /// Finalizes a rotation 
        /// </summary>
        public virtual void FinalizeRotation()
        {
            // Debug.Log("Finalize rotation");
            initRotation = true;
            // FinalizeReposition();
        }

        /// <summary>
        /// Scales the object uniformly
        /// </summary>
        /// <param name="scale">Uniform scale</param>
        public virtual void Scale(float scale)
        {
            currentScale = scale;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        // public virtual void FinalizeReposition()
        // {
        //     // ?
        // }

        public virtual void Delete()
        {
            Debug.Log("Deleting");

            Destroy(gameObject);
        }

        public virtual void Deselect()
        {

        }
    }
}