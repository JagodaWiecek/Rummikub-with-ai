using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]

///Klasa do reprezentacji 
public class Tile : MonoBehaviour
{
    // 4 colors, numers from 1 to 13
    [SerializeField]
    private int number;
    [SerializeField]
    private UnityEngine.Color numberColor;
    [SerializeField]
    private string Tilename;
    [SerializeField]
    private string symbol;
    [SerializeField]
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
        //bool z = GetPut();
        Debug.Log("Tile: "+getNumber()+" - "+ getSymbol() + " - " +getTilename() + " - "+ GetPut());
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
    public bool GetPut() { return this.put; }

    public Tile getTile()
    {
        return this;
    }

    public Tile(Tile existingTile)
    {
        this.number = existingTile.getNumber();
        this.numberColor = existingTile.GetColor();
        this.Tilename = existingTile.getTilename();
        this.symbol = existingTile.getSymbol();
        this.put = existingTile.GetPut();
    }

    public bool Equals(Tile tile ,Tile other)
    {
        if (tile.getNumber() != other.getNumber() || 
            tile.getSymbol() != other.getSymbol() ||
            tile.getTilename() != other.getTilename() ||
            tile.GetColor() != other.GetColor() ||
            tile.GetPut() != other.GetPut() )
            return false;
        else 
            return true;
    }

}
