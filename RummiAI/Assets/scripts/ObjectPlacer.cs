using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlacedObject(GameObject prefab, Vector3 position, ref Tile tile, Grid grid,int index)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = position;

       // Debug.Log("position w objectPlacer - " + position);
        //Debug.Log("grid position w objectPlacer - " + grid.WorldToCell(position));

        gameObject.GetComponent<Tile>().setNumer(tile.getNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().setColor(tile.GetColor());//ustawienie koloru napisu
        gameObject.GetComponent<Tile>().setTilename(tile.getTilename());
        gameObject.GetComponent<Tile>().setSymbol(tile.getSymbol());
        gameObject.GetComponent<Tile>().setPut(tile.GetPut());
        //Debug.Log("Position - "+ position);
        //Debug.Log("Position grid to cell - "+ grid.WorldToCell(position));
        //Debug.Log("Position cell to world - "+ grid.CellToWorld(grid.WorldToCell(position)));
        //gameObject.GetComponent<Tile>().SetPosition(grid.WorldToCell(position));
        gameObject.transform.SetParent(transform.parent.Find("PlacedTiles"));
        //transform.Find
        gameObject.name = tile.getTilename();
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshPro!");
        }
        textComponent.text = tile.getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
        textComponent.color = tile.GetColor(); //ustawienie koloru dla symbolu

        placedGameObjects.Add(gameObject);
        if(GameController.Instance != null)
        {
            Vector3Int positionofGrid = grid.WorldToCell(position);
            GameController.Instance.GetBoardDictionary().AddObjectAt(positionofGrid, tile);
            
             GameController.Instance.GetBoardDictionary().ReturnTileInDirectory(positionofGrid).ShowTiles();
        }
        else Debug.LogError("Nie znaleziono GameControllerInstance w ObjectPlacer!");
        //taketile.DestroyTile2D(index);
        // Debug.Log("Placer object - "+this.transform);
        // Debug.Log("Placer object dzieci - "+ this.transform.parent.parent.Find("Canvas/Player_Deck/Hand").childCount);//.Find("Canvas/Player_Deck/Hand").childCount
        //Transform transformObject = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand").GetChild(index);
        Transform transformObject = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand").GetChild(index);
        GameObject obj = transformObject.gameObject;
       // Debug.Log("Placer object obiekt - " + obj);
        Destroy(obj);
        Transform Indextransform = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand");
        //Debug.Log("Placer object obiekt dziecko  - " + transform.gameObject);
        StartCoroutine(SetNewIndex(Indextransform));

        return placedGameObjects.Count-1;
    }

    private IEnumerator SetNewIndex(Transform transform)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject newTile = transform.GetChild(i).gameObject;
            Tile tile = newTile.GetComponent<Tile>().getTile();//
            Button button = newTile.GetComponentInChildren<Button>();
            if (button == null)
            {
                Debug.LogError("Prefab does not contain a Button component!");
            } 
            int idx = transform.GetChild(i).GetSiblingIndex();
            TakeTile takeTile = transform.GetComponent<TakeTile>();
            button.onClick.AddListener(() => takeTile.OnButtonClick(tile, ref idx));
        }
        
        // Debug.Log("Draggable obiekt indeks - " + transform.childCount);
    }

    internal void RemoveObjectAt(int gameObjectIndex,Tile tile)
    {
        if (placedGameObjects.Count <= gameObjectIndex || 
            placedGameObjects[gameObjectIndex] == null)
            return;
       // tile.ShowTiles();
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
