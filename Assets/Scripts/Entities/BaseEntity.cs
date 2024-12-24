using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntity
{
    // given
    protected Node _currentNode;
    protected Vector2Int _dir;
    protected Vector2Int _gridSize;
    protected Action _onDeath;

    protected Transform _entityTransform;

    // calculated
    protected List<Node> _path;
    protected float _timer;
    protected float _moveDuration;
    protected bool _isDead;
  
    //protected SpriteRenderer _sprite;
    //protected Node _spriteNode;
 

    /// <param name="startNode"></param>
    /// <param name="color">color used to higlight nodes</param>
    /// <param name="onDeath">what happends when this entity dies</param>
    public BaseEntity(Node startNode, Vector2Int dir, Vector2Int gridSize, Action onDeath, Transform entityTransform)
    {
        _currentNode = startNode;
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
        if (_isDead) return true;

        if (_path.Count <= 0)
        {
            _path = NpcPath();
        }

        return !Move();
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

    protected virtual Node FindNextMove(Vector2Int direction, Node nextNode)
    {
        Node node = null;
        return node;
    }

    public virtual bool Move()
    {
       


        if (_timer < _moveDuration)
        {
            _timer += Time.deltaTime;
            _entityTransform.position = Vector3.Lerp(_currentNode.transform.position, _path[0].transform.position, _timer / _moveDuration);
            return false;
        }
        else
        {
            _entityTransform.position = _path[0].transform.position;
            _currentNode = _path[0];
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

            if (DirectionToNode(currentNode, connection.endNode) == direction)
                return true;
        }
        return false;
    }

    protected virtual Vector2Int DirectionToNode(Node nodeA, Node nodeB)
    {
        return nodeB.GridCoordinate - nodeA.GridCoordinate;
    }

    protected virtual bool WrongMoveCheck(Node neighbour, Node currentNode)
    {
        return false;
    }

}
