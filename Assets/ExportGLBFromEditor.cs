using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UntoldGarden.Utils;
using System.Linq;
using Pladdra;

public class ExportGLBFromEditor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Material[] materials = gameObject.GetComponentsInChildren<MeshRenderer>().SelectMany(mr => mr.sharedMaterials).ToArray();
        foreach (Material material in materials)
        {
            if (material.mainTexture == null)
                material.ColorToTexture();
        }
        ExportGLB.AdvancedExport(gameObject, Application.persistentDataPath + "/" + gameObject.name + ".glb");
    }
}
