using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
/// <summary>
/// do curriculum learning aby ustaliæ 
/// </summary>
public struct CurrculumLearningSetupMode
{
    /// <summary>    flag (true/false) Do³o¿enie jednej p³ytki </summary>
    public bool AddOneTile;
    /// <summary>   flag (true/false) Przesuwanie p³ytek na mapie</summary>
    public bool MoveExisting;        
    /// <summary>  flag (true/false) Samodzielne uk³adanie wszystkiego </summary>
    public bool PlaceAllManual;  
    /// <summary>   (3-14)     ile p³ytek agent ma na etap</summary>
    public int tileAmount;
    /// <summary>czy iloœæ p³ytek "tileAmount" bêdzie randomowana czy nie, True jak ma byæ ró¿ne co turê, False jak ma byæ tylko tyle ile jest wyznaczone w tileAmount</summary>
    public bool randomTileAmount;
    /// <summary>  (24-240) (10 linii) ile pól na mapie jest dostêpnych </summary>
    public int boardAvailability;    
    /// <summary>    szansa na jokera, 0 aby nie by³o w ogóle </summary>
    public int jokerChances; 
    /// <summary>    szansa na budowanie konkretnej sekwencji, 100 jest budowanie serii, 0 budowanie grupy, inne wartoœci daje naprzemiennoœæ </summary>
    public int set;
    /// <summary>    zmienna jako iloczyn do modyfikowania nagród poœrednich </summary>
    public float guidanceStrength;
    /// <summary>
    /// zmienna do decydowania czy na planszy s¹ ustawione p³ytki aby agent móg³ interaktowaæ z wiêkszym œrodowiskiem
    /// </summary>
    public bool preparedSeqOnBoard;
    /// <summary>
    /// zmienna wskazujaca ile bêdzie dodatkowych sekwencji gotowych na mapie, to iloœæ maksymalna, pomiêdzy 1 a seqAmount
    /// </summary>
    public int seqAmount;
    /// <summary>
    /// Flaga do okreœlenia czy jest wykorzystana funkcja do "podkradniecia" jednej lub dwóch p³ytek z rêki agenta i po³o¿ona z inn¹ sekwencj¹ na planszy
    /// do nauki modelu o podbieraniu p³ytek aby wy³o¿yæ swoj¹ niepe³n¹ sekwencjê
    /// </summary>
    public bool isSeqToFilch;


}

