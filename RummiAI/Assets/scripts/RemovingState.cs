using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IPlacementState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData tileData;
    ObjectPlacer objectPlacer;
    Tile tile;

    public RemovingState( Grid grid,
                         PreviewSystem previewSystem,
                         GridData tileData,
                         ObjectPlacer objectPlacer,
                         Tile tile)
    {
        
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.tileData = tileData;
        this.objectPlacer = objectPlacer;
        this.tile = tile;

        previewSystem.StartShowingRemovePreview();

    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (tileData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = tileData;
        }
        if (selectedData == null) 
        { 
            
        }
        else
        {

            gameObjectIndex = selectedData.getRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
                return;
            if (GameController.Instance != null || GameController.Instance.GetBoardDictionary().board.ContainsKey(gridPosition) == false)
            {
                if (GameController.Instance.GetBoardDictionary().board[gridPosition].GetPut())
                    Debug.Log("nie mo¿na usun¹æ ju¿ po³o¿onego obiektu");
                else
                {
                    
                    selectedData.RemoveObjectAt(gridPosition);
                    objectPlacer.RemoveObjectAt(gameObjectIndex);
                    objectPlacer.TakeBackTile(GameController.Instance.GetBoardDictionary().board[gridPosition].getTile());//tile
                    GameController.Instance.GetBoardDictionary().board.Remove(gridPosition);//Remove(keyToRemove)
                }
            }
            else return;
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        //Debug.Log("gridPosition on remove" + gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
        

    }


    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(tileData.CanPlaceObjectAt(gridPosition,Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
       previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }

}
