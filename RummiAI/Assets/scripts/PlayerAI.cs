using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UIElements;

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

        foreach(Tile tile in AIPlayerHand)
        {
            sensor.AddObservation(tile.GetNumber());
            sensor.AddObservation(tile.GetColor().r);
            sensor.AddObservation(tile.GetColor().g);
            sensor.AddObservation(tile.GetColor().b);
        }

        for(int z= minZ; z<= maxZ;z++)
        {
            for(int x= minX; x<= maxX;x++)
            {
                if(board.ContainsKey(new(x,0,z)))
                {
                    sensor.AddObservation(board[new(x,0,z)].GetNumber());
                    sensor.AddObservation(board[new(x,0,z)].GetColor().r);
                    sensor.AddObservation(board[new(x,0,z)].GetColor().g);
                    sensor.AddObservation(board[new(x,0,z)].GetColor().b);
                    sensor.AddObservation(board[new(x, 0, z)].GetPut() ? 1 : 0); 
                }
                else
                {
                    //puste pola
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

            //wybieranie karty w deku
            //jak ogarn¹æ by by³y uniwersalne do d³ugiœci deku
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
