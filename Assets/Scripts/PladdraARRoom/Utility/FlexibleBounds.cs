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
        Quaternion currentRotation = gameObject.transform.rotation;        
        Vector3 currentScale = gameObject.transform.localScale;
        
        gameObject.transform.rotation = Quaternion.Euler(0f,0f,0f);
        gameObject.transform.localScale = Vector3.one;
        
        IEnumerable<Renderer> renderers = gameObject.GetComponentsInChildren<Renderer>(false);
        IEnumerable<Collider> colliders = gameObject.GetComponentsInChildren<Collider>(false);
       
        // TODO: Include collider bounds
        Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
        foreach (Renderer child in renderers)
        {
            bounds.Encapsulate(child.bounds);
        }
        
        bounds.center -= gameObject.transform.position;
        
        gameObject.transform.rotation = currentRotation;
        gameObject.transform.localScale = currentScale;
        
        return bounds;
    }
}
