using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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


    // Start is called before the first frame update
    void Start()
    {
        undoButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.Instance.GetBoardDictionary().board.Count != 0 && GameController.Instance.GetBoardDictionaryList().Count == 0)
        {
            undoButton.gameObject.SetActive(true);
        }
        else if (GameController.Instance.GetBoardDictionaryList().Count !=0 &&
            !GameController.Instance.GetBoardDictionary().AreEqual(GameController.Instance.GetBoardDictionaryList()[GameController.Instance.GetBoardDictionaryList().Count - 1].board))
        {
            undoButton.gameObject.SetActive(true);
        }
        else undoButton.gameObject.SetActive(false);
    }

    public void NewTurn()
    {
        if(GameController.Instance != null)
        {
            if (GameController.Instance.GetBoardDictionary().board.Count != 0)
            {

                foreach (KeyValuePair<Vector3Int, Tile> tile in GameController.Instance.GetBoardDictionary().board)
                {
                    tile.Value.SetPut(true);
                }
               
                GameController.Instance.NewTurn();///zapisanie tablicy do listy
                objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
                placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
                GameController.Instance.GetPlayer().SaveListToCopy();


            }
            else Debug.Log("Na mapie nie ma ¿adnych p³ytek");

            //Debug.Log("GridData ma " + placementSystem.GetGridData().GetDictionary().Count + " obiektów");
        }
        else {
            Debug.LogError("NewTurn - problem z game instance");
            //return;

            
        }
    }

    void RemoveAllChildren()
    {
         for (int i = this.transform.childCount - 1; i >= 0; i--)
         {
             GameObject child = this.transform.GetChild(i).gameObject;
             Destroy(child);
         }
       /* foreach (GameObject obj in objectPlacer.GetplacedGameObjects())
        {
            if (obj != null)
            {
                Destroy(obj); // Usuwamy obiekt ze sceny
            }
        }*/
    }
    void Restore3DMap()
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
    void ShowPrefabs()
    {
        for (int i = 0; i < objectPlacer.GetplacedGameObjects().Count; i++)
        {
            //objectPlacer.GetplacedGameObjects().
        }
    }
    public void CancelMove()
    {
        if (GameController.Instance != null)
        { 
            if(GameController.Instance.GetBoardDictionaryList().Count!=0)
            {//usuniêcie tylko p³ytek których nie by³o w poprzedniej rundzie
                //chyba najlepiej miec kopie poprzedniej planszy, usun¹æ wszystko z planszy i odbudowanie przez kopie
                //objectPlacer
                //GridData
                //PlayerHand
                int index = GameController.Instance.GetBoardDictionaryList().Count - 1;
                
                placementSystem.GetGridData().SaveDictionary();
                GameController.Instance.GetPlayer().RestoreCopyList();
                //RemoveAllChildren();
                GameController.Instance.GetBoardDictionary().SaveDictionary(GameController.Instance.GetBoardDictionaryList()[index].board);
                //List<Tile> playerHand = GameController.Instance.GetPlayerHand();
                takeTile.ResetHand(GameController.Instance.GetPlayerHandCopy());
                //GameController.Instance.SetActualList(GameController.Instance.GetPlayerHandCopy(), copy: ref playerHand);
                //StartCoroutine(Restore3DMap());
                Restore3DMap();
                //objectPlacer.SetPlacedGameObjects();
                //SaveDictionary
            }
            else//usuniêcie wszystkich p³ytek i przywrócenie graczowi do rêki
            {
                if (GameController.Instance.GetBoardDictionary().board.Count != 0)
                {
                    objectPlacer.ClearplacedGameObjects();
                    takeTile.ResetHand(GameController.Instance.GetPlayerHandCopy());
                    placementSystem.GetGridData().GetDictionary().Clear();
                    List<Tile> playerHand = GameController.Instance.GetPlayerHand();
                    GameController.Instance.SetActualList(GameController.Instance.GetPlayerHandCopy(), copy: ref playerHand);
                    RemoveAllChildren();
                }
            }
            Debug.Log("Na planszy jest: " + this.transform.childCount);
            //takeTile.takeNewTile();
        }
        else Debug.LogError("CancelMove - problem z game instance");
    }

    //funkcja taketile bêdzie dodawa³a kartê graczowi jak i cofa³a wszystkie zmiany na mapie, dla optymalizacji, jesli nie bedzie zmian to nie wydarzy siê nic

    public void TakeTile()
    { 
        //sprawdzenie czy mapa jest taka sama
        //Reset mapy i tali gracza, czyli cancelmove
        //nowa tura
        takeTile.takeNewTile();
    }
}
