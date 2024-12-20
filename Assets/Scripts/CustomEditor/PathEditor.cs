#if UNITY_EDITOR
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
            if (Path.NodeArray != null && Path.NodeArray.Length > 0)
            {

                path.DestroyNodeArray();
            }
            else
                path.DestroyGridArray();
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



#endif
