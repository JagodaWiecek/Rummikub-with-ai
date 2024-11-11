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

    private void SortByNumbersOriginal()
    {
        playerHand.Sort((tile1, tile2) =>
        {
            int numberComparison = tile1.GetNumber().CompareTo(tile2.GetNumber());
            if (numberComparison == 0)
            {
                // Jeœli liczby s¹ takie same, sortuj po kolorze
                return tile1.GetColor().GetHashCode().CompareTo(tile2.GetColor().GetHashCode());
            }
            return numberComparison;
        });
    }
    private void SortByNumbersCopy()
    {
        playerHandCopy.Sort((tile1, tile2) =>
        {
            int numberComparison = tile1.GetNumber().CompareTo(tile2.GetNumber());
            if (numberComparison == 0)
            {
                // Jeœli liczby s¹ takie same, sortuj po kolorze
                return tile1.GetColor().GetHashCode().CompareTo(tile2.GetColor().GetHashCode());
            }
            return numberComparison;
        });
    }

    public void SortByNumbers()
    {
        SortByNumbersOriginal();
        SortByNumbersCopy();
    }

    private void SortByColorsOriginal()
    {
        //computerPlayerHand.Sort((tile1, tile2) => CompareColors(tile1.GetColor(), tile2.GetColor()));
        playerHand.Sort((tile1, tile2) =>
        {
            // Najpierw porównaj kolory
            int colorComparison = CompareColors(tile1.GetColor(), tile2.GetColor());
            if (colorComparison == 0)
            {
                // Jeœli kolory s¹ takie same, porównaj numery
                return tile1.GetNumber().CompareTo(tile2.GetNumber());
            }
            return colorComparison;
        });
    }
    private void SortByColorsCopy()
    {
        //computerPlayerHand.Sort((tile1, tile2) => CompareColors(tile1.GetColor(), tile2.GetColor()));
        playerHandCopy.Sort((tile1, tile2) =>
        {
            // Najpierw porównaj kolory
            int colorComparison = CompareColors(tile1.GetColor(), tile2.GetColor());
            if (colorComparison == 0)
            {
                // Jeœli kolory s¹ takie same, porównaj numery
                return tile1.GetNumber().CompareTo(tile2.GetNumber());
            }
            return colorComparison;
        });
    }

    public void SortByColors()
    {
        SortByColorsOriginal();
        SortByColorsCopy();
    }
    private static int CompareColors(Color color1, Color color2)
    {
        // Konwertowanie koloru na intensywnoœæ w skali szaroœci jako uproszczone porównanie
        float intensity1 = color1.r * 0.3f + color1.g * 0.59f + color1.b * 0.11f;
        float intensity2 = color2.r * 0.3f + color2.g * 0.59f + color2.b * 0.11f;
        return intensity1.CompareTo(intensity2);
    }

   public int FinalScore()
    {
        int score = 0;
        foreach(Tile tile in playerHand)
        {
            score += tile.GetNumber();
        }
        return score;
    }

}
