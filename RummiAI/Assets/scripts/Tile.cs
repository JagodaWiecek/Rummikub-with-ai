using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

///Klasa do reprezentacji 
public class Tile : MonoBehaviour
{
    // 4 colors, numers from 1 to 13
    private int number;
    private UnityEngine.Color numberColor;
    private string Tilename;
    private string symbol;
    private bool put;

    public Tile(int num, UnityEngine.Color col, string name,string symbol, bool put)
    {
        this.number = num;
        this.numberColor = col;
        this.Tilename = name;
        this.symbol = symbol;
        this.put = put;
    }

    public void ShowTiles()
    {
        Debug.Log("Tile: "+getNumber()+" - "+ getSymbol() + " - " +getTilename() );
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
    public void setPut(bool put) { this.put = put; }

    public int getNumber() { return this.number; }
    public UnityEngine.Color GetColor() { return this.numberColor; }
    public string getTilename() { return this.Tilename; }
    public string getSymbol() {  return this.symbol; }
    public bool getPut() { return this.put; }

    public Tile getTile()
    {
        return this;
    }

}
