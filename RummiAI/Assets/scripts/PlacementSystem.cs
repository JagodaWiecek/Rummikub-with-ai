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

    public InputManager GetInputManager() { return inputManager; }
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
        gridVisualization.SetActive(true);
        placementState = new MovingState(0, grid, preview, database, tileData, objectPlacer, inputManager);
        inputManager.onClicked += PlaceStructure;
        inputManager.onExit += StopPlacement;

        if (GameController.Instance != null) { if (GameController.Instance.GetBoardDictionary().board.ContainsKey(placementState.GetGridPosition()) == false) StopPlacement(); }
        else Debug.Log("Game controller nie jest zainicjowany w placement system");
        
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

    public void StopPlacement()
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

    }

    public GridData GetGridData() { return this.tileData; }
}
