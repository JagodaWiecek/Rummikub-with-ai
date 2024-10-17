using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Drawing;
/// <summary>
/// G³ówna klasa gry
/// </summary>
public class GameController : MonoBehaviour
{
    // Static instantion, globally available
    public static GameController Instance { get; private set; }
   // [SerializeField]
    private List<Tile> tiles; //bank gry
    private List<Tile> playerHand;//talia gracza
    private List<Tile> mrBot;
    private List<Tile> missBot;
    private List<Tile> mrAI;

   
    [SerializeField]
    PlacementSystem placementSystem;
    [SerializeField]
    TakeTile takeTile;

    Dictionary<Vector3Int, Tile> board = new();//istotne<x,0,z>

    private float temp =0;

    private void Awake()
    {
        // Only one instance of object
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    /// <summary>
    /// Funkcja, która w³¹cza siê przed pierwszymi klatkami
    /// </summary>
     void Start()
    {
        Debug.Log("Inicjacja Game Controller");
        SetTiles(ref this.tiles);
        //Debug.Log("Bank ma: " + this.tiles.Count + " p³ytek");
        takeTile = new ();
        SetPlayersHand(ref this.tiles, ref this.playerHand);
        SetPlayersHand(ref this.tiles, ref this.mrBot);
        SetPlayersHand(ref this.tiles, ref this.missBot);
        SetPlayersHand(ref this.tiles, ref this.mrAI);

        //takeTile.SetStartTile();

    }

    void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.Q))
            OnQKeyPressed();
    }
    /// <summary>
    /// Funkcja do zwrócenia ca³ej listy p³ytek
    /// </summary>
    /// <returns>lista tiles</returns>
    public List<Tile> GetTiles()
    {
        return this.tiles;
    }
    /// <summary>
    /// Funkcja do zwrócenia listy p³ytek na rêce gracza
    /// </summary>
    /// <returns>lista playerHand</returns>
    public List<Tile> GetPlayerHand()
    {
        return this.playerHand;
    }

    /// <summary>
    /// Dodanie wszystkich p³ytek wed³ug zasad planszówki
    /// </summary>
    /// <param name="tiles">pusta lista do wype³nienia</param>
    void SetTiles(ref List<Tile> tiles)
    {
        tiles = new ();
        int temp;
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 13; i++)///dodanie do banku p³ytek koloru czerwonego
            {
                temp = i + 1;
                tiles.Add(new ((temp), UnityEngine.Color.red, SetName(UnityEngine.Color.red) + "_" + temp.ToString(), temp.ToString(), false));//(int num, Color col, string name)
            }
            for(int i = 0;i < 13; i++)//pomarañczowy
            {
                temp = i + 1;
                tiles.Add(new ((temp), new UnityEngine.Color(1f, 0.50f, 0f), SetName(new UnityEngine.Color(1f, 0.50f, 0f)) + "_" + temp.ToString(), temp.ToString(), false));
            }
            for (int i = 0; i < 13; i++)//czarny
            {
                temp = i + 1;
                tiles.Add(new ((temp), UnityEngine.Color.black, SetName(UnityEngine.Color.black) + "_" + temp.ToString(), temp.ToString(), false));
            }
            for (int i = 0; i < 13; i++)//niebieski
            {
                temp = i + 1;
                tiles.Add(new ((temp), UnityEngine.Color.blue, SetName(UnityEngine.Color.blue) + "_" + temp.ToString(), temp.ToString(), false));
            }
        }
        tiles.Add(new (30, new UnityEngine.Color(0.5f, 0f, 0.5f), SetName(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 30.ToString(), "$", false));//fiolet
        tiles.Add(new (30, UnityEngine.Color.magenta, SetName(UnityEngine.Color.magenta) + "_" + 30.ToString(), "$", false));//magenta
    }

    /// <summary>
    /// Klasa do losowego przyznania kart do listy
    /// </summary>
    /// <param name="tiles">bank p³ytek</param>
    /// <param name="playerHand">talia docelowa</param>
    void SetPlayersHand(ref List<Tile> tiles, ref List<Tile> playerHand)
    {
        playerHand = new ();
        int TileIndex;
        for (int i =0;i< 14; i++)
        {
            TileIndex = Random.Range(0, (tiles.Count));
            playerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
        }
        Debug.Log(playerHand.Count);
    }
    /// <summary>
    /// Zwraca nazwê koloru w zale¿noœci od podanej zmiennej koloru
    /// </summary>
    /// <param name="color">kolor z klasy UnityEngine</param>
    /// <returns>zmienna tekstowa nazwy koloru</returns>
    string SetName(UnityEngine.Color color)
    {
        if (color == UnityEngine.Color.red) return "red";
        if (color == UnityEngine.Color.blue) return "blue";
        if (color == UnityEngine.Color.black) return "black";
        if (color == new UnityEngine.Color(1f, 0.50f, 0f)) return "orange"; // Pomarañczowy
        if (color == new UnityEngine.Color(0.5f, 0f, 0.5f)) return "purple_joker"; // Fioletowy
        if (color == UnityEngine.Color.magenta) return "magenta_joker";

        return "unknown";
        //return "";
    }

    void OnQKeyPressed()
    {
        placementSystem.StartRemoving();
    }

}


