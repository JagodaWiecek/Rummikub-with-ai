using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDictionary : MonoBehaviour
{

    public Dictionary<Vector3Int, Tile> board;
    // Start is called before the first frame update
    void Start()
    {
        //board = new Dictionary<Vector3Int, Tile>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // private Dictionary<int, string> myDictionary = new Dictionary<int, string>();
    public void AddObjectAt(Vector3Int gridPosition, Tile tile)
    {
        Tile newtile = new Tile(tile.getNumber(), tile.GetColor(), tile.getTilename(), tile.getSymbol(), tile.GetPut() );
        newtile.ShowTiles();
        Debug.Log(gridPosition);
        board[gridPosition] = newtile;
    }
    public Tile ReturnTileInDirectory(Vector3Int gridPosition)
    {
        Debug.Log(gridPosition);
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
}
