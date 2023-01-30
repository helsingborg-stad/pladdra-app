using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(Writer))]
public class WritingEditor : Editor
{

    [SerializeField] static float thickness = 0.1f;
    [SerializeField] static float scale = 1.0f;
    [SerializeField] static float width = 0.5f;
    [SerializeField] static float rotation = 0f;
    [SerializeField] static float shearValue = 1f;
    [SerializeField] static string editorText;
   // private SerializedProperty __Font3D;

    private void OnEnable()
    {
        Writer myScript = (Writer)target;
        thickness = myScript._thickness;
        scale = myScript._scale;
        width = myScript._width;
        rotation = myScript._rotation;
        shearValue = myScript._shear;
    }

    
    public override void OnInspectorGUI()
    {

        Texture2D inout = EditorGUIUtility.Load("Assets/3DText/Editor/Icons/inout.png") as Texture2D;
        Texture2D updown = EditorGUIUtility.Load("Assets/3DText/Editor/Icons/leftright.png") as Texture2D;
        Texture2D leftright = EditorGUIUtility.Load("Assets/3DText/Editor/Icons/updown.png") as Texture2D;
        Texture2D rotate = EditorGUIUtility.Load("Assets/3DText/Editor/Icons/rotate.png") as Texture2D;

        Writer myScript = (Writer)target;

        var AlignLeftStyle = new GUIStyle(GUI.skin.button);
        AlignLeftStyle.normal.textColor = Color.white;
        var AlignCenterStyle = new GUIStyle(GUI.skin.button);
        AlignCenterStyle.normal.textColor = Color.white;
        var AlignRightStyle = new GUIStyle(GUI.skin.button);
        AlignRightStyle.normal.textColor = Color.white;
        //style.active.textColor = Color.green;

        if (myScript.AlignLeft == true)
        {
            AlignLeftStyle.normal.textColor = new Color(3 / 100, 255 / 100, 119 / 100, 1f);
            AlignLeftStyle.fontSize = 16;
            AlignCenterStyle.normal.textColor = Color.white;
            AlignCenterStyle.fontSize = 10;
            AlignRightStyle.normal.textColor = Color.white;
            AlignRightStyle.fontSize = 10;
        }
        if (myScript.AlignCenter == true)
        {
            AlignLeftStyle.normal.textColor = Color.white;
            AlignLeftStyle.fontSize = 10;
            AlignCenterStyle.normal.textColor = new Color(3 / 100, 255 / 100, 119 / 100, 1f);
            AlignCenterStyle.fontSize = 16;
            AlignRightStyle.normal.textColor = Color.white;
            AlignRightStyle.fontSize = 10;
        }
        if (myScript.AlignRight == true)
        {
            AlignLeftStyle.normal.textColor = Color.white;
            AlignLeftStyle.fontSize = 10;
            AlignCenterStyle.normal.textColor = Color.white;
            AlignCenterStyle.fontSize = 10;
            AlignRightStyle.normal.textColor = new Color(3 / 100, 255 / 100, 119 / 100, 1f);
            AlignRightStyle.fontSize = 16;
        }





        GUI.skin.button.wordWrap = true;

        serializedObject.Update();
        Texture TextureLogo = (Texture)AssetDatabase.LoadAssetAtPath("Assets/3DText/Editor/Capture.png", typeof(Texture));
        GUILayout.Box(TextureLogo, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width * 0.1f)/*,GUILayout.MaxHeight(Screen.height * 0.1f)*/, GUILayout.MinHeight(Screen.height * 0.125f));

        
        
        //if (myScript.AlignLeft == true) { GUI.backgroundColor = new Color(3 / 100, 255 / 100, 119 / 100, 0.5f); }
        //else if (myScript.AlignLeft == false) { GUI.backgroundColor = new Color(3 / 100, 255 / 100, 119 / 100, 0.5f); }

        #region TextBox
        GUILayout.BeginVertical("Text", "window");
        //EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
        
        string editorText = EditorGUILayout.TextField(myScript.Textbox);
        if (myScript.Textbox != editorText)
        {
        myScript.TextBoxChange(editorText);
        }
        GUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        #region Inspector
        GUILayout.BeginVertical("Font Variables","window");
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        //DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });
        //EditorGUILayout.PropertyField(__Font3D);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("material"));
        base.OnInspectorGUI();
        GUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        #region Alignment
        GUILayout.BeginVertical("Alignment", "window");

        
        //EditorGUILayout.LabelField("Alignment", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal(GUIStyle.none);
        
