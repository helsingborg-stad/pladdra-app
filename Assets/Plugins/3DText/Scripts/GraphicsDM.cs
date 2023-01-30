using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsDM : MonoBehaviour
{
    public string word;
    //public Writer writer;
    public Font3D font3D;

    public Material material;

    private float fontwidth = 0f;
    public int charcterCount;
    Vector3 LetterPos;
    public float linespacing = 0.5f;
    private Matrix4x4 matrix;
    public float letterScale;
    public float letterDepth;
    public bool newWord;


    public void CreateWord()
    {
        int increment = 0;
        fontwidth = 0;
        charcterCount = 0;

        //Vector3 LetterPos;
        foreach (char letter in word)
        {
            Vector3 scale = new Vector3(letterScale, letterScale, letterDepth);
            

            LetterPos = new Vector3(transform.position.x + 0 + fontwidth, transform.position.y, transform.position.z);
            matrix = Matrix4x4.TRS(LetterPos, transform.rotation, scale);
            //GameObject Letter =
            FontMakerx(letter.ToString());//, LetterPos, transform.rotation);
            //Letter.name += letter.ToString();
            //Letter.transform.parent = empty.transform;
            increment++;
            charcterCount++;
            fontwidth = linespacing * increment;
        }

        
    }  
    
    public GameObject FontMakerx(string letter)
    {
        switch (letter)
        {
            case "A":
                Graphics.DrawMesh(font3D.A.GetComponent<MeshFilter>().sharedMesh, matrix, material,0); break;
            case "B":
                Graphics.DrawMesh(font3D.B.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "C":
                Graphics.DrawMesh(font3D.C.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "D":
                Graphics.DrawMesh(font3D.D.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "E":
                Graphics.DrawMesh(font3D.E.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "F":
                Graphics.DrawMesh(font3D.F.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "G":
                Graphics.DrawMesh(font3D.G.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "H":
                Graphics.DrawMesh(font3D.H.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "I":
                Graphics.DrawMesh(font3D.I.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "J":
                Graphics.DrawMesh(font3D.J.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "K":
                Graphics.DrawMesh(font3D.K.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "L":
                Graphics.DrawMesh(font3D.L.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "M":
                Graphics.DrawMesh(font3D.M.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "N":
                Graphics.DrawMesh(font3D.N.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "O":
                Graphics.DrawMesh(font3D.O.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "P":
                Graphics.DrawMesh(font3D.P.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "Q":
                Graphics.DrawMesh(font3D.Q.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "R":
                Graphics.DrawMesh(font3D.R.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "S":
                Graphics.DrawMesh(font3D.S.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "T":
                Graphics.DrawMesh(font3D.T.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "U":
                Graphics.DrawMesh(font3D.U.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "V":
                Graphics.DrawMesh(font3D.V.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "W":
                Graphics.DrawMesh(font3D.W.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "X":
                Graphics.DrawMesh(font3D.X.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "Y":
                Graphics.DrawMesh(font3D.Y.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "Z":
                Graphics.DrawMesh(font3D.Z.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;

            case "a":
                Graphics.DrawMesh(font3D.a.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "b":
                Graphics.DrawMesh(font3D.b.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "c":
                Graphics.DrawMesh(font3D.c.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "d":
                Graphics.DrawMesh(font3D.d.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "e":
                Graphics.DrawMesh(font3D.e.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "f":
                Graphics.DrawMesh(font3D.f.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "g":
                Graphics.DrawMesh(font3D.g.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "h":
                Graphics.DrawMesh(font3D.h.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "i":
                Graphics.DrawMesh(font3D.i.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "j":
                Graphics.DrawMesh(font3D.j.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "k":
                Graphics.DrawMesh(font3D.k.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "l":
                Graphics.DrawMesh(font3D.l.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "m":
                Graphics.DrawMesh(font3D.m.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "n":
                Graphics.DrawMesh(font3D.n.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "o":
                Graphics.DrawMesh(font3D.o.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "p":
                Graphics.DrawMesh(font3D.p.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "q":
                Graphics.DrawMesh(font3D.q.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "r":
                Graphics.DrawMesh(font3D.r.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "s":
                Graphics.DrawMesh(font3D.s.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "t":
                Graphics.DrawMesh(font3D.t.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "u":
                Graphics.DrawMesh(font3D.u.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "v":
                Graphics.DrawMesh(font3D.v.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "w":
                Graphics.DrawMesh(font3D.w.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "x":
                Graphics.DrawMesh(font3D.x.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "y":
                Graphics.DrawMesh(font3D.y.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "z":
                Graphics.DrawMesh(font3D.z.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case " ":
                //Graphics.DrawMesh(font3D.space.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0);
                break;

            case "0":
                Graphics.DrawMesh(font3D.zero.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "1":
                Graphics.DrawMesh(font3D.one.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "2":
                Graphics.DrawMesh(font3D.two.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "3":
                Graphics.DrawMesh(font3D.three.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "4":
                Graphics.DrawMesh(font3D.four.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "5":
                Graphics.DrawMesh(font3D.five.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "6":
                Graphics.DrawMesh(font3D.six.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "7":
                Graphics.DrawMesh(font3D.seven.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "8":
                Graphics.DrawMesh(font3D.eight.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;
            case "9":
                Graphics.DrawMesh(font3D.nine.GetComponent<MeshFilter>().sharedMesh, matrix, material, 0); break;

            /*case "!":
                Instantiate(font3D.exclamation, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "\"":
                Instantiate(font3D.quote, position, rotation, empty.transform).GetComponent<MeshRenderer>().material = material; ;
                break;
            case "£":
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
                Instantiate(font3D.seperator, position, rotation, empty.transform);
                break;*/

        }
        return (null);

    }

  

    public void meshword(string word)
    {
        int myInt = 0;
        MeshFilter[] myMesh = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter mesh in myMesh)
        {
            Mesh getmesh = mesh.sharedMesh;
            // will make the mesh appear in the Scene at origin position
            Graphics.DrawMesh(getmesh, transform.position + new Vector3(myInt, 0, 0), Quaternion.identity, material, 0);
            myInt++;
        }
    }
}
