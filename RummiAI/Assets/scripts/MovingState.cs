using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlacementSystem;

public class MovingState : IPlacementState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData tileData;
    ObjectPlacer objectPlacer;
    Tile tile;
    Vector3Int previousPosition;
    private InputManager inputManager;
    /// <summary>
    /// Konstruktor klasy MovingState, 
    /// klasa s³ó¿y do przesuwania istniej¹cych p³ytek 3d na mapie
    /// </summary>
    /// <param name="iD"></param>
    /// <param name="grid"></param>
    /// <param name="previewSystem"></param>
    /// <param name="database"></param>
    /// <param name="tileData"></param>
    /// <param name="objectPlacer"></param>
    /// <param name="inputManager"></param>
    public MovingState(int iD, Grid grid, PreviewSystem previewSystem, ObjectsDatabase database, GridData tileData, ObjectPlacer objectPlacer, InputManager inputManager)//, Vector3 previousPosition
    {
        this.ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.tileData = tileData;
        this.objectPlacer = objectPlacer;
        this.inputManager = inputManager;
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
       this.previousPosition = gridPosition;

        if (selectedObjectIndex > -1)
        {

            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                                                 database.objectsData[selectedObjectIndex].Size);
        }

        if (GameController.Instance.GetBoardDictionary().board.ContainsKey(this.previousPosition) == true)
            this.tile = GameController.Instance.GetBoardDictionary().board[this.previousPosition];
    }

    public Vector3Int GetGridPosition()
    {

        return this.previousPosition;
    }
 

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
         GridData selectedData = null;
         if (tileData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == true)
         {
             selectedData = this.tileData;
         }


         if (tileData != null)
         {
            if (!CheckPlacementValidity(gridPosition, ID))
                return;
            selectedObjectIndex = selectedData.getRepresentationIndex(this.previousPosition);
             if (selectedObjectIndex == -1)
                 return;

             selectedData.MoveObjectAt(gridPosition, this.previousPosition, database.objectsData[this.ID].Size);
             objectPlacer.MoveObjectTo(selectedObjectIndex, grid.CellToWorld(gridPosition));
             GameController.Instance.GetBoardDictionary().MoveObjectAt(gridPosition, this.previousPosition);
         }
         else return;
 
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        //Debug.Log("gridPosition on remove" + gridPosition);
        //previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));

    }
    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(tileData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
         bool placementValidity = tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);//zwraca false jak nie mozna postawiæ
        if (placementValidity && this.tile.CheckTileValidity(gridPosition))//
            return true;
        else return false;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (gridPosition.x > -10 && gridPosition.x < 9 && gridPosition.z > -5 && gridPosition.z < 3)
        {
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            //Debug.Log($"Pozycja grida updateState - {gridPosition}");
        }
    }

}
