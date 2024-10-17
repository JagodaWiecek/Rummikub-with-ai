using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TakeTile : MonoBehaviour
{

    public GameObject tilePrefab;
    public Transform parentTransform;
    GameObject newTile = null;

    [SerializeField]
    PlacementSystem ps;


    // Start is called before the first frame update
    void Start()
    {
        //tilePrefab = Resources.Load<GameObject>("Assets/Tile_2D_v2.prefab");
        //Debug.Log("inicjalizacja TakeTile");
        //Debug.Log($"TakeTile Game prefab is : {tilePrefab}");
        SetStartTile();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartTile()
    {
        // tilePrefab = Resources.Load<GameObject>("Assets/Tile_2D_v2.prefab");
        if (GameController.Instance != null && this.tilePrefab != null)//&& this.tilePrefab !=null
        {
            if (GameController.Instance.GetTiles().Count != 0 && GameController.Instance.GetPlayerHand().Count != 0)// GetPlayerHand()
            {
                List<Tile> hand = GameController.Instance.GetPlayerHand();
                //Debug.Log("Inicjacja TakeTile z GameControllerem");
               // while (tilePrefab == null)
               // {

               // }
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
        Debug.Log($"GetTileToHand: {this.newTile}");
        tile.ShowTiles();
        Debug.Log(tilePrefab);
        newTile = Instantiate(this.tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);

        newTile.transform.SetParent(this.transform);//ustawienie hierarchi
        newTile.GetComponent<Tile>().setNumer(tile.getNumber());//ustawienie numeru klasy
        newTile.GetComponent<Tile>().setColor(tile.GetColor());//ustawienie koloru klasy
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

        button.onClick.AddListener(() => OnButtonClick(tile));
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
            newTile.GetComponent<Tile>().setNumer(tiles[TileIndex].getNumber());//ustawienie numeru klasy
            newTile.GetComponent<Tile>().setColor(tiles[TileIndex].GetColor());//ustawienie koloru klasy
            newTile.name = tiles[TileIndex].getTilename();//ustawienie nazwy w hierarchi
            TextMeshProUGUI textComponent = newTile.transform.Find("Object/Number_Color").GetComponent<TextMeshProUGUI>();//odwo³anie siê do dziecka objektu
            textComponent.text = tiles[TileIndex].getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
            textComponent.color = tiles[TileIndex].GetColor(); //ustawienie koloru dla symbolu

            Tile tile = new Tile(tiles[TileIndex].getNumber(), tiles[TileIndex].GetColor(), tiles[TileIndex].getTilename(), tiles[TileIndex].getSymbol(), tiles[TileIndex].GetPut());
            Button button = newTile.GetComponentInChildren<Button>();
            if (button == null)
            {
                Debug.LogError("Prefab does not contain a Button component!");
            }
            
            button.onClick.AddListener(() => OnButtonClick(tile));
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
    private void OnButtonClick(Tile tile)
    {
        ps.StartPlacement(0, ref tile);
    }




}
