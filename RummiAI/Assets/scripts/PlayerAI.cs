using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Drawing;
using Unity.VisualScripting;
using System.Linq;
/// <summary>
/// Klasa PlayerIA implementuje metody dla agenta do wykonywania konkretnych funkcjonalnoœci
///i interakcji w œrodowisku. Klasa dziedziczy po interfejsie Agent, która jest
///dostarczana przez bibliotekê ML-Agents
/// </summary>

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

    int maxX = 11;
    int minX = -12;
    int maxZ = 5;
    int minZ = -4;

    float time;
    float currnetTime;

    public RewardData rewards;


    void Start()
    {
        
        switch (GameController.Instance.learningStep)
        {
            case 0:
                firstTurn = true;
                break;
            case 1:
                firstTurn = false;
                break;
            default:
                firstTurn = true;
                break;

        }

        time = 1f;
        currnetTime = time;
    }
    public override void OnEpisodeBegin()
    {
        switch (GameController.Instance.learningStep)
        {
            case 0:
                firstTurn = true;
                break;
            case 1:
                firstTurn = false;
                break;
            default:
                firstTurn = true;
                break;

        }
  

    }

    public override void CollectObservations(VectorSensor sensor)//, GridSensor)
    {
        var board = GameController.Instance.GetBoardDictionary().board;
        sensor.AddObservation(AIPlayerHand.Count/64); //ile p³ytek ma ai
        sensor.AddObservation(GameController.Instance.gameTurnManager.currentTurnTime/ GameController.Instance.gameTurnManager.turnTime);//obserwacja czasu tury
        sensor.AddObservation(firstTurn ? 1 : 0);//jeœli true to trzeba mieæ conajmniej 30 na start

        var allPlayers = GameController.Instance.GetAllPlayers();
        foreach (var player in allPlayers) 
        {
            sensor.AddObservation(player.tileAmount/64);
            sensor.AddObservation(player.firstTurn ? 1f : 0f);
        }
        float totalPlayers = GameController.Instance.GetAllPlayers().Count;
        sensor.AddObservation(GameController.Instance.gameTurnManager.currentPlayerId/(GameController.Instance.gameTurnManager.currentPlayerId/(GameController.Instance.GetAllPlayers().Count+1)));
        //obserwacje kolorów, numerów i indeksu p³ytki
        for (int i = 0; i < 64; i++)
        {
            //AIPlayerHand.Count
            if (i < AIPlayerHand.Count)
            {
                sensor.AddObservation(GetNormalizedNumber(AIPlayerHand[i].GetNumber()));
                sensor.AddObservation(GetNormalizedColor(AIPlayerHand[i].GetColor()));
                sensor.AddObservation(i);
            }
            else sensor.AddObservation(0f);
            //sensor.AddObservation(AIPlayerHand[i].GetColor().r);
            //sensor.AddObservation(AIPlayerHand[i].GetColor().g);
            //sensor.AddObservation(AIPlayerHand[i].GetColor().b);

        }
        //obserwacja mapy, ka¿dego koloru i numeru p³ytki, i zaznaczenie jej jako pustej jeœli nie ma p³ytki
        //float width = maxX - minX;
        //float height = maxZ - minZ;
        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                //sensor.AddObservation((x - minX) / width); // Pozycja x
                //sensor.AddObservation((z - minZ) / height); // Pozycja z
                //sensor.AddObservation(x); // Pozycja x
                //sensor.AddObservation(z);

                if (board.ContainsKey(new(x, 0, z)))
                {
                    sensor.AddObservation(GetNormalizedNumber(board[new(x, 0, z)].GetNumber()));
                    sensor.AddObservation(GetNormalizedColor(board[new(x, 0, z)].GetColor()));
                    sensor.AddObservation(board[new(x, 0, z)].GetPut() ? 1 : 0);
                    //sensor.AddObservation(board[new(x, 0, z)].GetColor().r);
                    //sensor.AddObservation(board[new(x, 0, z)].GetColor().g);
                    //sensor.AddObservation(board[new(x, 0, z)].GetColor().b);
                }
                else
                {
                    //puste pole
                    sensor.AddObservation(0f);
                    //sensor.AddObservation(0f);
                    //sensor.AddObservation(0f);
                    //sensor.AddObservation(0f);
                    //sensor.AddObservation(-1f);
                }
                
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
        if (GameController.Instance.gameTurnManager.currentPlayerId != myIndex) return;
        
            //wybór akcji 
            //0. k³adzenie
            //1. usuwanie
            //2. przesuwanie
            //3. pobranie nowej karty
            //4. anulowanie ruchu 
            //5. zakoñczenie tury (wzi¹æ pod uwagê koniec tury przy zakoñczeniu czasu)

            int xAction = actions.DiscreteActions[0];
            int zAction = actions.DiscreteActions[1];
            int akcjaAgenta = actions.DiscreteActions[2];
            int chosenTileIndex = actions.DiscreteActions[3];
            int xNewAction = actions.DiscreteActions[4];
            int zNewAction = actions.DiscreteActions[5];

            int xCoord = xAction + minX; //przekszta³cenie na koordynaty x
            int zCoord = zAction + minZ; // przekszta³cenie na koordynaty z
            //base.OnActionReceived(actions);
           // Debug.Log("wybrana akcja: "+ actions.DiscreteActions[2]);
            //Debug.Log("id karty: "+ actions.DiscreteActions[3]);
            
            int xNewCoord = xNewAction + minX; //przekszta³cenie na koordynaty x
            int zNewCoord = zNewAction + minZ; // przekszta³cenie na koordynaty z
    

            //currnetTime -= Time.deltaTime;
                //currnetTime = this.time;
                switch (akcjaAgenta)
                {
                    case 0:
                        if (chosenTileIndex < AIPlayerHand.Count)
                        {
                            PutTileAction(xCoord, zCoord, chosenTileIndex);
                        }
                        //Próba po³o¿enia nowej p³ytki gdy ai nie ma ju¿ p³ytek, kara
                        else AddReward(rewards.PNTNT); //TODO
                        Debug.Log("Akcja k³adzenia p³ytki");
                        break; 
                    case 1:
                        RemoveTileAction(xCoord, zCoord);
                        Debug.Log("Akcja usuniêcia p³ytki");
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
                    default:
                        break;
                }
            

            //dodawanie punktów:
            //dodanie p³ytki na mapê w dostêpnym miejscu
            //
            //pierwsza tura tylko ci¹g³e p³ytki 

            //DiscreteActions[4] DiscreteActions[5] dla nowych pozycji na mapie, w innych momentach nieu¿ywane
        

    }
    /// <summary>
    /// funkcja do ograniczenia decyzyjnoœci branchu dla agenta
    /// gdy¿ maksymalna iloœæ p³ytek jest konkretna a ai 
    /// </summary>
    /// <param name="actionMask"></param>
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        // Ga³êzie: 0:X, 1:Z, 2:TypAkcji, 3:P³ytka, 4:NewX, 5:NewZ
        //typ akcji:
        //0: k³adzenie p³ytki
        //1.usuwanie p³ytki
        //2. przesuniêcie p³ytki
        //3. pobranie p³ytki
        //4. anulowanie akcji
        //5. zakoñczenie tury
        
        int maxTiles = 64;   // Maksymalna liczba akcji w tej ga³êzi
        int availableTiles = AIPlayerHand.Count; // Liczba dostêpnych p³ytek w rêce agenta
        var board = GameController.Instance.GetBoardDictionary().board;

        bool isMyTurn = GameController.Instance.gameTurnManager.currentPlayerId == myIndex;
        if (!isMyTurn)
        {
            // Blokujemy wszystkie akcje w ga³êzi nr 2 (Typ Akcji)
            // Zak³adaj¹c, ¿e masz 6 typów akcji (0-5)
            for (int i = 0; i < 5; i++)
            {
                actionMask.SetActionEnabled(2, i, false);
            }
            // W tym momencie model nie mo¿e "wybraæ" niczego, 
            // co mog³oby wywo³aæ logikê w OnActionReceived.
            return;
        }

        // Wy³¹czanie akcji powy¿ej dostêpnych p³ytek
        if (AIPlayerHand.Count > 0)
        {
            for (int i = availableTiles; i < maxTiles; i++)
            {
                actionMask.SetActionEnabled(3, i, false); // Wy³¹czanie akcji
            }
        }
        if (AIPlayerHand.Count == 0) actionMask.SetActionEnabled(2, 0, false); //wy³¹czenie k³adzenia p³ytek gdy nie ma p³ytek w rêku
        
        //nie mo¿na usuwaæ 
        if (AIPlayerHand.Count == AIPlayerHandCopy.Count)
            actionMask.SetActionEnabled(2, 1, false);
            actionMask.SetActionEnabled(2, 5, false);
        if (board.Count == 0)
        {
            actionMask.SetActionEnabled(2, 1, false);
            actionMask.SetActionEnabled(2, 2, false);
        }
        if(firstTurn) actionMask.SetActionEnabled(2, 4, false);

        if (firstTurn && AIPlayerHand.Count == AIPlayerHandCopy.Count) //nie mo¿na przesuwaæ
        {
            actionMask.SetActionEnabled(2, 2, false);
        }
    }

    private float GetNormalizedColor(UnityEngine.Color c)
    {
        if (c == UnityEngine.Color.red) return 0.25f;
        if (c == UnityEngine.Color.blue) return 0.5f;
        if (c == new UnityEngine.Color(1f, 0.50f, 0f)) return 0.75f;
        if (c == UnityEngine.Color.black) return 1f;
        return 0f; //jokery
    }
    private float GetNormalizedNumber(int number)
    {
        return number / 30f;
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
        //Debug.Log("Nowa p³ytka dla agenta");
        if (tiles.Count != 0)
        {
            int TileIndex = Random.Range(0, (tiles.Count));
            AIPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
            SaveListToCopy();
            AddReward(rewards.TNT);//TODO
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
        float penalty = amount *(rewards.TNTPT); //taking back tiles
        AddReward(penalty);//TODO

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
                Vector3Int positionplusdwa = new Vector3Int(x+2, 0, z);
                Vector3Int positionminusjeden = new Vector3Int(x-1, 0, z);
                Vector3Int positionminusdwa = new Vector3Int(x-2, 0, z);
                if (board.ContainsKey(positionplusjeden) && board.ContainsKey(positionminusjeden))
                    AddReward(rewards.PTPBOT); //puting tile properly between other tiles
                else if (board.ContainsKey(positionplusjeden) || board.ContainsKey(positionminusjeden))
                    AddReward(rewards.PTPCTOT); //puting tile properly close to other tile
                else if ((board.ContainsKey(positionplusjeden) && board.ContainsKey(positionplusdwa))|| (board.ContainsKey(positionminusjeden) && board.ContainsKey(positionminusdwa)))
                    AddReward(rewards.PTPOLOROTT); //puting tile properly on left or right of two tiles
                else 
                    AddReward(rewards.PTPOB);//TODO puting tile properly on board
            }
            else AddReward(rewards.PTW);//TODO puting tile wrongly (invalid)
        }
        else AddReward(rewards.PNTNT);//TODO Puting new tile when there are no tiles in hand
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
            AddReward(rewards.MTPTDP);//TODO moving tile to different position that is not its previous position and it is not first turn
        }
        else
        {
            AddReward(rewards.MDDE);//TODO Moving destination doesn't exist
            return;
        }//TODO
        bool placementValidity = CheckPlacementValidity(newPosition, oldPosition, 0, tile);//jest wolne miejsce
        if (placementValidity)
        {
            moveTile(newPosition, oldPosition);
            AddReward(rewards.MTP);//TODO moving tile properly
        }
        else
        {
            AddReward(rewards.MDIW);//TODO Moving destination is wrong
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
                AddReward(rewards.DT);//TODO deliting tile
                AIPlayerHand.Add(GameController.Instance.GetBoardDictionary().board[position].getTile());
                removeTile(position);
            }
            else
            {
                AddReward(rewards.TRTTCNBR);//TODO trying removing tile that can't be removed, it is permanently put
            }
        }
        else AddReward(rewards.DBET);//TODO deleting not existing tile
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

            AddReward(rewards.UA);//TODO undo actions
        }
        else AddReward(rewards.UAWNCWM);//TODO undo actions when no changes were made
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
                    AddReward(rewards.IFTFP);//TODO If first turn is finished properly
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
                    int value = GameController.Instance.firstTurnController.CheckFirstTurnValidityValue(GameController.Instance.GetBoardDictionary().board);
                    float progress = Mathf.Clamp01(value / 30.0f);
                    float basePenalty = rewards.IFTIMW * 0.5f;
                    float varPenalty = rewards.IFTIMW * (1.0f - (progress));
                    AddReward(basePenalty + 0.5f* varPenalty);//TODO If first turn is made wrongly
                                   // Debug.Log("Agent Ÿle wy³o¿y³ siê w pierwszej turze");
                }
            }
            else
            {
                if (AIPlayerHand.Count >= AIPlayerHandCopy.Count)
                {
                    //TakeTileAction();
                    AddReward(rewards.WEOTTSAOT);//TODO When at the end of turn, the agent has the same amount of tile like at the beginning
                    //Debug.Log("Agent nic nie wy³o¿y³");
                }
                else
                {
                    SaveListToCopy();
                    AddReward(rewards.ATAHFT);//TODO after the turn the agent has fewer tiles
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
            AddReward(rewards.MILWAT);//TODO the map is left wrongly after turn
            //Debug.Log("Agent Ÿle zakoñczy³ ture");
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
        AddReward(rewards.TNT);//TODO Taking new tile
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
            SetReward(rewards.winReward);//TODO win
        }
        else
        {
            //int score = FinalScore();
            float penalty = AIPlayerHand.Count * (rewards.PFETIH);

            SetReward(rewards.lossPenalty); //loss
            AddReward(penalty);//TODO penalty for each tile in hand
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
        AddReward(rewards.WAWFMDT);//TODO when the agent won't finish moves during turn
        if (firstTurn) GameController.Instance.firstTurnController.Reset();
          
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

        if(firstTurn) GameController.Instance.firstTurnController.Increment(tile.GetNumber(),gridPosition);
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
