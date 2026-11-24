using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//s³u¿y do k³adzenia nowej p³ytki na mapê
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

    int maxX = 11;
    int minX = -12;
    int maxZ = 5;
    int minZ = -4;
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
        //Debug.Log("OnAction w PlacementState"+gridPosition);
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        Vector3Int minRange = new Vector3Int(minX, 0, minZ);
        Vector3Int maxRange = new Vector3Int(maxX, 0, maxZ);
        bool onMap = IsPositionInRange(gridPosition,minRange, maxRange);
        if (placementValidity == false || onMap == false)
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
       
        bool placementValidity = tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);//zwraca false jak nie mozna postawiæ
        if (placementValidity && this.tile.CheckTileValidity(gridPosition))
             return true;
        else return false;
        //return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private bool IsPositionInRange(Vector3 position, Vector3Int minRange, Vector3Int maxRange)
    {
        return position.x >= minRange.x && position.x <= maxRange.x &&
               position.y >= minRange.y && position.y <= maxRange.y &&
               position.z >= minRange.z && position.z <= maxRange.z;
    }




    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (gridPosition.x > minX - 1 && gridPosition.x < maxX + 1 && gridPosition.z > minZ - 1 && gridPosition.z < maxZ+1)// cellIndicator.transform.position.z = 19.15;
        {
            // cellIndicator.transform.position = grid.CellToWorld(gridPosition);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }
    }

    public Vector3Int GetGridPosition()
    {
    
        return Vector3Int.zero;
    }

}
