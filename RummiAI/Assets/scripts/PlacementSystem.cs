using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;


    [SerializeField]
    private ObjectsDatabase database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GridData tileData;

    private Renderer previewRenderer;

    private List<GameObject> placedGameObject = new();

    Tile tile;

    private void Start()
    {
        StopPlacement();
        tileData = new ();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>(); //0.4345407 0.8773585 0.6587523
    }

    public void StartPlacement(int ID, ref Tile tile)
    {
        StopPlacement();
        this.tile = tile;
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex <0) 
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.onClicked += PlaceStructure;
        inputManager.onExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.isPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        bool placementValidity = CheckPlacementValidity(gridPosition,selectedObjectIndex);
        if (placementValidity==false)
        {
            return;
        }

        GameObject gameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        gameObject.transform.position = grid.CellToWorld(gridPosition);



        gameObject.GetComponent<Tile>().setNumer(this.tile.getNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().setColor(this.tile.GetColor());//ustawienie koloru napisu
        gameObject.GetComponent<Tile>().setTilename(this.tile.getTilename());
        gameObject.GetComponent<Tile>().setSymbol(this.tile.getSymbol());
        gameObject.GetComponent<Tile>().setPut(this.tile.getPut());
        gameObject.transform.SetParent(this.transform);
        gameObject.name = this.tile.getTilename();
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshProUGUI!");
        }
        textComponent.text = this.tile.getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
        textComponent.color = this.tile.GetColor(); //ustawienie koloru dla symbolu

        placedGameObject.Add(gameObject);
        gameObject.GetComponent<Tile>().ShowTiles();

        tileData.AddObjectAt(gridPosition, 
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObject.Count -1);

        StopPlacement();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //Grid selectedData = ;
        return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.onClicked -= PlaceStructure;
        inputManager.onExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return; 
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
       // Debug.Log("grid position : x: "+gridPosition.x + " z: " + gridPosition.z);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? new UnityEngine.Color(0.4345407f, 0.8773585f, 0.6587523f) : Color.red;

        mouseIndicator.transform.position = mousePosition;

        if (gridPosition.x > -10 && gridPosition.x < 9 && gridPosition.z >-5 && gridPosition.z < 3)// cellIndicator.transform.position.z = 19.15;
       {
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        }
    }
}
