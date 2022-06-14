using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlexibleBounds : MonoBehaviour
{
    public void CalculateBoundsFromChildrenAndThen(GameObject gameObject, Action<Bounds> then)
    {
        then(CalculateBoundsFromChildren(gameObject, false));
    }    
    public Bounds CalculateBoundsFromChildren(GameObject gameObject)
    {
        return CalculateBoundsFromChildren(gameObject, false);
    }

    public Bounds CalculateBoundsFromChildren(GameObject gameObject, bool checkColliders)
    {
        IEnumerable<Renderer> allChildren = gameObject.GetComponentsInChildren<Renderer>(false);
        IEnumerable<Collider> allColliders = gameObject.GetComponentsInChildren<Collider>(false).Where(c => !gameObject.GetComponent<Collider>() || gameObject.GetComponent<Collider>() != c);

        Vector3 center = gameObject.transform.position;
        foreach (Renderer child in allChildren)
        {
            center += child.bounds.center;
        }

        if (checkColliders)
        {
            foreach (Collider collider in allColliders)
            {
                center += collider.bounds.center;
            }
        }
        center /= allChildren.Count();

        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in allChildren)
        {
            bounds.Encapsulate(child.bounds);
        }

        if (checkColliders)
        {
            foreach (Collider collider in allColliders)
            {
                bounds.Encapsulate(collider.bounds);
            }
        }

        Vector3 centerWithRelativeOffset = ((bounds.center - gameObject.transform.position) / gameObject.transform.localScale.x);
        Vector3 sizeWithRelativeOffset = (bounds.size / gameObject.transform.localScale.x);
        bounds = new Bounds(centerWithRelativeOffset, sizeWithRelativeOffset);

        return bounds;
    }
}
