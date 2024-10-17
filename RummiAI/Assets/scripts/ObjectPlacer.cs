using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlacedObject(GameObject prefab, Vector3 position, Tile tile, Grid grid)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = position;



        gameObject.GetComponent<Tile>().setNumer(tile.getNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().setColor(tile.GetColor());//ustawienie koloru napisu
        gameObject.GetComponent<Tile>().setTilename(tile.getTilename());
        gameObject.GetComponent<Tile>().setSymbol(tile.getSymbol());
        gameObject.GetComponent<Tile>().setPut(tile.GetPut());

        gameObject.GetComponent<Tile>().SetPosition(grid.WorldToCell(position));
        gameObject.transform.SetParent(transform.parent.Find("PlacedTiles"));
        //transform.Find
        gameObject.name = tile.getTilename();
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshProUGUI!");
        }
        textComponent.text = tile.getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
        textComponent.color = tile.GetColor(); //ustawienie koloru dla symbolu

        placedGameObjects.Add(gameObject);
        return placedGameObjects.Count-1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || 
            placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
