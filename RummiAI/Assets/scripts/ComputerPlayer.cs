using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class ComputerPlayer : MonoBehaviour
{
    [SerializeField]
    private List<Tile> computerPlayerHand = new();
    [SerializeField]
    private List<Tile> computerPlayerHandCopy = new();
    [SerializeField]
    private bool firstTurn = false;

    [SerializeField]
    public int myIndex;

    [SerializeField]
    public int countJoker;

    private float elapsedTime = 0;

    [SerializeField]
    private ObjectsDatabase database;
    [SerializeField]
    private ObjectPlacer objectPlacer;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    PlacementSystem placementSystem;

    int maxX = 8;
    int minX = -9;
    int maxZ = 2;
    int minZ = -4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.gameTurnManager.currentPlayerId == myIndex) 
        {
            elapsedTime += Time.deltaTime;
            //time = 2f;
            Debug.Log("moja tura: "+this.transform.name);
            //SortByNumbers();
            // Debug.Log(computerPlayerHand.Count);
            //StartCoroutine(ShowTilesInDeck());
            //SortByColors();
            //ShowTilesInDeck();


            if(elapsedTime >= 2f && elapsedTime < 4f)
            {
                List<List<Tile>> sequencesbyColors = FindSequentialColorSets(ref computerPlayerHand);
                if (sequencesbyColors.Count > 0)
                {
                    for (int i = sequencesbyColors.Count - 1; i >= 0; i--)
                    {
                        List<Tile> sequence = sequencesbyColors[i];
                        PutTilesOnBoard(sequence);
                        RemoveFromList(sequence);
                        sequencesbyColors.RemoveAt(i);
                    }
                }

            }
            if (elapsedTime >= 4f && elapsedTime < 6f)
            {
                List<List<Tile>> sequencesbyNumbers = FindSameNumberDifferentColorSets(ref computerPlayerHand);
                if (sequencesbyNumbers.Count > 0)
                {
                    for (int i = sequencesbyNumbers.Count - 1; i >= 0; i--)
                    {
                        List<Tile> sequence = sequencesbyNumbers[i];
                        PutTilesOnBoard(sequence);
                        RemoveFromList(sequence);
                        sequencesbyNumbers.RemoveAt(i); // Usuñ przetworzon¹ sekwencjê
                    }
                    //Debug.Log("sekwencji tych samych liczb: " + sequencesbyNumbers.Count);
                }
                
            }
            if (elapsedTime >= 6f)
            {
                elapsedTime = 0f;
                 // Resetujemy czas i tura++
                //jeœli liczba kart siê nie zmieni³a, to +1 karta, jeœli nie, to po prostu nowa tura
                if(computerPlayerHand.Count == computerPlayerHandCopy.Count)
                {
                    //nie by³o ruchu
                    List<Tile> board = GameController.Instance.GetTiles();
                    AddNewTile(ref board);
                    GameController.Instance.NewTurn();
                }
                else
                {
                    //by³ ruch
                    if (GetFirstTour())//by³ ruch wiêc jeœli by³a to pierwsza tura to ju¿ nie jest
                        EndFirstTour();
                    NewTurn();
                    SaveListToCopy();
                    //zapisaæ kopie
                }
                GameController.Instance.gameTurnManager.ChangeTurn();

            }
            //
        }
    }
    public void NewTurn()
    {
        if (GameController.Instance != null)
        {
            objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
            placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
        }
    }

    public void ShowTilesInDeck()
    {
        foreach (Tile tile in computerPlayerHand) 
        {
            
            Debug.Log("numer:"+tile.GetNumber() +", kolor: "+ GameController.Instance.SetNameForTile(tile.GetColor()));
            //wait(1f);
           // yield return new WaitForSeconds(1.5f);
        }
        Debug.Log("end for " + this.transform.name);


    }
    /// <summary>
    /// funkcja do sortowania listy po liczbach o ró¿nych kolorach
    /// </summary>
    public void SortByNumbers()
    {
        computerPlayerHand.Sort((tile1, tile2) =>
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
    /// <summary>
    /// funkcja do sortowania listy po kolei liczbami i kolorami
    /// </summary>
    public void SortByColors()
    {
        //computerPlayerHand.Sort((tile1, tile2) => CompareColors(tile1.GetColor(), tile2.GetColor()));
        computerPlayerHand.Sort((tile1, tile2) =>
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
    /// <summary>
    /// funkcja do poprawnego porównywania kolorów
    /// </summary>
    /// <param name="color1">kolor pierwszy do porównania</param>
    /// <param name="color2">kolor drugi do porównania</param>
    /// <returns></returns>
    private static int CompareColors(Color color1, Color color2)
    {
        // Konwertowanie koloru na intensywnoœæ w skali szaroœci jako uproszczone porównanie
        float intensity1 = color1.r * 0.3f + color1.g * 0.59f + color1.b * 0.11f;
        float intensity2 = color2.r * 0.3f + color2.g * 0.59f + color2.b * 0.11f;
        return intensity1.CompareTo(intensity2);
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
        countJoker = CountJoker(computerPlayerHand);
        //Debug.Log("ile p³ytek-kopii jest w klasie player: " + playerHandCopy.Count);
    }
    private int CountJoker(List<Tile> playerHand)
    {
        int count = 0;
        foreach (Tile tile in playerHand) 
        {
            if (tile.GetNumber() == 30)
                count++;
            
        }
        return count;
    }

    public List<List<Tile>> FindSequentialColorSets(ref List<Tile> tiles)
    {
        // Sortuj listê w miejscu najpierw po kolorze, a nastêpnie po numerze
        SortByColors();
        //tiles.SortByColors();

        List<List<Tile>> sequentialColorSets = new List<List<Tile>>();
        List<Tile> currentSet = new List<Tile>();
        int jokerCount = 0;

        for (int i = 0; i < tiles.Count; i++)
        {
            Tile currentTile = tiles[i];
            bool isJoker = currentTile.GetNumber() == 30;

            // Ignoruj duplikaty
            if (i > 0 && currentTile.GetNumber() == tiles[i - 1].GetNumber() &&
                currentTile.GetColor() == tiles[i - 1].GetColor())
            {
                continue;
            }

            if (isJoker)
            {
                // Jeœli kafelek jest jokerem, dodaj go do bie¿¹cego zestawu i zwiêksz licznik jokerów
                jokerCount++;
                currentSet.Add(currentTile);
            }
            else if (currentSet.Count == 0 ||
                     (currentTile.GetNumber() == currentSet[^1].GetNumber() + 1 &&
                      currentTile.GetColor() == currentSet[^1].GetColor()) ||
                     (jokerCount > 0 && currentTile.GetNumber() == currentSet[^1].GetNumber() + 2 &&
                      currentTile.GetColor() == currentSet[^1].GetColor()))
            {
                // Jeœli kafelek pasuje do sekwencji, dodaj go do bie¿¹cego zestawu
                currentSet.Add(currentTile);

                // Jeœli wykorzystujemy jokera do uzupe³nienia luki w sekwencji, zmniejszamy jego licznik
                if (jokerCount > 0 && currentTile.GetNumber() == currentSet[^2].GetNumber() + 2)
                {
                    jokerCount--;
                }
            }
            else
            {
                // Jeœli bie¿¹cy zestaw ma co najmniej 3 elementy, dodaj go do wynikowej listy
                if (IsValidSequence(currentSet))
                {
                    sequentialColorSets.Add(new List<Tile>(currentSet));
                }

                // Rozpocznij nowy zestaw, zresetuj licznik jokerów
                currentSet = new List<Tile> { currentTile };
                jokerCount = isJoker ? 1 : 0;
            }
        }

        // Sprawdzenie koñcowego zestawu po pêtli
        if (IsValidSequence(currentSet))
        {
            sequentialColorSets.Add(currentSet);
        }

        return sequentialColorSets;
    }

    private bool IsValidSequence(List<Tile> sequence)
    {
        if (sequence.Count < 3) return false;

        int sum = 0;
        int expectedNumber = sequence[0].GetNumber();

        foreach (Tile tile in sequence)
        {
            if (tile.GetNumber() == 30)
            {
                // Joker zastêpuje brakuj¹cy numer w sekwencji
                sum += expectedNumber;
            }
            else
            {
                sum += tile.GetNumber();
                expectedNumber = tile.GetNumber();
            }
            expectedNumber++;
        }

        // Jeœli to jest pierwsza tura, sprawdŸ czy suma wynosi co najmniej 30
        return !firstTurn || sum >= 30;
    }

    public List<List<Tile>> FindSameNumberDifferentColorSets(ref List<Tile> tiles)
    {
        // Sortuj listê w miejscu po numerze, a nastêpnie po kolorze
        SortByNumbers();

        List<List<Tile>> sameNumberDifferentColorSets = new List<List<Tile>>();
        List<Tile> currentSet = new List<Tile>();
        int jokerCount = 0;

        for (int i = 0; i < tiles.Count; i++)
        {
            Tile currentTile = tiles[i];
            bool isJoker = currentTile.GetNumber() == 30;

            // Ignoruj duplikaty tego samego numeru i koloru
            if (i > 0 && currentTile.GetNumber() == tiles[i - 1].GetNumber() &&
                currentTile.GetColor() == tiles[i - 1].GetColor())
            {
                continue;
            }

            if (isJoker)
            {
                // Jeœli kafelek jest jokerem, dodaj go do bie¿¹cego zestawu i zwiêksz licznik jokerów
                jokerCount++;
                currentSet.Add(currentTile);
            }
            else if (currentSet.Count == 0 ||
                     (currentTile.GetNumber() == currentSet[^1].GetNumber() &&
                      currentTile.GetColor() != currentSet[^1].GetColor()) ||
                     (jokerCount > 0 && currentTile.GetNumber() == currentSet[^1].GetNumber() &&
                      currentTile.GetColor() != currentSet[^1].GetColor()))
            {
                // Jeœli kafelek pasuje do sekwencji (ta sama liczba, inny kolor), dodaj go do bie¿¹cego zestawu
                currentSet.Add(currentTile);

                // Jeœli wykorzystujemy jokera do uzupe³nienia luki w sekwencji, zmniejszamy jego licznik
                if (jokerCount > 0 && currentTile.GetColor() != currentSet[^2].GetColor())
                {
                    jokerCount--;
                }
            }
            else
            {
                // Jeœli bie¿¹cy zestaw ma co najmniej 3 elementy, dodaj go do wynikowej listy
                if (IsValidSequenceColor(currentSet))
                {
                    sameNumberDifferentColorSets.Add(new List<Tile>(currentSet));
                }

                // Rozpocznij nowy zestaw, zresetuj licznik jokerów
                currentSet = new List<Tile> { currentTile };
                jokerCount = isJoker ? 1 : 0;
            }
        }

        // Sprawdzenie koñcowego zestawu po pêtli
        if (IsValidSequenceColor(currentSet))
        {
            sameNumberDifferentColorSets.Add(currentSet);
        }

        return sameNumberDifferentColorSets;
    }

    private bool IsValidSequenceColor(List<Tile> sequence)
    {
        if (sequence.Count < 3 || sequence.Count > 4) return false;

        int sum = 0;
        int expectedNumber = sequence[0].GetNumber();
        HashSet<Color> uniqueColors = new HashSet<Color>();

        foreach (Tile tile in sequence)
        {
            if (tile.GetNumber() == 30)
            {
                // Joker zastêpuje brakuj¹cy numer w sekwencji
                sum += expectedNumber;
            }
            else
            {
                sum += tile.GetNumber();
                expectedNumber = tile.GetNumber();
                uniqueColors.Add(tile.GetColor());
            }
            //expectedNumber++;
        }
        if (uniqueColors.Count != sequence.Count) return false;
        // Jeœli to jest pierwsza tura, sprawdŸ, czy suma wynosi co najmniej 30
        return !firstTurn || sum >= 30;
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
        //this.computerPlayerHandCopy.Add(tile);
    }
    public void AddTileToCopyList(Tile tile)
    {
        this.computerPlayerHand.Add(tile);
    }

    public void AddNewTile(ref List<Tile> tiles)
    {
        int TileIndex = Random.Range(0, (tiles.Count));
        computerPlayerHand.Add(tiles[TileIndex]);
        tiles.RemoveAt(TileIndex);
        SaveListToCopy();
        countJoker = CountJoker(computerPlayerHand);
    }

    public void PutTilesOnBoard(List<Tile> sequention)
    {
        List<Vector3Int> chosenSpace = FreeSpaceToPut(sequention.Count);
        for(int i = 1,j=0;i< (chosenSpace.Count-1);i++,j++)
        {

            PutTile(chosenSpace[i], sequention[j]);
        }

        //wylosowanie wartoœci na mape dla x i z
        //nie mo¿na wyjœæ poza mape
        //wylosowane pole nie mo¿e byæ zajête
        //istotne przy dodawaniu:
        //dodanie do objectplacer
        //dodanie do GridData
        //dodanie do gameboard
        //Debug.Log(chosenSpace);
    }
    /// <summary>
    /// funkcja do wylosowania pozycji do po³o¿enia p³ytki dla gracza komputerowego
    /// oraz sprawdzenie czy pozycje s¹ poprawne dla niego
    /// </summary>
    /// <param name="tileAmount">iloœæ p³ytek, jak¹ gracz chce postawiæ</param>
    /// <returns>listê pozycji, które zostan¹ zajête w wersji integer</returns>
    public List<Vector3Int> FreeSpaceToPut(int tileAmount)
    {
        var board = GameController.Instance.GetBoardDictionary().board;
        //int maxX = 8;
        //int minX = -9;
        //int maxZ = 2;
        //int minZ = -4;
        List < Vector3Int > list = new List < Vector3Int >();

        int levelZ = Random.Range(minZ, (maxZ + 1));
        int levelX = Random.Range(minX, ((maxX+1) - tileAmount));
        //int levelZ = -4;
        //int levelX = -9;
        int amountToOccupy = tileAmount+ 2;
        Vector3Int position;

        int i = 0;
        while (i < amountToOccupy)
        {
            position = new Vector3Int(levelX, 0, levelZ);
            if (board.ContainsKey(position))
            {
                levelZ = Random.Range(minZ, maxZ + 1);
                levelX = Random.Range(minX, maxX + 1 - tileAmount);
                list.Clear();
                i = 0; 
            }
            else
            {
                list.Add(position);
                levelX++;
                i++;
            }
        }



        return list;
    }

    void PutTile(Vector3Int gridPosition,Tile tile)
    {

        int index = objectPlacer.PlacedObject(database.objectsData[0].Prefab, grid.CellToWorld(gridPosition),ref tile, grid);

        placementSystem.GetGridData().AddObjectAt(gridPosition,
            database.objectsData[0].Size,
            database.objectsData[0].ID,
            index);
        tile.ShowTiles();
        Debug.Log("na pozycji:" + gridPosition);
    }

    void RemoveFromList(List<Tile> sequence)
    {
        foreach (Tile tile in sequence) {
            //computerPlayerHand
            for (int i = 0; i < computerPlayerHand.Count; i++)
            {
                if (computerPlayerHand[i].Equals(tile, computerPlayerHand[i]))
                {
                    computerPlayerHand.RemoveAt(i);
                    break;
                }

            }
        }
    }
}
