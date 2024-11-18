using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UIElements;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

public class PlayerAI : Agent
{//MonoBehaviour


    [SerializeField]
    private List<Tile> AIPlayerHand = new();
    [SerializeField]
    private List<Tile> AIPlayerHandCopy = new();
    [SerializeField]
    private bool firstTurn;

    [SerializeField]
    public int myIndex;

    private float elapsedTime = 0;

    [SerializeField]
    private ObjectsDatabase database;
    [SerializeField]
    private ObjectPlacer objectPlacer;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    PlacementSystem placementSystem;

    int maxX = 10;
    int minX = -11;
    int maxZ = 4;
    int minZ = -4;

    void Start()
    {
        //firstTurn = true;
        //Time.timeScale = 0.2f;
        //int beginTileNumber = 14;
        // var actionSpec = ActionSpec.MakeDiscrete(beginTileNumber);
        // SetActionSpec(actionSpec);
    }
    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin();
        firstTurn = true;


    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public override void CollectObservations(VectorSensor sensor)
    {
        var board = GameController.Instance.GetBoardDictionary().board;
        sensor.AddObservation(AIPlayerHand.Count);
        sensor.AddObservation(firstTurn ? 1 : 0);//jeœli true to trzeba mieæ conajmniej 30 na start



        for (int i = 0; i < AIPlayerHand.Count; i++)
        {
            sensor.AddObservation(AIPlayerHand[i].GetNumber());
            sensor.AddObservation(AIPlayerHand[i].GetColor().r);
            sensor.AddObservation(AIPlayerHand[i].GetColor().g);
            sensor.AddObservation(AIPlayerHand[i].GetColor().b);
            sensor.AddObservation(i);
        }

        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (board.ContainsKey(new(x, 0, z)))
                {
                    sensor.AddObservation(board[new(x, 0, z)].GetNumber());
                    sensor.AddObservation(board[new(x, 0, z)].GetColor().r);
                    sensor.AddObservation(board[new(x, 0, z)].GetColor().g);
                    sensor.AddObservation(board[new(x, 0, z)].GetColor().b);
                    sensor.AddObservation(board[new(x, 0, z)].GetPut() ? 1 : 0);
                }
                else
                {
                    //puste pole
                    sensor.AddObservation(0);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0);
                }
                sensor.AddObservation(x); // Pozycja x
                sensor.AddObservation(z); // Pozycja z
            }
        }
        //do obserwacji
        //co ma na rêce
        //ile ma na rêce
        //ile ju¿ jest na mapie p³ytek, jakie i gdzie
        //base.CollectObservations(sensor);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (GameController.Instance.gameTurnManager.currentPlayerId == myIndex)
        {
            int xAction = actions.DiscreteActions[0];
            int zAction = actions.DiscreteActions[1];
            int chosenTileIndex = actions.DiscreteActions[2];

            int xCoord = xAction - 11; //przekszta³cenie na koordynaty x
            int zCoord = zAction - 4; // przekszta³cenie na koordynaty z
            //base.OnActionReceived(actions);
            Debug.Log("x: " + xCoord + " z: " + zCoord);

            //wybór akcji 
            //0. k³adzenie
            //1. usuwanie
            //2. przesuwanie
            //3. pobranie nowej karty
            //4. anulowanie ruchu 
            //5. zakoñczenie tury (wzi¹æ pod uwagê koniec tury przy zakoñczeniu czasu)


            //dodawanie punktów:
            //dodanie p³ytki na mapê w dostêpnym miejscu
            //
            //pierwsza tura tylko ci¹g³e p³ytki p³ytki

            //DiscreteActions[4] DiscreteActions[5] dla nowych pozycji na mapie, w innych momentach nieu¿ywane
        }

    }
    /// <summary>
    /// funkcja do ograniczenia decyzyjnoœci branchu dla agenta
    /// gdy¿ maksymalna iloœæ p³ytek jest konkretna a ai 
    /// </summary>
    /// <param name="actionMask"></param>
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        int branchIndex = 3; // indeks branch do ograniczeñ
        int maxTiles = 64;   // Maksymalna liczba akcji w tej ga³êzi
        int availableTiles = AIPlayerHand.Count; // Liczba dostêpnych p³ytek w rêce agenta

        // Wy³¹czanie akcji powy¿ej dostêpnych p³ytek
        for (int i = availableTiles; i < maxTiles; i++)
        {
            actionMask.SetActionEnabled(branchIndex, i, false); // Wy³¹czanie akcji
        }
    }

    public List<Tile> GetList() { return this.AIPlayerHand; }
    public List<Tile> GetListCopy() { return this.AIPlayerHandCopy; }
    public bool GetFirstTour() { return this.firstTurn; }
    public void EndFirstTour() { this.firstTurn = false; }

    public void AddNewTile(ref List<Tile> tiles)
    {
        if (tiles.Count != 0)
        {
            int TileIndex = Random.Range(0, (tiles.Count));
            AIPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
            SaveListToCopy();
            //countJoker = CountJoker(AIPlayerHand);
        }
        else GameController.Instance.EndGame();
    }

    public void SaveListToCopy()
    {
        AIPlayerHandCopy.Clear();
        foreach (Tile tile in AIPlayerHand)
        {
            AIPlayerHandCopy.Add(tile);
        }

    }

    public void SetPlayersHand(ref List<Tile> tiles, int idx)
    {
        // playerHand = new ();
        int TileIndex;
        for (int i = 0; i < 14; i++)
        {
            TileIndex = Random.Range(0, (tiles.Count));
            tiles[TileIndex].ShowTiles();
            AIPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
        }
        //Debug.Log("ile p³ytek jest w klasie player: "+playerHand.Count);
        SaveListToCopy();
        this.myIndex = idx;
        //countJoker = CountJoker(computerPlayerHand);
        //Debug.Log("ile p³ytek-kopii jest w klasie player: " + playerHandCopy.Count);
    }
    /// <summary>
    /// funkcja do sprawdzenia czy po³o¿enie p³ytki w wybranym miejscu jest poprawne
    /// jeœli tak to daæ nagrode
    /// jesli nie to ukaraæ
    /// </summary>
    /// <param name="x">wybrana przez agenta wartoœæ x</param>
    /// <param name="z">wybrana przez agenta wartoœæ z</param>
    /// <param name="indeks">indeks p³ytki w tali agenta do po³o¿enia</param>
    void PutTileAction(int x, int z, int indeks)
    {
        //sprawdziæ czy mo¿na po³o¿yæ
    }
    /// <summary>
    /// Funkcja do przesuwania p³ytek
    /// </summary>
    /// <param name="oldX">stara pozycja p³ytki x</param>
    /// <param name="oldZ">stara pozycja p³ytki z</param>
    /// <param name="newX">nowa pozycja x dla p³ytki</param>
    /// <param name="newZ">nowa pozycja z dla p³ytki</param>
    void MoveTileAction(int oldX, int oldZ, int newX, int newZ)
    {
        //sprawdziæ
        //jeœli pierwsza tura to wszystkie przesuwane p³ytki musz¹ mieæ bool false
        //sprawdziæ czy nowa pozycja jest zajêta

    }
    /// <summary>
    /// funkcja do usuniêcia p³ytki z wybranej przez agenta pozycji
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void RemoveTileAction(int x, int z)
    {//p³ytka nie zawsze mo¿e byæ usuniêta
     //sprawdziæ czy mo¿e usun¹æ, jeœli nie ukaraæ
     //jeœli tak to daæ mniejsz¹ karê bo dodaje to p³ytki do talii

    }

    void UndoAction()
    {
        //przyznaæ ujemne punkty za anulowanie
        //w zale¿noœci od iloœci wracaj¹cych p³ytek
    }

    void EndTurnAction()
    {
        //sprawdziæ poprawnoœæ mapy
        //zakoñczenie tury jesli jest poprawnie
        //zmiana statusu firstTurn na false jeœli jest true
        //

    }
    void TakeTileAction()
    {
        //nie chcemy by bra³ nowe p³ytki, ujemne punkty
        //anulowanie zmian na mapie jeœli siê pojawi³y
    }
        
    /// <summary>
    /// funkcja do po³o¿enia p³ytki na mapie
    /// </summary>
    /// <param name="gridPosition">pozycja wybrana dla p³ytki</param>
    /// <param name="tile">konkteny obiekt do po³o¿enia na mapie</param>
    void PutTile(Vector3Int gridPosition, Tile tile)
    {//jeszcze sprawdzenia poprawnoœci

        int index = objectPlacer.PlacedObject(database.objectsData[0].Prefab, grid.CellToWorld(gridPosition), ref tile, grid);

        placementSystem.GetGridData().AddObjectAt(gridPosition,
            database.objectsData[0].Size,
            database.objectsData[0].ID,
            index);
        tile.ShowTiles();

        //do usuniêcia postawiona p³ytka
        //Debug.Log("na pozycji:" + gridPosition);
    }
    void moveTile(Vector3Int newGridPosition, Vector3Int oldGridPosition)
    {//jeszcze sprawdzenia poprawnoœci
        Dictionary<Vector3Int, Tile> board = GameController.Instance.GetBoardDictionary().board;

        int selectedObjectIndex = placementSystem.GetGridData().getRepresentationIndex(oldGridPosition);
        placementSystem.GetGridData().MoveObjectAt(newGridPosition, oldGridPosition, database.objectsData[0].Size);
        objectPlacer.MoveObjectTo(selectedObjectIndex, grid.CellToWorld(newGridPosition));
        GameController.Instance.GetBoardDictionary().MoveObjectAt(newGridPosition, oldGridPosition);
    }
    void removeTile(Vector3Int gridPosition)
    {
        //jeszcze sprawdzenia poprawnoœci
        //oraz dodanie p³ytki spowrotem do deku
        int gameObjectIndex = placementSystem.GetGridData().getRepresentationIndex(gridPosition);
        placementSystem.GetGridData().RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(gameObjectIndex);
        GameController.Instance.GetBoardDictionary().board.Remove(gridPosition);
    }

    //sprawdzanie mapy i funkcje undo
}
