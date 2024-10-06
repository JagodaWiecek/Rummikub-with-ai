using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{
    // 4 colors, numers from 1 to 13
    public int number;
    public Color numberColor;

    public Tile(int num, Color col)
    {
        number = num;
        numberColor = col;
    }

}
