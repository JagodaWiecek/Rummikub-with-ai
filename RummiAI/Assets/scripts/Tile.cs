using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Tile 
{
    // 4 colors, numers from 1 to 13
    public int number;
    public UnityEngine.Color numberColor;
    public string name;

    public Tile(int num, UnityEngine.Color col, string name)
    {
        this.number = num;
        this.numberColor = col;
        this.name = name;
    }

    public void ShowVariables()
    {
        Debug.Log("Tile: "+this.number+" "+ this.numberColor+" "+ this.name);
    }

}
