using System;
using System.Collections;
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


    private void Start()
    {
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

        TurnSetUp();
    }

    protected override void TurnSetUp()
    {
        _behaviourTree = null;

        // Selectors 
        BT_Selector amIDistractedSelector = new("am i distracted?");
        BT_Repeater findDirRepeater = new("did i find a sutabile direction");
        BT_Selector raycastSelector = new("did the raycast hit?");

        // Sequences
        _behaviourTree = new("TurnTree");
        BT_Sequence afterDead = new("do this after you know you died");
        BT_Sequence distractedSequence = new("do this while distracted");
        BT_Sequence findPossibleNodesSequence = new("do this after finding a possible node");
        BT_Sequence raycastSequence = new("do this while you check for the raycast");

        // Process

        BaseProcess amIDistractedProcess = new(new CheckDecorator(this, AmIDistracted));
        BaseProcess getNextDirProcess = new(new CheckDecorator(this, BoardPice.NextDirection));
        BaseProcess findPossibleNodesProcess = new(new FindNodeDecorator(this, BoardPice.FindPossibleNodes));
        BaseProcess findNodesWhileDistractedProcess = new(new CheckDecorator(this, BoardPice.FindEndNodeWhileDistracted));
        BaseProcess raycastProcess = new(new CheckDecorator(this, BoardPice.Raycast));
        BaseProcess moveToEndNodeProcess = new(new CheckDecorator(this, BoardPice.MoveTwoardsEndNode));
        BaseProcess killPlayerProcess = new(new CheckDecorator(this, KillPlayer));


        // Leafs

        BT_Leaf distractedCheck = new("distracted check", amIDistractedProcess);
        BT_Leaf getNextDir = new("gets the next dir", getNextDirProcess);
        BT_Leaf findPossibleNodes = new("finds all the possible nodes you can go twoards", findPossibleNodesProcess);
        BT_Leaf findNodesWhileDistracted = new("finds where to go while distracted", findNodesWhileDistractedProcess);
        BT_Leaf raycastCheck = new("checks if the raycast hit", raycastProcess);
        BT_Leaf moveToEndNode = new("moves to the endNode", moveToEndNodeProcess);
        BT_Leaf killPlayer = new("kill the player", killPlayerProcess);


        // constructing BT:
        _behaviourTree.AddChild(amIDistractedSelector);
        amIDistractedSelector.AddChild(distractedSequence);
        amIDistractedSelector.AddChild(findDirRepeater);
        findDirRepeater.AddChild(findPossibleNodesSequence);
        findDirRepeater.AddChild(getNextDir);

        // findPossibleNodeSequence:
        findPossibleNodesSequence.AddChild(findPossibleNodes);
        findPossibleNodesSequence.AddChild(raycastSelector);

        // distractedSequence:
        distractedSequence.AddChild(distractedCheck);
        distractedSequence.AddChild(findNodesWhileDistracted);
        distractedSequence.AddChild(raycastSelector);
        raycastSelector.AddChild(raycastSequence);
        raycastSelector.AddChild(moveToEndNode);

        // raycastSequence:
        raycastSequence.AddChild(raycastCheck);
        raycastSequence.AddChild(moveToEndNode);
        raycastSequence.AddChild(killPlayer);
    }

    private BT_Node.Status KillPlayer()
    {
        // you lose sad not pogchampion 
        Debug.Log("you lose, sad. not pogchampion");
        return BT_Node.Status.Success;
    }

    public override void PiceCange(BaseEntity newPice)
    {

    }

    public void Interact(PlayerController player)
    {
        IsDead = true;
        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
        if (_isPlayerAbleToTransformIntoMe)
            player.PiceCange(BoardPice);
    }



    public override void StartTurn()
    {
        base.StartTurn();
    }

    protected override IEnumerator TakeTurn()
    {
        return base.TakeTurn();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}