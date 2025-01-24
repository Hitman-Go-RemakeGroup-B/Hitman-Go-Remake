using System;
using System.Collections;
using System.Collections.Generic;
using BT;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // serialized:
    public DirectionsList PossibleDirections;
    public Vector2Int Dir;
    public float TimeToReachNextNode;
    public bool IsImmobile;
    public Color HilightColor;

    // hidden:
    [HideInInspector] public bool IsDead = false;
    [HideInInspector] public bool IsDistracted = false;
    [HideInInspector] public bool IsKilling = false;
    [HideInInspector] public bool IsFirstTurn = false;
    [HideInInspector] public Path Path;
    [HideInInspector] public Node CurrentNode;
    [HideInInspector] public float Timer;
    [HideInInspector] public Vector2Int GridSize;
    [HideInInspector] public BaseEntity BoardPice;
    [HideInInspector] public List<Node> PossibleNodes;
    [HideInInspector] public List<Node> PossibleNodeDirections;
    [HideInInspector] public List<Node> FoundPath;
    [HideInInspector] public int Index;
    [HideInInspector] public int DirIndex;
    [HideInInspector] public Vector3 StartPos;
    [HideInInspector] public Node EndNode;
    [HideInInspector] public Vector2Int OldDir;
    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected BT_Sequence _behaviourTree;
    public Action OnTurnEnd;
    public Action<MeshFilter, EntityType, MeshRenderer> OnChangeBoardPiece;

    // action:
    public delegate Node GetNode(Vector2Int coordinates);
    public delegate List<Node> PathFinding(Vector2Int startNode, Vector2Int endNode, WrongMove wrongMove);

    public GetNode NodeFromCoordinates;
    public PathFinding FindPath;

    private void Awake()
    {
        StartPos = transform.position;
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public virtual void PiceCange(BaseEntity newPice,EntityType type)
    {
        
    }

    protected virtual void TurnSetUp()
    {
        // AI: DONE | PlAYER: DONE
        _behaviourTree = null;
    }

    /// <summary>
    /// check a given boolean to see if it's true or false
    /// </summary>
    /// <returns>
    /// success if true failure if false
    /// </returns>
    protected BT_Node.Status BoolCheck(bool bolean)
    {
        if (bolean)
            return BT_Node.Status.Success;

        return BT_Node.Status.Failure;
    }

    protected BT_Node.Status AmIDistracted()
    {
        return BoolCheck(IsDistracted);
    }


    public virtual void StartTurn()
    {
        if (_behaviourTree == null)
            return;

        StopAllCoroutines();
        StartCoroutine(TakeTurn());
    }

    protected virtual IEnumerator TakeTurn()
    {
        while (_behaviourTree.Process() == BT_Node.Status.Running)
            yield return null;

        _behaviourTree.Reset();
        OnTurnEnd?.Invoke();
    }

    public virtual void Death()
    {

    }

    protected BT_Node.Status HighlightNodes(List<Node> nodesToHiglight)
    {
        foreach (Node node in nodesToHiglight)
        {
            node.OnColorChange?.Invoke(node, HilightColor, true);
        }
        return BT_Node.Status.Success;
    }

    protected BT_Node.Status DeselectNodes(List<Node> nodesToHiglight)
    {
        foreach (Node node in nodesToHiglight)
        {
            node.OnColorChange?.Invoke(node, HilightColor, false);
        }
        return BT_Node.Status.Success;
    }
}

[System.Serializable]
public struct Vector2IntList
{
    public Vector2Int[] vector2Ints;
}
[System.Serializable]
public struct DirectionsList
{
    public List<Vector2IntList> list;
}