public struct RewardsCurrculumLearning
{
    /// <summary> zmienna do okreœlenia czy agent przegra³ czy wygra³, True dla wygra³ wiêc set nagroda, False dla przegra³ wiêc set kara </summary>
    public bool isWin;
}

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
    [SerializeField]
    protected int endTurnCount;

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
    int xSize = 24;
    int zSize = 10;

    float time;
    float currnetTime;


    public RewardData rewards;

    //For curriculum learning
    public CurriculumLearningTrainer curriculumLearningTrainer;
    CurrculumLearningSetupMode currculumLearningSetupMode;
    protected int trainingIndex; //index do modyfikowania etapu treningu wewn¹trz sceny 6
    bool[] allowedActions;
    protected HashSet<int> takenSpots;//zmienna do przechowywania zajêtych pozycji w przestrzeni 1d
    RewardsCurrculumLearning rewardsCurrculumLearning;

    void Start()
    {

        if (GameController.Instance.gameIndex == 4 || GameController.Instance.gameIndex == 6)
        {
            firstTurn = false;
        }

        time = 1f;
        currnetTime = time;
        takenSpots = new HashSet<int>();
    }
    public override void Initialize()
    {
        base.Initialize();
       // if (GameController.Instance != null && GameController.Instance.gameIndex == 6) Debug.Log("zainicjowane");
       if(curriculumLearningTrainer != null)
        {
            //Debug.Log("zainicjowane");
            this.trainingIndex = curriculumLearningTrainer.LoadProgress();
        }
        else { this.trainingIndex = 0; }//unused
          
    }
    public override void OnEpisodeBegin()
    {
        //if (GameController.Instance.gameIndex == 6)
        //    this.trainingIndex = curriculumLearningTrainer.LoadProgress();
        if (GameController.Instance.gameIndex == 4 || GameController.Instance.gameIndex == 6)
        {
            firstTurn = false;
        }

        //if (curriculumLearningTrainer != null)
        //{
        //    string myBehaviorName = GetComponent<Unity.MLAgents.Policies.BehaviorParameters>().BehaviorName;
        //}
        //trainingIndex = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("training_index", 0);
        endTurnCount = 0;
        takenSpots = new HashSet<int>();
        //SetupTrainingStage(trainingIndex);
    }

    public override void CollectObservations(VectorSensor sensor)//, GridSensor)
    {
        var board = GameController.Instance.GetBoardDictionary().board;
        sensor.AddObservation(GameController.Instance.gameTurnManager.currentTurnTime / GameController.Instance.gameTurnManager.turnTime);
        sensor.AddObservation(firstTurn ? 1 : 0);

        BufferSensorComponent bufferSensor = GetComponent<BufferSensorComponent>();
        List<Tile> hand = (GameController.Instance.gameIndex == 7)
        ? GameController.Instance.GetCP_AI().GetList(): AIPlayerHand;
        sensor.AddObservation(hand.Count / 64f);

        foreach (var tile in hand) //obserwacje dla buffora
        {
            float[] tileData = new float[6];
            int colorIndex = GetNormalizedColor(tile.GetColor());
            bool isJoker = colorIndex == -1;
            tileData[0] = GetNormalizedNumber(tile.GetNumber());
            tileData[1] = isJoker ? 1f : 0f;
            if (isJoker)
            {
                tileData[2] = 0f;
                tileData[3] = 0f;
                tileData[4] = 0f;
                tileData[5] = 0f;
            }
            else
            {
                tileData[2] = (colorIndex == 0) ? 1f : 0f;
                tileData[3] = (colorIndex == 1) ? 1f : 0f;
                tileData[4] = (colorIndex == 2) ? 1f : 0f;
                tileData[5] = (colorIndex == 3) ? 1f : 0f;
            }

            bufferSensor.AppendObservation(tileData);
        } //bufor

        var allPlayers = GameController.Instance.GetAllPlayers();
        foreach (var player in allPlayers)
        {
            sensor.AddObservation(player.tileAmount / 64f);
            sensor.AddObservation(player.firstTurn ? 1f : 0f);
        } //inni gracze
        float totalPlayers = GameController.Instance.GetAllPlayers().Count + 1;
        sensor.AddObservation((float)GameController.Instance.gameTurnManager.currentPlayerId / totalPlayers);

        for (int z = minZ; z <= maxZ; z++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                var pos = new Vector3Int(x, 0, z); 
                if (board.ContainsKey(pos))
                {
                    var tile = board[pos];
                    int colorIndex = GetNormalizedColor(tile.GetColor());
                    bool isJoker = colorIndex == -1;

                    sensor.AddObservation(1f); //is occupied
                    sensor.AddObservation(GetNormalizedNumber(tile.GetNumber())); //nobmer
                    sensor.AddObservation(isJoker ? 1f : 0f); //joker flag

                    sensor.AddOneHotObservation(colorIndex, 4); // 4 encodingi

                    sensor.AddObservation(tile.GetPut() ? 1f : 0f);// put flag
                }
                else
                {
                    //puste pole
                    sensor.AddObservation(0f); // occupied
                    sensor.AddObservation(0f); // number
                    sensor.AddObservation(0f); // joker

                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);

                    sensor.AddObservation(0f); // put
                }
                
            }
        } //mapa
        //do obserwacji
        //co ma na rêce
        //ile ma na rêce
        //ile ju¿ jest na mapie p³ytek, jakie i gdzie
        //base.CollectObservations(sensor);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (GameController.Instance.gameIndex == 7) return;
        if (GameController.Instance.gameTurnManager.currentPlayerId != myIndex) return;
        AddReward(rewards.SP);
        //wybór akcji 
        //0. k³adzenie
        //1. usuwanie
        //2. przesuwanie
        //3. pobranie nowej karty
        //4. anulowanie ruchu 
        //5. zakoñczenie tury (wzi¹æ pod uwagê koniec tury przy zakoñczeniu czasu)
        //int positionXZ = actions.DiscreteActions[0];
        var (xTarget,zTarget) = From1Dto2D(actions.DiscreteActions[0]);
            //int  = actions.DiscreteActions[1];

        int akcjaAgenta = actions.DiscreteActions[1];
        Debug.Log("Akcja: "+ akcjaAgenta);
        int chosenTileIndex = actions.DiscreteActions[2];

        var (xOld, zOld) = From1Dto2D(actions.DiscreteActions[3]);
        if (GameController.Instance.gameIndex != 6)
        {
            switch (akcjaAgenta)
            {
                case 0:

                    if (chosenTileIndex < AIPlayerHand.Count)
                    {
                        PutTileAction(xTarget, zTarget, chosenTileIndex);
                    }
                    //Próba po³o¿enia nowej p³ytki gdy ai nie ma ju¿ p³ytek, kara
                    else AddReward(rewards.PNTNT); //TODO
                    Debug.Log("Akcja k³adzenia p³ytki");
                    break;
                case 1:
                    RemoveTileAction(xOld, zOld);
                    Debug.Log("Akcja usuniêcia p³ytki");
                    break;
                case 2:
                    MoveTileAction(xTarget, zTarget, xOld, zOld);
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

            if (GameController.Instance.gameTurnManager.turnController.CheckMap())
            {
                AddReward(rewards.SR);
            }
        }
        else
        {
            switch (akcjaAgenta)
            {
                case 0:

                    if (AIPlayerHand.Count > 0)
                        PutTileAction_CurriculumLearning(xTarget, zTarget, chosenTileIndex); // uczniowa funkcja dla cl
                    //else
                    
                    //Próba po³o¿enia nowej p³ytki gdy ai nie ma ju¿ p³ytek, kara
                    //else AddReward(rewards.PNTNT); //TODO
                    Debug.Log("Akcja k³adzenia p³ytki CL");
                    break;
                case 1:
                    RemoveTileAction(xOld, zOld); // uczniowa funkcja dla cl
                    Debug.Log("Akcja usuniêcia p³ytki CL");
                    break;
                case 2:
                    MoveTileAction_CurriculumLearning(xTarget, zTarget, xOld, zOld); // uczniowa funkcja dla cl
                    Debug.Log("Akcja przesuniêcia p³ytki CL");
                    break;
                case 3:
                    TakeTile_CurriculumLearning(); // uczniowa funkcja dla cl
                    Debug.Log("Akcja pobrania nowej p³ytki CL");
                    break;
                case 4:
                    UndoAction(); // uczniowa funkcja dla cl
                    Debug.Log("Akcja anulowania wszystkich ruchów CL");
                    break;
                case 5:
                    EndTurnAction_CurriculumLearning(); // uczniowa funkcja dla cl
                    Debug.Log("Akcja zakoñczenia tury CL");
                    break;
                default:
                    break;
            }
        }

    }
    /// <summary>
    /// funkcja do ograniczenia decyzyjnoœci branchu dla agenta
    /// gdy¿ maksymalna iloœæ p³ytek jest konkretna a ai 
    /// </summary>
    /// <param name="actionMask"></param>
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        // Ga³êzie: 0:XZ, 1:TypAkcji, 2:P³ytka, 3:NewXZ
        // old: 0:XZ, 1:z, 2:TypAkcji, 3:P³ytka, 4:NewX, 5:NewZ
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
        //actionMask.SetActionEnabled(0, 0, false);
        // actionMask.SetActionEnabled(1, 0, false);
        if (AIPlayerHand.Count > 0)
        {
            for (int i = availableTiles; i < maxTiles; i++) // nie mo¿na wybraæ indeksu p³ytki gdy nie ma tylu p³ytek w rêce, max 64, nie mozna dobrac np 18 jeœli agent ma 5 p³ytek
            {
                actionMask.SetActionEnabled(2, i, false); // Wy³¹czanie akcji
            }
        }
        if (board.Count == 0)
        {
            actionMask.SetActionEnabled(1, 1, false); //nie mo¿na usuwaæ
            actionMask.SetActionEnabled(1, 2, false);//nie mo¿na przestawiaæ
        }
        if (AIPlayerHand.Count == AIPlayerHandCopy.Count)
        {
            actionMask.SetActionEnabled(1, 1, false);//nie mo¿na usun¹æ
            actionMask.SetActionEnabled(1, 4, false);//nie mo¿na undo zrobiæ
            
        }
        if (firstTurn && AIPlayerHand.Count == AIPlayerHandCopy.Count) //jeœli mamy pierwsz¹ turê i agent nic nie wy³o¿y³
        {
            actionMask.SetActionEnabled(1, 2, false); //nie mo¿na przesuwaæ
        }
        else if (AIPlayerHand.Count == 0) actionMask.SetActionEnabled(1, 0, false); //wy³¹czenie k³adzenia p³ytek gdy nie ma p³ytek w rêku

        MaskPostions(ref actionMask); //

        if (GameController.Instance.gameIndex != 6)
        {
            //gdy mapa jest pusta

            //if(firstTurn) actionMask.SetActionEnabled(2, 4, false);


            if (AIPlayerHand.Count == AIPlayerHandCopy.Count) actionMask.SetActionEnabled(1, 5, false);//nie mo¿na zakoñczyæ tury
            if (!GameController.Instance.gameTurnManager.turnController.CheckMap()) //jeœli mapa jest zakoñczona niepoprawnie
            {
                actionMask.SetActionEnabled(1, 5, false); //zablokowane koñczenie tury
            }
            //gdy mamy tyle samo p³ytek co na pocz¹tku tury
            
        }
        if(GameController.Instance.gameIndex == 6)
        {
            CurriculumLearningActionMask(ref actionMask);
        }

    }

    protected void MaskPostions(ref IDiscreteActionMask actionMask)
    {
        var board = GameController.Instance.GetBoardDictionary().board;
        for (int z = minZ; z <= maxZ; z++)
            for (int x = minX;x <= maxX; x++)
            {
                if (board.ContainsKey(new Vector3Int(x,0,z)))
                {
                    actionMask.SetActionEnabled(0, From2Dto1D(x, z), false);
                }
                else
                {
                    if(board.Count == 0 && From2Dto1D(x, z) != 0)
                        actionMask.SetActionEnabled(3, From2Dto1D(x, z), false);
                    else if(board.Count != 0)
                        actionMask.SetActionEnabled(3, From2Dto1D(x, z), false);

                }
                    
            }
    }    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        if (GameController.Instance.gameIndex == 7)
        {
            var decisions = GameController.Instance.GetCP_AI().expertDecisions;
            if (decisions.Count > 0)
            {
                var decision = decisions.Dequeue();

                // BEZPIECZNIK: upewnij siê, ¿e wartoœci nie wychodz¹ poza zakres
                discreteActions[0] = Mathf.Clamp(decision.PositionXZ, 0, 239);
                discreteActions[1] = Mathf.Clamp(decision.ActionType, 0, 5);
                discreteActions[2] = Mathf.Clamp(decision.TileIndex, 0, 63);
                discreteActions[3] = Mathf.Clamp(decision.OldPositionXZ, 0, 239);

                Debug.Log($"Nagrywam: " + PrintAction(decision) );
                Debug.Log("Aktywna akcja"); 
            }
            else { Debug.Log("Pusta akcja"); }
        }
    }

    private int GetNormalizedColor(UnityEngine.Color c)
    {
        if (c == UnityEngine.Color.red) return 0;
        if (c == UnityEngine.Color.blue) return 1;
        if (c == new UnityEngine.Color(1f, 0.5f, 0f)) return 2;
        if (c == UnityEngine.Color.black) return 3;
        return -1; // joker
    }
    private float GetNormalizedNumber(int number)
    {
        if (number < 30)
            return (number - 1f) / 12f;
        else return -1;
       
    }

    private int From2Dto1D(int x, int z) 
    {
        int xNorm = x - minX;
        int zNorm = z - minZ;
        int index = (zNorm * xSize) + xNorm;
        return index;
    }
    private (int x, int z) From1Dto2D(int xz)
    {
        int x = xz%xSize;
        int z = xz/xSize;
        x = x + minX;
        z = z+ minZ;

        return (x,z);
    }

    string PrintAction(AIDecision decision)
    {
        string toPrint = "akcja";
        var (x, z) = From1Dto2D(decision.PositionXZ);
        var (newx, newz) = From1Dto2D(decision.OldPositionXZ);
        if (decision.ActionType == 0)
        {
            toPrint += " po³o¿enia p³ytki";// na pozycje x = "+x+", z = "+z ;
        }
        else if(decision.ActionType == 2)
        {
            toPrint += " przesuniêcia p³ytki ";// z pozycji x = " + x + ", z = " + z+ " na pozycje x = "+ newx + ", z = "+ newz;
        }
        else if(decision.ActionType == 3)
        {
            toPrint += " pobrania p³ytki ";
        }
        else if(decision.ActionType == 5)
        {
            toPrint += " nowej tury ";
        }
        return toPrint;
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
    /// <summary>
    /// funkcja do zwrócenia p³ytek do tali agenta
    /// agent dostaje kary w zale¿noci od iloœci przywracanych p³ytek,
    /// ale to jest do curriculum learning, gdzie agent nie dostaje kary
    /// </summary>
    public void RestoreCopyList_CurriculumLearning()
    {
        float amount = AIPlayerHandCopy.Count - AIPlayerHand.Count;
        AIPlayerHand.Clear();
        foreach (Tile tile in AIPlayerHandCopy)
        {
            AIPlayerHand.Add(tile);
        }
        float penalty = amount * (rewards.TNTPT); //taking back tiles
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
                else if ((board.ContainsKey(positionplusjeden) && board.ContainsKey(positionplusdwa)) || (board.ContainsKey(positionminusjeden) && board.ContainsKey(positionminusdwa)))
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
    void MoveTileAction( int newX, int newZ,int oldX, int oldZ)
    {
        Vector3Int oldPosition = new Vector3Int(oldX, 0, oldZ);
        Vector3Int newPosition = new Vector3Int(newX, 0, newZ);
        var board = GameController.Instance.GetBoardDictionary().board;
        if (!board.ContainsKey(oldPosition) || board.ContainsKey(newPosition))
        {
            AddReward(rewards.MDDE);
            return;
        }
        Tile tile;
        if(firstTurn) //jeœli jest pierwsza tura
        {
            if(!GameController.Instance.GetBoardDictionary().board[oldPosition].GetPut()) 
                // gdy p³ytka istnieje na pozycji iii w pierwszej turze p³ytka jest po³o¿ona przez samego agenta, nie mo¿na przesuwaæ p³ytkami przeciwników podczas pierwszej tury
            {
                tile = board[oldPosition].getTile();
                AddReward(rewards.MTPTDP);//TODO moving tile to different position that is not its previous position
            }
            else//p³ytka jest postawiona przez innego gracza, kara powrót
            {
                AddReward(rewards.MDIW);
                return;
            }
        }
        else//jak nie ma pierwszej tury
        {
            tile = board[oldPosition].getTile();
        }
        
        bool placementValidity = CheckPlacementValidity(newPosition, oldPosition, 0, tile);//czy przeniesienie jest poprawne
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
                RestoreCopyList_CurriculumLearning();
                GameController.Instance.gameTurnManager.turnController.Restore3DMap();
            }
            else
            {
                if (GameController.Instance.GetBoardDictionary().board.Count != 0)
                {
                    objectPlacer.ClearplacedGameObjects();
                    GameController.Instance.GetBoardDictionary().board.Clear();
                    RestoreCopyList_CurriculumLearning();
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
           
            
                float handPressurePenalty = AIPlayerHand.Count * rewards.PFETIH;
                AddReward(handPressurePenalty);
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
                        if (AIPlayerHand.Count == 0)
                        {
                            GameController.Instance.EndGame();
                        }
                    }
                    else
                    {
                        int value = GameController.Instance.firstTurnController.CheckFirstTurnValidityValue(GameController.Instance.GetBoardDictionary().board);
                        float progress = Mathf.Clamp01(value / 30.0f);
                        //float basePenalty = rewards.IFTIMW * 0.5f;
                        //float varPenalty = rewards.IFTIMW * (1.0f - (progress));
                        // AddReward(basePenalty + 0.5f* varPenalty);//TODO If first turn is made wrongly
                        // Debug.Log("Agent Ÿle wy³o¿y³ siê w pierwszej turze");
                        float partialReward = (progress * rewards.IFTFP) * 1f;
                        AddReward(rewards.IFTIMW + partialReward);
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
    public int FinalScore()
    {
        int score = 0;
        foreach (Tile tile in AIPlayerHand)
        {
            score += tile.GetNumber();
        }

        return score;

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
        
        AddReward(rewards.TNT);//TODO Taking new tile
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
            float penalty = FinalScore() * (rewards.PFETIH);
            if(GameController.Instance.gameIndex == 7)
            {
                penalty = GameController.Instance.GetCP_AI().FinalScore();
            }
                
            SetReward(rewards.lossPenalty); //loss
            AddReward(penalty);//TODO penalty for each tile in hand
            //kary w zale¿noœci od wyniku
        }
        EndEpisode();   
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
        if (firstTurn)
        {
            GameController.Instance.firstTurnController.Decrease(GameController.Instance.GetBoardDictionary().board[gridPosition].GetNumber(), gridPosition);

        }
        GameController.Instance.GetBoardDictionary().board.Remove(gridPosition);
    }



    public Dictionary<Vector3Int, Tile>  returnAvailableBoard(int positionsAmount)
    {
        Dictionary<Vector3Int, Tile> availablePositions = new Dictionary<Vector3Int, Tile>();

        return availablePositions;
    }

    public void CurriculumLearningActionMask(ref IDiscreteActionMask actionMask)
    {
        //actionMask.SetActionEnabled(1, 3, false);
        for(int i =0;i< allowedActions.Length;i++) //zablokowanie akcji która zosta³a okreœlona false przy ustawieniach
        { //tylko 0 - k³adzenie i 5 - koniec tury dostêpne
            if (!allowedActions[i]) //te co maj¹ false to blokujemy
            {
                actionMask.SetActionEnabled(1, i, false);
            }

            // p³ytki na mapie tylko te dostêpne, na start 24

        }
        for (int i = currculumLearningSetupMode.boardAvailability; i < zSize * xSize; i++) //nie mo¿na wybraæ pozycji która jest ograniczona
        {
            actionMask.SetActionEnabled(0, i, false);
            actionMask.SetActionEnabled(3, i, false);
        }
        if (AIPlayerHand.Count == 0) actionMask.SetActionEnabled(1, 0, false); //nie mo¿na mieæ akcji k³adzenia p³ytki gdy nie ma p³ytek w rêce




    }

    public void PrepareToTrain(ref List<Tile> tiles, int idx)
    {
        PrepareVariables(ref this.allowedActions, ref this.currculumLearningSetupMode);
        SetPlayersHand_TrainingFunction(ref tiles, idx, ref currculumLearningSetupMode.tileAmount, currculumLearningSetupMode.set, currculumLearningSetupMode.jokerChances, currculumLearningSetupMode.randomTileAmount);
        if (currculumLearningSetupMode.AddOneTile) 
            TrainingPutSeq(currculumLearningSetupMode.tileAmount, currculumLearningSetupMode.boardAvailability, AIPlayerHand); //k³adzie sekwencjê na planszê
        else if (currculumLearningSetupMode.MoveExisting) 
            TrainingMoveTile(currculumLearningSetupMode.tileAmount, currculumLearningSetupMode.boardAvailability, AIPlayerHand);

        if (currculumLearningSetupMode.preparedSeqOnBoard) PutingManySeq(ref tiles, currculumLearningSetupMode.seqAmount, currculumLearningSetupMode.tileAmount, currculumLearningSetupMode.boardAvailability);
    }

    /// <summary>
    /// Funkcja do przypisania p³ytek do jednego gracza w trakcie treningu, w zale¿noœci od etapu treningu
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="idx"></param>
    public void SetPlayersHand_TrainingFunction(ref List<Tile> tiles, int idx,ref int tileAmount, int Set, int Joker,bool randomTileAmount)
    {
        AIPlayerHand = new();
        AIPlayerHandCopy = new();
        bool isJoker = Random.Range(0, 100) < Joker;
        bool isSet = Random.Range(0, 100) < Set;
        if (isSet)
            tileAmount = Mathf.Clamp(tileAmount, 3, 13);
        else
            tileAmount = Mathf.Clamp(tileAmount, 3, 4); 
        if (randomTileAmount) 
            tileAmount = Random.Range(3, tileAmount + 1);
            // playerHand = new ();S
            //int TileIndex;
            //int amount = 3

            int normalTilesAmount = isJoker ? tileAmount - 1 : tileAmount;
        UnityEngine.Color[] cols = { UnityEngine.Color.red, new UnityEngine.Color(1f, 0.5f, 0f), UnityEngine.Color.black, UnityEngine.Color.blue };
        if (isSet)
        {
            int randomColorIdx = Random.Range(0, 4);

            UnityEngine.Color selectedCol = cols[randomColorIdx];
            int totalSpan = isJoker ? normalTilesAmount + 1 : normalTilesAmount;
            int maxPossibleStart = 13 - totalSpan + 1;
            int startNum = Random.Range(1, maxPossibleStart + 1);

            for (int i = 0; i < normalTilesAmount; i++)
            {
                int targetNum = startNum + i;
                int foundIndex = tiles.FindIndex(t => t.GetNumber() == targetNum && t.GetColor() == selectedCol);

                if (foundIndex != -1)
                {
                    AIPlayerHand.Add(tiles[foundIndex]);
                    tiles.RemoveAt(foundIndex);
                }
            }
            if (isJoker)
            {
                int joker = tiles.FindIndex(t => t.GetNumber() == 30); // Twoje jokery maj¹ nr 30
                if (joker != -1)
                {
                    AIPlayerHand.Add(tiles[joker]);
                    tiles.RemoveAt(joker);
                }
            }
        }
        else //group
        {
            int targetNum = Random.Range(1, 14);
            List<UnityEngine.Color> availableColors = new List<UnityEngine.Color>(cols);

            for (int i = 0; i < normalTilesAmount; i++)
            {
                int colorIdx = Random.Range(0, availableColors.Count);
                UnityEngine.Color selectedCol = availableColors[colorIdx];

                int foundIndex = tiles.FindIndex(t => t.GetNumber() == targetNum && t.GetColor() == selectedCol);

                if (foundIndex != -1)
                {
                    AIPlayerHand.Add(tiles[foundIndex]);
                    tiles.RemoveAt(foundIndex);
                    availableColors.RemoveAt(colorIdx);
                }
            }
            if (isJoker)
            {
                int joker = tiles.FindIndex(t => t.GetNumber() == 30); // Twoje jokery maj¹ nr 30
                if (joker != -1)
                {
                    AIPlayerHand.Add(tiles[joker]);
                    tiles.RemoveAt(joker);
                }
            }
        }

        //Debug.Log("ile p³ytek jest w klasie player: "+playerHand.Count);
        SaveListToCopy();
        PrintList();
        this.myIndex = idx;
    }
    /// <summary>
    /// Funkcja do debugowania
    /// </summary>
    void PrintList()
    {
        string toPrint = string.Empty;
        foreach (Tile tile in AIPlayerHand)
        {
            toPrint += tile.GetTilename() + " ";
        }
        Debug.Log(toPrint);
    }
    private float DistanceOnBoard(float reward, Dictionary<Vector3Int, Tile> board, Vector3Int position)
    {
        if (board.Count() - 1 <= 0)
            return reward;

        int x = position.x; int y = position.y; int z = position.z;
        float totalDistance = 0f;
        int count = 0;

        foreach (var existingPos in board.Keys)
        {
            if (existingPos == position) continue;
            float dist = Vector3.Distance(position, existingPos);
            totalDistance += dist;
            count++;
        }
        float avgDistance = totalDistance / count;
        float maxInfluenceRange = 10.0f;

        float normalized = Mathf.Clamp01((avgDistance - 1f) / (maxInfluenceRange - 1f));
        float distanceReward = Mathf.Lerp(reward, 0f, normalized);

        Debug.Log("Œrednia nagroda: " + distanceReward);

        return Mathf.Max(0f, distanceReward);
    }
    void PutTileAction_CurriculumLearning(int x, int z, int indeks) //TODOCL wiemy ¿e zawsze bêdzie 
    {
        //sprawdziæ czy mo¿na po³o¿yæ
        var board = GameController.Instance.GetBoardDictionary().board;
        
        
            Vector3Int position = new Vector3Int(x, 0, z);
            bool placementValidity = CheckPlacementValidity(position, 0, indeks);

        if (placementValidity) //poprane k³adzenie
        {
            PutTile(position, AIPlayerHand[indeks]);
            AIPlayerHand.RemoveAt(indeks);

            Vector3Int positionplusjeden = new Vector3Int(x + 1, 0, z);
            Vector3Int positionplusdwa = new Vector3Int(x + 2, 0, z);
            Vector3Int positionminusjeden = new Vector3Int(x - 1, 0, z);
            Vector3Int positionminusdwa = new Vector3Int(x - 2, 0, z);
            if (board.ContainsKey(positionplusjeden) && board.ContainsKey(positionminusjeden))
                AddReward(0.2f); //TODOCL ting tile properly between other tiles
            else if (board.ContainsKey(positionplusjeden) || board.ContainsKey(positionminusjeden))
                AddReward(0.15f); //TODOCL puting tile properly close to other tile
            else if ((board.ContainsKey(positionplusjeden) && board.ContainsKey(positionplusdwa)) || (board.ContainsKey(positionminusjeden) && board.ContainsKey(positionminusdwa))) 
                AddReward(0.2f); //TODOCL puting tile properly on left or right of two tiles
            else
                AddReward(DistanceOnBoard(0.1f, board, position));//TODOCL punktacja odleg³oœciowa, aby agent dawa³ bli¿ej p³ytki


        }
        else AddReward(-0.1f);//TODOCL
        
    }

    void MoveTileAction_CurriculumLearning(int newX, int newZ, int oldX, int oldZ) //TODOCL
    {
        Vector3Int oldPosition = new Vector3Int(oldX, 0, oldZ);
        Vector3Int newPosition = new Vector3Int(newX, 0, newZ);
        var board = GameController.Instance.GetBoardDictionary().board;
        if (!board.ContainsKey(oldPosition) || board.ContainsKey(newPosition))
        {
            AddReward(rewards.MDDE);
            return;
        }
        Tile tile;
        if (firstTurn) //jeœli jest pierwsza tura
        {
            if (!GameController.Instance.GetBoardDictionary().board[oldPosition].GetPut())
            // gdy p³ytka istnieje na pozycji iii w pierwszej turze p³ytka jest po³o¿ona przez samego agenta, nie mo¿na przesuwaæ p³ytkami przeciwników podczas pierwszej tury
            {
                tile = board[oldPosition].getTile();
                AddReward(rewards.MTPTDP);//TODO moving tile to different position that is not its previous position
            }
            else//p³ytka jest postawiona przez innego gracza, kara powrót
            {
                AddReward(rewards.MDIW);
                return;
            }
        }
        else//jak nie ma pierwszej tury
        {
            tile = board[oldPosition].getTile();
        }

        bool placementValidity = CheckPlacementValidity(newPosition, oldPosition, 0, tile);//jest wolne miejsce
        if (placementValidity)
        {
            moveTile(newPosition, oldPosition);
           // AddReward(rewards.MTP);//TODO moving tile properly

            Vector3Int positionplusjeden = new Vector3Int(newX + 1, 0, newZ);
            Vector3Int positionplusdwa = new Vector3Int(newX + 2, 0, newZ);
            Vector3Int positionminusjeden = new Vector3Int(newX - 1, 0, newZ);
            Vector3Int positionminusdwa = new Vector3Int(newX - 2, 0, newZ);
            if (board.ContainsKey(positionplusjeden) && board.ContainsKey(positionminusjeden))
                AddReward(0.2f); //TODOCL puting tile properly between other tiles
            else if (board.ContainsKey(positionplusjeden) || board.ContainsKey(positionminusjeden))
                AddReward(0.15f); //TODOCL puting tile properly close to other tile
            else if ((board.ContainsKey(positionplusjeden) && board.ContainsKey(positionplusdwa)) || (board.ContainsKey(positionminusjeden) && board.ContainsKey(positionminusdwa)))
                AddReward(0.2f); //TODOCL puting tile properly on left or right of two tiles
            else
                AddReward(DistanceOnBoard(0.1f, board, newPosition));//TODOCL punktacja odleg³oœciowa, aby agent dawa³ bli¿ej p³ytki
        }
        else
        {
            AddReward(rewards.MDIW);//TODO Moving destination is wrong
            return;
        }
    }
    void RemoveTileAction_CurriculumLearning(int x, int z) //MAYBE TODO
    {

    }
    /// <summary>
    /// zakoñczenie tury oraz ustalenie flag w trakcie curriculum learning
    /// </summary>
    void EndTurnAction_CurriculumLearning() //TODO
    {
        //jeœli ma tak¹ sam¹ iloœæ p³ytek to nic nie po³o¿y³
        //albo jest przesuwanie, ale to wtedy musi byæ mapa poprawna 
        //i tak mapê trzeba sprawdzic bo jak bêdzie mniej p³ytek w rêce to mapa musi byæ poprawna
        //jak coœ niepoprawne bêdzie to resetujemy mapê i endepisode lokalnie
        //jak poprawne to endgame globalne
        //na pewnym etapie k³adzenie nie mo¿e koñczyæ siê od razu,
        //czy mo¿e zrobiæ jak nie ma w ogóle p³ytek? a jak ma to kontynuacja 
        if (AIPlayerHand.Count == 0 ) //nie ma p³ytek w rêce
        {
            if (GameController.Instance.gameTurnManager.turnController.CheckMap())
            {
                rewardsCurrculumLearning.isWin = true;
                GameController.Instance.EndGame(); //win
            }
            else //mapa jest Ÿle skoñczona
            {
                if (allowedActions[2])//agent mo¿e przesuwaæ p³ytki, kontynuacja nauk ale musi byæ kara
                {
                    AddReward(-0.1f); //kara, mo¿e przesuwaæ a nie robi //TODOCL

                }
                else//agent nie mo¿e przesuwaæ p³ytek, koniec gry, p³ytki wracaj¹ do rêki agenta
                {
                    rewardsCurrculumLearning.isWin = false;
                    EndGame_CurriculumLearning();
                }


            }// nie ma p³ytek ale mapa jest 
        }
        else
        {
            if (GameController.Instance.gameTurnManager.turnController.CheckMap())//mapa zostawiona poprawnie ale s¹ p³ytki
            {
                //ma p³ytki na rêce ale mapa zostawiona dobrze
                //musi byæ kara bo agent powinien zostawiæ wszystkie p³ytki
                AddReward(-0.1f); //TODOCL
            }
            else//s¹ p³ytki w rêce ale mapa niepoprawna
            {
                if (AIPlayerHand.Count < AIPlayerHandCopy.Count)
                {
                    //niepoprawna mapa ale agent zrobi³ akcje po³o¿enia p³ytki
                    //mo¿e jeszcze kontynuowaæ 
                    AddReward(0.1f); //TODOCL
                }
                else 
                {
                    //nie wykona³ ruchu a zakoñczy³ turê, musi byæ kara
                    AddReward(-0.1f); //TODOCL
                }
                
                
            }

        }
        //Do przemyœlenia, jak uczyæ 

        
        //List<Tile> tiles = GameController.Instance.GetGameBank();
        //AddNewTile(ref tiles);
        //AddReward(rewards.TNT);//TODO Taking new tile
        //if (firstTurn) GameController.Instance.firstTurnController.Reset();
                   
        //else AddReward(0);//reward
        //undo
    }
    /// <summary>
    /// funkcja do zakoñczenia gry, tylko gdy gameIndex==6
    /// </summary>
    public void EndGame_CurriculumLearning()
    {
        float winReward = 0f;
        if(rewardsCurrculumLearning.isWin)
        {
            //ju¿ siê reset zrobi
            winReward = 1.0f;
        }
        else
        {
            winReward = -1.0f;
            Revoke_CurriculumLearning();
        }

        SetReward(winReward); //TODOCL
        curriculumLearningTrainer.AddResult(winReward,ref this.trainingIndex);
        EndEpisode();
        //Reset();
    }

    public void TakeTile_CurriculumLearning()
    {
        // Ma³a kara bo nie chcemy by dobiera³ ale czasami musi
        Revoke_CurriculumLearning();
        List<Tile> tiles = GameController.Instance.GetGameBank();
        AddNewTile(ref tiles);
        AddReward(0.01f); //TODOCL
        GameController.Instance.gameTurnManager.ChangeTurn();
    }
    /// <summary>
    /// Funkcja do ustalania zmiennych w konkretnych etapach nauki
    /// </summary>
    /// <param name="availablePositions">ile pozycji na mapie jest dostêpnych</param>
    /// <param name="isJoker">czy mamy jokera czy nie, 0 aby nie by³o, 100 aby by³ zawsze, wartoœæ pomiêdzy aby by³ losowo</param>
    /// <param name="amountOfTiles">ile p³ytek ma mieæ agent w rêce na pocz¹tku ka¿dej gry</param>
    /// <param name="set">czy mamy seriê do po³o¿enia czy grupê, 100 aby seria, 0 aby grupa, wartoœæ pomiêdzy aby losowo pomiêdzy</param>
    void PrepareVariables(ref bool[] allowedActions, ref CurrculumLearningSetupMode currculumLearningSetupMode)
    {
        //czy jest joker
        //ile pozycji na mapie jest dostepnych
        //ile p³ytek w danym momencie ma gracz
        //czy seria czy grupa
        //jakie akcje s¹ dozwolone:
        //  0 k³adzenie p³ytek
        //  1 usuniêcie p³ytki
        //  2 przesuniêcie p³ytki
        //  3 dobranie p³ytki
        //  4 undo
        //  5 koniec tury
        int oneLine = 24; //ile jedna linia mo¿e mieæ, max 10

        if (this.trainingIndex == 0) //dodawanie p³ytek do serii - ju¿ s¹ dwie p³ytki i musi do³o¿yæ 3.
        {
            currculumLearningSetupMode.boardAvailability = oneLine*10;
            currculumLearningSetupMode.jokerChances = 0;
            currculumLearningSetupMode.tileAmount = 3;
            currculumLearningSetupMode.randomTileAmount = false;
            currculumLearningSetupMode.set = 100;
            currculumLearningSetupMode.PlaceAllManual = true; 
            currculumLearningSetupMode.AddOneTile = false;
            currculumLearningSetupMode.MoveExisting = false;
            currculumLearningSetupMode.guidanceStrength = 1.0f;
            currculumLearningSetupMode.preparedSeqOnBoard = false;
            currculumLearningSetupMode.isSeqToFilch = true;
            currculumLearningSetupMode.seqAmount = 10;
            allowedActions = new bool[] { true, false,false,false,false,true };
        }
        if (this.trainingIndex == 1) //k³adzenie ca³ych sekwencji 3
        {
            currculumLearningSetupMode.boardAvailability = oneLine;
            currculumLearningSetupMode.jokerChances = 0;
            currculumLearningSetupMode.tileAmount = 3;
            currculumLearningSetupMode.randomTileAmount = false;
            currculumLearningSetupMode.set = 100;
            currculumLearningSetupMode.PlaceAllManual = true;
            currculumLearningSetupMode.AddOneTile = false;
            currculumLearningSetupMode.MoveExisting = false;
            currculumLearningSetupMode.guidanceStrength = 1.0f;
            currculumLearningSetupMode.preparedSeqOnBoard = false;
            currculumLearningSetupMode.isSeqToFilch = false;
            currculumLearningSetupMode.seqAmount = 5;
            allowedActions = new bool[] { true, false, false, false, false, true };

        }
        if (this.trainingIndex == 2) //to samo ale grupy - dwie p³ytki i dok³adanie jednej
        {
            currculumLearningSetupMode.boardAvailability = oneLine;
            currculumLearningSetupMode.jokerChances = 0;
            currculumLearningSetupMode.tileAmount = 3;
            currculumLearningSetupMode.randomTileAmount = false;
            currculumLearningSetupMode.set = 0;
            currculumLearningSetupMode.PlaceAllManual = false;
            currculumLearningSetupMode.AddOneTile = true;
            currculumLearningSetupMode.MoveExisting = false;
            currculumLearningSetupMode.guidanceStrength = 1.0f;
            currculumLearningSetupMode.preparedSeqOnBoard = false;
            currculumLearningSetupMode.isSeqToFilch = false;
            currculumLearningSetupMode.seqAmount = 5;
            allowedActions = new bool[] { true, false, false, false, false, true };
        }
        if (this.trainingIndex == 3) //to samo ale grupy - dwie p³ytki i dok³adanie jednej
                                     // TYMCZASOWO PRZESUWANIE P£YTEK
        {
            currculumLearningSetupMode.boardAvailability = oneLine;
            currculumLearningSetupMode.jokerChances = 0;
            currculumLearningSetupMode.tileAmount = 3;
            currculumLearningSetupMode.set = 0;
            currculumLearningSetupMode.PlaceAllManual = true;
            currculumLearningSetupMode.AddOneTile = false;
            currculumLearningSetupMode.MoveExisting = false;
            currculumLearningSetupMode.guidanceStrength = 1.0f;
            currculumLearningSetupMode.preparedSeqOnBoard = false;
            currculumLearningSetupMode.seqAmount = 5;
            allowedActions = new bool[] { true, false, false, false, false, true };
        }
    }
    /// <summary>
    /// funkcja do znalezienia pozycji do po³o¿enia p³ytek na mapie dla serii/grupy
    /// </summary>
    /// <param name="tileAmount">ile p³ytek w rêce ma agent</param>
    /// /// <param name="availableBoardAmount">jakie pozycje na mapie s¹ dostêpne, jest to np 24 oznaczaj¹ca ¿e mamy pozycje wolne w przedziale (0-23)</param>
    /// <returns></returns>
    private List<Vector3Int> FreeSpaceToPut(int tileAmount, int availableBoardAmount)
    {
        int safeStart;
        int availableLines;
        do 
        {
            availableLines = AmountOfLines((availableBoardAmount)); //ile mamy dostepnych linii, od 0 do 9 (czyli 10 linii), to zwróci o 1 za du¿o ale pozbywamy siê tego przy z
            int z = Random.Range(0, availableLines); //jak¹ linie ostatecznie bêdziemy zajmowaæ, do zajmowania pozycja z
            var (first, last) = RangePositions(z, availableLines, tileAmount);
            safeStart = Random.Range(first, last + 1); 
            //funkcja musi zwróciæ false gdy jest bezpieczne miejsce znalezione

        } while(freeSpot(safeStart, availableLines));

        //ostateczny wybór, zabezpieczenie przed 
        List <Vector3Int> list = new List<Vector3Int>();
        for (int i = 0; i < tileAmount;i++)
        {   
            var (xPos, zPos) = From1Dto2D(safeStart+i);
            list.Add(new Vector3Int(xPos, 0, zPos));
            takenSpots.Add(safeStart + i);
        }
        return list;
    }
    private (List<Vector3Int>,int) FreeSpaceToMove(int tileAmount, int availableBoardAmount) 
    {
        int safeStart;
        int availableLines;
        do
        {
            availableLines = AmountOfLines((availableBoardAmount)); //ile mamy dostepnych linii, od 0 do 9 (czyli 10 linii), to zwróci o 1 za du¿o ale pozbywamy siê tego przy z
            int z = Random.Range(0, availableLines); //jak¹ linie ostatecznie bêdziemy zajmowaæ, do zajmowania pozycja z
            var (first, last) = RangePositions(z, availableLines, tileAmount);
            safeStart = Random.Range(first, last + 1);
            //funkcja musi zwróciæ false gdy jest bezpieczne miejsce znalezione

        } while (freeSpot(safeStart, tileAmount));

        //ostateczny wybór, zabezpieczenie przed 
        List<Vector3Int> list = new List<Vector3Int>();
        for (int i = 0; i < tileAmount; i++)
        {
            var (xPos, zPos) = From1Dto2D(safeStart + i);
            list.Add(new Vector3Int(xPos, 0, zPos));
            takenSpots.Add(safeStart + i);
        }
        // return list;
        do
        {
            availableLines = AmountOfLines((availableBoardAmount));
            int z = Random.Range(0, availableLines); 
            var (first, last) = RangePositions(z, availableLines, 1);
            safeStart = Random.Range(first, last + 1);

        } while (freeSpot(safeStart, 1));


        return (list, safeStart);
    }

    void TrainingPutSeq(int tileAmount, int availableBoardAmount, List<Tile> sequention)
    {
        //lista p³ytek, dok³adanie jednej, losowanie która p³ytka nie bêdzie po³o¿ona
        //universalne co do iloœci p³ytek dla agenta
        //p³ytki musz¹ mieæ os razu zmienn¹ put na true
        //decyzja która p³ytka zostaje w rêce
        List<Vector3Int> chosenSpace = FreeSpaceToPut(tileAmount, availableBoardAmount);
        int indexOut = Random.Range(0, tileAmount); //wartoœæ do wybrania losowego która p³ytka NIE zostanie po³o¿ona

        for (int i = 0, j = 0; i < chosenSpace.Count ; i++)
        {
            if(i == indexOut)
            {
                j++;
                continue;
            }
            PutTile(chosenSpace[i], sequention[j]);
            AIPlayerHand.RemoveAt(j);
        }
        SaveListToCopy();
        objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
        placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
        GameController.Instance.gameTurnManager.EndTurn();//zmiana wszystkich p³ytek na planszy na put = true

    }

    void TrainingMoveTile(int tileAmount, int availableBoardAmount, List<Tile> sequention)
    {
        //lista p³ytek, przek³adanie p³ytek, tak by by³y od siebie w odleg³oœci 2 
        //universalne co do iloœci p³ytek dla agenta
        //p³ytki musz¹ mieæ os razu zmienn¹ put na true
        //wszystkie p³ytki s¹ po³o¿one na planszy, ale jedna z dala od sekwencji
        var (chosenSpace,singleTile) = FreeSpaceToMove(tileAmount, availableBoardAmount);
        int indexOut = Random.Range(0, tileAmount);
        for (int i = 0, j = 0; i < chosenSpace.Count; i++)
        {
            if (i == indexOut)
            {
                var (x, z) = From1Dto2D(singleTile);
                PutTile(new Vector3Int (x,0,z), sequention[j]);
                AIPlayerHand.RemoveAt(j);
                continue;
            }
            PutTile(chosenSpace[i], sequention[j]);
            AIPlayerHand.RemoveAt(j);
        }
        SaveListToCopy();
        objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
        placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
        GameController.Instance.gameTurnManager.EndTurn();//zmiana wszystkich p³ytek na planszy na put = true
    }

    private int AmountOfLines(int availableBoardAmount)
    {

        return availableBoardAmount/xSize;
    }
    private (int first, int last) RangePositions(int z, int availableBoardAmount, int tileAmount)
    {
        int startOfLine = z * xSize; // 0, 24, 48, 72,96,120,144,168,192,216
        int endOfLine = startOfLine + (xSize - 1); //23, 47, 71, 95, 119, 143, 167, 191, 215, 239
        int first = startOfLine + 1; //1, 25, 49, 73, 97, 121, 145, 169, 193, 217
        int last = endOfLine - tileAmount; //20, 44, 68, 92, 116, 140, 164, 188, 212, 236
        return (first, last);
    }

    bool freeSpot(int potentialStart, int tileAmount)
    {
        //takenSpots
        for (int i = -2; i< (tileAmount+2);i++)
        {
            int indexToCheck = potentialStart + i;
            if (takenSpots.Count <= 0) return false;
            if (takenSpots.Contains(indexToCheck))
            {
                return true; // Znaleziono zajête miejsce / duplikat
            }

        }
        return false;
    }
    /// <summary>
    /// funkcja do cofniêcia akcji wykonanych przez agenta,
    /// bez nadawania kary, s³u¿y do zresetowania akcji agenta gdy zrobi³ coœ Ÿle i zakoñczy³ turê
    /// </summary>
    void Revoke_CurriculumLearning()
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
            if (firstTurn) GameController.Instance.firstTurnController.Reset();

            
        }
        
    }
    /// <summary>
    /// funkcja do "zabrania" jednej p³ytki od agenta aby zrobiæ seriê lub grupê z t¹ p³ytk¹ - 
    /// ale tak ¿e ta sekwencja "prze¿yje" bez tej jednej p³ytki - 
    /// funkcja s³u¿y do nauczenia modelu u¿ywania p³ytek z planszy do po³o¿enia dwóch lub jedn¹ p³ytkê, 
    /// agent powinien podebraæ p³ytkê od innej sekwencji i zrobiæ w³asn¹,
    /// <param name="tilesToExtend">ile p³ytek zostanie zabrane z rêki agenta</param>
    /// <param name="tileAmount">ile p³ytek bêdzie w sekwencji na mapie, jeœli tilesToExtend = 1 to minimum musi byæ 4, gdy tilesToExtend = 2 to minimum musi byæ 5</param>
    /// </summary>
    void SeqToFilch(int tilesToExtend,int tileAmount, ref List<Tile> tiles)
    {
        //ile p³ytek zabraæ z rêki agenta
        int idx = Random.Range(0, AIPlayerHand.Count);//index p³ytki który zostanie podebrany
        List<Tile> seq = new List<Tile>();

        SaveListToCopy();

    }
    /// <summary>
    /// Funkcja do po³ozenia losowej iloœci gotowych sekwencji na mapie
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="seqAmount"></param>
    /// <param name="tileAmount"></param>
    /// <param name="availableBoardAmount"></param>
    void PutingManySeq(ref List<Tile> tiles, int seqAmount, int tileAmount, int availableBoardAmount)
    {
        seqAmount = Random.Range(1, seqAmount + 1);
        for (int i = 0; i < seqAmount; i++)
        {
            List<Tile> seq = new List<Tile>();
            //int tileAmount = 3;
            SeqPreparing(ref tiles, ref seq, ref tileAmount, 100, 100, true);
            if (seq.Count > 0)
            {
                //po³o¿enie p³ytek na wolne miejsce
                TrainingPutingManySeq(tileAmount, availableBoardAmount, ref seq);
            }
            else continue;
        }
    }

    /// <summary>
    /// Funkcja do przygotowania pojedyñczej sekwencji do po³o¿enia
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="seq">lista która zostanie potem u¿yta</param>
    /// <param name="tileAmount">ile p³ytek ma mieæ sekwencja</param>
    /// <param name="Set">czy bêdzie grupa czy seria</param>
    /// <param name="Joker">jaka jest szansa na jokera</param>
    /// <param name="randomTileAmount">czy iloœæ p³ytek w sekwencji bêdzie losowa z przedzia³u czy zawsze sta³a</param>
    void SeqPreparing(ref List<Tile> tiles, ref List<Tile> seq, ref int tileAmount, int Set, int Joker, bool randomTileAmount)
    {
        //AIPlayerHand = new();
        //AIPlayerHandCopy = new();
        //int joker = tiles.FindIndex(t => t.GetNumber() == 30);
        if (tiles.FindIndex(t => t.GetNumber() == 30) == -1)
        {
            Joker = 0;
        }
        bool isJoker = Random.Range(0, 100) < Joker;
        bool isSet = Random.Range(0, 100) < Set;
        if (isSet)
            tileAmount = Mathf.Clamp(tileAmount, 3, 13);
        else
            tileAmount = Mathf.Clamp(tileAmount, 3, 4);

        if (randomTileAmount)
            tileAmount = Random.Range(3, tileAmount + 1);
        int normalTilesAmount = isJoker ? tileAmount - 1 : tileAmount;
        // playerHand = new ();S
        //int TileIndex;
        //int amount = 3
        //zabezpieczyæ przed tworzeniem sekwencji w których brakuje p³ytek



        UnityEngine.Color[] cols = { UnityEngine.Color.red, new UnityEngine.Color(1f, 0.5f, 0f), UnityEngine.Color.black, UnityEngine.Color.blue };
        bool sequenceReady = false;
        int safetyIterator = 0;
        while (!sequenceReady && safetyIterator < 100)
        {
            safetyIterator++;
            List<int> foundIndices = new List<int>();
            bool allFound = true;
            if (isSet)
            {

                int randomColorIdx = Random.Range(0, 4);
                UnityEngine.Color selectedCol = cols[randomColorIdx];
                int totalSpan = isJoker ? normalTilesAmount + 1 : normalTilesAmount;
                int maxPossibleStart = 13 - totalSpan + 1;
                int startNum = Random.Range(1, maxPossibleStart + 1);

                

                for (int i = 0; i < normalTilesAmount; i++)
                {
                    int targetNum = startNum + i;
                    int foundIndex = tiles.FindIndex(t => t.GetNumber() == targetNum && t.GetColor() == selectedCol);

                    if (foundIndex != -1) foundIndices.Add(foundIndex);
                    else { allFound = false; break; }
                }
            }
            else //group
            {
                int targetNum = Random.Range(1, 14);
                List<UnityEngine.Color> availableColors = new List<UnityEngine.Color>(cols);

                for (int i = 0; i < normalTilesAmount; i++)
                {
                    int colorIdx = Random.Range(0, availableColors.Count);
                    UnityEngine.Color selectedCol = availableColors[colorIdx];

                    int foundIndex = tiles.FindIndex(t => t.GetNumber() == targetNum && t.GetColor() == selectedCol);

                    if (foundIndex != -1)
                    {
                        foundIndices.Add(foundIndex);
                        availableColors.RemoveAt(colorIdx);
                    }
                    else { allFound = false; break; }
                }     
            }
            int jokerIdx = -1;
            if (allFound && isJoker)
            {
                jokerIdx = tiles.FindIndex(t => t.GetNumber() == 30);
                if (jokerIdx == -1) allFound = false;
            }
            if (allFound)
            {
                if (isJoker) foundIndices.Add(jokerIdx);

                foreach (int idx in foundIndices.OrderByDescending(i => i))
                {
                    seq.Add(tiles[idx]);
                    tiles.RemoveAt(idx);
                }
                if (isSet)
                {
                    
                    seq = seq.OrderBy(t => t.GetNumber()).ToList();
                }
                sequenceReady = true;
            }
            else
                foundIndices.Clear();
        }

        //Debug.Log("ile p³ytek jest w klasie player: "+playerHand.Count);
        //SaveListToCopy();
        //PrintList();
       // this.myIndex = idx;
    }

    /// <summary>
    /// Funkcja do po³o¿enia ca³ej sekwnecji na mapie, 
    /// </summary>
    /// <param name="tileAmount">ile miejsc na planszy trzeba zareserwowaæ</param>
    /// <param name="availableBoardAmount"></param>
    /// <param name="sequention"></param>
    void TrainingPutingManySeq(int tileAmount, int availableBoardAmount,ref List<Tile> sequention)
    {
        List<Vector3Int> chosenSpace = FreeSpaceToPut(tileAmount, availableBoardAmount);

        for (int i = 0; i < chosenSpace.Count; i++)
        {
            
            PutTile(chosenSpace[i], sequention[0]);
            sequention.RemoveAt(0);
        }
        
        //SaveListToCopy();
        objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
        placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
        GameController.Instance.gameTurnManager.EndTurn();//zmiana wszystkich p³ytek na planszy na put = true

    }


    void CreateSeq(ref List<Tile> tiles, ref List<Tile> seq, ref int tileAmount, int Set, int Joker, bool randomTileAmount)
    {
        //AIPlayerHand = new();
        //AIPlayerHandCopy = new();
        //int joker = tiles.FindIndex(t => t.GetNumber() == 30);
        if (tiles.FindIndex(t => t.GetNumber() == 30) == -1) Joker = 0;
        bool isJoker = Random.Range(0, 100) < Joker;

        bool isSet;
        UnityEngine.Color seqColor = new UnityEngine.Color();
        //bool firstOrLast = Random>r
        if (seq.Count == 2)
        {
            isSet = true;
            seqColor = seq[0].GetColor();
        }
        else isSet = Random.Range(0, 100) < Set;

        //wykrywanie jakie liczby i kolory


        //bool 
        if (isSet || seq.Count>1)
            tileAmount = Mathf.Clamp(tileAmount, 3, 13-seq.Count);
        else
            tileAmount = 3;

        if (randomTileAmount)
            tileAmount = Random.Range(3, tileAmount + 1);
        int normalTilesAmount = isJoker ? tileAmount - 1 : tileAmount;
        // playerHand = new ();S
        //int TileIndex;
        //int amount = 3
        //zabezpieczyæ przed tworzeniem sekwencji w których brakuje p³ytek



        UnityEngine.Color[] cols = { UnityEngine.Color.red, new UnityEngine.Color(1f, 0.5f, 0f), UnityEngine.Color.black, UnityEngine.Color.blue };
        bool sequenceReady = false;
        int safetyIterator = 0;
        while (!sequenceReady && safetyIterator < 100)
        {
            safetyIterator++;
            List<int> foundIndices = new List<int>();
            bool allFound = true;
            if (isSet)
            {

                int randomColorIdx = Random.Range(0, 4);
                UnityEngine.Color selectedCol = cols[randomColorIdx];
                int totalSpan = isJoker ? normalTilesAmount + 1 : normalTilesAmount;
                int maxPossibleStart = 13 - totalSpan + 1;
                int startNum = Random.Range(1, maxPossibleStart + 1);



                for (int i = 0; i < normalTilesAmount; i++)
                {
                    int targetNum = startNum + i;
                    int foundIndex = tiles.FindIndex(t => t.GetNumber() == targetNum && t.GetColor() == selectedCol);

                    if (foundIndex != -1) foundIndices.Add(foundIndex);
                    else { allFound = false; break; }
                }
            }
            else //group
            {
                int targetNum = Random.Range(1, 14);
                List<UnityEngine.Color> availableColors = new List<UnityEngine.Color>(cols);

                for (int i = 0; i < normalTilesAmount; i++)
                {
                    int colorIdx = Random.Range(0, availableColors.Count);
                    UnityEngine.Color selectedCol = availableColors[colorIdx];

                    int foundIndex = tiles.FindIndex(t => t.GetNumber() == targetNum && t.GetColor() == selectedCol);

                    if (foundIndex != -1)
                    {
                        foundIndices.Add(foundIndex);
                        availableColors.RemoveAt(colorIdx);
                    }
                    else { allFound = false; break; }
                }
            }
            int jokerIdx = -1;
            if (allFound && isJoker)
            {
                jokerIdx = tiles.FindIndex(t => t.GetNumber() == 30);
                if (jokerIdx == -1) allFound = false;
            }
            if (allFound)
            {
                if (isJoker) foundIndices.Add(jokerIdx);

                foreach (int idx in foundIndices.OrderByDescending(i => i))
                {
                    seq.Add(tiles[idx]);
                    tiles.RemoveAt(idx);
                }
                if (isSet)
                {

                    seq = seq.OrderBy(t => t.GetNumber()).ToList();
                }
                sequenceReady = true;
            }
            else
                foundIndices.Clear();
        }

      
    }
}
