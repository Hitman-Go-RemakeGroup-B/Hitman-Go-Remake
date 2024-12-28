#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TestScript))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestScript taarget = (TestScript)target;

        if (GUILayout.Button("helloWorld"))
        {
            taarget.MegaDebug();
        }

        EditorGUI.TextArea(new(0, 150, 200, 200),taarget.vector.ToString());
    }
}

#endif