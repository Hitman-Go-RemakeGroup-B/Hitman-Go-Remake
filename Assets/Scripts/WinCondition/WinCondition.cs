using System;
using UnityEngine;

public class WinCondition : MonoBehaviour
{

    public static Action<bool> Queen;
    public static Action<bool> King;
    public static Action<int> MinTurns;
    public static Action<int> Enemy;

    private SaveLevel level;
    public SaveData Data;

    [SerializeField] int numberOfEnemy;
    [SerializeField] int minTurn;
    [SerializeField]LevelScriptableObject Nlevel;

    private void Start()
    {
        if (Data == null)
        {
            Data = new SaveData(Nlevel.Nlevel);
        }
    }

    private void OnEnable()
    {
        Queen += HavingQueen;
        King += KillKing;
        MinTurns += CalcolateTurn;
        Enemy += DeathEnemy;
    }

    private void OnDisable()
    {
        Queen -= HavingQueen;
        King -= KillKing;
        MinTurns -= CalcolateTurn;
        Enemy -= DeathEnemy;
    }

    private void HavingQueen(bool Queen)
    {
        level.QueenEnding = Queen;
    }

    private void KillKing(bool King)
    {
        level.KillKing=King;
    }

    private void CalcolateTurn(int Turn)
    {
        if (Turn<=minTurn)
        {
            level.MinTurns = true;
        }
    }

    private void DeathEnemy(int Enemy)
    {
        if (Enemy == 0)
        {
            level.NoEnemy = true;
            level.EveryEnemy = false;
        }else if(Enemy==numberOfEnemy) {
            level.NoEnemy = false;
            level.EveryEnemy = true;            
        }
    }

    public void SaveLevel()
    {
        level=new SaveLevel();
        SaveDataJson.levelAction?.Invoke(level);
    }

}
