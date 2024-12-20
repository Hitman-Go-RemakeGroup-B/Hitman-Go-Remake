using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntity
{
    protected Node _currentNode;
    protected List<Node> _currentPath;
    protected Color _hilightColor;
    protected Action _onDeath;
    protected float _timer;
    protected float _moveDuration;
    protected Transform _entityTransform;
    protected Vector2Int _dir;
    protected Vector2Int _gridSize;
    protected virtual List<Node> FindPath(Node endNode)
    {
        return new List<Node>();
    }


    /// <param name="startNode"></param>
    /// <param name="color">color used to higlight nodes</param>
    /// <param name="onDeath">what happends when this entity dies</param>
    public BaseEntity(Node startNode, Color color, Action onDeath, float moveDuration, Transform entityTransform)
    {
        _currentNode = startNode;
        _currentPath = new List<Node>();
        _hilightColor = color;
        _onDeath += onDeath;
        _timer = 0;
        _moveDuration = moveDuration;
        _entityTransform = entityTransform;
    }

    public virtual void Die()
    {
        _onDeath?.Invoke();
    }

    public virtual Node NpcPath()
    {
        // basicly find Connections 
        // for the knight = L shape 2x2
        // for the bishop = X shape 2x2
        // for the Rook = + shape 2x2
        // for the Pawn = + shape 1x1
        return null;
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

    protected List<Node> RetracePath(Node startNode, Node endNode)
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

    public virtual void ChangeDestination(Node endNode)
    {
        _currentPath = FindPath(endNode);
    }

    public virtual void HiglightNextMove(Node node)
    {
        
    }

    protected virtual bool MovementCheck(Node neighbour)
    {
        return true;
    }

    protected virtual bool RayCheck()
    {
        return false;
    }

    public virtual bool Move()
    {
        // the movement of the pice
        if (_currentNode != null)
            return true;

        return false;
    }
}

public class PawnEntity : BaseEntity
{
    public PawnEntity(Node startNode, Color color, Action onDeath, float moveDuration, Transform entityTransform) : base(startNode, color, onDeath, moveDuration, entityTransform)
    { }

    protected override List<Node> FindPath(Node endNode)
    {
        Node startNode = _currentNode;


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

                Node neighbour = Connection.endNode;
                if (closedSet.Contains(neighbour))
                    continue;

                if (MovementCheck(neighbour))
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

    public override Node NpcPath()
    {
        Node newNode = null;
        int newNodeX = _currentNode.GridCoordinate.x + _dir.x;
        int newNodeY = _currentNode.GridCoordinate.y + _dir.y;

        if (newNodeX < _gridSize.x && newNodeY < _gridSize.y)
        {
            newNode = Path.NodeArray[newNodeX, newNodeY];
        }
        else
        {
            _dir *= -1;
            return NpcPath();
        }
        return newNode;
    }

    public override void HiglightNextMove(Node node)
    {
        
        node.Higlight = _hilightColor;

    }
    protected override bool MovementCheck(Node neighbour)
    {
        int neighbourX = neighbour.GridCoordinate.x;
        int neighbourY = neighbour.GridCoordinate.y;
        int currentX = _currentNode.GridCoordinate.x;
        int currentY = _currentNode.GridCoordinate.y;
        if (neighbourX + neighbourY == currentX + currentY || Mathf.Abs(neighbourX - neighbourY) == Mathf.Abs(currentX - currentY)) // so basicly if they are oblique
            return true;
        return false;
    }

    public override bool Move()
    {
        
        if (_timer >= _moveDuration)
        {
            _entityTransform.position = Vector3.Lerp(_currentNode.transform.position, _currentPath[0].transform.position, _timer / _moveDuration);
            _timer += Time.deltaTime;
            return false;
        }
        else 
        {
            _entityTransform.position = _currentPath[0].transform.position;
            _currentNode = _currentPath[0];
            _currentPath.RemoveAt(0);
            if(_currentPath.Count <= 0 )
                // reset normal movement
            _timer = 0;
            return true;
        }
    }

    public override void Die()
    {
        base.Die();
    }
}

public class RookEntity : BaseEntity
{
    public RookEntity(Node startNode, Color color, Action onDeath, float moveDuration, Transform entityTransform) : base(startNode, color, onDeath, moveDuration, entityTransform)
    {
    }
}

public class BishopEntity : BaseEntity
{
    public BishopEntity(Node startNode, Color color, Action onDeath, float moveDuration, Transform entityTransform) : base(startNode, color, onDeath, moveDuration, entityTransform)
    {
    }
}

public class knightEntity : BaseEntity
{
    public knightEntity(Node startNode, Color color, Action onDeath, float moveDuration, Transform entityTransform) : base(startNode, color, onDeath,moveDuration, entityTransform)
    {
    }
}
