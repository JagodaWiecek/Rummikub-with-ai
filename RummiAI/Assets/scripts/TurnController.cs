using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
/// <summary>
/// Kontroler tur gry, sprawdza czy wielkoœci sekwencji na mapie s¹
///poprawne, przekazuje informacje o b³êdach do interfejsu
/// </summary>
public class TurnController : MonoBehaviour
{
    [SerializeField]
    TakeTile takeTile;

    [SerializeField]
    PlacementSystem placementSystem;

    [SerializeField]
    private GridData tileData;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabase database;

    [SerializeField]
    UnityEngine.UI.Button undoButton;

    [SerializeField]
    UnityEngine.UI.Button endTurn;

    [SerializeField]
    UnityEngine.UI.Button takeTileButton;

    [SerializeField]
    GameObject napis;
    float setNapisTime = 1f;
    float napisTime;

    int maxX = 11;
    int minX = -12;
    int maxZ = 5;
    int minZ = -4;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance.gameIndex != 1 && GameController.Instance.gameIndex != 4 && 
            GameController.Instance.gameIndex != 5 && GameController.Instance.gameIndex != 6 && GameController.Instance.gameIndex != 7)
        {
            undoButton.gameObject.SetActive(false);
            endTurn.gameObject.SetActive(false);
        }
        if (napis != null) { 
            napis.gameObject.SetActive(false);
        }
        napisTime = setNapisTime;
    }

    // Update is called once per frame
    void Update()
    {//gameTurnManager
        if (GameController.Instance.gameIndex != 1 && GameController.Instance.gameIndex != 4 && 
            GameController.Instance.gameIndex != 5 && GameController.Instance.gameIndex != 6 && GameController.Instance.gameIndex != 7)
        {
            if (GameController.Instance.gameTurnManager.currentPlayerId == 0)
            {
                takeTileButton.gameObject.SetActive(true);
                if (MapContents())
                {
                    undoButton.gameObject.SetActive(true);
                    endTurn.gameObject.SetActive(true);
                }
                else
                {
                    undoButton.gameObject.SetActive(false);
                    endTurn.gameObject.SetActive(false);
                }
            }
            else
            {
                takeTileButton.gameObject.SetActive(false);
                undoButton.gameObject.SetActive(false);
                endTurn.gameObject.SetActive(false);
            }
        }

        if (napis != null  && napis.activeSelf) {
            napisTime -= Time.deltaTime;

            if (napisTime <= 0f)
            {
                napis.gameObject.SetActive(false);
                napis.gameObject.GetComponentInChildren<Text>().text = "";
                napisTime = setNapisTime;

            }
        }

    }

    public void NewTurn()
    {
        if(GameController.Instance != null)
        {
            if (GameController.Instance.GetBoardDictionary().board.Count != 0)
            {


                if (CheckMap())
                {
                    if (GameController.Instance.GetPlayerHand().Count != 0)
                    { //koniec gry
                        //if(GameController.Instance.GetPlayer().GetFirstTour())

                        if (GameController.Instance.GetPlayer().GetFirstTour())//jeœli to pierwsza tura
                        {
                            if (GameController.Instance.firstTurnController.CheckFirstTurnValidity(GameController.Instance.GetBoardDictionary().board))
                            {
                                GameController.Instance.GetPlayer().EndFirstTour();
                                GameController.Instance.firstTurnController.Reset();

                                foreach (KeyValuePair<Vector3Int, Tile> tile in GameController.Instance.GetBoardDictionary().board)
                                {
                                    tile.Value.SetPut(true);
                                }

                                GameController.Instance.NewTurn();///zapisanie tablicy do listy
                                objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
                                placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
                                GameController.Instance.GetPlayer().SaveListToCopy();
                                placementSystem.StopPlacement();
                                GameController.Instance.gameTurnManager.ChangeTurn();
                            }
                            else
                            {
                                Debug.Log("W pierwszej turze nale¿y wy³o¿yc sumê conajmniej = 30");
                                ShowNapis("W pierwszej turze nale¿y wy³o¿yæ sumê conajmniej 30 lub wiêcej");
                                return;
                            }
                        }
                        else
                        {
                            if (GameController.Instance.GetPlayer().GetList().Count < GameController.Instance.GetPlayer().GetListCopy().Count)
                            {
                                foreach (KeyValuePair<Vector3Int, Tile> tile in GameController.Instance.GetBoardDictionary().board)
                                {
                                    tile.Value.SetPut(true);
                                }

                                GameController.Instance.NewTurn();///zapisanie tablicy do listy
                                objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
                                placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
                                GameController.Instance.GetPlayer().SaveListToCopy();
                                placementSystem.StopPlacement();
                                GameController.Instance.gameTurnManager.ChangeTurn();
                            }
                            else
                            {
                                Debug.Log("Nale¿y wy³o¿yæ conajmniej jedn¹ p³ytkê");
                                ShowNapis("Nale¿y wy³o¿yæ co najmniej jedn¹ p³ytkê");
                            }
                        }
                    }
                    else GameController.Instance.EndGame();
                }
                else { Debug.Log("Nie poprawnie zakoñczona mapa");
                    ShowNapis("Mapa zosta³a niepoprawnie zakoñczona");
                }

            }
            else Debug.Log("Na mapie nie ma ¿adnych p³ytek");

            //Debug.Log("GridData ma " + placementSystem.GetGridData().GetDictionary().Count + " obiektów");
        }
        else {
            Debug.LogError("NewTurn - problem z game instance");
            //return;

            
        }
    }
    /// <summary>
    /// Sprawdza ci¹gi p³ytek na mapie, jeœli jakieœ s¹
    /// sprawdza czy ka¿dy ci¹g ma conajmniej 3 lub wiêcej p³ytek
    /// </summary>
    /// <returns>true jeœli jest poprawnie, conajmniej 3, jeœli nie, zwraca false</returns>
    public bool CheckMap()
    {
            Dictionary<Vector3Int, Tile> board = GameController.Instance.GetBoardDictionary().board;
            ///grid
            /// przedzia³:
            /// x miêdzy -9 a 8
            /// z miêdzy 2 a -4
            Vector3Int sprawdzanaLokalizacja = new();
            for(int z = minZ;z <= maxZ;z++)
            {
                for(int x = minX; x <= maxX; x++)
                {
                    sprawdzanaLokalizacja = new(x, 0, z);
                    if (board.ContainsKey(sprawdzanaLokalizacja))
                    {
                        int licznik = 0;
                       // Debug.Log("Zawiera p³ytkê - " + sprawdzanaLokalizacja);
                        while(board.ContainsKey(sprawdzanaLokalizacja))
                        {
                            licznik++;
                            x++;
                            sprawdzanaLokalizacja = new(x, 0, z);
                        }
                        if (licznik < 3)  return false; 
                        
                    }
                }
            }
            return true; 
    }
    public void RemoveAllChildren()
    {
         for (int i = this.transform.childCount - 1; i >= 0; i--)
         {
             GameObject child = this.transform.GetChild(i).gameObject;
             Destroy(child);
         }

    }
    public void Reset()
    {
        placementSystem.GetGridData().Reset();
        RemoveAllChildren();
    }
    /// <summary>
    /// funkcja do przywrócenia pozycji przed zmian¹ mapy
    /// </summary>
    public void Restore3DMap()
    {

        for (int i =0;i< objectPlacer.GetplacedGameObjectsCopy().Count;i++)
        {
            if (objectPlacer.GetplacedGameObjectsCopy()[i] == null)
                continue;

            foreach (KeyValuePair<Vector3Int, PlacementData> entry in placementSystem.GetGridData().GetDictionary())
            {
                
                if (entry.Value.PlacedObjectIndex == i)
                {
                    GameObject obj = objectPlacer.GetplacedGameObjectsCopy()[i];
                    obj.transform.position = grid.CellToWorld(entry.Key);
                }
            }
        }
        objectPlacer.RemoveObjectsNotInCopy();
    }
    /// <summary>
    /// przycisk dla u¿ytkownika do anulowania wykonywanych czynnoœci w danej turze
    /// </summary>
    public void CancelMove()
    {
        if (GameController.Instance != null)
        { 
            if(GameController.Instance.GetBoardDictionaryList().Count!=0)
            {
                int index = GameController.Instance.GetBoardDictionaryList().Count - 1;
                
                placementSystem.GetGridData().RestoreCopyDictionary();
                GameController.Instance.GetPlayer().RestoreCopyList();
                GameController.Instance.GetBoardDictionary().SaveDictionary(GameController.Instance.GetBoardDictionaryList()[index].board);
                takeTile.ResetHand(GameController.Instance.GetPlayerHandCopy());
                Restore3DMap();
            }
            else//usuniêcie wszystkich p³ytek i przywrócenie graczowi do rêki
            {
                if (GameController.Instance.GetBoardDictionary().board.Count != 0)
                {
                    objectPlacer.ClearplacedGameObjects();
                    GameController.Instance.GetBoardDictionary().board.Clear();
                    takeTile.ResetHand(GameController.Instance.GetPlayerHandCopy());
                    placementSystem.GetGridData().GetDictionary().Clear();
                    List<Tile> playerHand = GameController.Instance.GetPlayerHand();
                    GameController.Instance.SetActualList(GameController.Instance.GetPlayerHandCopy(), copy: ref playerHand);
                    RemoveAllChildren();
                }
            }
            if (GameController.Instance.GetPlayer().GetFirstTour())
                GameController.Instance.firstTurnController.Reset();

            //takeTile.takeNewTile();
            placementSystem.StopPlacement();
        }
        else Debug.LogError("CancelMove - problem z game instance");
    }

    //funkcja taketile bêdzie dodawa³a kartê graczowi jak i cofa³a wszystkie zmiany na mapie, dla optymalizacji, jesli nie bedzie zmian to nie wydarzy siê nic
    /// <summary>
    /// Funkcja, s³u¿¹ca do dodania nowej karty do talii gracza
    /// funkcja ta koñczy turê
    /// jeœli na mapie zosta³y dokonane zmiany, zostan¹ one cofniête
    /// </summary>
    public void TakeTile()
    {

        if(MapContents())
        {
            takeTile.AddNewToCopy();
            CancelMove();
        }
        else takeTile.takeNewTile();

        // NewTurn();
        GameController.Instance.NewTurn();///zapisanie tablicy do listy
        objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
        placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
        GameController.Instance.GetPlayer().SaveListToCopy();///nadpisanie kopii
        placementSystem.StopPlacement();
        GameController.Instance.gameTurnManager.ChangeTurn();


    }
    /// <summary>
    /// funkcja do sprawdzenia czy mapa na dan¹ turê zosta³a zmieniona czy nie
    /// </summary>
    /// <returns>true dla zmiany, false dla niezmienionej mapy</returns>
    public bool MapContents()
    {
        if (GameController.Instance.GetBoardDictionaryList().Count == 0 && GameController.Instance.GetBoardDictionary().board.Count != 0)
            return true;
        else if(GameController.Instance.GetBoardDictionaryList().Count != 0 &&
            !GameController.Instance.GetBoardDictionary().AreEqual(GameController.Instance.GetBoardDictionaryList()[GameController.Instance.GetBoardDictionaryList().Count - 1].board))
            return true;
        return false;
    }

    public void ShowNapis(String input)
    {
        if (napis != null)
        {
            //napis.gameObject.fi.GetComponent<Text>().text = input;
            napis.gameObject.GetComponentInChildren<Text>().text = input;
                 napis.gameObject.SetActive(true);
               
        }

    }
}
