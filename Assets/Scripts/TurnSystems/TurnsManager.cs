using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    public Action OnPlayerTurnStart;
    public Action OnEnemiesTurnStart;
    public float _moveDuration = 1f;
    public float MoveDuration;

    private List<AiController> _enemies = new();
    private PlayerController _playerController;
    private int _nOfEnemies;
    private int _indexEnemiesTurnEnd;

    private void Awake()
    {
        MoveDuration = _moveDuration;
        _playerController = FindObjectOfType<PlayerController>();
        _enemies = FindObjectsOfType<AiController>().ToList();
        _nOfEnemies = _enemies.Count;
        _playerController.OnTurnSetupDone += StartGame;
        OnPlayerTurnStart += _playerController.StartTurn;
        _playerController.OnTurnEnd += PlayerTurnEnd;
        foreach (var enemy in _enemies)
        {
            enemy.OnTurnEnd += EnemyTurnEnd;
            OnEnemiesTurnStart += enemy.StartTurn;
            enemy.OnDeath += OnEnemyDeath;
        }

    }

    private void StartGame()
    {
        OnPlayerTurnStart?.Invoke();
    }
   

    private void OnEnemyDeath(AiController controller)
    {
        controller.OnTurnEnd -= EnemyTurnEnd;
        controller.OnDeath -= OnEnemyDeath;
        OnEnemiesTurnStart -= controller.StartTurn;
        _nOfEnemies--;
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

    private void OnDisable()
    {
        _playerController.OnTurnEnd -= PlayerTurnEnd;
        OnPlayerTurnStart -= _playerController.StartTurn;

        foreach (var enemy in _enemies)
        {
            OnEnemyDeath(enemy);
        }
    }
}