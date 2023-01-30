using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobbingSpinning : MonoBehaviour
{
    public float power = 0.1f;
    public float frequency = 1f;
    public float delay = 0.1f;

    public bool y;
    public bool x;
    public bool z;

    public bool canRotate;
    public Vector3 rotationSpeed;

    Vector3 StartPos = new Vector3();
    Vector3 TempPos = new Vector3();

    Vector3 StartRot = new Vector3();
    Vector3 TempRot = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        TempPos = StartPos;

        if (x)
        {
            TempPos.x += Mathf.Sin((Time.fixedTime + delay) * Mathf.PI * frequency) * power;
            transform.position = TempPos;
        }

        if (y)
        {
            TempPos.y += Mathf.Sin((Time.fixedTime + delay) * Mathf.PI * frequency) * power;
            transform.position = TempPos;
        }

        if (z)
        {
            TempPos.z += Mathf.Sin((Time.fixedTime + delay) * Mathf.PI * frequency) * power;
            transform.position = TempPos;
        }

        if(canRotate)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
         
    }
}
