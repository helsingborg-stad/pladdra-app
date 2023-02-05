using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.ARSandbox.Dialogues.Data;
using UnityEngine;
using UntoldGarden.Utils;

namespace Pladdra.ARSandbox.Dialogues
{
    /// <summary>
    /// Controller for interactive objects that the user has placed in the scene.
    /// Updates the proposal when the object is repositioned.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlacedObjectController : InteractiveObjectController
    {

        Coroutine updateProposal;
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

        public override void SetRotation(float rotation)
        {
            if (updateProposal != null)
            {
                StopCoroutine(updateProposal);
            }
            base.SetRotation(rotation);
            updateProposal = StartCoroutine(UpdateProposal());
        }

        public override void Delete()
        {
            project.ProposalHandler.RemoveObject(this);
            base.Delete();
        }

        IEnumerator UpdateProposal()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            project.ProposalHandler.UpdateProposal(id, transform.localPosition, transform.rotation.eulerAngles.y, currentScale);
        }
    }
}