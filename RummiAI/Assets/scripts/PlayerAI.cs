using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UIElements;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine.Tilemaps;
using TreeEditor;
//using System;

public class PlayerAI : Agent
{//MonoBehaviour


    [SerializeField]
    private List<Tile> AIPlayerHand;
    [SerializeField]
    private List<Tile> AIPlayerHandCopy;
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
        firstTurn = true;
        //int beginTileNumber = 14;
        // var actionSpec = ActionSpec.MakeDiscrete(beginTileNumber);
        // SetActionSpec(actionSpec);
    }
    public override void OnEpisodeBegin()
    {
        //base.OnEpisodeBegin();
        

        //Time.timeScale = 0.2f;

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public override void CollectObservations(VectorSensor sensor)
    {
        var board = GameController.Instance.GetBoardDictionary().board;
        sensor.AddObservation(AIPlayerHand.Count);
        sensor.AddObservation(GameController.Instance.gameTurnManager.currentTurnTime);//obserwacja czasu tury
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
            int chosenTileIndex = actions.DiscreteActions[3];

            int xCoord = xAction - 11; //przekszta³cenie na koordynaty x
            int zCoord = zAction - 4; // przekszta³cenie na koordynaty z
            //base.OnActionReceived(actions);
           // Debug.Log("wybrana akcja: "+ actions.DiscreteActions[2]);
            //Debug.Log("id karty: "+ actions.DiscreteActions[3]);
            int xNewAction = actions.DiscreteActions[4];
            int zNewAction = actions.DiscreteActions[5];
            int xNewCoord = xAction - 11; //przekszta³cenie na koordynaty x
            int zNewCoord = zAction - 4; // przekszta³cenie na koordynaty z
            //wybór akcji 
            //0. k³adzenie
            //1. usuwanie
            //2. przesuwanie
            //3. pobranie nowej karty
            //4. anulowanie ruchu 
            //5. zakoñczenie tury (wzi¹æ pod uwagê koniec tury przy zakoñczeniu czasu)

            int akcjaAgenta = actions.DiscreteActions[2];
            switch (akcjaAgenta)
            {
                case 0:
                    if (AIPlayerHand.Count != 0)
                    {
                        PutTileAction(xCoord, zCoord, chosenTileIndex);
                    }
                    Debug.Log("Akcja k³adzenia p³ytki");
                    break; 
                case 1:
                    RemoveTileAction(xCoord, zCoord);
                    Debug.Log("Akcja usiniêcia p³ytki");
                    break;
                case 2:
                    MoveTileAction(xCoord, zCoord, xNewCoord, zNewCoord);
                    Debug.Log("Akcja przesuniêcia p³ytki");
                    break;
                case 3:
                    TakeTileAction();
                    Debug.Log("Akcja pobrania nowej p³ytki");
                    break;
                case 4:
                    UndoAction();
                    Debug.Log("Akcja anulowania wszystkich ruchów");
                    break;
                case 5:
                    EndTurnAction();
                    Debug.Log("Akcja zakoñczenia tury");
                    break;
            }
         

            //dodawanie punktów:
            //dodanie p³ytki na mapê w dostêpnym miejscu
            //
            //pierwsza tura tylko ci¹g³e p³ytki 

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
        if (AIPlayerHand.Count > 0)
        {
            for (int i = availableTiles; i < maxTiles; i++)
            {
                actionMask.SetActionEnabled(branchIndex, i, false); // Wy³¹czanie akcji
            }
        }
    }

    public List<Tile> GetList() { return this.AIPlayerHand; }
    public List<Tile> GetListCopy() { return this.AIPlayerHandCopy; }
    public bool GetFirstTour() { return this.firstTurn; }
    public void EndFirstTour() { this.firstTurn = false; Debug.Log("Agent zakoñczy³ swoj¹ pierwsz¹ turê"); }
    /// <summary>
    /// dodanie nowej p³ytki do tali gracza
    /// </summary>
    /// <param name="tiles"></param>
    public void AddNewTile(ref List<Tile> tiles)
    {
        Debug.Log("Nowa p³ytka dla agenta");
        if (tiles.Count != 0)
        {
            int TileIndex = Random.Range(0, (tiles.Count));
            AIPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
            SaveListToCopy();
            AddReward(-0.5f);//TODO
            //countJoker = CountJoker(AIPlayerHand);
        }
        else GameController.Instance.EndGame();
    }
    /// <summary>
    /// zapisanie nowej kopii
    /// </summary>
    public void SaveListToCopy()
    {
        AIPlayerHandCopy.Clear();
        foreach (Tile tile in AIPlayerHand)
        {
            AIPlayerHandCopy.Add(tile);
        }

    }
    /// <summary>
    /// funkcja do zwrócenia p³ytek do tali agenta
    /// agent dostaje kary w zale¿noci od iloœci przywracanych p³ytek
    /// </summary>
    public void RestoreCopyList()
    {
        float amount = AIPlayerHandCopy.Count - AIPlayerHand.Count;
        AIPlayerHand.Clear();
        foreach (Tile tile in AIPlayerHandCopy)
        {
            AIPlayerHand.Add(tile);
        }
        float reward = amount *(-0.5f);
        AddReward(reward);//TODO

    }

    public void SetPlayersHand(ref List<Tile> tiles, int idx)
    {
        AIPlayerHand = new();
        AIPlayerHandCopy = new();
        // playerHand = new ();
        int TileIndex;
        for (int i = 0; i < 14; i++)
        {
            TileIndex = Random.Range(0, (tiles.Count));
           // tiles[TileIndex].ShowTiles();
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
        var board = GameController.Instance.GetBoardDictionary().board;
        if (AIPlayerHand.Count > 0) { 
            Vector3Int position = new Vector3Int(x, 0, z);
            bool placementValidity = CheckPlacementValidity(position, 0, indeks);

            if (placementValidity)
            {
                PutTile(position, AIPlayerHand[indeks]);
                AIPlayerHand.RemoveAt(indeks);
                Vector3Int positionplusjeden = new Vector3Int(x+1, 0, z);
                Vector3Int positionminusjeden = new Vector3Int(x-1, 0, z);
                if (board.ContainsKey(positionplusjeden) || board.ContainsKey(positionminusjeden))
                    AddReward(4f);
                else 
                    AddReward(2f);//TODO
            }
            else AddReward(-0.1f);//TODO
        }
        else AddReward(-0.05f);//TODO
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex, int index)
    {

        bool placementValidity = placementSystem.GetGridData().CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);//zwraca false jak nie mozna postawiæ
        if (placementValidity && AIPlayerHand[index].CheckTileValidity(gridPosition))
            return true;
        else return false;
        //return tileData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
    private bool CheckPlacementValidity(Vector3Int gridPosition, Vector3Int previousPosition, int selectedObjectIndex, Tile tile)
    {
        bool placementValidity = placementSystem.GetGridData().CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);//zwraca false jak nie mozna postawiæ
        if (placementValidity && tile.CheckMovedTileValidity(gridPosition, previousPosition))//CheckTileValidity
            return true;
        else return false;
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
        Vector3Int oldPosition = new Vector3Int(oldX, 0, oldZ);
        Vector3Int newPosition = new Vector3Int(oldX, 0, oldZ);
        Tile tile;
        if (GameController.Instance.GetBoardDictionary().board.ContainsKey(oldPosition) &&//wybrana pozycja istnieje
            (firstTurn &&
            !GameController.Instance.GetBoardDictionary().board[oldPosition].GetPut()))//w pierwszej turze mo¿na poruszaæ tylko nowo postawionym p³ytkami
        {
            tile = GameController.Instance.GetBoardDictionary().board[oldPosition].getTile();
            AddReward(0.25f);//TODO 
        }
        else
        {
            AddReward(-0.05f);//TODO
            return;
        }//TODO
        bool placementValidity = CheckPlacementValidity(newPosition, oldPosition, 0, tile);//jest wolne miejsce
        if (placementValidity)
        {
            moveTile(newPosition, oldPosition);
            AddReward(0.5f);//TODO
        }
        else
        {
            AddReward(-0.05f);//TODO
            return;
        }
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
        Vector3Int position = new Vector3Int(x, 0, z);

        if(GameController.Instance.GetBoardDictionary().board.ContainsKey(position) )
        {
            //reward dla wykrycia pozycji
            if (!GameController.Instance.GetBoardDictionary().board[position].GetPut())
            {
                AddReward(-0.4f);//TODO
                AIPlayerHand.Add(GameController.Instance.GetBoardDictionary().board[position].getTile());
                removeTile(position);
            }
            else
            {
                AddReward(-0.1f);//TODO
            }
        }
        else AddReward(-0.2f);//TODO
    }

    /// <summary>
    /// funkcja do anulowania porzednich ruchów dla agenta
    /// </summary>
    void UndoAction()
    {
        //przyznaæ ujemne punkty za anulowanie
        //w zale¿noœci od iloœci wracaj¹cych p³ytek
        if (GameController.Instance.gameTurnManager.turnController.MapContents())
        {

            if (GameController.Instance.GetBoardDictionaryList().Count != 0)
            {
                int index = GameController.Instance.GetBoardDictionaryList().Count - 1;
                placementSystem.GetGridData().RestoreCopyDictionary();
                GameController.Instance.GetBoardDictionary().SaveDictionary(GameController.Instance.GetBoardDictionaryList()[index].board);
                RestoreCopyList();
                GameController.Instance.gameTurnManager.turnController.Restore3DMap();
            }
            else
            {
                if (GameController.Instance.GetBoardDictionary().board.Count != 0)
                {
                    objectPlacer.ClearplacedGameObjects();
                    GameController.Instance.GetBoardDictionary().board.Clear();
                    RestoreCopyList();
                    placementSystem.GetGridData().GetDictionary().Clear();
                    GameController.Instance.gameTurnManager.turnController.RemoveAllChildren();
                }
            }
            if(firstTurn) GameController.Instance.firstTurnController.Reset();

            AddReward(-2f);//TODO
        }
        else AddReward(-0.01f);//TODO
    }

    void EndTurnAction()
    {
        //sprawdziæ poprawnoœæ mapy
        //zakoñczenie tury jesli jest poprawnie
        //zmiana statusu firstTurn na false jeœli jest true
        //
        if (GameController.Instance.gameTurnManager.turnController.CheckMap())
        {
            if (firstTurn)
            {
                if (GameController.Instance.firstTurnController.CheckFirstTurnValidity(GameController.Instance.GetBoardDictionary().board))
                {
                    EndFirstTour();
                    AddReward(1f);//TODO
                    SaveListToCopy();
                    GameController.Instance.firstTurnController.Reset();
                    GameController.Instance.gameTurnManager.ChangeTurn();
                    objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
                    placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
                    GameController.Instance.NewTurn();
                    //zapisaæ mapê
                    if(AIPlayerHand.Count==0)
                    {
                        GameController.Instance.EndGame();
                    }
                }
                else
                {
                    AddReward(-0.2f);//TODO
                    Debug.Log("Agent Ÿle wy³o¿y³ siê w pierwszej turze");
                }
            }
            else
            {
                if (AIPlayerHand.Count == AIPlayerHandCopy.Count)
                {
                    //TakeTileAction();
                    AddReward(-0.3f);//TODO
                    Debug.Log("Agent nic nie wy³o¿y³");
                }
                else
                {
                    SaveListToCopy();
                    AddReward(1f);//TODO
                    GameController.Instance.gameTurnManager.ChangeTurn();
                    objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
                    placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
                    GameController.Instance.NewTurn();
                    if (AIPlayerHand.Count == 0)
                    {
                        GameController.Instance.EndGame();
                    }
                }
            }

        }
        else
        {
            AddReward(-0.1f);//TODO
            Debug.Log("Agent Ÿle zakoñczy³ ture");
        }
    }
    void TakeTileAction()
    {
        //nie chcemy by bra³ nowe p³ytki, ujemne punkty
        //anulowanie zmian na mapie jeœli siê pojawi³y
        //undo wy uniwersalne zrobiæ
        UndoAction();
        List<Tile> tiles = GameController.Instance.GetGameBank();
        AddNewTile(ref tiles);
        AddReward(-0.5f);//TODO
        if (firstTurn) GameController.Instance.firstTurnController.Reset();

        GameController.Instance.gameTurnManager.ChangeTurn();
        //undo
    }
    /// <summary>
    /// funkcja do wykonania na koñcu gry
    /// </summary>
    public void EndGame()
    {
        if(AIPlayerHand.Count == 0)
        {
            AddReward(10f);//TODO
        }
        else
        {
            //int score = FinalScore();
            float reward = AIPlayerHand.Count * (-0.3f);
            AddReward(reward);//TODO
            //kary w zale¿noœci od wyniku
        }
        EndEpisode();
        //Reset();
    }

    public void EndOfTime()
    {
        //funkcja undo
        UndoAction();
        List<Tile> tiles = GameController.Instance.GetGameBank();
        AddNewTile(ref tiles);
        AddReward(-1f);//TODO
        if (firstTurn) GameController.Instance.firstTurnController.Reset();
        AddReward(-1f);//TODO
        //dodanie p³ytki
        //restore mapy
        //kara
    }
        
    /// <summary>
    /// funkcja do po³o¿enia p³ytki na mapie
    /// </summary>
    /// <param name="gridPosition">pozycja wybrana dla p³ytki</param>
    /// <param name="tile">konkteny obiekt do po³o¿enia na mapie</param>
    void PutTile(Vector3Int gridPosition, Tile tile)
    {//jeszcze sprawdzenia poprawnoœci

        if(firstTurn) GameController.Instance.firstTurnController.Increment(tile.GetNumber(), grid.WorldToCell(gridPosition));
        int index = objectPlacer.PlacedObject(database.objectsData[0].Prefab, grid.CellToWorld(gridPosition), ref tile, grid);

        placementSystem.GetGridData().AddObjectAt(gridPosition,
            database.objectsData[0].Size,
            database.objectsData[0].ID,
            index);
       // tile.ShowTiles();

        //do usuniêcia postawiona p³ytka
        //Debug.Log("na pozycji:" + gridPosition);
    }
    void moveTile(Vector3Int newGridPosition, Vector3Int oldGridPosition)
    {//jeszcze sprawdzenia poprawnoœci
        Dictionary<Vector3Int, Tile> board = GameController.Instance.GetBoardDictionary().board;
        if (firstTurn)
        {
            GameController.Instance.firstTurnController.ChangePosition(newGridPosition, oldGridPosition);
        }
        int selectedObjectIndex = placementSystem.GetGridData().getRepresentationIndex(oldGridPosition);
        placementSystem.GetGridData().MoveObjectAt(newGridPosition, oldGridPosition, database.objectsData[0].Size);
        objectPlacer.MoveObjectTo(selectedObjectIndex, grid.CellToWorld(newGridPosition));
        GameController.Instance.GetBoardDictionary().MoveObjectAt(newGridPosition, oldGridPosition);

    }
    /// <summary>
    /// funkcja do usuniêcia p³ytki z mapy
    /// </summary>
    /// <param name="gridPosition">pozycja na mapie</param>
    void removeTile(Vector3Int gridPosition)
    {
        //jeszcze sprawdzenia poprawnoœci
        //oraz dodanie p³ytki spowrotem do deku
        int gameObjectIndex = placementSystem.GetGridData().getRepresentationIndex(gridPosition);
        placementSystem.GetGridData().RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(gameObjectIndex);
        if (GameController.Instance.GetPlayer().GetFirstTour())
        {
            GameController.Instance.firstTurnController.Decrease(GameController.Instance.GetBoardDictionary().board[gridPosition].GetNumber(), gridPosition);

        }
        GameController.Instance.GetBoardDictionary().board.Remove(gridPosition);
    }


    public int FinalScore()
    {
        int score = 0;
        foreach (Tile tile in AIPlayerHand)
        {
            score += tile.GetNumber();
        }

        return score;

    }
}
