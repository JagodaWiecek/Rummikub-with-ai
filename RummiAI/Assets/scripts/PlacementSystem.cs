using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    // [SerializeField]
    // private GameObject mouseIndicator;// cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;


    [SerializeField]
    private ObjectsDatabase database;
    //private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GridData tileData;

    //private Renderer previewRenderer;

    // private List<GameObject> placedGameObject = new();
    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    IPlacementState placementState;


    Tile tile;
    //[SerializeField]
    //GameObject prefab;


    private void Start()
    {
        gridVisualization.SetActive(false);
        StopPlacement();
        tileData = new();
        //previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartPlacement(int ID, ref Tile tile, ref int index)//
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        this.tile = tile;
        placementState = new PlacementState(ID, grid, preview, database, tileData, objectPlacer, tile, index);
        inputManager.onClicked += PlaceStructure;
        inputManager.onExit += StopPlacement;

    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        placementState = new RemovingState(grid, preview, tileData, objectPlacer, this.tile);
        inputManager.onClicked += PlaceStructure;
        inputManager.onExit += StopPlacement;
    }

    public void StartMowing()
    {

        StopPlacement();
        //Vector3 Position = inputManager.GetMapPosition();
        gridVisualization.SetActive(true);
        placementState = new MovingState(0, grid, preview, database, tileData, objectPlacer, inputManager);
        inputManager.onClicked += PlaceStructure;
        inputManager.onExit += StopPlacement;
        
        if (GameController.Instance != null)
        {
            //this.stopPlacement();
            if (GameController.Instance.GetBoardDictionary().board.ContainsKey(placementState.GetGridPosition()) == false)
            { 
                //Debug.Log($"pozycja na lewy przycisk myszy jest zajêta: {placementState.GetGridPosition()}");
                //gridVisualization.SetActive(true);
                StopPlacement();
            }

            
        }
        else
        {
            Debug.Log("Game controller nie jest zainicjowany w placement system");
        }

        //Debug.Log($"pozycja myszki{Position}, i pozycja jej grida {grid.WorldToCell(Position)}");
        //if (placementState.CheckTile())
        // StopPlacement();

    }

    private void PlaceStructure()
    {
        if (inputManager.isPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        placementState.OnAction(gridPosition);

        StopPlacement();
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    //Grid selectedData = ;
    //    return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {

        if (placementState == null)
            return;
        gridVisualization.SetActive(false);
        placementState.EndState();
        inputManager.onClicked -= PlaceStructure;
        inputManager.onExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        placementState = null;
    }

   // public delegate void StopPlacementDelegate();
        


    private void Update()
    {
        if (placementState == null)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
       if(lastDetectedPosition != gridPosition)
        {
            placementState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
       // if (placementState is MovingState && placementState.GetGridPosition() == new Vector3Int(-20, -20, -20))
       //     placementState.SetGridPosition(gridPosition);

    }

    public GridData GetGridData() { return this.tileData; }
}
