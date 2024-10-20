using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : IPlacementState
{
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabase database;
    GridData tileData;
    ObjectPlacer objectPlacer;
    Tile tile;
    Vector3 previousPosition;


    public MovingState(int iD, Grid grid, PreviewSystem previewSystem, ObjectsDatabase database, GridData tileData, ObjectPlacer objectPlacer, Tile tile, Vector3 previousPosition)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.tileData = tileData;
        this.objectPlacer = objectPlacer;
        this.tile = tile;
        this.previousPosition = previousPosition;   
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        throw new System.NotImplementedException();
    }
}
