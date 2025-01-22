using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinCondition : MonoBehaviour
{


    public static Action<bool> Queen;
    public static Action<bool> King;
    public static Action<int> MinTurns;
    public static Action<int> noEnemy;
    public static Action<int> KillEnemy;
    public static Action Win;

    private SaveLevel level;
    private int starCount;
    public SaveData Data;

    [SerializeField] GameObject panel;
    [SerializeField] int numberOfEnemy;
    [SerializeField] int minTurn;
    [SerializeField] LevelScriptableObject Nlevel;
    [SerializeField] Image[] Star;
    //
    private void Awake()
    {
        level = new SaveLevel();
    }
    private void Start()
    {
        Star = new Image[3];
        starCount = 0;
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
        noEnemy += NoEnemy;
        KillEnemy += Killenemy;
        Win += WinUI;
    }

    private void OnDisable()
    {
        Queen -= HavingQueen;
        King -= KillKing;
        MinTurns -= CalcolateTurn;
        noEnemy -= NoEnemy;
        KillEnemy += Killenemy;
        Win -= WinUI;
    }

    private void HavingQueen(bool Queen)
    {
        if (Queen)
        {
            starCount++;
            var tempColor = Star[starCount - 1].color;
            tempColor.a = 1f;
            Star[starCount - 1].color = tempColor;
        }
        level.QueenEnding = Queen;
    }

    private void KillKing(bool King)
    {
        if (King)
        {
            starCount++;
            var tempColor = Star[starCount - 1].color;
            tempColor.a = 1f;
            Star[starCount - 1].color = tempColor;
        }
        level.KillKing = King;
    }

    private void CalcolateTurn(int Turn)
    {
        if (Turn <= minTurn)
        {
            level.MinTurns = true;
            starCount++;
            var tempColor = Star[starCount - 1].color;
            tempColor.a = 1f;
            Star[starCount - 1].color = tempColor;
        }
    }

    private void NoEnemy(int Enemy)
    {
        if (Enemy == 0)
        {
            level.NoEnemy = true;
            starCount++;
            var tempColor = Star[starCount - 1].color;
            tempColor.a = 1f;
            Star[starCount - 1].color = tempColor;
        }
        else
        {
            level.NoEnemy = false;
        }
    }

    private void Killenemy(int Enemy)
    {
        if (Enemy == numberOfEnemy)
        {
            level.EveryEnemy = true;
            starCount++;
            var tempColor = Star[starCount - 1].color;
            tempColor.a = 1f;
            Star[starCount - 1].color = tempColor;
        }
        else
        {
            level.EveryEnemy = false;
        }
    }
    private void GetIndex(){
        string scene = SceneManager.GetActiveScene().name;
        level.LevelIndex= int.Parse(scene.Split(' ')[1])-1;
    }


    private void WinUI()
    {
        GetIndex();
        SaveDataJson.levelAction?.Invoke(level);
        panel.SetActive(true);
    }

}
