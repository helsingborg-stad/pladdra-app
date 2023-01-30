using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{

    public GameObject pointer;
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

    private Transform _position;

    public void MakeNewWord(string word)
    {
        Writer myWriter = Instantiate(writerGO, _position.position,_position.rotation,null);
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
    }

    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
            pointer.transform.position = hit.point;
            }

            if (Input.GetMouseButtonDown(0))
        {
            _position = pointer.transform;
            MakeNewWord(Random.Range(1, 10).ToString() + "Hp");
        }
    }
    
}
