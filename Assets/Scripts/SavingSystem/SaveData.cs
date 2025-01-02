using System.Collections.Generic;

public class SaveData
{

    private int NLevel;

    public int LevelIndex;

    public List<bool> KillKing { get; set; }

    public List<bool> MinTurns { get; set; }

    public List<bool> NoEnemy{ get; set; }

    public List<bool> EveryEnemy{ get; set; }

    public List<bool> QueenEnding { get; set; }

    public bool Music;

    public bool SFX;

    public SaveData() {
        LevelIndex = 0;
        Music = true;
        SFX = true;
    
    }

}
