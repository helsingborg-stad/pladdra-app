
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Writer : MonoBehaviour
{
    [HideInInspector]
    public string Textbox;

    public Font3D font3D;
    public Material material;

    [HideInInspector]
    public GameObject empty;
    [HideInInspector]
    public float fontwidth = 0f;
    public float spaceWidth = 1f;

    Vector3 LetterPos;

    [HideInInspector]
    public float _thickness = 2.0f;
    [HideInInspector]
    public float _scale = 30.0f;
    [HideInInspector]
    public float _width = 0.5f;
    [HideInInspector]
    public float _rotation = 0;
    [HideInInspector]
    public float _shear = 1;

    [HideInInspector]
    public int charcterCount;

    [HideInInspector]
    public bool AlignLeft;
    [HideInInspector]
    public bool AlignCenter;
    [HideInInspector]
    public bool AlignRight;
    [HideInInspector]
    public float alignment;

    public GameObject FontMaker(string letter, Vector3 position, Quaternion rotation)
    {
        switch (letter)
        {
            case "A":
                Instantiate(font3D.A, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material;
                break;
            case "B":
                Instantiate(font3D.B, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "C":
                Instantiate(font3D.C, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "D":
                Instantiate(font3D.D, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "E":
                Instantiate(font3D.E, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "F":
                Instantiate(font3D.F, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "G":
                Instantiate(font3D.G, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "H":
                Instantiate(font3D.H, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "I":
                Instantiate(font3D.I, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "J":
                Instantiate(font3D.J, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "K":
                Instantiate(font3D.K, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "L":
                Instantiate(font3D.L, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "M":
                Instantiate(font3D.M, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "N":
                Instantiate(font3D.N, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "O":
                Instantiate(font3D.O, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "P":
                Instantiate(font3D.P, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "Q":
                Instantiate(font3D.Q, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "R":
                Instantiate(font3D.R, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "S":
                Instantiate(font3D.S, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "T":
                Instantiate(font3D.T, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "U":
                Instantiate(font3D.U, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "V":
                Instantiate(font3D.V, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "W":
                Instantiate(font3D.W, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "X":
                Instantiate(font3D.X, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "Y":
                Instantiate(font3D.Y, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "Z":
                Instantiate(font3D.Z, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;

            case "a":
                Instantiate(font3D.a, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "b":
                Instantiate(font3D.b, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "c":
                Instantiate(font3D.c, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "d":
                Instantiate(font3D.d, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "e":
                Instantiate(font3D.e, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "f":
                Instantiate(font3D.f, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "g":
                Instantiate(font3D.g, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "h":
                Instantiate(font3D.h, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "i":
                Instantiate(font3D.i, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "j":
                Instantiate(font3D.j, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "k":
                Instantiate(font3D.k, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "l":
                Instantiate(font3D.l, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "m":
                Instantiate(font3D.m, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "n":
                Instantiate(font3D.n, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "o":
                Instantiate(font3D.o, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "p":
                Instantiate(font3D.p, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "q":
                Instantiate(font3D.q, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "r":
                Instantiate(font3D.r, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "s":
                Instantiate(font3D.s, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "t":
                Instantiate(font3D.t, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "u":
                Instantiate(font3D.u, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "v":
                Instantiate(font3D.v, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "w":
                Instantiate(font3D.w, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "x":
                Instantiate(font3D.x, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "y":
                Instantiate(font3D.y, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "z":
                Instantiate(font3D.z, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case " ":
                Instantiate(font3D.space, position, rotation, empty.transform);
                break;

            case "0":
                Instantiate(font3D.zero, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "1":
                Instantiate(font3D.one, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "2":
                Instantiate(font3D.two, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "3":
                Instantiate(font3D.three, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "4":
                Instantiate(font3D.four, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "5":
                Instantiate(font3D.five, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "6":
                Instantiate(font3D.six, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "7":
                Instantiate(font3D.seven, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "8":
                Instantiate(font3D.eight, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "9":
                Instantiate(font3D.nine, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;

            case "!":
                Instantiate(font3D.exclamation, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "\"":
                Instantiate(font3D.quote, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "�":
                Instantiate(font3D.pound, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "$":
                Instantiate(font3D.dollar, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "%":
                Instantiate(font3D.percent, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "^":
                Instantiate(font3D.uparrow, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "&":
                Instantiate(font3D.ampersand, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "*":
                Instantiate(font3D.astrix, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "(":
                Instantiate(font3D.openbracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case ")":
                Instantiate(font3D.closebracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "-":
                Instantiate(font3D.minus, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "_":
                Instantiate(font3D.underscore, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "=":
                Instantiate(font3D.equals, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "+":
                Instantiate(font3D.add, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "{":
                Instantiate(font3D.opencurlybracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "}":
                Instantiate(font3D.closecurlybracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "[":
                Instantiate(font3D.opensquarebracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "]":
                Instantiate(font3D.closesquarebracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case ":":
                Instantiate(font3D.colon, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case ";":
                Instantiate(font3D.semicolon, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "'":
                Instantiate(font3D.apostrophe, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "@":
                Instantiate(font3D.at, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "<":
                Instantiate(font3D.openpointybracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case ">":
                Instantiate(font3D.closepointybracket, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case ",":
                Instantiate(font3D.comma, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case ".":
                Instantiate(font3D.fullstop, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "/":
                Instantiate(font3D.slash, position, rotation, empty.transform);
                break;
            case "?":
                Instantiate(font3D.question, position, rotation, empty.transform);
                break;
            case "\\":
                Instantiate(font3D.backslash, position, rotation, empty.transform);
                break;
            case "|":
                Instantiate(font3D.seperator, position, rotation, empty.transform);
                break;
            case "\n":
                //line = line +1;
                break;

        }

        return (empty);

    }

    bool isInit = false;

    private void Init()
    {
        empty = transform.GetComponent<Transform>().gameObject;
        isInit = true;
    }


    public void CreateWord()
    {
        if (!isInit)
        {
            Init();
        }
        int increment = 0;
        fontwidth = 0;
        charcterCount = 0;
        // empty.name = null;
        //Vector3 LetterPos;

        // ! Had to reverse this for some odd reason
        for (int i = Textbox.Length - 1; i >= 0; i--)
        {
            LetterPos = new Vector3(empty.transform.position.x + 0 + fontwidth, empty.transform.position.y, empty.transform.position.z);
            GameObject Letter = FontMaker(Textbox[i].ToString(), LetterPos, empty.transform.rotation);
            // Letter.name += Textbox[i].ToString();
            //Letter.transform.parent = empty.transform;
            increment++;
            charcterCount++;
            fontwidth = _width * increment;
        }
    }

    public void ClearWord()
    {

        if (empty.transform.childCount > 0)
        {
            DestroyImmediate(empty.transform.GetChild(0).gameObject);
            ClearWord();
            charcterCount = 0;
        }

    }

    public void TextBoxChange(string _Text)
    {
        Textbox = _Text;
    }

    public void width(float width)
    {
        _width = width;
        float incrament = 0;
        foreach (Transform child in empty.transform)
        {

            child.transform.GetComponentInChildren<Transform>().localPosition = Vector3.right * -_width * incrament - Vector3.right * -alignment;
            incrament++;
        }

    }

    public void Rotation(float rotation)
    {
        _rotation = rotation;
        foreach (Transform child in empty.transform)
        {
            child.transform.GetComponentInChildren<Transform>().localRotation = Quaternion.Euler(0, _rotation, 0);
        }
    }


    public void Scale(float shear, float scale, float thickness)
    {
        _thickness = thickness;
        _scale = scale;
        _shear = shear;
        foreach (Transform child in empty.transform)
        {

            child.transform.localScale = new Vector3(_scale * _shear, _scale, _scale * _thickness);

        }
    }

    public void AddModuleToWord(int mode)
    {
        if (GetComponent<bobbingSpinning>() != true)
        {
            empty.AddComponent<bobbingSpinning>();
            var module = empty.GetComponent<bobbingSpinning>();

            if (mode == 0)
            {
                module.x = true;
            }
            if (mode == 1)
            {
                module.y = true;
            }
            if (mode == 2)
            {
                module.z = true;
            }
            if (mode == 3)
            {
                module.canRotate = true;
                module.rotationSpeed = new Vector3(0, 25, 0);
            }

        }
        else
        {
            var module = empty.GetComponent<bobbingSpinning>();

            if (mode == 0)
            {
                module.x = true;
            }
            if (mode == 1)
            {
                module.y = true;
            }
            if (mode == 2)
            {
                module.z = true;
            }
            if (mode == 3)
            {
                module.canRotate = true;
                module.rotationSpeed = new Vector3(0, 25, 0);
            }
        }
    }

    public void AddModuleToLetters(int mode)
    {
        float newDelay = 0.0f;
        foreach (Transform child in empty.transform)
        {
            if (child.gameObject.GetComponent<bobbingSpinning>() != true)
            {
                child.gameObject.AddComponent<bobbingSpinning>();
            }

            var module = child.GetComponents<bobbingSpinning>();



            foreach (bobbingSpinning script in module)
            {
                script.delay = newDelay;
                newDelay += 0.1f;
                //script.y = true;

                if (mode == 0)
                {
                    script.x = true;
                }
                if (mode == 1)
                {
                    script.y = true;
                }
                if (mode == 2)
                {
                    script.z = true;
                }
                if (mode == 3)
                {
                    script.canRotate = true;
                    script.rotationSpeed = new Vector3(0, 25, 0);
                }
            }
        }
    }

    public void addRigidbodyToWord()
    {
        // if (GetComponent<Rigidbody>() != true)
        // {


        //     foreach (Transform child in empty.transform)
        //     {
        //         child.gameObject.GetComponent<MeshCollider>().convex = true;
        //     }
        //     empty.AddComponent<Rigidbody>();
        // }

    }



    public void addRigidbodyToLetters()
    {
        // foreach (Transform child in empty.transform)
        // {
        //     if (child.gameObject.GetComponent<Rigidbody>() != true)
        //     {
        //         child.gameObject.GetComponent<MeshCollider>().convex = true;
        //         child.gameObject.AddComponent<Rigidbody>();
        //     }
        // }
    }

    public void RemoveModules()
    {
        DestroyImmediate(GetComponent<bobbingSpinning>());
        foreach (Transform child in empty.transform)
        {
            DestroyImmediate(child.GetComponent<bobbingSpinning>());
        }
    }

    public void RemoveRigidbodys()
    {
        DestroyImmediate(GetComponent<Rigidbody>());
        foreach (Transform child in empty.transform)
        {
            DestroyImmediate(child.GetComponent<Rigidbody>());
        }
    }

    public void addCollider()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(fontwidth / 9, _scale, _thickness);
    }

    public void complete()
    {
        DestroyImmediate(GetComponent<Writer>());
    }

    public void Align()
    {
        if (AlignLeft)
        {
            alignment = 0;
        }
        if (AlignCenter)
        {
            alignment = (fontwidth - _width) / 2;
        }
        if (AlignRight)
        {
            alignment = fontwidth - _width;
        }


        foreach (Transform child in empty.transform)
        {
            //  child.gameObject.transform.position = new Vector3(alignment, 0, 0);
            child.transform.GetComponentInChildren<Transform>().localPosition = Vector3.right * alignment;
        }
        ClearWord();
        CreateWord();
    }

    /*public void meshDeform(float multiple)
    {
        foreach (Transform child in empty.transform)
        {
          Mesh mesh = child.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;

            //mesh.Clear();
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] += normals[i] + new Vector3(multiple,multiple,multiple);
            }
            mesh.vertices = vertices;
        }
    }*/
}
