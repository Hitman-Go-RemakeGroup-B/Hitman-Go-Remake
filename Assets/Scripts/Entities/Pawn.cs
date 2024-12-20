using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : BaseEntityV2
{
    public Pawn(Node startNode, Vector2Int dir, Vector2Int gridSize, Color color, Action onDeath, Transform entityTransform) : base(startNode, dir, gridSize, color, onDeath, entityTransform)
    {
    }

    public override bool Turn()
    {
        return base.Turn();
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

    protected override bool WrongMoveCheck(Node neighbour)
    {
        int neighbourX = neighbour.GridCoordinate.x;
        int neighbourY = neighbour.GridCoordinate.y;
        int currentX = _currentNode.GridCoordinate.x;
        int currentY = _currentNode.GridCoordinate.y;
        if (neighbourX + neighbourY == currentX + currentY || Mathf.Abs(neighbourX - neighbourY) == Mathf.Abs(currentX - currentY)) // so basicly if they are oblique
            return true;
        return false;
    }

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

                if (WrongMoveCheck(neighbour))
                    continue;

                int nodeDistance = GetNodeDistance( neighbour, currentNode);
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
