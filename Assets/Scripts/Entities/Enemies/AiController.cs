using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [SerializeField] EntityType entityType;
    [SerializeField] private Color _higlightColor;

    Path path;
    public BaseEntity BoardPice;
    bool isDead = false;
    private Node _startNode;

    private void Awake()
    {
        path = FindObjectOfType<Path>();
        TurnsManager.OnEnemiesTurnStart += StartTurn;

        _startNode = path.NodeFromWorldPos(transform.position);

        switch (entityType)
        {
            case EntityType.Pawn:
                BoardPice = new PawnEntity(_startNode, _higlightColor, Death, TurnsManager.MoveDuration, transform);
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
        if (isDead)
        {
            TurnsManager.OnEnemiesTurnEnd?.Invoke(); return;
        }

        while (BoardPice.Move());

        TurnsManager.OnEnemiesTurnEnd?.Invoke();

    }
}
