using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Drawing;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;
using System;
/// <summary>
/// G³ówna klasa gry
/// </summary>
public class GameController : MonoBehaviour
{
    // Static instantion, globally available
    public static GameController Instance { get; private set; }

    [SerializeField]
    public int gameIndex;///inne rzeczy zostan¹ za³adowane w zale¿noœci od indeksu

    [SerializeField]
    private List<Tile> mainBank; //bank gry
    [SerializeField]
    private List<Tile> playerHand;//talia gracza
    //[SerializeField]
    //private List<Tile> playerHandCopy;//talia gracza
    //private List<Tile> mrBot , mrBotCopy;
    // private List<Tile> missBot, missBotCopy;
    //private List<Tile> ComputerPlayer, ComputerPlayerCopy;
    [SerializeField]
    Player player;
    [SerializeField]
    ComputerPlayer mrComputerPlayer;
    [SerializeField]
    ComputerPlayer missComputerPlayer;
    [SerializeField]
    ComputerPlayer ComputerPlayer;
    //[SerializeField]
    //PlayerAI AI;


    [SerializeField]
    PlacementSystem placementSystem;

    public BoardDictionary boardDictionary;
    [SerializeField]
    List<BoardDictionary> boardList;

    [SerializeField]
    Grid grid;

    public FirstTurnController firstTurnController;
    public GameTurnManager gameTurnManager;

    [SerializeField]
    GameObject endGameObject;

    [SerializeField]
    GameObject firstPlace;
    [SerializeField]
    GameObject secondPlace;
    [SerializeField]
    GameObject thirdPlace;
    [SerializeField]
    GameObject fourthPlace;

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
        
        endGameObject.SetActive(false);
        SetTiles(ref this.mainBank);
        
        boardList = new();
        boardDictionary = new();
        player = new();

        if (gameIndex == 0)
        {
            player.SetPlayersHand(ref this.mainBank);
            mrComputerPlayer.SetPlayersHand(ref this.mainBank, 1);
            missComputerPlayer.SetPlayersHand(ref this.mainBank, 2);
            ComputerPlayer.SetPlayersHand(ref this.mainBank, 3);
        }
        else if (gameIndex == 1)
        {
            mrComputerPlayer.SetPlayersHand(ref this.mainBank, 1);
            missComputerPlayer.SetPlayersHand(ref this.mainBank, 2);
            ComputerPlayer.SetPlayersHand(ref this.mainBank, 3);
            //ai z indeksem 0
        }
        else if (gameIndex == 2) 
        {
            player.SetPlayersHand(ref this.mainBank);
            mrComputerPlayer.SetPlayersHand(ref this.mainBank, 1);
            missComputerPlayer.SetPlayersHand(ref this.mainBank, 2);
            //ai z indeksem 3
        }
        else if (gameIndex == 3) 
        {
            player.SetPlayersHand(ref this.mainBank);
            //ai z indeksem 1
        }
        else
        {
            player.SetPlayersHand(ref this.mainBank);
        }
        playerHand = player.GetList();
        //playerHandCopy = player.GetListCopy();
       // firstTurnController = new();

