using System.Collections.Generic;

[System.Serializable]

public class SaveData
{

    public int LevelIndex;

    public List<bool> KillKing;

    public List<bool> MinTurns;

    public List<bool> NoEnemy;

    public List<bool> EveryEnemy;

    public List<bool> QueenEnding;

    public bool Music;

    public bool SFX;

    public SaveData(int nLevel) {
        LevelIndex = 0;
        Music = true;
        SFX = true;
        KillKing = new List<bool>();
        MinTurns = new List<bool>();
        NoEnemy = new List<bool>();
        EveryEnemy = new List<bool>();
        QueenEnding = new List<bool>();
        for (int i = 0; i < nLevel; i++)
        {
            KillKing.Add(false);
            MinTurns.Add(false);
            NoEnemy.Add(false);
            EveryEnemy.Add(false);
            QueenEnding.Add(false);
        }
    }

}
