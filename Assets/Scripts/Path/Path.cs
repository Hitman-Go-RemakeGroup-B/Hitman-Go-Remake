using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Path : MonoBehaviour
{


    [Header("Setting")]
    public int RowsZ;
    public int CollumsX;

    [Tooltip("the space between each node")]
    public float UnitScale = 1;

    [Header("Prefabs")]
    public Node NodePrefab;
    public Line LinePrefab;


    [Header("Data")]
    [SerializeField] private PathDataSo _pathData;
    private List<GameObject> _lineList { get => _pathData.LineList; set => _pathData.LineList = value; }
    private GameObject[,] _gridArray { get => _pathData.GridArray; set => _pathData.GridArray = value; }
    public Node[,] NodeArray { get => _pathData.NodeArray; set => _pathData.NodeArray = value; }
    private Vector3 _generateFromPosition { get => _pathData.GenerateFromPosition; set => _pathData.GenerateFromPosition = value; }



    // public List<Line> Lines; viktor nation what are we feeling? jaybe!? jaybe not?

    private void Awake()
    {
        if (NodeArray == null)
            NodeArrayInizalization();
    }



    private void NodeArrayInizalization()
    {
        NodeArray = null;
        NodeArray = new Node[CollumsX, RowsZ];

        for (int i = 0; i < transform.childCount; i++)
        {
            Node child = transform.GetChild(i).GetComponent<Node>();
            NodeArray[child.GridCoordinate.x, child.GridCoordinate.y] = child;
        }

    }
    public List<Node> FindPath(Vector2Int startCoordinate, Vector2Int endCoordinate)
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

                Node neighbour = Connection.endNode;
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

    List<Node> RetracePath(Node startNode, Node endNode)
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
            NodeArrayInizalization();

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
            NodeArrayInizalization();

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
                        if (neighLines.endNode == node)
                        {
                            line = neighLines.AddComponent<Line>();
                            line.endNode = neighbour;
                            node.Connections[i] = line;
                        }
                    }
                    continue;
                }

                line = Instantiate(LinePrefab, nodePos, LinePrefab.transform.rotation, obj.transform);
                line.endNode = neighbour;
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

    //bool TestDirection(int x, int z, int step, int direction)
    //{
    //    switch (direction)
    //    {
    //        case 1:
    //            return z + 1 < Rows && GridArray[x, z + 1]?.GetComponent<Node>().Visited == step;

    //        case 2:
    //            return x + 1 < Collums && GridArray[x + 1, z]?.GetComponent<Node>().Visited == step;
    //        case 3:
    //            return z - 1 > -1 && GridArray[x, z - 1]?.GetComponent<Node>().Visited == step;
    //        case 4:
    //            return x - 1 > -1 && GridArray[x - 1, z]?.GetComponent<Node>().Visited == step;
    //    }

    //    return false;
    //}

    public Node NodeFromWorldPos(Vector3 givenVector3)
    {
        foreach (Node node in NodeArray)
        {
            if (node == null) continue;

            if (Vector3.Distance(node.transform.position, givenVector3) <= UnitScale / 2)
            {
                Debug.Log(node.name, node.gameObject);
                return node;
            }
        }

        return null;

    }
    public Node GetNodeFromCoordinate(Vector2Int givenCoordinate)
    {
        if (NodeArray[givenCoordinate.x, givenCoordinate.y])
            return NodeArray[givenCoordinate.x, givenCoordinate.y];

        Debug.LogError("No node found at " + givenCoordinate.ToString());
        return null;
    }



}

// public void GenerateLines()
// {
//      if (_gridArray == null) { Debug.LogWarning("Generate the grid first"); return; }
//      if(_lines != null)  { DestroyLines(); return;}
//      
//      foreach(Node node in _gridArray )
//      {
//          for (int x = -1; x <= 1; x++)
//{
//    for (int z = -1; z <= 1; z++)
//    {
//        if (x == 0 && z == 0) // this is pawn 
//            continue;
//        int checkX = node.GridCoordinate.x + x;
//        int checkZ = node.GridCoordinate.y + z;

//        if (checkX >= 0 && checkX < CollumsX && checkZ >= 0 && checkZ > RowsZ)
//        {
//            neighbours.Add(node);
//        }

//    }
//}
//      }
// }
