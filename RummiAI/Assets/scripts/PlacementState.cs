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

    public PlacementState(int ID, Grid grid, PreviewSystem previewSystem, ObjectsDatabase database, GridData tileData, ObjectPlacer objectPlacer, Tile tile)
    {
        this.ID = ID;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.tileData = tileData;
        this.objectPlacer = objectPlacer;
        this.tile = tile;

        //this.tile = tile;
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
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
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }
        int index = objectPlacer.PlacedObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), this.tile, grid);

        tileData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            index);

        // StopPlacement();
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //Grid selectedData = ;
        return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        // previewRenderer.material.color = placementValidity ? new UnityEngine.Color(0.4345407f, 0.8773585f, 0.6587523f) : Color.red;


        // mouseIndicator.transform.position = mousePosition;

        if (gridPosition.x > -10 && gridPosition.x < 9 && gridPosition.z > -5 && gridPosition.z < 3)// cellIndicator.transform.position.z = 19.15;
        {
            // cellIndicator.transform.position = grid.CellToWorld(gridPosition);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        }
    }

}
