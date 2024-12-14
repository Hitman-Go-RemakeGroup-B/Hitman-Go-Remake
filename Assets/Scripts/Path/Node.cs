using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [HideInInspector] 
    public Vector2Int GridCoordinate;

    public int HCost;
    public int GCost;
    public int FCost => HCost+GCost;
    //public Transform Position;
}
