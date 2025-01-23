using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public delegate bool WrongMove(int x, int y);

public class Path : MonoBehaviour
{
    [Header("Setting")]
    public int RowsZ;
    public int CollumsX;
    public Vector2Int WinNodeCoordinates;

    [Tooltip("the space between each node")]
    public float UnitScale = 1;

    [Header("Prefabs")]
    public Node NodePrefab;
    public Line LinePrefab;

    [Header("Data")]
    public static Node[,] NodeArray;

    private Node _winNode;
    private static List<GameObject> _lineList;
    private GameObject[,] _gridArray;
    private Vector3 _generateFromPosition;

    private Controller[] _controllers;
    // public List<Line> Lines; viktor nation what are we feeling? jaybe!? jaybe not?

    private void OnEnable()
    {
        PathInizalization();
        _controllers = FindObjectsOfType<Controller>();
        foreach (Controller controller in _controllers)
        {
            controller.FindPath = FindPath;
            controller.NodeFromCoordinates = GetNodeFromCoordinate;
            controller.CurrentNode = NodeFromWorldPos(controller.transform.position);
        }
    }

    private void OnDisable()
    {
        foreach (Controller controller in _controllers)
        {
            controller.FindPath -= FindPath;
            controller.NodeFromCoordinates -= GetNodeFromCoordinate;
        }
    }

    public void PathInizalization()
    {
        NodeArray = null;
        NodeArray = new Node[CollumsX, RowsZ];

        for (int i = 0; i < transform.childCount; i++)
        {
            Node child = transform.GetChild(i).GetComponent<Node>();
            child.OnColorChange += ChangeColor;
            child.NodeSpriteRenderer = child.GetComponent<SpriteRenderer>();
            NodeArray[child.GridCoordinate.x, child.GridCoordinate.y] = child;
        }
        _winNode = GetNodeFromCoordinate(WinNodeCoordinates);
        _winNode.IsWinNode = true;

#if UNITY_EDITOR
        _winNode.NodeSpriteRenderer.color = Color.yellow;
        _winNode.oldColor = Color.yellow;
#endif

    }

