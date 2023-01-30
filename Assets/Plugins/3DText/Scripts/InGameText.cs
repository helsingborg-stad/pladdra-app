using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameText : MonoBehaviour
{
    
    public Writer writerGO;
    public Font3D myFont;
    public Material material;
    public bool KeepWordTogether;

    [Range(0.1f, 2f)]
    public float spacing;
    [Range(0.1f, 1f)]
    public float shear;
    [Range(1f, 50f)]
    public float scale;
    [Range(0.1f, 20f)]
    public float thickness;
    [Range(-90f, 90f)]
    public float rotation;
    
    public UnityEvent OnWordCreated;


    // Update is called once per frame

    public void MakeNewWordInput(InputField input)
    {
        MakeNewWord(input.text);
    }

    public void MakeNewWord(string word)
    {
        Writer myWriter = Instantiate(writerGO, transform);
        myWriter.TextBoxChange(word);
        myWriter.material = material;
        myWriter.font3D = myFont;
        myWriter.CreateWord();
        myWriter.width(spacing);
        myWriter.Scale(shear, scale, thickness);
        myWriter.Rotation(rotation);
        if (KeepWordTogether)
        {
            myWriter.addRigidbodyToWord();
        }
        else
        {
            myWriter.addRigidbodyToLetters();
        }
        
        myWriter.complete();


        OnWordCreated.Invoke();
    }
}
