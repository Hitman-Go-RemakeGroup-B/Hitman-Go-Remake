using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SaveLevel
{

    public int LevelIndex;

    public bool KillKing { get; set; }

    public bool MinTurns { get; set; }

    public bool NoEnemy { get; set; }

    public bool EveryEnemy { get; set; }

    public bool QueenEnding { get; set; }

    public SaveLevel() {
        LevelIndex = 0;
        KillKing = true;
        MinTurns = true;
        NoEnemy = true;
        EveryEnemy = false;
        QueenEnding = false;
    }

    public SaveLevel(int level,bool killking,bool minturns,bool noenemy,bool everyenemy,bool queenending)
    {
        LevelIndex = level;
        KillKing = killking;
        MinTurns = minturns;
        NoEnemy = noenemy;
        EveryEnemy = everyenemy;
        QueenEnding = queenending;
    }

}
