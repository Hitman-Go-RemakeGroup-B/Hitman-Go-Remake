using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using BT;
using BT.Decorator;
using BT.Process;
using UnityEngine;

public class AiController : Controller, IInteractable
{
    public EntityType BoardPiceType;
    [SerializeField] private bool _isPlayerAbleToTransformIntoMe;
    public Action<AiController> OnDeath;
    UIManager _UI;
    List<Node> lastPossibleNodes;

    private void Start()
    {
        _UI = FindObjectOfType<UIManager>();
        IsFirstTurn = false;
        switch (BoardPiceType)
        {
            case EntityType.Pawn:
                BoardPice = new PawnEntity(this); break;

            case EntityType.Rook:
                BoardPice = new RookEntity(this); break;

            case EntityType.Bishop:
                BoardPice = new BishopEntity(this); break;

            case EntityType.Knight:
                BoardPice = new KnightEntity(this); break;
        }

        HilightColor = Color.red;

        TurnSetUp();
    }

    protected override void TurnSetUp()
    {
        _behaviourTree = null;
        // Repeaters
        BT_Repeater findDirRepeater = new("did i find a sutabile direction");
        BT_Repeater findNextDirRepeater = new("");

        // Selectors 
        BT_Selector raycastSelector = new("did the raycast hit?");

        // Sequences
        _behaviourTree = new("TurnTree");
        BT_Sequence afterDead = new("do this after you know you died");
        BT_Sequence findPossibleNodesSequence = new("do this after finding a possible node");
        BT_Sequence raycastSequence = new("do this while you check for the raycast");
        BT_Sequence moveToSequence = new("moving and then do the rest");

        // Process

        BaseProcess amIDistractedProcess = new(new CheckDecorator(this, AmIDistracted));
        BaseProcess getNextDirProcess = new(new CheckDecorator(this, BoardPice.NextDirection));
        BaseProcess findPossibleNodesProcess = new(new FindNodeDecorator(this, BoardPice.FindPossibleNodes));
        BaseProcess findNodesWhileDistractedProcess = new(new CheckDecorator(this, BoardPice.FindEndNodeWhileDistracted));
        BaseProcess raycastProcess = new(new CheckDecorator(this, BoardPice.Raycast));
        BaseProcess moveToEndNodeProcess = new(new CheckDecorator(this, BoardPice.MoveTwoardsEndNode));
        BaseProcess killPlayerProcess = new(new CheckDecorator(this, KillPlayer));
        BaseProcess highlightNodesProcess = new(new HighLightDecorator(this, HighlightNodes, PossibleNodes));
        BaseProcess deselectNodesProcess = new(new HighLightDecorator(this, DeselectNodes, PossibleNodes));


        // Leafs

        BT_Leaf distractedCheck = new("distracted check", amIDistractedProcess);
        BT_Leaf getNextDir = new("gets the next dir", getNextDirProcess);
        BT_Leaf findPossibleNodes = new("finds all the possible nodes you can go twoards", findPossibleNodesProcess);
        BT_Leaf findNodesWhileDistracted = new("finds where to go while distracted", findNodesWhileDistractedProcess);
        BT_Leaf raycastCheck = new("checks if the raycast hit", raycastProcess);
        BT_Leaf moveToEndNode = new("moves to the endNode", moveToEndNodeProcess);
        BT_Leaf killPlayer = new("kill the player", killPlayerProcess);
        BT_Leaf highlightNodes = new("highlight found nodes", highlightNodesProcess);
        BT_Leaf deselectNodes = new("deselectNodes", deselectNodesProcess);


        // constructing BT:
        _behaviourTree.AddChild(deselectNodes);
        _behaviourTree.AddChild(findDirRepeater);
        findDirRepeater.AddChild(findPossibleNodesSequence);
        findDirRepeater.AddChild(getNextDir);

        // findPossibleNodeSequence:
        findPossibleNodesSequence.AddChild(findPossibleNodes);
        findPossibleNodesSequence.AddChild(raycastSelector);

        // raycastSelector:
        raycastSelector.AddChild(raycastSequence);
        raycastSelector.AddChild(moveToSequence);

        // raycastSequence:
        raycastSequence.AddChild(raycastCheck);
        raycastSequence.AddChild(highlightNodes);
        raycastSequence.AddChild(moveToSequence);
        raycastSequence.AddChild(deselectNodes);
        raycastSequence.AddChild(killPlayer);

        // findNextDirRepeater
        findNextDirRepeater.AddChild(findPossibleNodes);
        findNextDirRepeater.AddChild(getNextDir);


        // moveToSequence: 
        moveToSequence.AddChild(moveToEndNode);
        moveToSequence.AddChild(findNextDirRepeater);
        moveToSequence.AddChild(highlightNodes);
    }

    private BT_Node.Status KillPlayer()
    {
        // you lose sad not pogchampion 
        Debug.Log("you lose, sad. not pogchampion");
        IsKilling = false;
        _UI.RetryLevel();
        return BT_Node.Status.Success;
    }

    public override void PiceCange(BaseEntity newPice, EntityType type)
    {

    }

    public void Interact(PlayerController player)
    {
        base.DeselectNodes(lastPossibleNodes);
        IsDead = true;
        OnDeath?.Invoke(this);
        if (_isPlayerAbleToTransformIntoMe)
            player.PiceCange(BoardPice, BoardPiceType);
        gameObject.SetActive(false);
    }



    public override void StartTurn()
    {
        base.StartTurn();
    }

    protected override IEnumerator TakeTurn()
    {
        while (_behaviourTree.Process() == BT_Node.Status.Running)
        {
            if (PossibleNodes.Count > 0)
            {
                lastPossibleNodes = PossibleNodes;
            }
            yield return null;
        }

        _behaviourTree.Reset();
        OnTurnEnd?.Invoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}