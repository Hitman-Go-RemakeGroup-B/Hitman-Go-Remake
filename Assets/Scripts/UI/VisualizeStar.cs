using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class VisualizeStar : MonoBehaviour
{
    [SerializeField]SaveDataJson saveDataJson;
    [SerializeField] Image[] Star;
    void Start()
    {
        saveDataJson.VisualizeStar(GetIndex()-1);
        Debug.Log(saveDataJson.starCount);
        for (int i = 0;i<saveDataJson.starCount;i++)
        {
            var tempColor = Star[i].color;
            tempColor.a = 1f;
            Star[i].color = tempColor;
        }
    }

    public int GetIndex()
    {
        return int.Parse(this.name.Split(' ')[1]);
    }
}
