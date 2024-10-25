using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTurn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewTurn()
    {
        if(GameController.Instance != null && GameController.Instance.GetBoardDictionary().board.Count!=0)
        {//KeyValuePair<int, string> kvp in dictionary
            //Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            foreach (KeyValuePair<Vector3Int, Tile> tile in GameController.Instance.GetBoardDictionary().board)
            {
                tile.Value.SetPut(true);
            }
            // GameController.Instance.GetBoardDictionary().board
            GameController.Instance.NewTurn();
        }
        else {
            Debug.Log("NewTurn - problem z game instance");
            //return;
        }
    }
}
