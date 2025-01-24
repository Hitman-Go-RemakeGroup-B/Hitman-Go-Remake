using System;
using System.Collections;
using System.Collections.Generic;
using BT;
using BT.Decorator;
using BT.Process;
using UnityEngine;

public class PlayerController : Controller
{

    public Action OnTurnSetupDone;
    public delegate int GetInt();
    public GetInt GetRemainginEnemies;
    public GetInt GetNumberOfTurns;

    [HideInInspector] public bool GotKing;
    [HideInInspector] public bool GotQueen;
    private void Start()
    {
        IsFirstTurn = true;
        BoardPice = new PawnEntity(this);
        OnChangeBoardPiece?.Invoke(meshFilter, EntityType.Pawn, meshRenderer);
        TurnSetUp();

    }

    protected override void TurnSetUp()
    {
        Dir = Vector2Int.zero;
        _behaviourTree = new("turn");
        IProcess findDirectionsProcess = new BaseProcess(new CheckDecorator(this, BoardPice.FindDirection));
        IProcess highlightDirectionsProcess = new BaseProcess(new HighLightDecorator(this, HighlightNodes, PossibleNodeDirections));
        IProcess chooseDirectionsProcess = new BaseProcess(new CheckDecorator(this, BoardPice.ChooseDirection));
        IProcess deselectDirectionsProcess = new BaseProcess(new HighLightDecorator(this, DeselectNodes, PossibleNodeDirections));
        IProcess findNodesProcess = new BaseProcess(new FindNodeDecorator(this, BoardPice.FindPossibleNodes));
        IProcess highlightNodesProcess = new BaseProcess(new HighLightDecorator(this, HighlightNodes, PossibleNodes));
        IProcess chooseNodesProcess = new BaseProcess(new CheckDecorator(this, BoardPice.ChooseEndNode));
        IProcess deselectNodesProcess = new BaseProcess(new HighLightDecorator(this, DeselectNodes, PossibleNodes));
        IProcess moveToEndNodeProcess = new BaseProcess(new CheckDecorator(this, BoardPice.MoveTwoardsEndNode));
        IProcess checkInteractablesProcess = new BaseProcess(new CheckDecorator(this, CheckIInteractable));
        IProcess checkWinNodeProcess = new BaseProcess (new CheckDecorator(this,CheckWinNode));

        BT_Leaf findDirections = new("finds the direction to go twoards", findDirectionsProcess);
        BT_Leaf highlightDirections = new("", highlightDirectionsProcess);
        BT_Leaf chooseDirections = new("chooses the direction to move twoards", chooseDirectionsProcess);
        BT_Leaf deselectDirections = new("", deselectDirectionsProcess);
        BT_Leaf findNodes = new("find all nodes", findNodesProcess);
        BT_Leaf highlightNodes = new("highlight found nodes", highlightNodesProcess);  
        BT_Leaf chooseNodes = new("choose found node", chooseNodesProcess);
        BT_Leaf deselectNodes = new("deselectNodes", deselectNodesProcess);           
        BT_Leaf moveToEndNode = new("move to end node", moveToEndNodeProcess);
        BT_Leaf checkInteractables = new("check if there's an interactable", checkInteractablesProcess);
        BT_Leaf checkWinNode = new("", checkWinNodeProcess);

        _behaviourTree.AddChild(findDirections);
        _behaviourTree.AddChild(highlightDirections);
        _behaviourTree.AddChild(chooseDirections);
        _behaviourTree.AddChild(deselectDirections);
        _behaviourTree.AddChild(moveToEndNode);
        _behaviourTree.AddChild(findNodes);
        _behaviourTree.AddChild(highlightNodes);
        _behaviourTree.AddChild(chooseNodes);
        _behaviourTree.AddChild(deselectNodes);
        _behaviourTree.AddChild(moveToEndNode);
        _behaviourTree.AddChild(checkInteractables);
        _behaviourTree.AddChild(checkWinNode);
        OnTurnSetupDone?.Invoke();
    }
    
    public override void PiceCange(BaseEntity newPice, EntityType type)
    {
        EndNode = null;
        BaseEntity newOne = newPice;
        StopAllCoroutines();
        newPice.controller = BoardPice.controller;
        BoardPice = newPice;
        Dir = Vector2Int.zero;
        PossibleNodes.Clear();
        PossibleNodeDirections.Clear();
        OnChangeBoardPiece(meshFilter, type, meshRenderer);
        base.DeselectNodes(PossibleNodes);
        TurnSetUp();
        OnTurnEnd?.Invoke();
    }
    
    public override void StartTurn()
    {
        base.StartTurn();
    }
    
    protected override IEnumerator TakeTurn()
    {
        return base.TakeTurn();
    }
    
    public override void Death()
    {
        base.Death();
    }


    BT_Node.Status CheckIInteractable()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, Vector3.one);
#if UNITY_EDITOR
        DebugExtensions.DrawBox(transform.position, transform.rotation, Vector3.one, Color.red, 99f);
#endif
        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
                interactable.Interact(this);
        }

        return BT_Node.Status.Success;
    }

    BT_Node.Status CheckWinNode() 
    {
        if (CurrentNode.IsWinNode)
        {
            // you win pogchamp
            WinCondition.noEnemy?.Invoke(GetRemainginEnemies.Invoke());
            WinCondition.KillEnemy?.Invoke(GetRemainginEnemies.Invoke());
            WinCondition.King?.Invoke(GotKing);
            WinCondition.Queen?.Invoke(GotQueen);
            WinCondition.MinTurns?.Invoke(GetNumberOfTurns.Invoke());
            WinCondition.Win?.Invoke();
            Debug.Log("you win pogchampion");
            return BT_Node.Status.Success;
        }
        return BT_Node.Status.Failure;
    }
}
