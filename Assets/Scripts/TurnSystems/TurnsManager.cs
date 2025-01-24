using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    public Action OnPlayerTurnStart;
    public Action OnEnemiesTurnStart;
    public float MoveDuration;
    public Action<int> OnWinEnemies;

    private List<AiController> _enemies = new();
    private PlayerController _playerController;
    private int _nOfEnemies;
    private int _nOfTurns = 0;
    private int _indexEnemiesTurnEnd;

    private void Awake()
    {
        _nOfTurns = 0;
        _playerController = FindObjectOfType<PlayerController>();
        _enemies = FindObjectsOfType<AiController>().ToList();
        _nOfEnemies = _enemies.Count;
        _playerController.OnTurnSetupDone += StartGame;
        _playerController.GetRemainginEnemies = () => _nOfEnemies;
        _playerController.GetNumberOfTurns = () => _nOfTurns;
        OnPlayerTurnStart += _playerController.StartTurn;
        //_playerController.TimeToReachNextNode = MoveDuration;

        _playerController.OnTurnEnd += PlayerTurnEnd;
        foreach (var enemy in _enemies)
        {
            //enemy.TimeToReachNextNode = MoveDuration;
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
        {
            return;
        }
        _indexEnemiesTurnEnd = 0;


        // start player's turn:
        OnPlayerTurnStart?.Invoke();
    }

    public void PlayerTurnEnd()
    {
        // startEnemiesTurn
        _nOfTurns++;
        if (_nOfEnemies <= 0)
        {
            EnemyTurnEnd();
            return;
        }

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