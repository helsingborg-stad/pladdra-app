using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public Vector3 RotateAmount = Vector3.zero;  // degrees per second to rotate in each axis. Set in inspector.

    
    void Update () {
        transform.Rotate(RotateAmount * Time.deltaTime);
    }
}
