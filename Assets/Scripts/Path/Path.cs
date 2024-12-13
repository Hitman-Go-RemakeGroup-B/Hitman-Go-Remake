using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Node[] Nodes;
    public Line[] Lines;

    public Node GetNodeFromTransform(Transform givenTransform)
    {
        foreach (Node node in Nodes)
        {
            if(node.Position == givenTransform)
                { return node; }
        }
        
        return null;
    }

    public Node GetNodeFromCoordinate(Vector2 givenCoordinate)
    {
        foreach (Node node in Nodes)
        {
            if (node.Coordinate == givenCoordinate)
            { return node; }
        }

        return null;
    }
}
