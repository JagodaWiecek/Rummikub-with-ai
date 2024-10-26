using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewTurn()
    {
        if(GameController.Instance != null)
        {
            if (GameController.Instance.GetBoardDictionary().board.Count != 0)
            {
                /*if (objectPlacer.ListAreEqual())
                    Debug.Log("Lista i kopia s¹ takie same");
                else
                    Debug.Log("Lista i kopia NIE s¹ takie same");*/
                foreach (KeyValuePair<Vector3Int, Tile> tile in GameController.Instance.GetBoardDictionary().board)
                {
                    tile.Value.SetPut(true);
                }
                // GameController.Instance.GetBoardDictionary().board
                GameController.Instance.NewTurn();
                objectPlacer.SetPlacedGameObjectsCopy();

               /* if (objectPlacer.ListAreEqual())
                    Debug.Log("Lista i kopia s¹ takie same");
                else
                    Debug.Log("Lista i kopia NIE s¹ takie same");*/
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
    }

    public void CancelMove()
    {
        if (GameController.Instance != null)
        { 
            if(GameController.Instance.GetBoardDictionaryList().Count!=0)
            {//usuniêcie tylko p³ytek których nie by³o w poprzedniej rundzie
                //chyba najlepiej miec kopie poprzedniej planszy, usun¹æ wszystko z planszy i odbudowanie przez kopie
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
}