        //this.playerHand.Add(new(30, new UnityEngine.Color(0.5f, 0f, 0.5f), SetName(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 30.ToString(), "$", false));
        //this.playerHand.Add(new(30, UnityEngine.Color.magenta, SetName(UnityEngine.Color.magenta) + "_" + 30.ToString(), "$", false));

  

    }


    /// <summary>
    /// Funkcja w³¹czaj¹ca siê co now¹ klatkê
    /// </summary>
    void Update()
    {
        // Vector3 position = placementSystem.GetInputManager().GetSelectedMapPosition();
        //Vector3 minRange = grid.CellToWorld(new Vector3Int(-9,0,-4));
        // Vector3 maxRange = grid.CellToWorld(new Vector3Int(8,0,2));
        if (gameIndex != 1 && gameTurnManager.currentPlayerId ==0)
        {
            if (Input.GetKeyDown(KeyCode.D))
                placementSystem.StartRemoving();
            if (Input.GetMouseButtonDown(1) && !placementSystem.GetInputManager().isPointerOverUI())
                placementSystem.StartMowing();
            if (Input.GetKeyDown(KeyCode.A)) gameTurnManager.ChangeTurn();
        }
        if(Input.GetKeyDown(KeyCode.Z)) Time.timeScale = 0f;
        if(Input.GetKeyDown(KeyCode.X)) Time.timeScale = 1f;
        //Time.timeScale = 0f;
    }

    /// <summary>
    /// Funkcja do zwrócenia ca³ej listy p³ytek
    /// </summary>
    /// <returns>lista tiles</returns>
    public List<Tile> GetGameBank() { return this.mainBank; }
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
    public ComputerPlayer GetMrComputerPlayer() { return this.mrComputerPlayer; }
    public ComputerPlayer GetMissComputerPlayer() { return this.missComputerPlayer; }
    public ComputerPlayer GetComputerPlayer() { return this.ComputerPlayer; }

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
                tiles.Add(new ((temp), UnityEngine.Color.red, SetNameForTile(UnityEngine.Color.red) + "_" + temp.ToString(), temp.ToString(), false));//(int num, Color col, string name)
            }
            for(int i = 0;i < 13; i++)//pomarañczowy
            {
                temp = i + 1;
                tiles.Add(new ((temp), new UnityEngine.Color(1f, 0.50f, 0f), SetNameForTile(new UnityEngine.Color(1f, 0.50f, 0f)) + "_" + temp.ToString(), temp.ToString(), false));
            }
            for (int i = 0; i < 13; i++)//czarny
            {
                temp = i + 1;
                tiles.Add(new ((temp), UnityEngine.Color.black, SetNameForTile(UnityEngine.Color.black) + "_" + temp.ToString(), temp.ToString(), false));
            }
            for (int i = 0; i < 13; i++)//niebieski
            {
                temp = i + 1;
                tiles.Add(new ((temp), UnityEngine.Color.blue, SetNameForTile(UnityEngine.Color.blue) + "_" + temp.ToString(), temp.ToString(), false));
            }
        }
        tiles.Add(new (30, new UnityEngine.Color(0.5f, 0f, 0.5f), SetNameForTile(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 30.ToString(), "$", false));//fiolet
        tiles.Add(new (30, UnityEngine.Color.magenta, SetNameForTile(UnityEngine.Color.magenta) + "_" + 30.ToString(), "$", false));//magenta
    }

    /// <summary>
    /// Zwraca nazwê koloru w zale¿noœci od podanej zmiennej koloru
    /// </summary>
    /// <param name="color">kolor z klasy UnityEngine</param>
    /// <returns>zmienna tekstowa nazwy koloru</returns>
    public string SetNameForTile(UnityEngine.Color color)
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
    /// <summary>
    /// Funkcja do wyœwietlenia ekrany zakoñczenia gry, nie skoñczona
    /// </summary>
    public void EndGame()
    {
        //funkcja oznaczaj¹ca koniec gry
        // string Napis;
        endGameObject.SetActive(true);
        //firstPlace.GetComponent<Text>().text = "";
       // secondPlace.GetComponent<Text>().text = ""; 
        //thirdPlace.GetComponent<Text>().text = ""; 
       // fourthPlace.GetComponent<Text>().text = "";
        if (gameIndex == 0)
        {//gracz i boty
            (string Name, int Score)[] playersScore = {
                (Name: "You", player.FinalScore()),
                (Name: missComputerPlayer.name,Score: missComputerPlayer.FinalScore()),
                (Name: mrComputerPlayer.name,Score: mrComputerPlayer.FinalScore()),
                (Name: ComputerPlayer.name,Score: ComputerPlayer.FinalScore())
            };
            Array.Sort(playersScore, (a, b) => a.Score.CompareTo(b.Score));

            if(playersScore[0].Score == 0)
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name;
            else
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name + " (" + playersScore[0].Score.ToString() + ") ";
            secondPlace.GetComponent<Text>().text = "2. " +playersScore[1].Name+ " ("+ playersScore[1].Score.ToString()+") ";
            thirdPlace.GetComponent<Text>().text = "3. " +playersScore[2].Name + " (" + playersScore[2].Score.ToString() + ") ";
            fourthPlace.GetComponent<Text>().text = "4. " +playersScore[3].Name + " (" + playersScore[3].Score.ToString() + ") ";
        }
        else if (gameIndex == 1)
        { //ai i boty

            (string Name, int Score)[] playersScore = {
                //(Name: AI.name, AI.FinalScore()),
                (Name: missComputerPlayer.name,Score: missComputerPlayer.FinalScore()),
                (Name: mrComputerPlayer.name,Score: mrComputerPlayer.FinalScore()),
                (Name: ComputerPlayer.name,Score: ComputerPlayer.FinalScore())
            };
            Array.Sort(playersScore, (a, b) => a.Score.CompareTo(b.Score));

            if (playersScore[0].Score == 0)
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name;
            else
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name + " (" + playersScore[0].Score.ToString() + ") ";
            secondPlace.GetComponent<Text>().text = "2. " + playersScore[1].Name + " (" + playersScore[1].Score.ToString() + ") ";
            thirdPlace.GetComponent<Text>().text = "3. " + playersScore[2].Name + " (" + playersScore[2].Score.ToString() + ") ";
            fourthPlace.GetComponent<Text>().text = "4. " + playersScore[3].Name + " (" + playersScore[3].Score.ToString() + ") ";
        }
        else if (gameIndex == 2)
        { //gracz, boty i ai
            (string Name, int Score)[] playersScore = {
                (Name: "You", player.FinalScore()),
                (Name: missComputerPlayer.name,Score: missComputerPlayer.FinalScore()),
                (Name: mrComputerPlayer.name,Score: mrComputerPlayer.FinalScore())
                //(Name: AI.name, AI.FinalScore()),
            };
            Array.Sort(playersScore, (a, b) => a.Score.CompareTo(b.Score));

            if (playersScore[0].Score == 0)
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name;
            else
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name + " (" + playersScore[0].Score.ToString() + ") ";
            secondPlace.GetComponent<Text>().text = "2. " + playersScore[1].Name + " (" + playersScore[1].Score.ToString() + ") ";
            thirdPlace.GetComponent<Text>().text = "3. " + playersScore[2].Name + " (" + playersScore[2].Score.ToString() + ") ";
            fourthPlace.GetComponent<Text>().text = "4. " + playersScore[3].Name + " (" + playersScore[3].Score.ToString() + ") ";
        }
        else if (gameIndex == 3)
        { //gracz i ai
            (string Name, int Score)[] playersScore = {
                (Name: "You", player.FinalScore()),
                //(Name: AI.name, AI.FinalScore()),
            };
            Array.Sort(playersScore, (a, b) => a.Score.CompareTo(b.Score));

            if (playersScore[0].Score == 0)
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name;
            else
                firstPlace.GetComponent<Text>().text = "1. " + playersScore[0].Name + " (" + playersScore[0].Score.ToString() + ") ";
            secondPlace.GetComponent<Text>().text = "2. " + playersScore[1].Name + " (" + playersScore[1].Score.ToString() + ") ";

            thirdPlace.GetComponent<Text>().text = "";
            fourthPlace.GetComponent<Text>().text = "";
        }

        Time.timeScale = 0f;
        //przekazanie listy plansz do ai
        //na naukê
        
    }


}


