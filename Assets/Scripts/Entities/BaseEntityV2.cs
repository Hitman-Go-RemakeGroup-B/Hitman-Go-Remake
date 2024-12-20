using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntityV2
{
    // given
    protected Node _currentNode;
    protected Vector2Int _dir;
    protected Vector2Int _gridSize;
    protected Action _onDeath;
    protected Color _hilightColor;
    protected Transform _entityTransform;

    // calculated
    protected List<Node> _path;
    protected float _timer;
    protected float _moveDuration;
    protected bool _isDead;



    /// <param name="startNode"></param>
    /// <param name="color">color used to higlight nodes</param>
    /// <param name="onDeath">what happends when this entity dies</param>
    public BaseEntityV2(Node startNode, Vector2Int dir, Vector2Int gridSize, Color color, Action onDeath ,Transform entityTransform)
    {
        _currentNode = startNode;
        _dir = dir;
        _gridSize = gridSize;
        _hilightColor = color;
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
    public virtual bool Turn()
    {
        if(_isDead) return true;

        if (_path.Count == 0)
        {
            _path = NpcPath();
        }

        return Move();
    }

    /// <summary>
    /// finds the next node to move twoards
    /// based on rules
    /// </summary>
    /// <returns></returns>
    public List<Node> NpcPath()
    {
        List<Node> newPath = new List<Node>();

        // implementata nei figli 

        return newPath;
    }

    public bool Move()
    {
        if (_timer >= _moveDuration)
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

    protected virtual bool WrongMoveCheck(Node neighbour)
    {
        return false;
    }


}