        if (GUILayout.Button("|--", AlignLeftStyle, GUILayout.Height(25)))
        {
            
            myScript.AlignLeft = true;
            myScript.AlignCenter = false;
            myScript.AlignRight = false;
            myScript.Align();
        }
        if (GUILayout.Button("-|-", AlignCenterStyle, GUILayout.Height(25)))
        {
            myScript.AlignLeft = false;
            myScript.AlignCenter = true;
            myScript.AlignRight = false;
            myScript.Align();
        }
        if (GUILayout.Button("--|", AlignRightStyle, GUILayout.Height(25)))
        {
            myScript.AlignLeft = false;
            myScript.AlignCenter = false;
            myScript.AlignRight = true;
            myScript.Align();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        #region Create
        GUILayout.BeginVertical("Create", "window");
        //EditorGUILayout.LabelField("Create", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal(GUIStyle.none);
        //GUI.backgroundColor = new Color(3/100,255/100,119/100,0.5f);
        if (GUILayout.Button("Build Word", GUILayout.Height(50)))
        {
            myScript.Textbox = editorText;
            myScript.ClearWord();
            myScript.CreateWord();
        }
        //GUI.backgroundColor = new Color(222 / 100, 59 / 100, 18 / 100, 0.5f);
        if (GUILayout.Button("Clear Word", GUILayout.Height(50)))
        {
            myScript.ClearWord();
        }
        

        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        #region Editables
        GUILayout.BeginVertical("Editables", "window");
        //GUI.backgroundColor = new Color(3 / 100, 255 / 100, 119 / 100, 0.5f);
        //EditorGUILayout.LabelField("Editables", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Depth");
        thickness = EditorGUILayout.Slider(thickness, 0.1f, 20.0f);

        EditorGUILayout.LabelField("Shear");
        shearValue = EditorGUILayout.Slider(shearValue, 0.1f, 1.0f);

        EditorGUILayout.LabelField("Scale");
        scale = EditorGUILayout.Slider(scale, 0.5f, 50.0f);

        EditorGUILayout.LabelField("Spacing");
        width = EditorGUILayout.Slider(width, 0.1f, 2.0f);

        EditorGUILayout.LabelField("Rotation");
        rotation = EditorGUILayout.Slider(rotation, -90.0f, 90.0f);

        // myScript.Scale(shearValue,scale,thickness);
        // myScript.width(width);
        // myScript.Rotation(rotation);
        GUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        #region Modules
        GUILayout.BeginVertical("Animation Modules", "window");

        EditorGUILayout.BeginHorizontal(GUIStyle.none);
        EditorGUILayout.LabelField("Words", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Letters", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        //GUI.backgroundColor = new Color(3 / 100, 255 / 100, 119 / 100, 0.5f);
        EditorGUILayout.BeginHorizontal(GUIStyle.none);

        if (GUILayout.Button(updown, GUILayout.Height(50)))
        {
            myScript.AddModuleToWord(0);
        }

        
        if (GUILayout.Button(updown, GUILayout.Height(50)))
        {
            myScript.AddModuleToLetters(0);
        }
       

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUIStyle.none);

        if (GUILayout.Button(leftright, GUILayout.Height(50)))
        {
            myScript.AddModuleToWord(1);
        }


        if (GUILayout.Button(leftright, GUILayout.Height(50)))
        {
            myScript.AddModuleToLetters(1);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUIStyle.none);

        if (GUILayout.Button(inout, GUILayout.Height(50)))
        {
            myScript.AddModuleToWord(2);
        }


        if (GUILayout.Button(inout, GUILayout.Height(50)))
        {
            myScript.AddModuleToLetters(2);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUIStyle.none);

        if (GUILayout.Button(rotate, GUILayout.Height(50)))
        {
            myScript.AddModuleToWord(3);
        }


        if (GUILayout.Button(rotate, GUILayout.Height(50)))
        {
            myScript.AddModuleToLetters(3);
        }

        EditorGUILayout.EndHorizontal();


        //GUI.backgroundColor = new Color(222 / 100, 59 / 100, 18 / 100, 0.5f);


        if (GUILayout.Button("Remove Modules", GUILayout.Height(25)))
        {
            myScript.RemoveModules();
        }
GUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        #region Rigidbodys
        GUILayout.BeginVertical("Rigidbodys", "window");
        //GUI.backgroundColor = new Color(3 / 100, 255 / 100, 119 / 100, 0.5f);
        //EditorGUILayout.LabelField("Rigidbodys", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal(GUIStyle.none);
        if (GUILayout.Button("Add Rigidbody to Word", GUILayout.Height(50)))
        {
            myScript.addRigidbodyToWord();
        }

        if (GUILayout.Button("Add Rigibody to Letters", GUILayout.Height(50)))
        {
            myScript.addRigidbodyToLetters();
        }
        EditorGUILayout.EndHorizontal();

        //GUI.backgroundColor = new Color(222 / 100, 59 / 100, 18 / 100, 0.5f);
        
        if (GUILayout.Button("Remove Rigidbodys", GUILayout.Height(25)))
        {
            myScript.RemoveRigidbodys();
        }
        EditorGUILayout.EndVertical();
        #endregion
        EditorGUILayout.Space();
        
        GUI.backgroundColor = new Color(239 / 100, 158 / 100, 45 / 100, 1f);
        EditorGUILayout.LabelField(" ", EditorStyles.boldLabel);
        if (GUILayout.Button("Complete", GUILayout.Height(50)))
        {
            myScript.complete();
        }
        
        
    }



}
