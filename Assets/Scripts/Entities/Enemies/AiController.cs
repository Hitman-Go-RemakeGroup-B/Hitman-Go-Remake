using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [SerializeField] Vector2Int _moveDir;
    [SerializeField] EntityType _entityType;
    [SerializeField] float _raycastDistance;

    Path path;
    public BaseEntity BoardPice;
    bool isDead = false;
    private Node _startNode;

    private void Awake()
    {
        path = FindObjectOfType<Path>();
        
    }

    private void OnEnable()
    {
        TurnsManager.OnEnemiesTurnStart += StartTurnCorutine;
    }
    private void OnDisable()
    {
        TurnsManager.OnEnemiesTurnStart -= StartTurnCorutine;
    }

    private void Start()
    {
        _startNode = path.NodeFromWorldPos(transform.position);
        switch (_entityType)
        {
            case EntityType.Pawn:
                BoardPice = new PawnEntity(_startNode, _moveDir, new(path.CollumsX, path.RowsZ), Death, transform);
                break;

            case EntityType.Rook:
                BoardPice = new RookEntity(_startNode, _moveDir, new(path.CollumsX, path.RowsZ), Death, transform);
                break;

            case EntityType.Bishop:
                BoardPice = new BishopEntity(_startNode, _moveDir, new(path.CollumsX, path.RowsZ), Death, transform);
                break;

            case EntityType.Knight:
                BoardPice = new KnightEntity(_startNode, _moveDir, new(path.CollumsX, path.RowsZ), Death, transform);
                break;
        }
    }

    private void Death()
    {
        isDead = true;
        // trow it somewhere??
    }

    private IEnumerator StartTurn()
    {
        if (isDead)
        {
            TurnsManager.OnEnemiesTurnEnd?.Invoke();
            yield return null;
        }


        if (BoardPice.RayCheck())
        {
            while (BoardPice.Move())
            {
                yield return null;
            }
            StopAllCoroutines();
        }


        while (BoardPice.TakeTurn())
        {
            yield return null;
        };

        TurnsManager.OnEnemiesTurnEnd?.Invoke();

    }

    public void StartTurnCorutine()
    {
        StartCoroutine(StartTurn());
    }
}
