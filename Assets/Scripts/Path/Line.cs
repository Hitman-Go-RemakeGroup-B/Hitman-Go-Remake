using UnityEngine;

[System.Serializable]
public class Line
{
    public Transform StartNodeTransform;
    public Transform EndNodeTransfrom;
    private Node _startNode;
    private Node _endNode;
}
