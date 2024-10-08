using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Tile : MonoBehaviour
{
    // 4 colors, numers from 1 to 13
    public int number;
    public UnityEngine.Color numberColor;
    public string Tilename;
    public string symbol;

    public Tile(int num, UnityEngine.Color col, string name,string symbol)
    {
        this.number = num;
        this.numberColor = col;
        this.Tilename = name;
        this.symbol = symbol;
    }

    public void ShowVariables()
    {
        Debug.Log("Tile: "+this.number+" "+ this.numberColor+" "+ this.Tilename + " "+ this.symbol );//
    }

}
