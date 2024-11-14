using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

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
    void Start()
    {
        firstTurn = true;
        Time.timeScale = 0.2f;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (GameController.Instance.gameTurnManager.currentPlayerId == myIndex)
        {
            int xAction = actions.DiscreteActions[0];
            int zAction = actions.DiscreteActions[1];

            int xCoord = xAction - 11; //przekszta³cenie na koordynaty x
            int zCoord = zAction - 4; // przekszta³cenie na koordynaty z
            //base.OnActionReceived(actions);
            Debug.Log("x: " + xCoord + " z: " + zCoord);
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
            AIPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
        }
        //Debug.Log("ile p³ytek jest w klasie player: "+playerHand.Count);
        SaveListToCopy();
        myIndex = idx;
        //countJoker = CountJoker(computerPlayerHand);
        //Debug.Log("ile p³ytek-kopii jest w klasie player: " + playerHandCopy.Count);
    }
}
