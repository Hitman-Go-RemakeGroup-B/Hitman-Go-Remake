using System;
using System.Collections.Generic;
using UnityEngine;

public class BishopEntity : BaseEntity
{
    public BishopEntity(Node startNode, Vector2Int dir, Vector2Int gridSize, Action onDeath, Transform entityTransform) : base(startNode, dir, gridSize, onDeath, entityTransform)
    {
        CurrentNode = startNode;
        _dir = dir;
        _gridSize = gridSize;

        _onDeath += onDeath;
        _entityTransform = entityTransform;

        _checkedDirectionsNum = 0;
        _moveDuration = TurnsManager.MoveDuration;
        _timer = 0;
        _path = new List<Node>();


        _directions = new Vector2Int[4] { new(1, 1), new(-1, -1), new(-1, 1), new(1, -1) };

        for (int i = 0; i < _directions.Length; i++)
        {
            if (_directions[i] == _dir)
            {
                _index = i;
                _dirToPreviousNode = _directions[i] * -1;
            }
        }
        //_path = NpcPath();
    }
    public override bool RayCheck()
    {
        return base.RayCheck();
    }
    public override bool TakeTurn()
    {
        if (_isDead) return true;

        if (_path.Count <= 0)
        {
            _path = NpcPath();
        }

        return !Move();// this will go in a while loop that's why i use the !
    }

    public override List<Node> NpcPath()
    {
        List<Node> newPath = new List<Node>();

        if (_endNode == null)
        {
            _checkedDirectionsNum++;
            ChangeDir();

            //Debug.LogError(_dir);
            return NpcPath();
        }
        _dirToPreviousNode = -_dir;
        _checkedDirectionsNum = 0;
        newPath.Add(_endNode);
        return newPath;
        //RetracePath(_currentNode, _endNode); this would make it go 1 node at a time



    }

    override public Node FindNextNodes(Node node)
    {
        Node nextNode = null;

        foreach (Line connection in node.Connections)
        {
            if (connection == null) continue;

            if (DirectionToNode(node, connection.EndNode) == _dir)
            {
                connection.EndNode.PreviousNode = node;
                nextNode = FindNextNodes(connection.EndNode);
                //Debug.Log(nextNode);
                if (nextNode != null)
                    return nextNode;
                else
                    return connection.EndNode;
            }
        }
        return null;
    }

    public override bool Move()
    {
        if (_timer < _moveDuration)
        {
            _timer += Time.deltaTime;
            _entityTransform.position = Vector3.Lerp(CurrentNode.transform.position, _path[0].transform.position, _timer / _moveDuration);
            return false;
        }
        else
        {
            _entityTransform.position = _path[0].transform.position;
            CurrentNode = _path[0];

            if (_endNode == _path[0])
            {
                ChangeDir();
                //Debug.LogError(_dir);
            }

            _path.RemoveAt(0);
            _timer = 0;

            return true;
        }
    }

    public override void OnAllert(Node node)
    {
        base.OnAllert(node);
    }

    protected override List<Node> RetracePath(Node startNode, Node endNode)
    {
        return base.RetracePath(startNode, endNode);
    }

    public override int GetNodeDistance(Node nodeA, Node nodeB)
    {
        return base.GetNodeDistance(nodeA, nodeB);
    }

    protected override bool WrongMoveCheck(Node neighbour, Node currentNode)
    {
        int neighbourX = Mathf.Abs(neighbour.GridCoordinate.x);
        int neighbourY = Mathf.Abs(neighbour.GridCoordinate.y);
        int currentX = Mathf.Abs(currentNode.GridCoordinate.x);
        int currentY = Mathf.Abs(currentNode.GridCoordinate.y);

        if (neighbourX + neighbourY > 1) // so basicly if they are in cross format +
            return true;
        return false;
    }

    override protected void ChangeDir()
    {
        // so when 

        _index += 1;
        if (_index >= _directions.Length)
        {
            _index = 0;
        }

        Vector2Int direction = _directions[_index];

        if (direction == _dirToPreviousNode)
        {
            ChangeDir();
            return;
        }

        if (_checkedDirectionsNum >= _directions.Length)
        {
            _dir = _dirToPreviousNode;

            return;
        }

        _dir = direction;
    }



    protected override Vector2Int DirectionToNode(Node currentNode, Node targetNode)
    {
        return base.DirectionToNode(currentNode, targetNode);
    }
    protected override bool HasConnection(Node currentNode, Node endNode, Vector2Int direction)
    {
        return base.HasConnection(currentNode, endNode, direction);
    }

    protected override List<Node> FindPath(Node endNode)
    {
        Node startNode = CurrentNode;

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
                return RetracePath(startNode, endNode);
            }

            foreach (Line Connection in currentNode.Connections)
            {
                if (Connection == null)
                    continue;

                Node neighbour = Connection.EndNode;
                if (closedSet.Contains(neighbour))
                    continue;

                if (WrongMoveCheck(neighbour, currentNode))
                    continue;

                int nodeDistance = GetNodeDistance(neighbour, currentNode);
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
}
