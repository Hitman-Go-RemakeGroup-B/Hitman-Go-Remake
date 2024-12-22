using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{

    public static Action OnPlayerTurnStart;
    public static Action OnPlayerTurnEnd;
    public static Action OnEnemiesTurnEnd;
    public static Action OnEnemiesTurnStart;
    public float _moveDuration = 1f;

    public static float MoveDuration;
    private static int _nOfEnemies = 0;
    private static int _indexEnemiesTurnEnd;

    private void Awake()
    {
        MoveDuration = _moveDuration;
        OnPlayerTurnEnd += PlayerTurnEnd;
        OnEnemiesTurnEnd += EnemyTurnEnd;
        List<AiController> enemies = FindObjectsByType<AiController>(FindObjectsSortMode.None).ToList();
        _nOfEnemies = enemies.Count-1;
    }

    private void EnemyTurnEnd()
    {
        _indexEnemiesTurnEnd++;
        if (_indexEnemiesTurnEnd < _nOfEnemies)
            return;

        _indexEnemiesTurnEnd = 0;

        // start player's turn:
         OnPlayerTurnStart?.Invoke();
    }

    public void PlayerTurnEnd()
    {
        // startEnemiesTurn
        OnEnemiesTurnStart?.Invoke(); // says to all enemies to start taking their turn 
    }
}
