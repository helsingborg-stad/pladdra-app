using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pladdra.DefaultAbility
{
    public class SceneObjectController : MonoBehaviour
    {
        protected ProposalManager proposalManager;
        protected UXManager uxManager;

        protected float initRotY;
        protected float rotation;
        protected bool initRotation;
        protected Vector2 initVector;

        protected bool initMove;
        protected Vector3 initPos;
        protected Vector3 initTouchPos;

        protected float currentScale;
        public float CurrentScale { get => currentScale; }
        // TODO Set the resource also from static generation
        
        /// <summary>
        /// Initializes the controller
        /// </summary>
        /// <param name="proposalManager">Reference to proposalManager</param>
        /// <param name="uxManager">Reference to UIManager</param>
        public virtual void Init(ProposalManager proposalManager, UXManager uxManager)
        {
            this.proposalManager = proposalManager;
            this.uxManager = uxManager;
        }
        public virtual void Select()
        {
            Debug.Log("Selected " + gameObject.name);
        }

        /// <summary>
        /// Moves the object to a new position
        /// </summary>
        /// <param name="position">The position to move the object to</param>
        public virtual void Move(Vector3 position)
        {
            //uxManager.Project.staticResourcesOrigin.transform.position = position;
            //transform.position = position;
            if (initMove == true)
            {
                initPos = transform.position;
                initTouchPos = position;
                initMove = false;
            }
            else
            {
                transform.position = initPos + (position - initTouchPos);
            }
        }

        /// <summary>
        /// Finalizes a move
        /// </summary>
        public virtual void FinalizeMove()
        {
            Debug.Log("Finalize move");
            initMove = true;
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
                Debug.Log("Init rotation");
                initRotY = transform.rotation.eulerAngles.y;
                initVector = touch2.position - touch1.position;
                initRotation = false;
            }
            else
            {
                rotation = Vector2.SignedAngle(initVector, touch2.position - touch1.position) - (uxManager.ObjectRotationSpeed);
                transform.rotation = Quaternion.Euler(0, initRotY - rotation, 0);
            }
        }

        /// <summary>
        /// Sets rotation for Y axis
        /// </summary>
        /// <param name="f">Float to set y to</param>
        public void SetRotation(float f)
        {
            transform.rotation = Quaternion.Euler(0, f, 0);
        }

        /// <summary>
        /// Finalizes a rotation 
        /// </summary>
        public virtual void FinalizeRotation()
        {
            Debug.Log("Finalize rotation");
            initRotation = true;
            FinalizeReposition();
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
        public virtual void FinalizeReposition()
        {
            // ?
        }

        public virtual void Delete()
        {
            // This happens in inherited classes
        }

        public virtual void Deselect()
        {

        }
    }
}