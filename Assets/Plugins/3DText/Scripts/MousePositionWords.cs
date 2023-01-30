using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionWords : MonoBehaviour
{
    public GraphicsDM myText;

    // Update is called once per frame
    void Update()
    {
        //string word = writer.Textbox.ToString();
        myText.CreateWord();
        myText.word = "Mouse Position= " + Input.mousePosition.ToString();
    }
}
