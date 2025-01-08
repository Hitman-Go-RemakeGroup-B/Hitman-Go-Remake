using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntity
{
    // given
    public Node CurrentNode;
    protected Vector2Int _dir;
    protected Vector2Int _gridSize;
    protected Action _onDeath;
    protected Vector2Int[] _directions;
    protected Transform _entityTransform;

    // calculated
    protected List<Node> _path;
    protected float _timer;
    protected float _moveDuration;
    protected bool _isDead;
    protected int _index;
    protected int _checkedDirectionsNum;
    protected Vector2Int _dirToPreviousNode;
    protected Node _endNode;

    //protected SpriteRenderer _sprite;
    //protected Node _spriteNode;


    public BaseEntity(Node startNode, Vector2Int dir, Vector2Int gridSize, Action onDeath, Transform entityTransform)
    {
        CurrentNode = startNode;
        _dir = dir;
        _gridSize = gridSize;

        _onDeath += onDeath;
        _entityTransform = entityTransform;

        _moveDuration = TurnsManager.MoveDuration;
        _timer = 0;
        _path = new List<Node>();

    }


    public virtual void OnAllert(Node node)
    {
        _path = FindPath(node);
    }

    // while Turn()
    public virtual bool TakeTurn()
    {
       

        if (_path.Count <= 0)
        {
            _path = NpcPath();
        }

        return !Move();
    }


    public virtual bool RayCheck()
    {
        _endNode = findNodesInLine(CurrentNode);

        if (_endNode == null)
        {
            _checkedDirectionsNum++;
            ChangeDir();
            return RayCheck();
        }
        _checkedDirectionsNum = 0;

        Vector3 direction = _endNode.transform.position - _entityTransform.position;
        direction.y = 0;
        float distance = direction.magnitude;
        direction = direction.normalized;
        Ray ray = new(_entityTransform.position, direction);
        Debug.DrawRay(_entityTransform.position, direction*distance, Color.red, 999f);
        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.transform.TryGetComponent<PlayerController>(out var player))
            {
                player.Death();
                _path.Clear();
                _path.Add(player.BoardPice.CurrentNode);
                return true;
            }
        }

        return false;
    }



    virtual protected void ChangeDir()
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


    /// <summary>
    /// finds the next node to move twoards
    /// based on rules
    /// </summary>
    /// <returns></returns>
    public virtual List<Node> NpcPath()
    {
        List<Node> newPath = new List<Node>();

        // implementata nei figli 

        return newPath;
    }

    //protected virtual Node FindNextMove(Vector2Int direction, Node nextNode)
    //{
    //    Node node = null;
    //    return node;
    //}

    public virtual bool Move()
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
            _path.RemoveAt(0);
            _timer = 0;


            return true;
        }
    }

    protected virtual List<Node> FindPath(Node endNode)
    {
        return new List<Node>();
    }


    public virtual int GetNodeDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridCoordinate.x - nodeB.GridCoordinate.x);
        int distZ = Mathf.Abs(nodeA.GridCoordinate.y - nodeB.GridCoordinate.y);

        if (distX > distZ)
            return 14 * distZ + 10 * (distX - distZ);
        else
            return 14 * distX + 10 * (distZ - distX);
    }

    protected virtual List<Node> RetracePath(Node startNode, Node endNode)
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

    protected virtual bool HasConnection(Node currentNode, Node endNode, Vector2Int direction)
    {
        foreach (Line connection in currentNode.Connections)
        {
            if (connection == null)
                continue;

            if (DirectionToNode(currentNode, connection.EndNode) == direction)
                return true;
        }
        return false;
    }

    protected virtual Vector2Int DirectionToNode(Node currentNode, Node targetNode)
    {
        return targetNode.GridCoordinate - currentNode.GridCoordinate;
    }

    protected virtual bool WrongMoveCheck(Node neighbour, Node currentNode)
    {
        return false;
    }


    public virtual Node findNodesInLine(Node node)
    {
        Node nextNode = null;

        foreach (Line connection in node.Connections)
        {
            if (connection == null) continue;

            if (DirectionToNode(node, connection.EndNode) == _dir)
            {
                connection.EndNode.PreviousNode = node;
                nextNode = findNodesInLine(connection.EndNode);
                //Debug.Log(nextNode);
                if (nextNode != null)
                    return nextNode;
                else
                    return connection.EndNode;
            }
        }
        return null;
    }
}
