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

    [SerializeField]
    private List<Tile> tiles; //bank gry
    [SerializeField]
    private List<Tile> playerHand;//talia gracza
    [SerializeField]
    private bool firstPlayersTurn;
    //private List<Tile> mrBot , mrBotCopy;
    // private List<Tile> missBot, missBotCopy;
    //private List<Tile> ComputerPlayer, ComputerPlayerCopy;
    [SerializeField]
    Player player;
    Player mrComputerPlayer;
    Player missComputerPlayer;
    Player ComputerPlayer;

   
    [SerializeField]
    PlacementSystem placementSystem;

    public BoardDictionary boardDictionary;
    [SerializeField]
    List<BoardDictionary> boardList;

    [SerializeField]
    Grid grid;

    public FirstTurnController firstTurnController;

    /// <summary>
    /// inicjuje instancje
    /// </summary>
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
        //Debug.Log("Inicjacja Game Controller");
        SetTiles(ref this.tiles);
        
        boardList = new();
        boardDictionary = new();
        //playerHandCopy = new();
       // mrBot = new();
       // missBot = new();
       // ComputerPlayer = new();
        player = new();
        mrComputerPlayer = new();
        missComputerPlayer = new();
        ComputerPlayer = new();
        player.SetPlayersHand(ref this.tiles);
        mrComputerPlayer.SetPlayersHand(ref this.tiles);
        missComputerPlayer.SetPlayersHand(ref this.tiles);
        ComputerPlayer.SetPlayersHand(ref this.tiles);
        playerHand = player.GetList();
       // firstTurnController = new();

       //this.playerHand.Add(new(30, new UnityEngine.Color(0.5f, 0f, 0.5f), SetName(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 30.ToString(), "$", false));
        //this.playerHand.Add(new(30, UnityEngine.Color.magenta, SetName(UnityEngine.Color.magenta) + "_" + 30.ToString(), "$", false));

        //Vector3Int position = new Vector3Int(0, 0, 0);

    }


    /// <summary>
    /// Funkcja w³¹czaj¹ca siê co now¹ klatkê
    /// </summary>
    void Update()
    {
       // Vector3 position = placementSystem.GetInputManager().GetSelectedMapPosition();
        //Vector3 minRange = grid.CellToWorld(new Vector3Int(-9,0,-4));
       // Vector3 maxRange = grid.CellToWorld(new Vector3Int(8,0,2));
        if (Input.GetKeyDown(KeyCode.D))
            placementSystem.StartRemoving();
        if (Input.GetMouseButtonDown(1) && !placementSystem.GetInputManager().isPointerOverUI())
            placementSystem.StartMowing();
    }
    public bool IsPositionInRange(Vector3 position, Vector3 minRange, Vector3 maxRange)
    {
        return position.x >= minRange.x && position.x <= maxRange.x &&
               position.y >= minRange.y && position.y <= maxRange.y &&
               position.z >= minRange.z && position.z <= maxRange.z;
    }
    /// <summary>
    /// Funkcja do zwrócenia ca³ej listy p³ytek
    /// </summary>
    /// <returns>lista tiles</returns>
    public List<Tile> GetTiles() { return this.tiles; }
    /// <summary>
    /// Funkcja do zwrócenia listy p³ytek na rêce gracza
    /// </summary>
    /// <returns>lista playerHand</returns>
    public List<Tile> GetPlayerHand() { return this.player.GetList(); }
    /// <summary>
    /// Funkcja do uzyskania kopii talii gracza, 
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetPlayerHandCopy(){ return this.player.GetListCopy(); }

    public Player GetPlayer() { return this.player; }
    public Player GetMrComputerPlayer() { return this.mrComputerPlayer; }
    public Player GetMissComputerPlayer() { return this.missComputerPlayer; }
    public Player GetComputerPlayer() { return this.ComputerPlayer; }

    /// <summary>
    /// Getter zmiennej klasowej, boardDictionary
    /// </summary>
    /// <returns></returns>
    public BoardDictionary GetBoardDictionary()
    {
        return this.boardDictionary;
    }
    /// <summary>
    /// Getter zmiennej klasowej, boardList
    /// </summary>
    /// <returns></returns>
    public List<BoardDictionary> GetBoardDictionaryList()
    {
        return this.boardList;
    }

    /// <summary>
    /// Dodanie nowego obiektu do listy boardList
    /// </summary>
    public void NewTurn()
    {
        BoardDictionary tempBoard = new();
        tempBoard.SaveToAnotherDictionary(GetBoardDictionary().board);
        this.boardList.Add(tempBoard);
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

    /// <summary>
    /// Skopiowanie zmiennych z jednej listy do drugiej
    /// </summary>
    /// <param name="original"> lista, z której zmienne s¹ kopiowane</param>
    /// <param name="copy"> lista, do której s¹ kopiowane dane</param>
    public void SetActualList( List<Tile> original, ref List<Tile> copy)
    {
        copy.Clear();
        foreach (Tile tile in original)
        {
            copy.Add(tile);
        }

    }
    /// <summary>
    /// Odzyskanie danych z poprzedniej planszy
    /// </summary>
    public void RestoreBoard()
    {
        if (this.boardDictionary.board.Count != 0)
        {
            this.boardDictionary.board.Clear();
            BoardDictionary bd = boardList[boardList.Count - 1];

            foreach (var kvp in bd.board)
            {
                this.boardDictionary.board[kvp.Key] = kvp.Value;
            }
        }
        else return;
        //boardList
    }

    // public void ResetList()


}


