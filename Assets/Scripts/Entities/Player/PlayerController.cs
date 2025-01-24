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

    int index;
    bool isDirectionsCheckeds;

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
        BT_Sequence foundNodesSequence = new("");
        BT_Sequence noFoundNodesSequence = new("");
        BT_Selector checkFoundNodesSelector = new("");
        BT_Repeater foundDirectionRepeater = new("");

        IProcess findDirectionsProcess = new BaseProcess(new CheckDecorator(this, BoardPice.FindDirection));
        IProcess findAllNodesProcess = new BaseProcess(new CheckDecorator(this, FindAllNodes));
        IProcess doneDirectionCheckProcess = new BaseProcess(new CheckDecorator(this, DoneDirectionsCheck));
        IProcess highlightDirectionsProcess = new BaseProcess(new HighLightDecorator(this, HighlightNodes, PossibleNodeDirections));
        IProcess chooseDirectionsProcess = new BaseProcess(new CheckDecorator(this, BoardPice.ChooseDirection));
        IProcess deselectDirectionsProcess = new BaseProcess(new HighLightDecorator(this, DeselectNodes, PossibleNodeDirections));
        IProcess findNodesProcess = new BaseProcess(new FindNodeDecorator(this, BoardPice.FindPossibleNodes));
        IProcess highlightNodesProcess = new BaseProcess(new HighLightDecorator(this, HighlightNodes, PossibleNodes));
        IProcess chooseNodesProcess = new BaseProcess(new CheckDecorator(this, BoardPice.ChooseEndNode));
        IProcess deselectNodesProcess = new BaseProcess(new HighLightDecorator(this, DeselectNodes, PossibleNodes));
        IProcess moveToEndNodeProcess = new BaseProcess(new CheckDecorator(this, BoardPice.MoveTwoardsEndNode));
        IProcess checkInteractablesProcess = new BaseProcess(new CheckDecorator(this, CheckIInteractable));
        IProcess checkWinNodeProcess = new BaseProcess(new CheckDecorator(this, CheckWinNode));
        IProcess checkFoundNodesProcess = new BaseProcess(new CheckDecorator(this, CheckedDirections));

        BT_Leaf findDirections = new("finds the direction to go twoards", findDirectionsProcess);
        BT_Leaf findAllNodes = new("", findAllNodesProcess);
        BT_Leaf doneDirectionCheck = new("", doneDirectionCheckProcess);
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
        BT_Leaf checkFoundNodes = new("", checkFoundNodesProcess);

        _behaviourTree.AddChild(findDirections);
        _behaviourTree.AddChild(foundDirectionRepeater);
        _behaviourTree.AddChild(checkFoundNodesSelector);

        checkFoundNodesSelector.AddChild(foundNodesSequence);
        checkFoundNodesSelector.AddChild(noFoundNodesSequence);


        foundDirectionRepeater.AddChild(findAllNodes);
        foundDirectionRepeater.AddChild(doneDirectionCheck);

        foundNodesSequence.AddChild(checkFoundNodes);
        foundNodesSequence.AddChild(highlightNodes);
        foundNodesSequence.AddChild(chooseNodes);
        foundNodesSequence.AddChild(deselectNodes);
        foundNodesSequence.AddChild(moveToEndNode);
        foundNodesSequence.AddChild(checkInteractables);
        foundNodesSequence.AddChild(checkWinNode);

        noFoundNodesSequence.AddChild(findNodes);
        noFoundNodesSequence.AddChild(highlightNodes);
        noFoundNodesSequence.AddChild(chooseNodes);
        noFoundNodesSequence.AddChild(deselectNodes);
        noFoundNodesSequence.AddChild(moveToEndNode);
        noFoundNodesSequence.AddChild(checkInteractables);
        noFoundNodesSequence.AddChild(checkWinNode);

        OnTurnSetupDone?.Invoke();
    }

    private BT_Node.Status CheckedDirections()
    {
        return BoolCheck(isDirectionsCheckeds);
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
        }
        return BT_Node.Status.Success;
    }

    BT_Node.Status FindAllNodes()
    {
        isDirectionsCheckeds = false;
        if (PossibleNodeDirections.Count <= 0)
            return BT_Node.Status.Success;

        Vector2Int direction = PossibleNodeDirections[index].GridCoordinate - CurrentNode.GridCoordinate;
        Dir = direction;
        BT_Node.Status status = BoardPice.FindPossibleNodes(PossibleNodeDirections[index], direction);

        if (status == BT_Node.Status.Success)
            status = BT_Node.Status.Failure;

        return status;
    }

    BT_Node.Status DoneDirectionsCheck()
    {

        index++;
        if (index > PossibleNodeDirections.Count - 1)
        {
            index = 0;
            isDirectionsCheckeds = true;
            return BT_Node.Status.Success;
        }
        
        return BT_Node.Status.Failure;
    }
}
