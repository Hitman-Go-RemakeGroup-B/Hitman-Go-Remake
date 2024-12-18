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

    private static int _nOfEnemies = 0;
    private static int _indexEnemiesTurnEnd;

    private void Awake()
    {
        OnPlayerTurnEnd += PlayerTurnEnd;
        OnEnemiesTurnEnd += EnemyTurnEnd;
        List<AiController> enemies = FindObjectsByType<AiController>(FindObjectsSortMode.None).ToList();
        _nOfEnemies = enemies.Count;
    }

    private void EnemyTurnEnd()
    {
        if (_indexEnemiesTurnEnd < _nOfEnemies)
            return;

        // activate player input:

        // start player's turn:
         OnPlayerTurnStart?.Invoke();
    }

    private void PlayerTurnEnd()
    {
        // deactivate player input

        // startEnemiesTurn
        OnEnemiesTurnStart?.Invoke(); // says to all enemies to start taking their turn 
    }
}
