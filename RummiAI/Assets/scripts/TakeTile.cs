using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TakeTile : MonoBehaviour
{

    public GameObject tilePrefab;//{ get; private set; }
    public Transform parentTransform;
    GameObject newTile = null;

    [SerializeField]
    PlacementSystem ps;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("inicjalizacja TakeTile");
        SetStartTile();///Wyœwietlenie talii gracza na ekran

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetStartTile()
    {
        
        if (GameController.Instance != null && this.tilePrefab != null)///Czy g³ówna klasa zosta³a zainicjowana i czy g³ówna talia nie jest pusta
        {
            if (GameController.Instance.GetTiles().Count != 0 && GameController.Instance.GetPlayerHand().Count != 0)// GetPlayerHand()
            {
                List<Tile> hand = GameController.Instance.GetPlayerHand();

                for(int i = 0;i< hand.Count; i++)
                {
                    //Tile tile = new Tile(hand[i].getNumber(), hand[i].GetColor(), hand[i].getTilename(), hand[i].getSymbol(), hand[i].GetPut()); 
                    Tile tile = hand[i];
                    GetTileToHand( tile);
                }

            }
            else Debug.Log("Inicjacja TakeTile bez GameControllera");

        }
        else Debug.Log("Nie zainicjowamy GameController w TakeTile");
    }

    public void GetTileToHand(Tile tile)
    {
      //  Debug.Log($"GetTileToHand: {this.newTile}");
        //tile.ShowTiles();
       // Debug.Log(tilePrefab);
        newTile = Instantiate(this.tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);

        newTile.transform.SetParent(this.transform);//ustawienie hierarchi
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
        button.onClick.AddListener(() => OnButtonClick(tile,ref idx));
        //tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
        LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku
    }

    ///Dodanie karty do rêki gracza
    public void takeNewTile()
    {
        ///odwo³anie do tali w innym skrypcie
        if (GameController.Instance != null && GameController.Instance.GetTiles().Count!=0)
        {
            List<Tile> tiles = GameController.Instance.GetTiles();//wykonanie referencji
            List<Tile> Hand = GameController.Instance.GetPlayerHand();
            int TileIndex = Random.Range(0, tiles.Count);

            //Debug.Log(tilePrefab);
            ///po³¹czenie prefab z nowym obiektem
            newTile = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            
            newTile.transform.SetParent(this.transform);//ustawienie hierarchi
            newTile.GetComponent<Tile>().setNumer(tiles[TileIndex].GetNumber());//ustawienie numeru klasy
            newTile.GetComponent<Tile>().SetColor(tiles[TileIndex].GetColor());//ustawienie koloru klasy
            newTile.GetComponent<Tile>().SetTilename(tiles[TileIndex].GetTilename());
            newTile.GetComponent<Tile>().SetSymbol(tiles[TileIndex].GetSymbol());
            newTile.name = tiles[TileIndex].GetTilename();//ustawienie nazwy w hierarchi
            TextMeshProUGUI textComponent = newTile.transform.Find("Object/Number_Color").GetComponent<TextMeshProUGUI>();//odwo³anie siê do dziecka objektu
            textComponent.text = tiles[TileIndex].GetSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
            textComponent.color = tiles[TileIndex].GetColor(); //ustawienie koloru dla symbolu

            Tile tile = new Tile(tiles[TileIndex].GetNumber(), tiles[TileIndex].GetColor(), tiles[TileIndex].GetTilename(), tiles[TileIndex].GetSymbol(), tiles[TileIndex].GetPut());
            Button button = newTile.GetComponentInChildren<Button>();
            if (button == null)
            {
                Debug.LogError("Prefab does not contain a Button component!");
            }
            int idx = newTile.transform.GetSiblingIndex();
            button.onClick.AddListener(() => OnButtonClick(tile,ref idx));//idx
            Hand.Add(tile);
            //Debug.Log($"W take tile jest {GameController.Instance.GetPlayerHand().Count} p³ytek");
            tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
            LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku

        }
        else
        {
            Debug.LogError("GameController nie jest zainicjowany b¹dŸ bank jest pusty");
        }
       
    }
    /// <summary>
    /// funkcja do uruchomienia funkcji po klikniêciu na objekt 2d
    /// </summary>
    /// <param name="tile"> zmienna do przekazania do innej klasy</param>
    public void OnButtonClick(Tile tile,ref int idx)//
    {
        ps.StartPlacement(0, ref tile, ref idx);
    }


}
