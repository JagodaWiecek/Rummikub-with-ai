using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Klasa do przechowywania informacji o pozycjach p³ytek na mapie.
///G³ównie s³u¿y do walidowania zasad gry planszowej w TurnController
/// </summary>
public class BoardDictionary : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Vector3Int, Tile> board;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    // private Dictionary<int, string> myDictionary = new Dictionary<int, string>();
    public void AddObjectAt(Vector3Int gridPosition, Tile tile)
    {
        Tile newtile = new Tile(tile.GetNumber(), tile.GetColor(), tile.GetTilename(), tile.GetSymbol(), tile.GetPut() );

        board[gridPosition] = newtile;
    }

    public void MoveObjectAt(Vector3Int newPositon, Vector3Int oldPosition)
    {
        AddObjectAt(newPositon, board[oldPosition]);
        board.Remove(oldPosition);
    }
    public Tile ReturnTileInDirectory(Vector3Int gridPosition)
    {
        //Debug.Log(gridPosition);
        return this.board[gridPosition];
    }

    public Dictionary<Vector3Int, Tile> GetBoard()
    {
        return this.board;
    }

    public BoardDictionary()
    {
        this.board = new Dictionary<Vector3Int, Tile>();
    }

    public bool AreEqual( Dictionary<Vector3Int, Tile> dictionary)
    {
        //this.board
        if (this.board.Count != dictionary.Count)
            return false;

        //Tile tile = new();
        foreach (var kvp in this.board)
        {
            if(!dictionary.ContainsKey(kvp.Key))
                return false;

            if (!this.board.TryGetValue(kvp.Key, out Tile value2))
                return false;

            
            if (!kvp.Value.Equals(kvp.Value, value2))//EqualityComparer<Tile>.Default.Equals(kvp.Value, value2)
                return false;
        }

        return true;
        //return true;
    }
    /// <summary>
    /// Funkcja do nadpisania zawartoœci s³ownika, zazwyczaj by odzyskaæ star¹ zawartoœæ
    /// </summary>
    /// <param name="copy">kopia wczeœniejszego s³ownika</param>
    public void SaveDictionary(Dictionary<Vector3Int, Tile> copy)
    {
        board.Clear();
        
        foreach (var entry in copy)
        {
            board[entry.Key] = entry.Value;
        }
    }

    public void SaveToAnotherDictionary(Dictionary<Vector3Int, Tile> board)
    {
       // this.board;
        foreach (var entry in board)
        {
            this.board[entry.Key] = entry.Value;
        }
    }
}
