using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pladdra.DefaultAbility
{
    public class SceneObjectController : MonoBehaviour
    {
        protected ProposalManager proposalManager;
        protected InteractionManager interactionManager;

        protected float initTouchDegree;
        protected float initRotY;
        protected float rotation;
        protected bool initRotation;

        protected float currentScale;
        public float CurrentScale { get => currentScale; }
        // TODO Set the resource also from static generation
        public virtual void Init(ProposalManager proposalManager, InteractionManager interactionManager)
        {
            this.proposalManager = proposalManager;
            this.interactionManager = interactionManager;
        }
        public virtual void Select()
        {
            Debug.Log("Selected " + gameObject.name);
        }

        public virtual void Move(Vector3 position)
        {
            transform.position = position;
        }

        public virtual void Rotate(Touch touch1, Touch touch2)
        {
            if (initRotation == true)
            {
                Debug.Log("Init rotation");
                initTouchDegree = Vector2.SignedAngle(touch1.position, touch2.position);
                initRotY = transform.rotation.eulerAngles.y;
                initRotation = false;
            }
            else
            {
                rotation = Vector2.SignedAngle(touch1.position, touch2.position) - (initTouchDegree * interactionManager.ObjectRotationSpeed);
                transform.rotation = Quaternion.Euler(0, initRotY - rotation, 0);
            }
        }

        public void Rotate(float f)
        {
            // TODO Make this use initital rotation and not clash with other rotation
            transform.Rotate(0, f, 0);
        }

        public void SetRotation(float f)
        {
            transform.rotation = Quaternion.Euler(0, f, 0);
        }

        public virtual void FinalizeRotation()
        {
            Debug.Log("Finalize rotation");
            initRotation = true;
            FinalizeReposition();
        }

        public virtual void Scale(float scale)
        {
            currentScale = scale;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        public virtual void FinalizeReposition()
        {

        }

        public virtual void Delete()
        {
            Debug.Log("Deleted");
        }

        public virtual void Deselect()
        {
            Debug.Log("Deselected " + gameObject.name);
        }
    }
}