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
        Debug.Log("Tile: "+GetNumber()+" - "+ GetSymbol() + " - " +GetTilename() + " - "+ GetPut());
    }

    public void setNumer(int numer)
    {
        this.number = numer; 
    }
    public void SetColor(UnityEngine.Color color)
    {
        this.numberColor = color;
    }
    public void SetTilename(string tilename)
    {
        this.Tilename = tilename;
    }
    public void SetSymbol(string symbol)
    {
        this.symbol = symbol;
    }
    public void SetPut(bool put) { this.put = put; }

    public int GetNumber() { return this.number; }
    public UnityEngine.Color GetColor() { return this.numberColor; }
    public string GetTilename() { return this.Tilename; }
    public string GetSymbol() {  return this.symbol; }
    public bool GetPut() { return this.put; }

    public Tile getTile()
    {
        return this;
    }

    public Tile(Tile existingTile)
    {
        this.number = existingTile.GetNumber();
        this.numberColor = existingTile.GetColor();
        this.Tilename = existingTile.GetTilename();
        this.symbol = existingTile.GetSymbol();
        this.put = existingTile.GetPut();
    }

    public bool Equals(Tile tile ,Tile other)
    {
        if (tile.GetNumber() != other.GetNumber() || 
            tile.GetSymbol() != other.GetSymbol() ||
            tile.GetTilename() != other.GetTilename() ||
            tile.GetColor() != other.GetColor() ||
            tile.GetPut() != other.GetPut() )
            return false;
        else 
            return true;
    }
    
    public bool CheckTileValidity(Vector3Int gridPosition)
    {

        Vector3Int plusjeden = new Vector3Int(gridPosition.x + 1, gridPosition.y, gridPosition.z);
        Vector3Int plusdwa = new Vector3Int(gridPosition.x + 2, gridPosition.y, gridPosition.z);
        Vector3Int minusjeden = new Vector3Int(gridPosition.x - 1, gridPosition.y, gridPosition.z);
        Vector3Int minusdwa = new Vector3Int(gridPosition.x - 2, gridPosition.y, gridPosition.z);

        var board = GameController.Instance.GetBoardDictionary().board;
        if (this.GetNumber() == 30)
        {
            if (board.ContainsKey(plusjeden) && board.ContainsKey(minusjeden))
            {
                // return false; //Obie strony maj¹ jakieœ p³ytki
                if (board.ContainsKey(plusdwa) && board.ContainsKey(minusdwa))
                {
                    if ((((board[plusdwa].GetNumber() - 1) == board[plusjeden].GetNumber() && board[plusjeden].GetColor() == board[plusdwa].GetColor()) ||
                        (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)) &&
                        (((board[minusdwa].GetNumber() + 1) == board[minusjeden].GetNumber() && board[minusjeden].GetColor() == board[minusdwa].GetColor()) ||
                        (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)) &&
                        (((board[plusdwa].GetNumber() - 3) == board[minusjeden].GetNumber() && board[plusjeden].GetColor() == board[plusdwa].GetColor()) ||
                        (board[plusdwa].GetNumber() == 30 || board[minusjeden].GetNumber() == 30)) &&
                        (((board[plusdwa].GetNumber() - 4) == board[minusdwa].GetNumber() && board[plusjeden].GetColor() == board[plusdwa].GetColor()) ||
                        (board[minusdwa].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)))
                    {
                        return true;
                    }
                    else return false;

                }
                else if (board.ContainsKey(plusdwa))
                {
                    if ((((board[plusdwa].GetNumber() - 1) == board[plusjeden].GetNumber() && board[plusjeden].GetColor() == board[plusdwa].GetColor()) ||
                        (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)) &&
                          (((board[minusjeden].GetNumber() + 2) == board[plusjeden].GetNumber() && board[plusjeden].GetColor() == board[minusjeden].GetColor()) ||
                        (board[minusjeden].GetNumber() == 30 || board[plusjeden].GetNumber() == 30)) &&
                        (((board[plusdwa].GetNumber() - 3) == board[minusjeden].GetNumber() && board[minusjeden].GetColor() == board[plusdwa].GetColor()) ||
                        (board[minusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)))
                    {
                        return true;
                    }
                    else if ((board[plusjeden].GetNumber() == board[plusdwa].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)) &&
                            (board[plusjeden].GetNumber() == board[minusjeden].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[minusjeden].GetNumber() == 30)) &&
                            (board[minusjeden].GetNumber() == board[plusdwa].GetNumber() || (board[minusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)) &&
                            board[plusjeden].GetColor() != board[plusdwa].GetColor() &&
                            board[minusjeden].GetColor() != board[plusdwa].GetColor() &&
                            board[plusjeden].GetColor() != board[minusjeden].GetColor())
                    {
                        //return false;
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy)) return false;
                        else return true;
                    }
                }
                else if (board.ContainsKey(minusdwa))
                {
                    if ((((board[minusdwa].GetNumber() + 3) == board[plusjeden].GetNumber() && board[plusjeden].GetColor() == board[minusdwa].GetColor()) ||
                            (board[plusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)) &&
                            (((board[minusjeden].GetNumber() + 2) == board[plusjeden].GetNumber() && board[plusjeden].GetColor() == board[minusjeden].GetColor()) ||
                            (board[minusjeden].GetNumber() == 30 || board[plusjeden].GetNumber() == 30)) &&
                            (((board[minusdwa].GetNumber() +1) == board[minusjeden].GetNumber() && board[minusjeden].GetColor() == board[minusdwa].GetColor()) ||
                            (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)))
                    {
                        return true;
                    }
                    else if ((board[plusjeden].GetNumber() == board[minusdwa].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)) &&
                            (board[plusjeden].GetNumber() == board[minusjeden].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[minusjeden].GetNumber() == 30)) &&
                            (board[minusjeden].GetNumber() == board[minusdwa].GetNumber() || (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)) &&
                            board[plusjeden].GetColor() != board[minusdwa].GetColor() &&
                            board[minusjeden].GetColor() != board[minusdwa].GetColor() &&
                            board[plusjeden].GetColor() != board[minusjeden].GetColor())
                    {
                        //return false;
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy)) return false;
                        else return true;
                    }
                }
                else
                {
                    if (((board[minusjeden].GetNumber() + 2) == board[plusjeden].GetNumber() && board[plusjeden].GetColor() == board[minusjeden].GetColor()) ||
                            (board[minusjeden].GetNumber() == 30 || board[plusjeden].GetNumber() == 30))
                    {
                        return true;
                    }
                    else if (board[plusjeden].GetNumber() == board[minusjeden].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[minusjeden].GetNumber() == 30))
                    {
                        return true;
                    }
                    else return false;
                }
                return false;
            }
            else if (board.ContainsKey(plusjeden))
            {
                //return false;\
                if (board.ContainsKey(plusdwa))
                {
                    if ((board[plusdwa].GetNumber() - 1) == board[plusjeden].GetNumber() && board[plusjeden].GetNumber() != 1 ||
                        board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)
                    {
                        //return true;
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            if ((board[plusjeden].GetNumber() == (board[plustrzy].GetNumber() - 2) || board[plusjeden].GetNumber() == (board[plusdwa].GetNumber() - 1) || (board[plusdwa].GetNumber() == (board[plustrzy].GetNumber() - 1)))
                                && (board[plusjeden].GetNumber() != 1 && board[plusdwa].GetNumber() != 2))
                            {
                                return true;
                            }
                            else if ((board[plusjeden].GetNumber() == board[plustrzy].GetNumber() || board[plusjeden].GetNumber() == board[plusdwa].GetNumber() ||
                                board[plustrzy].GetNumber() == board[plusdwa].GetNumber()) &&
                                board[plusjeden].GetColor() != board[plustrzy].GetColor() &&
                                board[plusjeden].GetColor() != board[plusdwa].GetColor())//trzy ró¿ne kolory
                            {
                                //return true;
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(pluscztery))

                                    return false;
                                else return true;
                            }
                            else return false;
                        }
                        return true;
                    }
                    else if ((board[plusjeden].GetNumber() == board[plusdwa].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30))
                        && board[plusjeden].GetColor() != board[plusdwa].GetColor())
                    {
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            if ((board[plusjeden].GetNumber() == board[plusdwa].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)) &&
                                (board[plustrzy].GetNumber() == board[plusdwa].GetNumber() || (board[plustrzy].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)) &&
                                (board[plusjeden].GetNumber() == board[plustrzy].GetNumber() || (board[plusjeden].GetNumber() == 30 || board[plustrzy].GetNumber() == 30)) &&
                                board[minusjeden].GetColor() != board[plusdwa].GetColor() &&
                                board[plustrzy].GetColor() != board[plusdwa].GetColor() &&
                                board[plusjeden].GetColor() != board[plustrzy].GetColor())
                            {
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(pluscztery))

                                    return false;
                                else return true;
                            }
                            else return false;
                        }

                        else return true;
                    }
                    else return false;
                }
                else return true;
            }
            else if (board.ContainsKey(minusjeden))//tylko po lewej
            {
                if (board.ContainsKey(minusdwa))
                {
                    if ((board[minusdwa].GetNumber() + 1) == board[minusjeden].GetNumber() && board[minusjeden].GetNumber() != 13 ||
                        board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)
                    {
                        //return true;
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy))
                        {
                            if ((board[minusjeden].GetNumber() == (board[minustrzy].GetNumber() + 2) || board[minusjeden].GetNumber() == (board[minusdwa].GetNumber() + 1) || (board[minusdwa].GetNumber() == (board[minustrzy].GetNumber() + 1)))
                                && (board[minusjeden].GetNumber() != 13 && board[minusdwa].GetNumber() != 12))
                            {
                                return true;
                            }
                            else if ((board[minusjeden].GetNumber() == board[minustrzy].GetNumber() || board[minusjeden].GetNumber() == board[minusdwa].GetNumber() ||
                                board[minustrzy].GetNumber() == board[minusdwa].GetNumber()) &&
                                board[minusjeden].GetColor() != board[minustrzy].GetColor() &&
                                board[minusjeden].GetColor() != board[minusdwa].GetColor())//trzy ró¿ne kolory
                            {
                                //return true;
                                Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(minuscztery))

                                    return false;
                                else return true;
                            }
                            else return false;
                        }
                        return true;
                    }
                    else if ((board[minusjeden].GetNumber() == board[minusdwa].GetNumber() || (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30))
                        && board[minusjeden].GetColor() != board[minusdwa].GetColor())
                    {
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy))
                        {
                            if ((board[minusjeden].GetNumber() == board[minusdwa].GetNumber() || (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)) &&
                                (board[minustrzy].GetNumber() == board[minusdwa].GetNumber() || (board[minustrzy].GetNumber() == 30 || board[minusdwa].GetNumber() == 30)) &&
                                (board[minusjeden].GetNumber() == board[minustrzy].GetNumber() || (board[minusjeden].GetNumber() == 30 || board[minustrzy].GetNumber() == 30)) &&
                                board[minusjeden].GetColor() != board[minusdwa].GetColor() &&
                                board[minustrzy].GetColor() != board[minusdwa].GetColor() &&
                                board[minusjeden].GetColor() != board[minustrzy].GetColor())
                            {
                                Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(minuscztery))

                                    return false;
                                else return true;
                            }
                            else return false;
                        }
                        else return true;
                    }
                    //return true;
                    else return false;
                }
                else return true;
            }
            else
                return true; ////Gdy stawiamy jokera, do sprawdzenia by nie stawiaæ przy kolorach po lewej od 1 i po prawej od 13 i by nie dodaæ jako pi¹ty od liczb
        }
        else
        {
            if (board.ContainsKey(plusjeden) && board.ContainsKey(minusjeden))//jedna karta bo obu stronach lub dwie, nie joker
            {
                if (board.ContainsKey(plusdwa) && board.ContainsKey(minusdwa))//cztery p³ytki
                {
                    //return false;
                    if ((((this.GetNumber() + 1) == board[plusjeden].GetNumber() && this.GetColor() == board[plusjeden].GetColor()) ||
                       (board[plusjeden].GetNumber() == 30)) &&
                       (((this.GetNumber() + 2) == board[plusdwa].GetNumber() && this.GetColor() == board[plusdwa].GetColor()) ||
                       (board[plusdwa].GetNumber() == 30)) &&
                       (((this.GetNumber() - 1) == board[minusjeden].GetNumber() && this.GetColor() == board[minusjeden].GetColor()) ||
                       (board[minusjeden].GetNumber() == 30)) &&
                       (((this.GetNumber() - 2) == board[minusdwa].GetNumber() && this.GetColor() == board[minusdwa].GetColor()) ||
                       board[minusdwa].GetNumber() == 30))
                    {
                        return true;
                    }
                    else return false;
                }
                else if (board.ContainsKey(plusdwa))//trzy p³ytki
                {
                    //return false; 
                    if ((((this.GetNumber() + 1) == board[plusjeden].GetNumber() && this.GetColor() == board[plusjeden].GetColor()) ||
                       (board[plusjeden].GetNumber() == 30)) &&
                       (((this.GetNumber() + 2) == board[plusdwa].GetNumber() && this.GetColor() == board[plusdwa].GetColor()) ||
                       (board[plusdwa].GetNumber() == 30)) &&
                       (((this.GetNumber() - 1) == board[minusjeden].GetNumber() && this.GetColor() == board[minusjeden].GetColor()) ||
                       (board[minusjeden].GetNumber() == 30)))
                        return true;
                    else if (((this.GetNumber() == board[plusjeden].GetNumber() && this.GetColor() != board[plusjeden].GetColor()) || (board[plusjeden].GetNumber() == 30))
                        && ((this.GetNumber() == board[minusjeden].GetNumber() && this.GetColor() != board[minusjeden].GetColor()) || (board[minusjeden].GetNumber() == 30)) &&
                        ((this.GetNumber() == board[plusdwa].GetNumber() && this.GetColor() != board[plusdwa].GetColor()) || (board[plusdwa].GetNumber() == 30)) &&
                        board[plusjeden].GetColor() != board[minusjeden].GetColor() &&
                        board[plusjeden].GetColor() != board[plusdwa].GetColor() &&
                        board[minusjeden].GetColor() != board[plusdwa].GetColor())
                    {
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy)) return false;
                        else return true;
                    }
                    else return false;
                }
                else if (board.ContainsKey(minusdwa))//trzy p³ytki
                {
                    //return false ;
                    if ((((this.GetNumber() + 1) == board[plusjeden].GetNumber() && this.GetColor() == board[plusjeden].GetColor()) ||
                            (board[plusjeden].GetNumber() == 30)) &&
                            (((this.GetNumber() - 2) == board[minusdwa].GetNumber() && this.GetColor() == board[minusdwa].GetColor()) ||
                            (board[minusdwa].GetNumber() == 30)) &&
                            (((this.GetNumber() - 1) == board[minusjeden].GetNumber() && this.GetColor() == board[minusjeden].GetColor()) ||
                            (board[minusjeden].GetNumber() == 30)))
                        return true;
                    else if (((this.GetNumber() == board[plusjeden].GetNumber() && this.GetColor() != board[plusjeden].GetColor()) || (board[plusjeden].GetNumber() == 30))
                        && ((this.GetNumber() == board[minusjeden].GetNumber() && this.GetColor() != board[minusjeden].GetColor()) || (board[minusjeden].GetNumber() == 30)) &&
                        ((this.GetNumber() == board[minusdwa].GetNumber() && this.GetColor() != board[minusdwa].GetColor()) || (board[minusdwa].GetNumber() == 30)) &&
                        board[plusjeden].GetColor() != board[minusjeden].GetColor() &&
                        board[plusjeden].GetColor() != board[minusdwa].GetColor() &&
                        board[minusjeden].GetColor() != board[minusdwa].GetColor())
                    {
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy)) return false;
                        else return true;
                    }
                    else return false;
                }
                else //Do tylko dwie karty do sprawdzenia
                {
                    //return false;plusjeden i minusjeden
                    if (((((this.GetNumber() + 1) == board[plusjeden].GetNumber() &&
                       this.GetColor() == board[plusjeden].GetColor())) ||
                       (board[plusjeden].GetNumber() == 30)) &&
                       (((this.GetNumber() - 1) == board[minusjeden].GetNumber() &&
                       this.GetColor() == board[minusjeden].GetColor()) ||
                       board[minusjeden].GetNumber() == 30))
                    {
                        return true;
                    }
                    if (((this.GetNumber() == board[plusjeden].GetNumber() && this.GetColor() != board[plusjeden].GetColor()) || (board[plusjeden].GetNumber() == 30))
                        && (this.GetNumber() == board[minusjeden].GetNumber() && this.GetColor() != board[minusjeden].GetColor()) || (board[minusjeden].GetNumber() == 30) &&
                        board[plusjeden].GetColor() != board[minusjeden].GetColor())
                    {
                        return true;
                    }
                    else return false;

                }

            }

            else if (board.ContainsKey(plusjeden))//1 karta po prawej
            {

                if (board.ContainsKey(plusdwa))//dwie karty po prawej stronie
                {
                    //bool 
                    //Ten sam kolor, ró¿ne cyfry
                    if (((((this.GetNumber() + 1) == board[plusjeden].GetNumber() &&
                       this.GetColor() == board[plusjeden].GetColor()) ||
                       board[plusjeden].GetNumber() == 30) &&
                       (((this.GetNumber() + 2) == board[plusdwa].GetNumber() &&
                       this.GetColor() == board[plusdwa].GetColor()) ||
                       (board[plusdwa].GetNumber() == 30 && board[plusjeden].GetNumber() != 13))))
                    {

                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        //Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            if (((this.GetNumber() + 3) == board[plustrzy].GetNumber() && this.GetColor() == board[plustrzy].GetColor()) ||
                                 board[plustrzy].GetNumber() == 30)
                            {
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(pluscztery))
                                {
                                    if (((this.GetNumber() + 4) == board[pluscztery].GetNumber() && this.GetColor() == board[pluscztery].GetColor()) ||
                                            board[pluscztery].GetNumber() == 30) return true;
                                    else return false;
                                }
                            }
                            else if ((this.GetNumber() == board[plustrzy].GetNumber() && this.GetColor() != board[plustrzy].GetColor()) ||
                                board[plustrzy].GetNumber() == 30)
                            {
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(pluscztery)) return false;

                            }
                            else return false;
                        }
                        return true;
                    }//te same cyfry, ró¿ne kolory
                    else if (((this.GetNumber() == board[plusjeden].GetNumber() &&
                        this.GetColor() != board[plusjeden].GetColor()) ||
                        board[plusjeden].GetNumber() == 30) &&
                        (this.GetNumber() == board[plusdwa].GetNumber() &&
                        this.GetColor() != board[plusdwa].GetColor() ||
                        board[plusdwa].GetNumber() == 30) &&
                        board[plusjeden].GetColor() != board[plusdwa].GetColor())
                    {
                        //return true;
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            //cztery kolory
                            if (((this.GetNumber() == board[plusjeden].GetNumber() &&
                            this.GetColor() != board[plusjeden].GetColor()) ||
                            board[plusjeden].GetNumber() == 30) &&
                            ((this.GetNumber() == board[plusdwa].GetNumber() &&
                            this.GetColor() != board[plusdwa].GetColor()) ||
                            board[plusdwa].GetNumber() == 30) &&
                            ((this.GetNumber() == board[plustrzy].GetNumber() &&
                            this.GetColor() != board[plustrzy].GetColor()) ||
                            board[plustrzy].GetNumber() == 30) &&
                            board[plustrzy].GetColor() != board[plusdwa].GetColor() &&
                            board[plustrzy].GetColor() != board[plusjeden].GetColor() &&
                            board[plusjeden].GetColor() != board[plusdwa].GetColor())
                            {
                                //return true;
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(pluscztery)) return false;
                                else return true;
                            }
                            else return false;
                        }
                        return true;

                    }
                    else return false;
                }
                else
                {
                    if ((((this.GetNumber() + 1) == board[plusjeden].GetNumber() &&
                        this.GetColor() == board[plusjeden].GetColor()) ||
                        board[plusjeden].GetNumber() == 30) ||
                        (this.GetNumber() == board[plusjeden].GetNumber() &&
                        this.GetColor() != board[plusjeden].GetColor()) ||
                        board[plusjeden].GetNumber() == 30)
                    {
                        return true;
                    }
                    else return false;
                }
            }

            else if (board.ContainsKey(minusjeden))//po lewej stronie
            {
                // return false;
                if (board.ContainsKey(minusdwa))//dwie karty po lewej stronie
                {
                    //Ten sam kolor, ró¿ne cyfry 2
                    if ((((this.GetNumber() - 1) == board[minusjeden].GetNumber() &&
                       this.GetColor() == board[minusjeden].GetColor()) ||
                       board[minusjeden].GetNumber() == 30) &&
                       (((this.GetNumber() - 2) == board[minusdwa].GetNumber() &&
                       this.GetColor() == board[minusdwa].GetColor()) ||
                       (board[minusdwa].GetNumber() == 30 && board[minusjeden].GetNumber() != 1)))
                    {
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        //Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);

                        if (board.ContainsKey(minustrzy))
                        {
                            if (((this.GetNumber() - 3) == board[minustrzy].GetNumber() && this.GetColor() == board[minustrzy].GetColor()) ||
                                board[minustrzy].GetNumber() == 30)
                            {
                                //return true;
                                Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(minuscztery))
                                {
                                    if (((this.GetNumber() - 4) == board[minuscztery].GetNumber() && this.GetColor() == board[minuscztery].GetColor()) ||
                                        board[minuscztery].GetNumber() == 30) return true;
                                    else return false;
                                }
                            }
                            else if ((this.GetNumber() == board[minustrzy].GetNumber() &&
                                    this.GetColor() != board[minustrzy].GetColor()) ||
                                    board[minustrzy].GetNumber() == 30)
                            {
                                //return true;
                                Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(minuscztery))
                                    return false;
                            }
                            else return false;
                        }
                        return true;
                    }
                    //te same cyfry, ró¿ne kolory
                    else if (((this.GetNumber() == board[minusjeden].GetNumber() &&
                        this.GetColor() != board[minusjeden].GetColor()) ||
                        board[minusjeden].GetNumber() == 30) &&
                        (this.GetNumber() == board[minusdwa].GetNumber() &&
                        this.GetColor() != board[minusdwa].GetColor() ||
                        board[minusdwa].GetNumber() == 30) &&
                        board[minusjeden].GetColor() != board[minusdwa].GetColor())
                    {
                        //return true;
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy))
                        {
                            //return false;
                            if (((this.GetNumber() == board[minusjeden].GetNumber() &&
                            this.GetColor() != board[minusjeden].GetColor()) ||
                            board[minusjeden].GetNumber() == 30) &&
                            ((this.GetNumber() == board[minusdwa].GetNumber() &&
                            this.GetColor() != board[minusdwa].GetColor()) ||
                            board[minusdwa].GetNumber() == 30) &&
                            ((this.GetNumber() == board[minustrzy].GetNumber() &&
                            this.GetColor() != board[minustrzy].GetColor()) ||
                            board[minustrzy].GetNumber() == 30) &&
                            board[minustrzy].GetColor() != board[minusdwa].GetColor() &&
                            board[minustrzy].GetColor() != board[minusjeden].GetColor() &&
                            board[minusjeden].GetColor() != board[minusdwa].GetColor())
                            {
                                //return true;
                                Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(minuscztery)) return false;
                                else return true;
                            }
                            else return false;
                        }
                        return true;
                    }
                    else return false;

                }
                else
                {   //Sprawdzenie czy obie karty przestrzegaj¹ zasad
                    if ((((this.GetNumber() - 1) == board[minusjeden].GetNumber() &&
                        this.GetColor() == board[minusjeden].GetColor()) ||
                        board[minusjeden].GetNumber() == 30) ||
                        ((this.GetNumber() == board[minusjeden].GetNumber() &&
                        this.GetColor() != board[minusjeden].GetColor()) ||
                        board[minusjeden].GetNumber() == 30))
                    {
                        return true;
                    }
                    else return false;
                }
            }
            else return true;
        }

    }

}
