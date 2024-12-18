using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEntity
{
    protected Node _currentNode;
    protected List<Node> _currentPath;
    protected Color _hilightColor;
    protected Action _onDeath;
    protected virtual List<Node> FindPath(Node endNode)
    {
        return new List<Node>();
    }


    /// <param name="startNode"></param>
    /// <param name="color">color used to higlight nodes</param>
    /// <param name="onDeath">what happends when this entity dies</param>
    public BaseEntity(Node startNode, Color color, Action onDeath)
    {
        _currentNode = startNode;
        _currentPath = new List<Node>();
        _hilightColor = color;
        _onDeath += onDeath;
    }

    public virtual void Die()
    {
        _onDeath?.Invoke();
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

    public virtual void HiglightNextMove()
    {
        // basicly find Connections 
        // for the knight = L shape 2x2
        // for the bishop = X shape 2x2
        // for the Rook = + shape 2x2
        // for the Pawn = + shape 1x1
    }

    protected virtual bool MovementCheck(Node neighbour)
    {
        return true;
    }

    protected virtual bool RayCheck()
    {
        return false;
    }
}

public class PawnEntity : BaseEntity
{
    public PawnEntity(Node startNode, Color color, Action onDeath) : base(startNode, color, onDeath)
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

    public override void HiglightNextMove()
    {
        foreach (Line connection in _currentNode.Connections)
        {
            if (connection == null)
                continue;

            Node neighbour = connection.endNode;

            if (MovementCheck(neighbour))
                continue;
            connection.endNode.Higlight = _hilightColor;

        }
    }
    protected override bool MovementCheck(Node neighbour)
    {
        int nX = neighbour.GridCoordinate.x;
        int nY = neighbour.GridCoordinate.y;
        int cX = _currentNode.GridCoordinate.x;
        int cY = _currentNode.GridCoordinate.y;
        if (nX + nY == cX + cY || Mathf.Abs(nX - nY) == Mathf.Abs(cX - cY))
            return true;
        return false;
    }

    public override void Die()
    {
        base.Die();
    }
}

public class RookEntity : BaseEntity
{
    public RookEntity(Node startNode, Color color, Action onDeath) : base(startNode, color, onDeath)
    {
    }
}

public class BishopEntity : BaseEntity
{
    public BishopEntity(Node startNode, Color color, Action onDeath) : base(startNode, color, onDeath)
    {
    }
}

public class knightEntity : BaseEntity
{
    public knightEntity(Node startNode, Color color, Action onDeath) : base(startNode, color, onDeath)
    {
    }
}
