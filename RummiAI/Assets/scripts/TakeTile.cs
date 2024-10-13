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
    //private Tile tile = null;


    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartTile()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.GetTiles().Count != 0)
                Debug.Log("Inicjacja TakeTile z GameControllerem");
            else Debug.Log("Inicjacja TakeTile bez GameControllera");

        }
        else Debug.Log("Nie zainicjowamy GameController w TakeTile");
    }

    //Dodanie karty do rêki gracza
    public void takeNewTile()
    {
        ///odwo³anie do tali w innym skrypcie
        if (GameController.Instance != null && GameController.Instance.GetTiles().Count!=0)
        {
            List<Tile> tiles = GameController.Instance.GetTiles();//wykonanie referencji
           // Debug.Log("1vW banku jest: " + GameController.Instance.GetTiles().Count);
           // Debug.Log("2vW banku jest: " + tiles.Count);
            int TileIndex = Random.Range(0, tiles.Count);
            

           // Debug.Log("1vW banku jest: " + GameController.Instance.GetTiles()[TileIndex].getNumber() +" "+ GameController.Instance.GetTiles()[TileIndex].getSymbol() + " " + GameController.Instance.GetTiles()[TileIndex].getTilename());
           // Debug.Log("2vW banku jest: " + tiles[TileIndex].getNumber() + " " + tiles[TileIndex].getSymbol() + " " + tiles[TileIndex].getTilename());
            // Debug.Log(tiles[1]);
            // Debug.Log(tiles.Count);
            // Debug.Log("Wziêto p³ytkê");
            //po³¹czenie prefab z nowym obiektem
            newTile = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            
            newTile.transform.SetParent(this.transform);//ustawienie hierarchi
            newTile.GetComponent<Tile>().setNumer(tiles[TileIndex].getNumber());//ustawienie numeru klasy
            newTile.GetComponent<Tile>().setColor(tiles[TileIndex].GetColor());//ustawienie koloru klasy
            newTile.name = tiles[TileIndex].getTilename();//ustawienie nazwy w hierarchi
            TextMeshProUGUI textComponent = newTile.transform.Find("Object/Number_Color").GetComponent<TextMeshProUGUI>();//odwo³anie siê do dziecka objektu
            textComponent.text = tiles[TileIndex].getSymbol(); //wpisanie na textmesh symbolu widocnego dla gracza
            textComponent.color = tiles[TileIndex].GetColor(); //ustawienie koloru dla symbolu

            Tile tile = new Tile(tiles[TileIndex].getNumber(), tiles[TileIndex].GetColor(), tiles[TileIndex].getTilename() + "_" + tiles[TileIndex].getNumber(), tiles[TileIndex].getSymbol(), tiles[TileIndex].getPut());
            Button button = newTile.GetComponentInChildren<Button>();
            if (button == null)
            {
                Debug.LogError("Prefab does not contain a Button component!");
            }
            
            button.onClick.AddListener(() => OnButtonClick(tile));
           // Debug.Log("3vW banku jest: " + tiles.Count);
            tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
           // Debug.Log("4vW banku jest: " + GameController.Instance.GetTiles()[TileIndex].getNumber());
            LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku

        }
        else
        {
            Debug.LogError("GameController nie jest zainicjowany b¹dŸ bank jest pusty");
        }
       
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile"> zmienna do przekazania do innej klasy</param>
    private void OnButtonClick(Tile tile)
    {
        ps.StartPlacement(0, ref tile);
    }

}
