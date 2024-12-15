using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Line[] Connections;
    [HideInInspector] public Vector2Int GridCoordinate;
    [HideInInspector] public int HCost;
    [HideInInspector] public int GCost;
    [HideInInspector] public Node PreviousNode;
    public int FCost => HCost+GCost;
    //public Transform Position;
}
