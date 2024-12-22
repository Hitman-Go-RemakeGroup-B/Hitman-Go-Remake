using System;
using UnityEngine;

public class BishopEntity : BaseEntity
{
    public BishopEntity(Node startNode, Vector2Int dir, Vector2Int gridSize, Action onDeath, Transform entityTransform) : base(startNode, dir, gridSize, onDeath, entityTransform)
    {
    }
}
