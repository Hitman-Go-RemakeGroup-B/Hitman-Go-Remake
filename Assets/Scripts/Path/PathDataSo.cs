using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PathSo", menuName = "Assets/PathData")]
public class PathDataSo : ScriptableObject
{
    public Node[,] NodeArray;
    [HideInInspector] public List<GameObject> LineList;
    [HideInInspector] public GameObject[,] GridArray;
    [HideInInspector] public Vector3 GenerateFromPosition;
}
