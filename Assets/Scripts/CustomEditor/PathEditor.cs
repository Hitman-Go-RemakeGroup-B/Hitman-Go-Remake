using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
[CustomEditor(typeof(Path))]

public class PathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Path path = (Path)target;

        base.OnInspectorGUI();
        

        if (GUILayout.Button("Generate Grid"))
        {
            path.GenerateGrid();
        }

        if (GUILayout.Button("Destroy Grid"))
        {
            path.DestroyGrid();
        }
       
    }
}
