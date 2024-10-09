using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakeTile : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform parentTransform;
    GameObject newTile = null;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Dodanie karty do rêki gracza
    public void takeNewTile()
    {
        ///odwo³anie do tali w innym skrypcie
        if (GameController.Instance != null && GameController.Instance.tiles.Count!=0)
        {
            List<Tile> tiles = GameController.Instance.tiles;//wykonanie referencji
            Debug.Log("W banku jest: " + GameController.Instance.tiles.Count);
            int TileIndex = Random.Range(0, tiles.Count);

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
            tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
            LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku
        }
        else
        {
            Debug.LogError("GameController nie jest zainicjowany b¹dŸ bank jest pusty");
        }
       
    }

}
