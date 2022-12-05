using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.DefaultAbility.Data;
using UnityEngine;
using UntoldGarden.Utils;

namespace Pladdra.DefaultAbility
{
    public class PlacedObjectController : SceneObjectController
    {
        public PladdraResource Resource;
        string id;
        public string Id { get => id; }

        Coroutine updateProposal;
        public override void Init(ProposalManager proposalManager, InteractionManager interactionManager)
        {
            base.Init(proposalManager, interactionManager);
            id = Guid.NewGuid().ToString();

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();

            // TODO Move to collider extensions
            Bounds bounds = gameObject.GetBounds();
            bounds.size = Vector3.Scale(bounds.size, transform.localScale);
            float scaleFactor = 3f;
            bounds.size = Vector3.Scale(bounds.size, new Vector3(scaleFactor, scaleFactor, scaleFactor));
            bounds.center -= transform.position;
            boxCollider.size = bounds.size;
            boxCollider.center = bounds.center;
        }

        public override void Move(Vector3 position)
        {
            if (updateProposal != null)
            {
                StopCoroutine(updateProposal);
            }
            base.Move(position);
            updateProposal = StartCoroutine(UpdateProposal());
        }

        public override void Rotate(Touch touch1, Touch touch2)
        {
            if (updateProposal != null)
            {
                StopCoroutine(updateProposal);
            }
            base.Rotate(touch1, touch2);
            updateProposal = StartCoroutine(UpdateProposal());
        }

        IEnumerator UpdateProposal()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            proposalManager.UpdateProposal(id, transform.localPosition, transform.rotation.eulerAngles.y, currentScale);
        }

        public override void Delete()
        {
            base.Delete();
            proposalManager.RemoveObject(this);
        }

        public override void Select()
        {
            base.Select();
            gameObject.SetAllChildLayers("Selected");
        }

        public override void Deselect()
        {
            base.Deselect();
            gameObject.SetAllChildLayers("Default");
        }

    }
}