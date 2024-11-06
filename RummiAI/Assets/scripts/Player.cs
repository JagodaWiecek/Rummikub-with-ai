using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private List<Tile> playerHand = new() ;
    [SerializeField]
    private List<Tile> playerHandCopy = new();
    [SerializeField]
    private bool firstTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Tile> GetList() {  return this.playerHand; }
    public List<Tile> GetListCopy() { return this.playerHandCopy; }
    public bool GetFirstTour() {  return this.firstTurn; }

    /// <summary>
    /// funkcja ustawia zmienn¹ bool gracza na false, co oznacza ¿e gracz ju¿ ma za sob¹ pierwsz¹ turê gry
    /// wartoœæ zmiennej ju¿ nie zmieni siê w trakcie gry na nic innego
    /// </summary>
    public void EndFirstTour() {  this.firstTurn = false; }

    public void AddTileToList(Tile tile)
    {
        this.playerHand.Add(tile); 
    }

    public void AddTileToCopyList(Tile tile)
    {
        this.playerHandCopy.Add(tile);
    }
    /// <summary>
    /// Funkcja, która nadpisuje zawartoœæ zmiennej "kopii" na aktualn¹ listê
    /// </summary>
    public void SaveListToCopy()
    {
        playerHandCopy.Clear();
        foreach (Tile tile in playerHand)
        {
            playerHandCopy.Add(tile);
        }

    }
    /// <summary>
    /// funkcja do odzyskania zmiennych z kopii do oryginalnej listy
    /// </summary>
    public void RestoreCopyList()
    {
        playerHand.Clear();
        foreach (Tile tile in playerHandCopy)
        {
            playerHand.Add(tile);
        }

    }

    public void SetPlayersHand(ref List<Tile> tiles)
    {
        // playerHand = new ();
        int TileIndex;
        for (int i = 0; i < 14; i++)
        {
            TileIndex = Random.Range(0, (tiles.Count));
            playerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
        }
        //Debug.Log("ile p³ytek jest w klasie player: "+playerHand.Count);
        SaveListToCopy();
        //Debug.Log("ile p³ytek-kopii jest w klasie player: " + playerHandCopy.Count);
    }

}
