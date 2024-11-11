using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
/// <summary>
/// Klasa s³u¿¹ca do stworzenia p³ytki jako obiekty 2D na mapie dla gracza
/// </summary>
public class TakeTile : MonoBehaviour
{

    public GameObject tilePrefab;/// szablon obiektu 
   // public Transform parentTransform; //
    GameObject newTile = null;///tworzony obiekt

    [SerializeField]
    PlacementSystem ps;/// referencja do obiektu z hierarchi do tworzenia obiektu 3D
    
    


    ///Funkcja inicjuj¹ca siê jako pierwsza
    void Start()
    {
        //Debug.Log("inicjalizacja TakeTile: "+this.transform);
        //tilesAmount
        //Component[] components = this.transform.parent.parent.Find("Take_tile_button/Take_tile_button").GetComponents<Component>();
        
       // Debug.Log("inicjalizacja TakeTile: " + textComponent.text);
        SetStartTile();///Wyœwietlenie talii gracza na ekran
        SetButtonNumber();


    }

/*
    //Funkcja wykonuj¹ca siê regularnie
    void Update()
    {

    }*/

    /// <summary>
    /// Funkcja do stworzenia talii p³ytek dla gracza na pocz¹tku gry
    /// </summary>
    public void SetStartTile()
    {
        
        if (GameController.Instance != null && this.tilePrefab != null)///Czy g³ówna klasa zosta³a zainicjowana i czy g³ówna talia nie jest pusta
        {
            if (GameController.Instance.GetGameBank().Count != 0 && GameController.Instance.GetPlayerHand().Count != 0)// GetPlayerHand()
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
    public void SortByColors()
    {
        GameController.Instance.GetPlayer().SortByColors();
        ResetHand(GameController.Instance.GetPlayerHand());
    }
    public void SortByNumbers()
    {
        GameController.Instance.GetPlayer().SortByNumbers();
        ResetHand(GameController.Instance.GetPlayerHand());
    }
    /// <summary>
    /// Funkcja do zresetowania zawartoœci talii gracza na ekranie
    /// </summary>
    /// <param name="tiles"> Lista zawieraj¹ca obiekty klasy Tile</param>
    public void ResetHand(List<Tile> tiles)//g³ównie dla kopii
    {
        //this.transform.C
        for (int i = this.transform.childCount - 1; i >= 0; i--) 
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        StartCoroutine(PutTiles(tiles));


    }
    /// <summary>
    /// funkcja do ponownego postawienia p³ytek na bazie listy
    /// </summary>
    /// <param name="tiles">Lista klasy Tiles</param>
    /// <returns>wykonuje siê w korutynie</returns>
    private IEnumerator PutTiles(List<Tile> tiles)
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < tiles.Count; i++)
        {
            Tile tile = tiles[i];
            GetTileToHand(tile);
        }

    }

    /// <summary>
    /// Funkcja do storzenia obiektu w grze
    /// </summary>
    /// <param name="tile">zmienna do ustawienia komponentu obiektu</param>
    public void GetTileToHand(Tile tile)
    {
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

        Button button = newTile.GetComponentInChildren<Button>();
        if (button == null)
        {
            Debug.LogError("Prefab does not contain a Button component!");
        }
        int idx = newTile.transform.GetSiblingIndex();
        button.onClick.AddListener(() => OnButtonClick(tile,ref idx));
        LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku
    }

 
    ///Dodanie karty do rêki gracza
    public void takeNewTile()
    {
        ///odwo³anie do tali w innym skrypcie 
        if (GameController.Instance != null)
        {
            if (GameController.Instance.GetGameBank().Count != 0)
            {
                List<Tile> tiles = GameController.Instance.GetGameBank();//wykonanie referencji
                List<Tile> Hand = GameController.Instance.GetPlayerHand();
                List<Tile> copy = GameController.Instance.GetPlayerHandCopy();
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
                button.onClick.AddListener(() => OnButtonClick(tile, ref idx));//idx
                Hand.Add(tile);
                copy.Add(tile);
                //Debug.Log($"W take tile jest {GameController.Instance.GetPlayerHand().Count} p³ytek");
                tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
                SetButtonNumber();
                LayoutElement le = newTile.AddComponent<LayoutElement>();//dodanie objektu do widoku

            }
            else GameController.Instance.EndGame();
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
    /// <param name="idx"> jest to indeks obiektu w hierarchi wy odwo³aæ siê do odpowiedniego obiektu po naciœniêciu przycisku</param>
    public void OnButtonClick(Tile tile,ref int idx)//
    {
        ps.StartPlacement(0, ref tile, ref idx);
    }

    public void AddNewToCopy()
    {
        if (GameController.Instance != null && GameController.Instance.GetGameBank().Count != 0)
        {
            List<Tile> tiles = GameController.Instance.GetGameBank();//wykonanie referencji
            List<Tile> copy = GameController.Instance.GetPlayerHandCopy();
            int TileIndex = Random.Range(0, tiles.Count);

            Tile tile = new Tile(tiles[TileIndex].GetNumber(), tiles[TileIndex].GetColor(), tiles[TileIndex].GetTilename(), tiles[TileIndex].GetSymbol(), tiles[TileIndex].GetPut());

            copy.Add(tile);

            tiles.RemoveAt(TileIndex);//usuniêcie p³ytki z g³ównego banku
            SetButtonNumber();

        }
        else
        {
            Debug.LogError("GameController nie jest zainicjowany b¹dŸ bank jest pusty");
        }
    }
    /// <summary>
    /// Funkcja do aktualizacji napisu na przycisku
    /// napis informuje u¿ytkownika ile jest p³ytek w banku gry
    /// numer zmniejsza siê po ka¿dym pobraniu karty
    /// </summary>
    public void SetButtonNumber()
    {
        if (GameController.Instance != null)
        {
            Text textComponent = this.transform.parent.parent.Find("Take_tile_button/Title").GetComponent<Text>();
            int liczba = GameController.Instance.GetGameBank().Count;
            textComponent.text = "Take a tile (" + liczba + ")";
        }
        else Debug.LogError("Problem z Game Instance");
    }

    public void DisableAllButtons()
    {
        Transform parentTransform = this.transform;
        foreach (Transform child in parentTransform)
        {
            //if (child.GetComponent<Button>()
            Button button = child.GetComponentInChildren<Button>();
            if (button != null)
            {
                // Wy³¹czamy komponent Button
                button.interactable = false;
            }
        }
    }
    public void EnableAllButtons()
    {
        Transform parentTransform = this.transform;
        foreach (Transform child in parentTransform)
        {
            Button button = child.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }

}
