using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Drawing;

public class GameController : MonoBehaviour
{
    // Static instantion, globally available
    public static GameController Instance { get; private set; }
   // [SerializeField]
    private List<Tile> tiles; //bank gry
    private List<Tile> playerHand;//talia gracza
    private List<Tile> MrBot;
    private List<Tile> MissBot;
    private List<Tile> MrAI;

    // Start is called before the first frame update
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


    void Start()
    {
        Debug.Log("Inicjacja Game Controller");
        SetTiles(ref this.tiles);

        Debug.Log("Bank ma: " + this.tiles.Count + " p³ytek");
        TakeTile takeTile = new TakeTile();
        SetPlayersHand(ref this.tiles, ref this.playerHand);
        SetPlayersHand(ref this.tiles, ref this.MrBot);
        SetPlayersHand(ref this.tiles, ref this.MissBot);
        SetPlayersHand(ref this.tiles, ref this.MrAI);
        takeTile.SetStartTile();


    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Tile> GetTiles()
    {
        return this.tiles;
    }
    public List<Tile> GetPlayerHand()
    {
        return this.playerHand;
    }

    /// <summary>
    /// Dodanie wszystkich p³ytek wed³óg zasad planszówki
    /// </summary>
    /// <param name="tiles">pusta lista do wype³nienia</param>
    void SetTiles(ref List<Tile> tiles)
    {
        tiles = new List<Tile>();
        int temp=0;
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 13; i++)///dodanie do banku p³ytek koloru czerwonego
            {
                temp = i + 1;
                tiles.Add(new Tile((temp), UnityEngine.Color.red, setName(UnityEngine.Color.red) + "_" + (temp), (temp).ToString()));//(int num, Color col, string name)
            }
            for(int i = 0;i < 13; i++)//pomarañczowy
            {
                temp = i + 1;
                tiles.Add(new Tile((temp), new UnityEngine.Color(1f, 0.647f, 0f), setName(new UnityEngine.Color(1f, 0.647f, 0f)) + "_" + (temp), (temp).ToString()));
            }
            for (int i = 0; i < 13; i++)//czarny
            {
                temp = i + 1;
                tiles.Add(new Tile((temp), UnityEngine.Color.black, setName(UnityEngine.Color.black) + "_" + (temp), (temp).ToString()));
            }
            for (int i = 0; i < 13; i++)//niebieski
            {
                temp = i + 1;
                tiles.Add(new Tile((temp), UnityEngine.Color.blue, setName(UnityEngine.Color.blue) + "_" + (temp), (temp).ToString()));
            }
        }
        tiles.Add(new Tile(30, new UnityEngine.Color(0.5f, 0f, 0.5f), setName(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 30,"$"));//fiolet
        tiles.Add(new Tile(30, UnityEngine.Color.magenta, setName(UnityEngine.Color.magenta) + "_" + 30, "$"));//magenta
    }

    void SetPlayersHand(ref List<Tile> tiles, ref List<Tile> playerHand)
    {
        for(int i =0;i< 14; i++)
        {
            int TileIndex = Random.Range(0, tiles.Count);
            playerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
        }
        
    }
    //Zwraca nazwê koloru w zale¿noœci od podanej zmiennej koloru
    string setName(UnityEngine.Color color)
    {
        if (color == UnityEngine.Color.red) return "red";
        if (color == UnityEngine.Color.blue) return "blue";
        if (color == UnityEngine.Color.black) return "black";
        if (color == new UnityEngine.Color(1f, 0.647f, 0f)) return "orange"; // Pomarañczowy
        if (color == new UnityEngine.Color(0.5f, 0f, 0.5f)) return "purple_joker"; // Fioletowy
        if (color == UnityEngine.Color.magenta) return "magenta_joker";

        return "unknown";
        //return "";
    }

}


