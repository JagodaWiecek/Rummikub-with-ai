using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Drawing;

public class GameController : MonoBehaviour
{
    // Static instantion, globally available
    public static GameController Instance { get; private set; }

    public List<Tile> tiles; // 
    // Start is called before the first frame update
    private void Awake()
    {
        // Only one instance of object
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }


    void Start()
    {
        SetTiles(ref this.tiles);

        Debug.Log("Bank ma: " + this.tiles.Count + " p³ytek");
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Tile> GetTiles()
    {
        return this.tiles;
    }
    void SetTiles(ref List<Tile> tiles)
    {
        tiles = new List<Tile>();
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 13; i++)///dodanie do banku p³ytek koloru czerwonego
            {
                tiles.Add(new Tile((i + 1), UnityEngine.Color.red, setName(UnityEngine.Color.red) + "_" + (i + 1), (i + 1).ToString()));//(int num, Color col, string name)
            }
            for(int i = 0;i < 13; i++)//pomarañczowy
            {
                tiles.Add(new Tile((i + 1), new UnityEngine.Color(1f, 0.647f, 0f), setName(new UnityEngine.Color(1f, 0.647f, 0f)) + "_" + (i + 1), (i + 1).ToString()));
            }
            for (int i = 0; i < 13; i++)//czarny
            {
                tiles.Add(new Tile((i + 1), UnityEngine.Color.black, setName(UnityEngine.Color.black) + "_" + (i + 1), (i + 1).ToString()));
            }
            for (int i = 0; i < 13; i++)//niebieski
            {
                tiles.Add(new Tile((i + 1), UnityEngine.Color.blue, setName(UnityEngine.Color.blue) + "_" + (i + 1), (i + 1).ToString()));
            }
        }
        tiles.Add(new Tile(30, new UnityEngine.Color(0.5f, 0f, 0.5f), setName(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 30,"$"));//fiolet
        tiles.Add(new Tile(30, UnityEngine.Color.magenta, setName(UnityEngine.Color.magenta) + "_" + 30, "$"));//magenta
    }
    //Return name of color based on input value
    string setName(UnityEngine.Color color)
    {
        if (color == UnityEngine.Color.red) return "red";
        if (color == UnityEngine.Color.blue) return "blue";
        if (color == UnityEngine.Color.black) return "black";
        if (color == new UnityEngine.Color(1f, 0.647f, 0f)) return "orange"; // Pomarañczowy
        if (color == new UnityEngine.Color(0.5f, 0f, 0.5f)) return "purple_joker"; // Fioletowy
        if (color == UnityEngine.Color.magenta) return "magenta_joker";

        return "unknown";
        //return "";
    }

}


