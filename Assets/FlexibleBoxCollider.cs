using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FlexibleBoxCollider : MonoBehaviour
{
    public FlexibleBounds flexibleBounds;

    // Start is called before the first frame update
    void Start()
    {
        SetBoxColliderSize();
    }

    public void SetBoxColliderSize()
    {
        var bounds = flexibleBounds.CalculateBoundsFromChildren(gameObject);
        var boxCollider = GetComponent<BoxCollider>();

        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
    }
}
