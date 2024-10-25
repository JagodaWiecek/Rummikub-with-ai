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
        if (placementValidity && this.tile.CheckTileValidity(gridPosition))//&& CheckTiles(gridPosition)
             return true;
        else return false;
        //return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
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

    public Vector3Int GetGridPosition()
    {
    
        return Vector3Int.zero;
    }

    public void SetGridPosition(Vector3Int gridPosition)
    {
        Vector3Int gp = gridPosition;
    }
}
