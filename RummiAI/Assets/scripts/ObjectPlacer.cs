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

    [SerializeField]
    private GameObject tilePrefab;
    GameObject newTile = null;

    public int PlacedObject(GameObject prefab, Vector3 position, ref Tile tile, Grid grid,int index)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = position;

       // Debug.Log("position w objectPlacer - " + position);
        //Debug.Log("Tile put : "+ tile.GetPut());

        gameObject.GetComponent<Tile>().setNumer(tile.getNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().setColor(tile.GetColor());//ustawienie koloru napisu
        gameObject.GetComponent<Tile>().setTilename(tile.getTilename());
        gameObject.GetComponent<Tile>().setSymbol(tile.getSymbol());
        gameObject.GetComponent<Tile>().setPut(tile.GetPut());//
        gameObject.transform.SetParent(transform.parent.Find("PlacedTiles"));
        gameObject.name = tile.getTilename();
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshPro!");
        }
        textComponent.text = tile.getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
        textComponent.color = tile.GetColor(); //ustawienie koloru dla symbolu
        //tile.setPut(true);
        placedGameObjects.Add(gameObject);
        if(GameController.Instance != null)
        {
            Vector3Int positionofGrid = grid.WorldToCell(position);
            GameController.Instance.GetBoardDictionary().AddObjectAt(positionofGrid, tile);
            
             //GameController.Instance.GetBoardDictionary().ReturnTileInDirectory(positionofGrid).ShowTiles();
        }
        else Debug.LogError("Nie znaleziono GameControllerInstance w ObjectPlacer!");
        Transform transformObject = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand").GetChild(index);
        GameObject obj = transformObject.gameObject;
        Destroy(obj);
        Transform Indextransform = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand");
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

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || 
            placedGameObjects[gameObjectIndex] == null)
            return;
       // tile.ShowTiles();
       
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;

    }

    public void TakeBackTile(Tile tile)
    {
        Transform transformObject = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand");
        newTile = Instantiate(this.tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newTile.transform.SetParent(transformObject);//ustawienie hierarchi
        newTile.GetComponent<Tile>().setNumer(tile.getNumber());//ustawienie numeru klasy
        newTile.GetComponent<Tile>().setColor(tile.GetColor());//ustawienie koloru klasy
        newTile.GetComponent<Tile>().setTilename(tile.getTilename());
        newTile.GetComponent<Tile>().setSymbol(tile.getSymbol());
        newTile.name = tile.getTilename();//ustawienie nazwy w hierarchi
        TextMeshProUGUI textComponent = newTile.transform.Find("Object/Number_Color").GetComponent<TextMeshProUGUI>();//odwo³anie siê do dziecka objektu
        textComponent.text = tile.getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
        textComponent.color = tile.GetColor(); //ustawienie koloru dla symbolu

        // Tile tile = new Tile(tiles[TileIndex].getNumber(), tiles[TileIndex].GetColor(), tiles[TileIndex].getTilename(), tiles[TileIndex].getSymbol(), tiles[TileIndex].getPut());
        Button button = newTile.GetComponentInChildren<Button>();
        if (button == null)
        {
            Debug.LogError("Prefab does not contain a Button component!");
        }
        int idx = newTile.transform.GetSiblingIndex();

        TakeTile takeTile = transformObject.GetComponent<TakeTile>();
        //Debug.Log("newTile transform - "+newTile.transform);
        //Debug.Log("takeTile transform - " + transform);
        button.onClick.AddListener(() => takeTile.OnButtonClick(tile, ref idx));
        //tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
        LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku
    }


}