    public List<Node> FindPath(Vector2Int startCoordinate, Vector2Int endCoordinate, WrongMove wrongMove)
    {
        Node startNode = GetNodeFromCoordinate(startCoordinate);
        Node endNode = GetNodeFromCoordinate(endCoordinate);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode); ;
            }

            foreach (Line Connection in currentNode.Connections)
            {
                if (Connection == null)
                    continue;

                Node neighbour = Connection.EndNode;
                if (closedSet.Contains(neighbour))
                    continue;

                int nodeDistance = GetNodeDistance(currentNode, neighbour);
                int newCostNeighbour = currentNode.GCost - nodeDistance;

                if (newCostNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newCostNeighbour;
                    neighbour.HCost = GetNodeDistance(neighbour, endNode);
                    neighbour.PreviousNode = currentNode;
                    openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.PreviousNode;
        }
        path.Reverse();
        return path;
    }

    public int GetNodeDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridCoordinate.x - nodeB.GridCoordinate.x);
        int distZ = Mathf.Abs(nodeA.GridCoordinate.y - nodeB.GridCoordinate.y);

        if (distX > distZ)
            return 14 * distZ + 10 * (distX - distZ);
        else
            return 14 * distX + 10 * (distZ - distX);
    }

    public List<Node> GetNeighboursNodes(Node node)
    {
        if (NodeArray == null)
            PathInizalization();

        List<Node> neighbours = new List<Node>();

        //! change this for Knight
        //x = -2; x<= 2; x++  same thing with the z

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Knight = (x=0, z=0) & abs(x)+abs(z) != 3
                if (x == 0 && z == 0) // this is pawn
                    continue;
                int checkX = node.GridCoordinate.x + x;
                int checkZ = node.GridCoordinate.y + z;

                if (checkX >= 0 && checkX < CollumsX && checkZ >= 0 && checkZ < RowsZ)
                {
                    if (NodeArray[checkX, checkZ] != null)
                        neighbours.Add(NodeArray[checkX, checkZ]);
                }
            }
        }
        return neighbours;
    }

    public void GenerateGrid()
    {
        if (NodePrefab == null) { Debug.LogWarning("You forgot the NodePrefab!!"); return; }
        if (_gridArray != null) DestroyGridArray();
        Debug.Log("Generating grid");
        //if (NodeGrid != null) { DestroyGrid(); }

        _gridArray = new GameObject[CollumsX, RowsZ];
        _generateFromPosition = transform.position; //new Vector3(transform.position.x + 1f / 2f, 0, transform.position.z + 1f / 2f);
        //-Vector3.right * Rows/2 - Vector3.forward*Collums/2
        for (int x = 0; x < CollumsX; x++)
        {
            for (int z = 0; z < RowsZ; z++)
            {
                Node node = Instantiate(NodePrefab, new Vector3(_generateFromPosition.x + UnitScale * x, _generateFromPosition.y, _generateFromPosition.z + UnitScale * z), NodePrefab.transform.rotation, transform);
                node.GridCoordinate = new Vector2Int(x, z);
                node.gameObject.name = node.gameObject.name + " " + node.GridCoordinate.ToString();
                _gridArray[x, z] = node.gameObject;
                //* Debug.Log(node.GridCoordinate.ToString(),node.gameObject);
            }
        }
    }

    private void ChangeColor(Node node, Color color, bool isHiglighting)
    {
        if (isHiglighting)
        {
            node.oldColor = node.NodeSpriteRenderer.color;
            node.NodeSpriteRenderer.color = color;
            return;
        }

        node.NodeSpriteRenderer.color = node.oldColor;
    }

    public void DestroyGridArray()
    {
        if (_gridArray == null) { Debug.LogWarning("don't press this button if there is no grid >:("); return; }
        if (_lineList != null) { DestroyLines(); }

        Debug.Log("there is no grid in ba sing se (destroying grid)");
        foreach (GameObject obj in _gridArray)
        {
            DestroyImmediate(obj);
        }
        _gridArray = null;
        NodeArray = null;
    }

    public void DestroyNodeArray()
    {
        if (NodeArray == null) { Debug.LogWarning("don't press this button if there is no grid >:("); return; }
        if (_lineList != null) { DestroyLines(); }

        Debug.Log("there is no grid in ba sing se (destroying grid)");
        foreach (Node node in NodeArray)
        {
            if (node == null)
                continue;

            DestroyImmediate(node.gameObject);
        }
        _gridArray = null;
        NodeArray = null;
    }

    public void GenerateLines()
    {
        if (LinePrefab == null) { Debug.LogError("Hai dimenticato il LinePrefab"); return; }
        if (_gridArray == null) { Debug.LogWarning("Generate the grid first"); return; }
        if (_lineList != null) { DestroyLines(); }

        if (NodeArray == null)
            PathInizalization();

        Debug.Log("generating Lines");
        List<Node> usedNodes = new List<Node>();
        _lineList = new();
        foreach (GameObject obj in _gridArray)
        {
            if (obj == null)
                continue;

            Vector3 nodePos = obj.transform.position;
            Node node = obj.GetComponent<Node>();
            List<Node> neighbours = GetNeighboursNodes(node);
            node.Connections = new Line[neighbours.Count];
            usedNodes.Add(node);

            for (int i = 0; i < neighbours.Count; i++)
            {
                Node neighbour = neighbours[i];
                Line line;

                if (usedNodes.Contains(neighbour))
                {
                    foreach (Line neighLines in neighbour.Connections)
                    {
                        if (neighLines.EndNode == node)
                        {
                            line = neighLines.AddComponent<Line>();
                            line.StartNode = node;
                            line.EndNode = neighbour;
                            node.Connections[i] = line;
                        }
                    }
                    continue;
                }

                line = Instantiate(LinePrefab, nodePos, LinePrefab.transform.rotation, obj.transform);
                line.EndNode = neighbour;
                node.Connections[i] = line;
                LineRenderer lineRend = line.GetComponent<LineRenderer>();
                lineRend.positionCount = 2;
                lineRend.SetPosition(0, node.transform.position);
                lineRend.SetPosition(1, neighbour.transform.position);
                lineRend.enabled = true;
                _lineList.Add(lineRend.gameObject);
            }
        }
    }

    public void DestroyLines()
    {
        if (_lineList == null) { Debug.LogWarning("Maybe you should make the lines before deleting them"); return; }

        Debug.Log("there is no lines in ba sing se");
        foreach (GameObject line in _lineList)
        {
            DestroyImmediate(line);
        }
        _lineList = null;
    }

    public Node NodeFromWorldPos(Vector3 givenVector3)
    {
        foreach (Node node in NodeArray)
        {
            if (node == null) continue;

            if (Vector3.Distance(node.transform.position, givenVector3) <= UnitScale / 2)
            {
                return node;
            }
        }

        return null;
    }

    public Node GetNodeFromCoordinate(Vector2Int givenCoordinate)
    {
        if (givenCoordinate.x >= CollumsX || givenCoordinate.y >= RowsZ || givenCoordinate.y < 0 || givenCoordinate.x < 0)
            return null;

        if (NodeArray[givenCoordinate.x, givenCoordinate.y])
            return NodeArray[givenCoordinate.x, givenCoordinate.y];

        Debug.LogError("No node found at " + givenCoordinate.ToString());
        return null;
    }
}