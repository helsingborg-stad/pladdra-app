using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThumbnail : MonoBehaviour
{
    public Texture2D thumbnail;
    // Start is called before the first frame update
    void Start()
    {
        thumbnail = RuntimePreviewGenerator.GenerateModelPreview(
            transform,
            256,
            256,
            true
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
