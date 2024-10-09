using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Tile : MonoBehaviour
{
    // 4 colors, numers from 1 to 13
    private int number;
    private UnityEngine.Color numberColor;
    private string Tilename;
    private string symbol;

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

    public void setNumer(int numer)
    {
        this.number = numer; 
    }
    public void setColor(UnityEngine.Color color)
    {
        this.numberColor = color;
    }
    public void setTilename(string tilename)
    {
        this.Tilename = tilename;
    }
    public void setSymbol(string symbol)
    {
        this.symbol = symbol;
    }

    public int getNumber() { return this.number; }
    public UnityEngine.Color GetColor() { return this.numberColor; }
    public string getTilename() { return this.Tilename; }
    public string getSymbol() {  return this.symbol; }

}
