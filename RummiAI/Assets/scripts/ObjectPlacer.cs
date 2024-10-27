using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    [SerializeField]
    private List<GameObject> placedGameObjectsCopy = new();

    [SerializeField]
    private GameObject tilePrefab;
    GameObject newTile = null;
    

    /// <summary>
    /// Czyœci liste obiektów
    ///mo¿liwe u¿ycia gdy
    ///bêdzie trzeba nadpisaæ listê zawartoœci¹ kopii
    ///bêdzie reset mapy
    /// </summary>
    public void ClearplacedGameObjects()
    {
        placedGameObjects.Clear(); 
    }
    public void SetPlacedGameObjectsCopy()
    {
        placedGameObjectsCopy.Clear();
        foreach (GameObject gameObject in placedGameObjects)
        {
            placedGameObjectsCopy.Add(gameObject);
        }
        
    }
    //Destroy(placedGameObjects[index]);
    public void RemoveObjectsNotInCopy()
    {
        // Przechodzimy przez listê placedGameObjects, tworz¹c kopiê obiektów do usuniêcia
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (GameObject obj in placedGameObjects)
        {
            // Jeœli obiektu nie ma w placedGameObjectsCopy, dodajemy go do listy do usuniêcia
            if (!placedGameObjectsCopy.Contains(obj))
            {
                objectsToRemove.Add(obj);
            }
        }

        // Usuwamy wszystkie obiekty z listy i ze sceny
        foreach (GameObject obj in objectsToRemove)
        {
            placedGameObjects.Remove(obj);
            Destroy(obj); // Usuwanie obiektu ze sceny
        }
    }
    public void SetPlacedGameObjects()
    {
        placedGameObjects.Clear();
        foreach (GameObject gameObject in placedGameObjectsCopy)
        {
            placedGameObjects.Add(gameObject);
        }
    }
    public List<GameObject> GetplacedGameObjects()
    {
        return this.placedGameObjects;
    }
    public List<GameObject> GetplacedGameObjectsCopy()
    {
        return this.placedGameObjectsCopy;
    }

    public bool ListAreEqual()
    {
        if (this.placedGameObjects.Count != this.placedGameObjectsCopy.Count)
            return false;
        for (int i = 0; i < this.placedGameObjects.Count; i++) 
        {
            if (placedGameObjects[i] != placedGameObjectsCopy[i])
                return false;
        }
        return true;
    }
    public int PlacedObject(GameObject prefab, Vector3 position, ref Tile tile, Grid grid,int index)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = position;

        // Debug.Log("position w objectPlacer - " + position);
        //Debug.Log("Tile put : "+ tile.GetPut());
        
        gameObject.GetComponent<Tile>().setNumer(tile.GetNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().SetColor(tile.GetColor());//ustawienie koloru napisu
        gameObject.GetComponent<Tile>().SetTilename(tile.GetTilename());
        gameObject.GetComponent<Tile>().SetSymbol(tile.GetSymbol());
        gameObject.GetComponent<Tile>().SetPut(tile.GetPut());//
        gameObject.transform.SetParent(transform.parent.Find("PlacedTiles"));
        gameObject.name = tile.GetTilename();
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshPro!");
        }
        textComponent.text = tile.GetSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
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
        FindAndRemoveTile(tile);
        // Tile tileinList = obj.GetComponent<Tile>().getTile() ;
        // tileinList.ShowTiles();
        Transform Indextransform = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand");
        StartCoroutine(SetNewIndex(Indextransform));

        return placedGameObjects.Count-1;
    }

    private void FindAndRemoveTile(Tile tile)
    {
        List<Tile> Hand = GameController.Instance.GetPlayerHand();

        for (int i = 0; i < Hand.Count; i++) {
            if (Hand[i].Equals(tile, Hand[i])) 
            { 
                Hand.RemoveAt(i);
                break;
            }

        }
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

    internal void MoveObjectTo(int gameObjectIndex, Vector3 newPosition)
    {
        
        if (placedGameObjects.Count <= gameObjectIndex ||
            placedGameObjects[gameObjectIndex] == null)
            return;

        
        placedGameObjects[gameObjectIndex].transform.position = newPosition;
    }

    public void TakeBackTile(Tile tile)
    {
        List<Tile> Hand = GameController.Instance.GetPlayerHand();
        Transform transformObject = this.transform.parent.parent.Find("Canvas/Player_Deck/Hand");
        newTile = Instantiate(this.tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newTile.transform.SetParent(transformObject);//ustawienie hierarchi
        newTile.GetComponent<Tile>().setNumer(tile.GetNumber());//ustawienie numeru klasy
        newTile.GetComponent<Tile>().SetColor(tile.GetColor());//ustawienie koloru klasy
        newTile.GetComponent<Tile>().SetTilename(tile.GetTilename());
        newTile.GetComponent<Tile>().SetSymbol(tile.GetSymbol());
        newTile.name = tile.GetTilename();//ustawienie nazwy w hierarchi
        TextMeshProUGUI textComponent = newTile.transform.Find("Object/Number_Color").GetComponent<TextMeshProUGUI>();//odwo³anie siê do dziecka objektu
        textComponent.text = tile.GetSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
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
        Hand.Add(tile);
       // Debug.Log("Gracz ma w rence: " + Hand.Count + " p³ytek");
        LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku
    }

    public int CreateObject(GameObject prefab, Vector3 position, ref Tile tile, Grid grid)
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.position = position;


        gameObject.GetComponent<Tile>().setNumer(tile.GetNumber());//ustawienie numeru klasy
        gameObject.GetComponent<Tile>().SetColor(tile.GetColor());//ustawienie koloru napisu
        gameObject.GetComponent<Tile>().SetTilename(tile.GetTilename());
        gameObject.GetComponent<Tile>().SetSymbol(tile.GetSymbol());
        gameObject.GetComponent<Tile>().SetPut(tile.GetPut());//
        gameObject.transform.SetParent(transform.parent.Find("PlacedTiles"));
        gameObject.name = tile.GetTilename();
        TextMeshPro textComponent = gameObject.transform.Find("Object/Number_Color").GetComponent<TextMeshPro>();
        // Sprawdzanie, czy tileTest nie jest null
        if (textComponent == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshPro!");
        }
        textComponent.text = tile.GetSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
        textComponent.color = tile.GetColor(); //ustawienie koloru dla symbolu
       
        placedGameObjects.Add(gameObject);
        if (GameController.Instance != null)
        {
            Vector3Int positionofGrid = grid.WorldToCell(position);
            GameController.Instance.GetBoardDictionary().AddObjectAt(positionofGrid, tile);

            //GameController.Instance.GetBoardDictionary().ReturnTileInDirectory(positionofGrid).ShowTiles();
        }
        else Debug.LogError("Nie znaleziono GameControllerInstance w ObjectPlacer!");


        return placedGameObjects.Count - 1;
    }

}
