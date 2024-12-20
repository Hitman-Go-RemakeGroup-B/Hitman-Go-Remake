using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [SerializeField] EntityType entityType;
    [SerializeField] private Node _startNode;
    [SerializeField] private Color _higlightColor;
    
    Node lastNode;
    List<Node> currentPath;
    Path path;
    float timer = 0f;
    public BaseEntity BoardPice;
    bool isDead = false;

    private void Awake()
    {
        TurnsManager.OnEnemiesTurnStart += StartTurn;

        switch (entityType)
        {
            case EntityType.Pawn:
                BoardPice = new PawnEntity(_startNode, _higlightColor, Death,TurnsManager.MoveDuration,transform);
                break;

            case EntityType.Rook:
                BoardPice = new RookEntity(_startNode, _higlightColor, Death, TurnsManager.MoveDuration, transform);
                break;

            case EntityType.Bishop:
                BoardPice = new BishopEntity(_startNode, _higlightColor, Death, TurnsManager.MoveDuration, transform);
                break;

            case EntityType.Knight:
                BoardPice = new knightEntity(_startNode, _higlightColor, Death, TurnsManager.MoveDuration, transform);
                break;
        }
    }

    private void Death()
    {
        isDead = true;
        // trow it somewhere??
    }

    private void Start()
    {
        TurnsManager.OnEnemiesTurnStart += StartTurn;
    }



    private void StartTurn()
    {
        if (isDead) TurnsManager.OnEnemiesTurnEnd?.Invoke(); return;

        while (BoardPice.Move()) ;

        TurnsManager.OnEnemiesTurnEnd?.Invoke();

    }
}
