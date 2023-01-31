using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlaneVis : MonoBehaviour
{
    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Pladdra.ARDebug.PlaneVisualiser>().AddARObject(go);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
