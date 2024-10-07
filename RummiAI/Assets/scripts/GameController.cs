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

    public Tile[] tiles; // 
    // Start is called before the first frame update
    private void Awake()
    {
        // Only one instance of object
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Zapobieganie zniszczeniu GameControllera przy zmianie sceny
        }
        else
        {
            Destroy(gameObject); // Usuniêcie zduplikowanej instancji
        }
    }


    void Start()
    {
        SetTiles(ref this.tiles);

        for(int i = 0; i < this.tiles.Length; i++)
        {
            this.tiles[i].ShowVariables();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetTiles(ref Tile[] tiles)
    {
        tiles = new Tile[7];
        for (int i = 0; i < 5; i++) 
        {
            tiles[i] = new Tile((i + 1), UnityEngine.Color.red, setName(UnityEngine.Color.red)+"_"+ (i + 1));//(int num, Color col, string name)
            
        }
        tiles[5] = new Tile(0, new UnityEngine.Color(0.5f, 0f, 0.5f), setName(new UnityEngine.Color(0.5f, 0f, 0.5f)) + "_" + 0);//fiolet
        tiles[6] = new Tile(0, UnityEngine.Color.magenta, setName(UnityEngine.Color.magenta) + "_" + 0);//magenta
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


