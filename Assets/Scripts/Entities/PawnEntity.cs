﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PawnEntity : BaseEntity
{
    public PawnEntity(Node startNode, Vector2Int dir, Vector2Int gridSize, Action onDeath, Transform entityTransform, float rayDistance) : base(startNode, dir, gridSize, onDeath, entityTransform, rayDistance)
    {
    }

    public override bool TakeTurn()
    {
        return base.TakeTurn();
    }



    public override bool Move()
    {
        return base.Move();
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
        int neighbourX = neighbour.GridCoordinate.x;
        int neighbourY = neighbour.GridCoordinate.y;
        int currentX = currentNode.GridCoordinate.x;
        int currentY = currentNode.GridCoordinate.y;

        if (neighbourX + neighbourY == currentX + currentY || Mathf.Abs(neighbourX - neighbourY) == Mathf.Abs(currentX - currentY)) // so basicly if they are oblique
            return true;
        return false;
    }

    public override List<Node> NpcPath()
    {
        List<Node> newPath = new List<Node>();

        Vector2Int newCoordinates = _currentNode.GridCoordinate + _dir;


        if ((newCoordinates.x < 0 || newCoordinates.x >= _gridSize.x) || (newCoordinates.y < 0 || newCoordinates.y >= _gridSize.y))
        {
            _dir *= -1;
            return NpcPath();
        }

        Node targetNode = Path.NodeArray[newCoordinates.x, newCoordinates.y];


        if (WrongMoveCheck(targetNode, _currentNode))
        {
            newPath = FindPath(targetNode);
            return newPath;
        }

        if (!HasConnection(_currentNode, targetNode, _dir))
        {
            _dir *= -1;
            return NpcPath();
        }


        newPath.Add(targetNode);

        return newPath;
    }

    protected override Vector2Int DirectionToNode(Node nodeA, Node nodeB)
    {
        return base.DirectionToNode(nodeA, nodeB);
    }
    protected override bool HasConnection(Node currentNode, Node endNode, Vector2Int direction)
    {
        return base.HasConnection(currentNode, endNode, direction);
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
