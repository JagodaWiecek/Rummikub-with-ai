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

    Tile tile;

    private void Start()
    {
        StopPlacement();
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
        GameObject gameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        gameObject.transform.position = grid.CellToWorld(gridPosition);

        gameObject.GetComponent<Tile>().setNumer(this.tile.getNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().setColor(this.tile.GetColor());//ustawienie koloru napisu
        gameObject.transform.SetParent(this.transform);
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshProUGUI!");
        }
        textComponent.text = this.tile.getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
         textComponent.color = this.tile.GetColor(); //ustawienie koloru dla symbolu
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
        mouseIndicator.transform.position = mousePosition;

        if (grid.CellToWorld(gridPosition).x > 7.875 && grid.CellToWorld(gridPosition).x < 28 && grid.CellToWorld(gridPosition).z >8 && grid.CellToWorld(gridPosition).z < 18.5)// cellIndicator.transform.position.z = 19.15;
        {
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        }
    }
}
