using System;
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
    [HideInInspector] public SpriteRenderer NodeSpriteRenderer;
    [HideInInspector] public Color oldColor;
    public delegate void NodeHiglight(Node node, Color color, bool isHiglight);
    public NodeHiglight OnColorChange;
    public int FCost => HCost+GCost;

    private void Awake()
    {
        NodeSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

}
