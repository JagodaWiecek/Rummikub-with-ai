using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IPlacementState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData tileData;
    ObjectPlacer objectPlacer;
    Tile tile;
    int index;

    public PlacementState(int ID,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabase database,
                          GridData tileData,
                          ObjectPlacer objectPlacer,
                          Tile tile,
                          int index)//
    {
        this.ID = ID;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.tileData = tileData;
        this.objectPlacer = objectPlacer;
        this.tile = tile;
        this.index = index;
        //this.prefab = prefab;

        //this.tile = tile;
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        //bool tilesValidity = CheckTiles(gridPosition);&& tilesValidity == false
        if (selectedObjectIndex > -1 )
        {
            // gridVisualization.SetActive(true);
            // cellIndicator.SetActive(true);
            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                                                 database.objectsData[selectedObjectIndex].Size);
        }
        else throw new System.Exception($"No object with ID {this.ID}");

    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();

    }

    public void OnAction(Vector3Int gridPosition)
    {
       // Debug.Log("OnAction w PlacementState"+gridPosition);
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        
        if (placementValidity == false )
        {
            return;
        }
        int index = objectPlacer.PlacedObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition),ref this.tile, grid, this.index);

        tileData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        // StopPlacement();
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
       
    }
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
       // bool tilesValidity = ; //zwraca false jak s¹siedzi s¹ wbrew zasadom
        bool placementValidity = tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);//zwraca false jak nie mozna postawiæ
        if (placementValidity && CheckTiles(gridPosition))
             return true;
        else return false;
        //return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    private bool CheckTiles(Vector3Int gridPosition)
    {
        Vector3Int plusjeden = new Vector3Int(gridPosition.x+1, gridPosition.y, gridPosition.z);
        Vector3Int plusdwa = new Vector3Int(gridPosition.x+2, gridPosition.y, gridPosition.z);
        Vector3Int minusjeden = new Vector3Int(gridPosition.x-1, gridPosition.y, gridPosition.z);
        Vector3Int minusdwa = new Vector3Int(gridPosition.x-2, gridPosition.y, gridPosition.z);

        var board = GameController.Instance.GetBoardDictionary().board;
        if (this.tile.getNumber() == 30)
        {
            if(board.ContainsKey(plusjeden) && board.ContainsKey(minusjeden))
            {
               // return false; //Obie strony maj¹ jakieœ p³ytki
                if(board.ContainsKey(plusdwa) && board.ContainsKey(minusdwa))
                {
                    if((((board[plusdwa].getNumber() - 1) == board[plusjeden].getNumber() && board[plusjeden].GetColor()== board[plusdwa].GetColor()) ||
                        (board[plusjeden].getNumber() == 30 || board[plusdwa].getNumber() == 30)) &&
                        (((board[minusdwa].getNumber() + 1) == board[minusjeden].getNumber() && board[minusjeden].GetColor() == board[minusdwa].GetColor()) ||
                        (board[minusjeden].getNumber() == 30 || board[minusdwa].getNumber() == 30)))
                    {
                        return true;
                    }
                    else return false;
                    
                }
                else if(board.ContainsKey(plusdwa))
                {
                    if( (( (board[plusdwa].getNumber() - 1) == board[plusjeden].getNumber() && board[plusjeden].GetColor() == board[plusdwa].GetColor()) ||
                        (board[plusjeden].getNumber() == 30 || board[plusdwa].getNumber() == 30)) &&
                          (((board[minusjeden].getNumber() + 2) == board[plusjeden].getNumber() && board[plusjeden].GetColor() == board[minusjeden].GetColor()) ||
                        (board[minusjeden].getNumber() == 30 || board[plusjeden].getNumber() == 30)))
                    {
                        return true; 
                    }
                    else if ((board[plusjeden].getNumber() == board[plusdwa].getNumber() || (board[plusjeden].getNumber() == 30 || board[plusdwa].getNumber() == 30)) &&
                            (board[plusjeden].getNumber() == board[minusjeden].getNumber() || (board[plusjeden].getNumber() == 30 || board[minusjeden].getNumber() == 30)) &&
                            (board[minusjeden].getNumber() == board[plusdwa].getNumber() || (board[minusjeden].getNumber() == 30 || board[plusdwa].getNumber() == 30)) &&
                            board[plusjeden].GetColor() != board[plusdwa].GetColor() &&
                            board[minusjeden].GetColor() != board[plusdwa].GetColor() &&
                            board[plusjeden].GetColor() != board[minusjeden].GetColor())
                    {
                        //return false;
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if(board.ContainsKey(plustrzy)) return false;
                        else return true;
                    }
                }
                else if(board.ContainsKey(minusdwa))
                {
                    if ((((board[minusdwa].getNumber() + 3) == board[plusjeden].getNumber() && board[plusjeden].GetColor() == board[minusdwa].GetColor()) ||
                            (board[plusjeden].getNumber() == 30 || board[minusdwa].getNumber() == 30)) &&
                            (((board[minusjeden].getNumber() + 2) == board[plusjeden].getNumber() && board[plusjeden].GetColor() == board[minusjeden].GetColor()) ||
                            (board[minusjeden].getNumber() == 30 || board[plusjeden].getNumber() == 30)))
                    {
                        return true;
                    }
                    else if ((board[plusjeden].getNumber() == board[minusdwa].getNumber() || (board[plusjeden].getNumber() == 30 || board[minusdwa].getNumber() == 30)) &&
                            (board[plusjeden].getNumber() == board[minusjeden].getNumber() || (board[plusjeden].getNumber() == 30 || board[minusjeden].getNumber() == 30)) &&
                            (board[minusjeden].getNumber() == board[minusdwa].getNumber() || (board[minusjeden].getNumber() == 30 || board[minusdwa].getNumber() == 30)) &&
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
                    if( ((board[minusjeden].getNumber() + 2) == board[plusjeden].getNumber() && board[plusjeden].GetColor() == board[minusjeden].GetColor()) ||
                            (board[minusjeden].getNumber() == 30 || board[plusjeden].getNumber() == 30))
                    {
                        return true;
                    }
                    else if(board[plusjeden].getNumber() == board[minusjeden].getNumber() || (board[plusjeden].getNumber() == 30 || board[minusjeden].getNumber() == 30))
                    {
                        return true ;
                    }
                    else return false;
                }
                return false;
            }
            else if(board.ContainsKey(plusjeden))
            {
                //return false;\
                if (board.ContainsKey(plusdwa))
                {
                    if ((board[plusdwa].getNumber() - 1) == board[plusjeden].getNumber() && board[plusjeden].getNumber() != 1 ||
                        board[plusjeden].getNumber() == 30 || board[plusdwa].getNumber() == 30)
                    {
                        //return true;
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        { 
                            if ((board[plusjeden].getNumber() == (board[plustrzy].getNumber() - 2) || board[plusjeden].getNumber() == (board[plusdwa].getNumber() - 1) || (board[plusdwa].getNumber() == (board[plustrzy].getNumber() - 1)))
                                && (board[plusjeden].getNumber() != 1 && board[plusdwa].getNumber() != 2))
                            {
                                return true;
                            }
                            else if ((board[plusjeden].getNumber() == board[plustrzy].getNumber() || board[plusjeden].getNumber() == board[plusdwa].getNumber() ||
                                board[plustrzy].getNumber() == board[plusdwa].getNumber()) &&
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
                    else if ((board[plusjeden].getNumber() == board[plusdwa].getNumber() || (board[plusjeden].getNumber()==30 || board[plusdwa].getNumber() ==30))
                        && board[plusjeden].GetColor() != board[plusdwa].GetColor())
                    {
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            if ((board[plusjeden].getNumber() == board[plusdwa].getNumber() || (board[plusjeden].getNumber() == 30 || board[plusdwa].getNumber() == 30)) &&
                                (board[plustrzy].getNumber() == board[plusdwa].getNumber() || (board[plustrzy].getNumber() == 30 || board[plusdwa].getNumber() == 30)) &&
                                (board[plusjeden].getNumber() == board[plustrzy].getNumber() || (board[plusjeden].getNumber() == 30 || board[plustrzy].getNumber() == 30)) &&
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
                if(board.ContainsKey(minusdwa))
                {
                    if ((board[minusdwa].getNumber() + 1) == board[minusjeden].getNumber() && board[minusjeden].getNumber() != 13 ||
                        board[minusjeden].getNumber() == 30 || board[minusdwa].getNumber() == 30)
                    {
                        //return true;
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy))
                        { 
                            if ((board[minusjeden].getNumber() == (board[minustrzy].getNumber() + 2) || board[minusjeden].getNumber() == (board[minusdwa].getNumber() + 1) || (board[minusdwa].getNumber() == (board[minustrzy].getNumber() + 1)))
                                && (board[minusjeden].getNumber() != 13 && board[minusdwa].getNumber() != 12))
                            {
                                return true;
                            }
                            else if ((board[minusjeden].getNumber() == board[minustrzy].getNumber() || board[minusjeden].getNumber() == board[minusdwa].getNumber() ||
                                board[minustrzy].getNumber() == board[minusdwa].getNumber()) &&
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
                    else if ((board[minusjeden].getNumber() == board[minusdwa].getNumber() || (board[minusjeden].getNumber() == 30 || board[minusdwa].getNumber()==30) ) 
                        && board[minusjeden].GetColor() != board[minusdwa].GetColor())
                    {
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy))
                        {
                            if ((board[minusjeden].getNumber() == board[minusdwa].getNumber()  || (board[minusjeden].getNumber() == 30 || board[minusdwa].getNumber() == 30) ) &&
                                (board[minustrzy].getNumber() == board[minusdwa].getNumber() || (board[minustrzy].getNumber() == 30 || board[minusdwa].getNumber() == 30)) &&
                                (board[minusjeden].getNumber() == board[minustrzy].getNumber() || (board[minusjeden].getNumber() == 30 || board[minustrzy].getNumber() == 30)) &&
                                board[minusjeden].GetColor() != board[minusdwa].GetColor() &&
                                board[minustrzy].GetColor() != board[minusdwa].GetColor() &&
                                board[minusjeden].GetColor() != board[minustrzy].GetColor()) {
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
            { if(board.ContainsKey(plusdwa) && board.ContainsKey(minusdwa))//cztery p³ytki
                {
                    //return false;
                    if ( (( (this.tile.getNumber() + 1) == board[plusjeden].getNumber() && this.tile.GetColor() == board[plusjeden].GetColor()) ||
                       (board[plusjeden].getNumber() == 30)) &&
                       (((this.tile.getNumber() + 2) == board[plusdwa].getNumber() && this.tile.GetColor() == board[plusdwa].GetColor()) ||
                       (board[plusdwa].getNumber() == 30)) &&
                       (((this.tile.getNumber() -1) == board[minusjeden].getNumber() && this.tile.GetColor() == board[minusjeden].GetColor()) ||
                       (board[minusjeden].getNumber() == 30)) &&
                       (((this.tile.getNumber() - 2) == board[minusdwa].getNumber() && this.tile.GetColor() == board[minusdwa].GetColor()) ||
                       board[minusdwa].getNumber() == 30))
                    {
                        return true;
                    }
                    else return false;
                }
                else if(board.ContainsKey(plusdwa))//trzy p³ytki
                {
                    //return false; 
                    if ((((this.tile.getNumber() + 1) == board[plusjeden].getNumber() && this.tile.GetColor() == board[plusjeden].GetColor()) ||
                       (board[plusjeden].getNumber() == 30)) &&
                       (((this.tile.getNumber() + 2) == board[plusdwa].getNumber() && this.tile.GetColor() == board[plusdwa].GetColor()) ||
                       (board[plusdwa].getNumber() == 30)) &&
                       (((this.tile.getNumber() - 1) == board[minusjeden].getNumber() && this.tile.GetColor() == board[minusjeden].GetColor()) ||
                       (board[minusjeden].getNumber() == 30)))
                        return true;
                    else if(((this.tile.getNumber() == board[plusjeden].getNumber() && this.tile.GetColor() != board[plusjeden].GetColor()) || (board[plusjeden].getNumber() == 30))
                        && ((this.tile.getNumber() == board[minusjeden].getNumber() && this.tile.GetColor() != board[minusjeden].GetColor()) || (board[minusjeden].getNumber() == 30)) &&
                        ((this.tile.getNumber() == board[plusdwa].getNumber() && this.tile.GetColor() != board[plusdwa].GetColor()) || (board[plusdwa].getNumber() == 30)) &&
                        board[plusjeden].GetColor() != board[minusjeden].GetColor() &&
                        board[plusjeden].GetColor() != board[plusdwa].GetColor() &&
                        board[minusjeden].GetColor() != board[plusdwa].GetColor())
                    {
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if(board.ContainsKey(plustrzy)) return false;
                        else return true; 
                    }    
                    else return false;
                }
              else if(board.ContainsKey(minusdwa))//trzy p³ytki
                {
                    //return false ;
                    if ((((this.tile.getNumber() + 1) == board[plusjeden].getNumber() && this.tile.GetColor() == board[plusjeden].GetColor()) ||
                            (board[plusjeden].getNumber() == 30)) &&
                            (((this.tile.getNumber() - 2) == board[minusdwa].getNumber() && this.tile.GetColor() == board[minusdwa].GetColor()) ||
                            (board[minusdwa].getNumber() == 30)) &&
                            (((this.tile.getNumber() - 1) == board[minusjeden].getNumber() && this.tile.GetColor() == board[minusjeden].GetColor()) ||
                            (board[minusjeden].getNumber() == 30)))
                                return true;
                    else if (((this.tile.getNumber() == board[plusjeden].getNumber() && this.tile.GetColor() != board[plusjeden].GetColor()) || (board[plusjeden].getNumber() == 30))
                        && ((this.tile.getNumber() == board[minusjeden].getNumber() && this.tile.GetColor() != board[minusjeden].GetColor()) || (board[minusjeden].getNumber() == 30)) &&
                        ((this.tile.getNumber() == board[minusdwa].getNumber() && this.tile.GetColor() != board[minusdwa].GetColor()) || (board[minusdwa].getNumber() == 30)) &&
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
                    if(( (((this.tile.getNumber() + 1) == board[plusjeden].getNumber() &&
                       this.tile.GetColor() == board[plusjeden].GetColor())) ||
                       (board[plusjeden].getNumber() == 30)) && 
                       (((this.tile.getNumber() - 1) == board[minusjeden].getNumber() &&
                       this.tile.GetColor() == board[minusjeden].GetColor()) ||
                       board[minusjeden].getNumber() == 30))
                    {
                        return true;
                    }
                    if( ((this.tile.getNumber() == board[plusjeden].getNumber() && this.tile.GetColor() != board[plusjeden].GetColor()) || (board[plusjeden].getNumber() == 30))
                        && (this.tile.getNumber() == board[minusjeden].getNumber() && this.tile.GetColor() != board[minusjeden].GetColor()) || (board[minusjeden].getNumber() == 30) &&
                        board[plusjeden].GetColor() != board[minusjeden].GetColor())
                    {
                        return true;
                    }
                    else return false ;

                }
                
            }

            else if (board.ContainsKey(plusjeden))//1 karta po prawej
            {

                if (board.ContainsKey(plusdwa))//dwie karty po prawej stronie
                {
                    //bool 
                    //Ten sam kolor, ró¿ne cyfry
                    if (( (((this.tile.getNumber() + 1) == board[plusjeden].getNumber() &&
                       this.tile.GetColor() == board[plusjeden].GetColor() ) ||
                       board[plusjeden].getNumber() == 30)  &&
                       (( (this.tile.getNumber() + 2) == board[plusdwa].getNumber() &&
                       this.tile.GetColor() == board[plusdwa].GetColor() ) ||
                       (board[plusdwa].getNumber() == 30 && board[plusjeden].getNumber() != 13))))
                    {
                     
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        //Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            if (((this.tile.getNumber() + 3) == board[plustrzy].getNumber() && this.tile.GetColor() == board[plustrzy].GetColor()) ||
                                 board[plustrzy].getNumber() == 30)
                            {
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(pluscztery)) 
                                {
                                    if (((this.tile.getNumber() + 4) == board[pluscztery].getNumber() && this.tile.GetColor() == board[pluscztery].GetColor()) ||
                                            board[pluscztery].getNumber() == 30) return true;
                                    else return false;
                                }
                            }
                            else if((this.tile.getNumber() == board[plustrzy].getNumber() && this.tile.GetColor() != board[plustrzy].GetColor()) ||
                                board[plustrzy].getNumber() == 30)
                            {
                                Vector3Int pluscztery = new Vector3Int(gridPosition.x + 4, gridPosition.y, gridPosition.z);
                                if(board.ContainsKey(pluscztery)) return false;
                                 
                            }
                            else return false;
                        }
                        return true;
                    }//te same cyfry, ró¿ne kolory
                    else if (((this.tile.getNumber() == board[plusjeden].getNumber() &&
                        this.tile.GetColor() != board[plusjeden].GetColor()) ||
                        board[plusjeden].getNumber() == 30) &&
                        (this.tile.getNumber() == board[plusdwa].getNumber() &&
                        this.tile.GetColor() != board[plusdwa].GetColor() ||
                        board[plusdwa].getNumber() == 30 ) &&
                        board[plusjeden].GetColor() != board[plusdwa].GetColor())
                    {
                        //return true;
                        Vector3Int plustrzy = new Vector3Int(gridPosition.x + 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(plustrzy))
                        {
                            //cztery kolory
                            if( ((this.tile.getNumber() == board[plusjeden].getNumber() &&
                            this.tile.GetColor() != board[plusjeden].GetColor()) ||
                            board[plusjeden].getNumber() == 30) &&
                            ((this.tile.getNumber() == board[plusdwa].getNumber() &&
                            this.tile.GetColor() != board[plusdwa].GetColor()) ||
                            board[plusdwa].getNumber() == 30) &&
                            ((this.tile.getNumber() == board[plustrzy].getNumber() &&
                            this.tile.GetColor() != board[plustrzy].GetColor()) ||
                            board[plustrzy].getNumber() == 30) &&
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
                    if ((((this.tile.getNumber() + 1) == board[plusjeden].getNumber() &&
                        this.tile.GetColor() == board[plusjeden].GetColor()) ||
                        board[plusjeden].getNumber() == 30) ||
                        (this.tile.getNumber() == board[plusjeden].getNumber() &&
                        this.tile.GetColor() != board[plusjeden].GetColor()) ||
                        board[plusjeden].getNumber() == 30)
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
                    if ( (((this.tile.getNumber() - 1) == board[minusjeden].getNumber() &&
                       this.tile.GetColor() == board[minusjeden].GetColor()) ||
                       board[minusjeden].getNumber()==30) &&
                       (((this.tile.getNumber() - 2) == board[minusdwa].getNumber() &&
                       this.tile.GetColor() == board[minusdwa].GetColor()) ||
                       (board[minusdwa].getNumber() == 30 && board[minusjeden].getNumber() != 1)))
                    {
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        //Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);

                        if(board.ContainsKey(minustrzy))
                        {
                            if(((this.tile.getNumber() - 3)== board[minustrzy].getNumber() && this.tile.GetColor() == board[minustrzy].GetColor()) ||
                                board[minustrzy].getNumber() == 30)
                            {
                                //return true;
                                Vector3Int minuscztery = new Vector3Int(gridPosition.x - 4, gridPosition.y, gridPosition.z);
                                if (board.ContainsKey(minuscztery))
                                {
                                    if(((this.tile.getNumber() - 4) == board[minuscztery].getNumber() && this.tile.GetColor() == board[minuscztery].GetColor()) ||
                                        board[minuscztery].getNumber() == 30) return true;
                                    else return false;
                                }
                            }
                            else if((this.tile.getNumber() == board[minustrzy].getNumber() &&
                                    this.tile.GetColor() != board[minustrzy].GetColor()) ||
                                    board[minustrzy].getNumber() == 30)
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
                    else if (((this.tile.getNumber() == board[minusjeden].getNumber() &&
                        this.tile.GetColor() != board[minusjeden].GetColor()) ||
                        board[minusjeden].getNumber()==30) &&
                        (this.tile.getNumber() == board[minusdwa].getNumber() &&
                        this.tile.GetColor() != board[minusdwa].GetColor() ||
                        board[minusdwa].getNumber() == 30) &&
                        board[minusjeden].GetColor() != board[minusdwa].GetColor())
                    {
                        //return true;
                        Vector3Int minustrzy = new Vector3Int(gridPosition.x - 3, gridPosition.y, gridPosition.z);
                        if (board.ContainsKey(minustrzy))
                        {
                            //return false;
                            if ( ((this.tile.getNumber() == board[minusjeden].getNumber() &&
                            this.tile.GetColor() != board[minusjeden].GetColor()) ||
                            board[minusjeden].getNumber() == 30) &&
                            ((this.tile.getNumber() == board[minusdwa].getNumber() &&
                            this.tile.GetColor() != board[minusdwa].GetColor()) ||
                            board[minusdwa].getNumber() == 30) &&
                            ((this.tile.getNumber() == board[minustrzy].getNumber() &&
                            this.tile.GetColor() != board[minustrzy].GetColor()) ||
                            board[minustrzy].getNumber() == 30) &&
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
                    if ((( (this.tile.getNumber() - 1) == board[minusjeden].getNumber() &&
                        this.tile.GetColor() == board[minusjeden].GetColor()) ||
                        board[minusjeden].getNumber()==30) ||
                        ((this.tile.getNumber() == board[minusjeden].getNumber() &&
                        this.tile.GetColor() != board[minusjeden].GetColor() )||
                        board[minusjeden].getNumber() == 30)   )
                    {
                        return true;
                    }
                    else return false;
                }
            }
            else return true;
        }
        
    }



    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (gridPosition.x > -10 && gridPosition.x < 9 && gridPosition.z > -5 && gridPosition.z < 3)// cellIndicator.transform.position.z = 19.15;
        {
            // cellIndicator.transform.position = grid.CellToWorld(gridPosition);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }
    }


}
