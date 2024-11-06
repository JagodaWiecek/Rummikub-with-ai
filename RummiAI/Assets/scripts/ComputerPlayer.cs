using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : MonoBehaviour
{
    [SerializeField]
    private List<Tile> computerPlayerHand = new();
    [SerializeField]
    private List<Tile> computerPlayerHandCopy = new();
    [SerializeField]
    private bool firstTurn = true;

    [SerializeField]
    public int myIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ComputerPlayerMove()
    {

    }
    /// <summary>
    /// zapisanie zawartoœci g³ównej tablicy do kopii
    /// </summary>
    public void SaveListToCopy()
    {
        computerPlayerHandCopy.Clear();
        foreach (Tile tile in computerPlayerHand)
        {
            computerPlayerHandCopy.Add(tile);
        }

    }
    /// <summary>
    /// ustawienie losowe p³ytek dla kopmuterowego gracza na pocz¹tku gry
    /// </summary>
    /// <param name="tiles"></param>
    public void SetPlayersHand(ref List<Tile> tiles,int idx)
    {
        // playerHand = new ();
        int TileIndex;
        for (int i = 0; i < 14; i++)
        {
            TileIndex = Random.Range(0, (tiles.Count));
            computerPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
        }
        //Debug.Log("ile p³ytek jest w klasie player: "+playerHand.Count);
        SaveListToCopy();
        myIndex = idx;
        //Debug.Log("ile p³ytek-kopii jest w klasie player: " + playerHandCopy.Count);
    }

    public List<Tile> GetList() { return this.computerPlayerHand; }
    public List<Tile> GetListCopy() { return this.computerPlayerHandCopy; }
    public bool GetFirstTour() { return this.firstTurn; }

    /// <summary>
    /// funkcja ustawia zmienn¹ bool gracza na false, co oznacza ¿e gracz ju¿ ma za sob¹ pierwsz¹ turê gry
    /// wartoœæ zmiennej ju¿ nie zmieni siê w trakcie gry na nic innego
    /// </summary>
    public void EndFirstTour() { this.firstTurn = false; }

    public void AddTileToList(Tile tile)
    {
        this.computerPlayerHand.Add(tile);
        this.computerPlayerHandCopy.Add(tile);
    }
}
