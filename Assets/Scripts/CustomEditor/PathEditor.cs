using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
[CustomEditor(typeof(Path))]

public class PathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        Path path = (Path)target;

        if (GUILayout.Button("Generate Grid"))
        {
            path.GenerateGrid();
        }

        if (GUILayout.Button("Destroy Grid"))
        {
            path.DestroyGrid();
        }

        if (GUILayout.Button("Generate Lines"))
        {
            path.GenerateLines();
        }

        if (GUILayout.Button("Destroy Lines"))
        {
            path.DestroyLines();
        }

    }
}
