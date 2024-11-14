using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private bool firstTurn;

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

    int maxX = 10;
    int minX = -11;
    int maxZ = 4;
    int minZ = -4;

    // Start is called before the first frame update
    void Start()
    {
        firstTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.gameTurnManager.currentPlayerId == myIndex) 
        {
            elapsedTime += Time.deltaTime;
            //time = 2f;
            //Debug.Log("moja tura: "+this.transform.name);
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
               // else elapsedTime = 4f;

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
                //else elapsedTime = 6f;


            }
            if(elapsedTime >= 6f && elapsedTime < 8f)
            {//przeanalizowaæ czy mo¿na coœ dodaæ
                if (!GetFirstTour())
                {
                    //funkcja do znalezienia wolnych miejsc obok sekwencji
                    //przeiterowania deku i znalezienie czy mo¿na postawiæ p³ytkê
                    //funkcja w tile sie przyda do walidacji
                    var board = GameController.Instance.GetBoardDictionary().board;
                    ExtendSequence(ref board , ref this.computerPlayerHand);
                }
                //else elapsedTime = 8f;

            }
            if (elapsedTime >= 8f)
            {
                elapsedTime = 0f;
                 // Resetujemy czas i tura++
                //jeœli liczba kart siê nie zmieni³a, to +1 karta, jeœli nie, to po prostu nowa tura
                if(computerPlayerHand.Count == computerPlayerHandCopy.Count)
                {
                    //nie by³o ruchu
                    List<Tile> board = GameController.Instance.GetGameBank();
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
                if (computerPlayerHand.Count == 0)
                    GameController.Instance.EndGame();
            }
            //
        }
    }
    /// <summary>
    /// funkcja do zapisania kontenerów i przejœcia do nowej tury 
    /// gdy zosta³ wykonany jakiœ ruch
    /// </summary>
    public void NewTurn()
    {
        if (GameController.Instance != null)
        {
            objectPlacer.SetPlacedGameObjectsCopy();///zapisanie kopii objectPlacer
            placementSystem.GetGridData().SaveCopyDictionary();///zapisanie kopii GridData
            GameController.Instance.NewTurn();
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
        SortByColors();

        List<List<Tile>> sequentialColorSets = new List<List<Tile>>();
        List<Tile> currentSet = new List<Tile>();
        List<Tile> jokerTiles = new List<Tile>();

        for (int i = 0; i < tiles.Count; i++)
        {
            Tile currentTile = tiles[i];
            bool isJoker = currentTile.GetNumber() == 30;

            if (isJoker && jokerTiles.Count < 2)
            {
                jokerTiles.Add(currentTile);
                continue;
            }
            else if (currentSet.Count == 0 ||
                     (currentTile.GetColor() == currentSet[^1].GetColor() &&
                      currentTile.GetNumber() == currentSet[^1].GetNumber() + 1))
            {
                currentSet.Add(currentTile);
            }
            else if (currentTile.GetColor() == currentSet[^1].GetColor() &&
                     currentTile.GetNumber() == currentSet[^1].GetNumber())
            {
                continue;
            }
            else
            {
                if (currentSet[^1].GetNumber() != 13 && jokerTiles.Count == 1)
                {
                    currentSet.AddRange(jokerTiles);
                    jokerTiles.Clear();
                }
                else if (currentSet[^1].GetNumber() < 12)
                {
                    currentSet.AddRange(jokerTiles);
                    jokerTiles.Clear();
                }

                if (IsSequenceValid(currentSet))
                {
                    sequentialColorSets.Add(new List<Tile>(currentSet));
                }

                currentSet = new List<Tile> { currentTile };
                jokerTiles.Clear();
            }
        }

        currentSet.AddRange(jokerTiles);
        jokerTiles.Clear();
        if (IsSequenceValid(currentSet))
        {
            sequentialColorSets.Add(currentSet);
        }

        
        if (firstTurn && !IsTotalSumValid(sequentialColorSets))
        {
            sequentialColorSets.Clear(); // Jeœli suma nie spe³nia wymogu, czyszczona jest lista
        }

        return sequentialColorSets;
    }

    /// Funkcja do sprawdzania, czy sekwencja ma co najmniej 3 p³ytki
    private bool IsSequenceValid(List<Tile> sequence)
    {
        return sequence.Count >= 3;
    }

    /// Funkcja do sprawdzania, czy ca³kowita suma wartoœci sekwencji jest >= 30, jeœli jest pierwsza tura
    private bool IsTotalSumValid(List<List<Tile>> sequences)
    {
        int totalSum = 0;

        foreach (List<Tile> sequence in sequences)
        {
            int expectedNumber = sequence[0].GetNumber();

            foreach (Tile tile in sequence)
            {
                totalSum += tile.GetNumber() == 30 ? expectedNumber : tile.GetNumber();
                if (tile.GetNumber() != 30) expectedNumber = tile.GetNumber() + 1;
            }
        }

        return totalSum >= 30;
    }

    private bool IsJokerValidForSequence(List<Tile> sequence, Tile joker)
    {
        // Jeœli sekwencja zawiera ju¿ dwa jokery lub nie jest wystarczaj¹co d³uga, aby dodaæ jokera, zwróæ true
        if (sequence.Count < 2) return true;

        // Ostatnia i przedostatnia liczba w sekwencji
        int lastNum = sequence[^1].GetNumber();
        int secondLastNum = sequence[^2].GetNumber();

        // Jeœli sekwencja jest ci¹g³a, nie mo¿emy dodaæ jokera na koñcu
        if (lastNum == secondLastNum + 1)
        {
            return false;
        }

        // Joker mo¿e zostaæ dodany tylko na pocz¹tek sekwencji, jeœli brakuje jednej liczby
        if (lastNum == secondLastNum + 2)
        {
            return true; // Joker mo¿e zast¹piæ brakuj¹c¹ liczbê na pocz¹tku (np. 11, 12, 13)
        }

        // W przeciwnym razie joker nie jest dozwolony
        return false;
    }

    /// <summary>
    /// funkcja do wytworzenia sekwencji, któr¹ komputerowy gracz mo¿e wy³o¿yæ na planszê
    /// </summary>
    /// <param name="tiles"></param>
    /// <returns></returns>
    public List<List<Tile>> FindSameNumberDifferentColorSets( ref List<Tile> tiles)
    {
        
        SortByNumbers();//sortowanie po numerach a potem po kolorach

        List<List<Tile>> sameNumberDifferentColorSets = new List<List<Tile>>();
        List<Tile> currentSet = new List<Tile>();
        List<Tile> jokerTiles = new List<Tile>();


        for (int i = 0; i < tiles.Count; i++)
        {
            Tile currentTile = tiles[i];
            bool isJoker = currentTile.GetNumber() == 30;

            if (isJoker && jokerTiles.Count < 2)
            {
                jokerTiles.Add(currentTile);
                continue;
            }
            else if (currentSet.Count == 0 ||
                     (currentTile.GetColor() != currentSet[^1].GetColor() &&
                      currentTile.GetNumber() == currentSet[^1].GetNumber()))
            {
                currentSet.Add(currentTile);
            }
            else if (currentTile.GetColor() == currentSet[^1].GetColor() &&
                     currentTile.GetNumber() == currentSet[^1].GetNumber())
            {
                continue;
            }
            else
            {
                // Sprawdzamy, czy mo¿emy dodaæ jokera na koñcu sekwencji
                if (jokerTiles.Count > 0 && IsJokerValidForSequence(currentSet, jokerTiles[0]))
                {
                    currentSet.Add(jokerTiles[0]);
                    jokerTiles.Clear();
                }

                if (IsSequenceValid(currentSet))
                {
                    sameNumberDifferentColorSets.Add(new List<Tile>(currentSet));
                }

                currentSet = new List<Tile> { currentTile };
                jokerTiles.Clear();
            }
        }

        if (jokerTiles.Count > 0 && IsJokerValidForSequence(currentSet, jokerTiles[0]))
        {
            currentSet.Add(jokerTiles[0]);
            jokerTiles.Clear();
        }

        if (IsSequenceValid(currentSet))
        {
            sameNumberDifferentColorSets.Add(currentSet);
        }

        // Final check for the entire list of sequences
        if (firstTurn && !IsTotalSumValid(sameNumberDifferentColorSets))
        {
            sameNumberDifferentColorSets.Clear();
        }



        return sameNumberDifferentColorSets;
    }


    /// <summary>
    /// funkcja zwraca informacje czy wprowadzona sekwencja spe³nia wszystkie za³o¿enia
    /// i czy jest poprawna do po³o¿enia na planszê
    /// </summary>
    /// <param name="sequence">lista p³ytek wygenerowane przez funkcje</param>
    /// <returns>true jeœli sekwencja jest poprana, false gdy sekwencja jest wadliwa</returns>
    private bool IsValidSequenceColor(List<Tile> sequence)
    {
        if (sequence.Count < 3 || sequence.Count > 4) return false;

        HashSet<Color> uniqueColors = new HashSet<Color>();

        foreach (Tile tile in sequence)
        {
                uniqueColors.Add(tile.GetColor());
            
            //expectedNumber++;
        }
        if (uniqueColors.Count != sequence.Count) return false;
        return true;
        // Jeœli to jest pierwsza tura, sprawdŸ, czy suma wynosi co najmniej 30
       // return !firstTurn || sum >= 30;
    }
    private bool IsTotalSumValidColor(List<List<Tile>> sequences)
    {
        int totalSum = 0;

        foreach (List<Tile> sequence in sequences)
        {
            int expectedNumber = sequence[0].GetNumber();

            foreach (Tile tile in sequence)
            {
                totalSum += tile.GetNumber() == 30 ? expectedNumber : tile.GetNumber();
                if (tile.GetNumber() != 30) expectedNumber = tile.GetNumber() ;
            }
        }

        return totalSum >= 30;
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
        if (tiles.Count != 0)
        {
            int TileIndex = Random.Range(0, (tiles.Count));
            computerPlayerHand.Add(tiles[TileIndex]);
            tiles.RemoveAt(TileIndex);
            SaveListToCopy();
            countJoker = CountJoker(computerPlayerHand);
        }
        else GameController.Instance.EndGame();
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
        int errorAmount = 0;
        int i = 0;
        while (i < amountToOccupy)
        {
            position = new Vector3Int(levelX, 0, levelZ);
            if (board.ContainsKey(position))
            {
                levelZ = Random.Range(minZ, maxZ + 1);
                levelX = Random.Range(minX, maxX + 1 - tileAmount);
                list.Clear();
                errorAmount++;
                if (errorAmount >= 5000) Debug.LogError("nie ma miejsca na planszy");
                i = 0; 
            }
            else
            {
                list.Add(position);
                levelX++;
                i++;
                errorAmount = 0;
            }
        }



        return list;
    }
    /// <summary>
    /// funkcja do po³o¿enia p³ytki na mapie
    /// </summary>
    /// <param name="gridPosition">pozycja wybrana dla p³ytki</param>
    /// <param name="tile">konkteny obiekt do po³o¿enia na mapie</param>
    void PutTile(Vector3Int gridPosition,Tile tile)
    {

        int index = objectPlacer.PlacedObject(database.objectsData[0].Prefab, grid.CellToWorld(gridPosition),ref tile, grid);

        placementSystem.GetGridData().AddObjectAt(gridPosition,
            database.objectsData[0].Size,
            database.objectsData[0].ID,
            index);
        tile.ShowTiles();
        //Debug.Log("na pozycji:" + gridPosition);
    }
    void ExtendSequence(ref Dictionary<Vector3Int, Tile> board,ref List<Tile> handTiles)
    {
        List<Vector3Int> freePositions =  FindSequencesNeighbours(board);
        freePositions= MoveBugableSequence(ref board, freePositions);
        bool ifBreak = false;
        for(int i = 0;i< handTiles.Count;i++)
        {
            foreach (Vector3Int position in freePositions) 
            {
                if (handTiles[i].CheckTileValidity(position))
                {
                    //postawiæ p³ytkê 
                    PutTile(position, handTiles[i]);
                    handTiles.RemoveAt(i);
                    freePositions.Remove(position);
                    Vector3Int plusjeden = new((position.x + 1), 0, position.z);
                    Vector3Int plusdwa = new((position.x +2), 0, position.z);
                    Vector3Int minusjeden = new((position.x - 1), 0, position.z);
                    Vector3Int minusdwa = new((position.x - 2), 0, position.z);
                    if(board.ContainsKey(plusjeden) && board.ContainsKey(minusdwa))//przesuwamy ten po prawej
                    {
                        List<Vector3Int> oldPositions = new();
                        oldPositions.Add(position);
                        int x = (position.x + 1);
                        int z = position.z;
                        Vector3Int iteratePosition = new(x, 0, z);
                        while (board.ContainsKey(iteratePosition))
                        {
                            oldPositions.Add(iteratePosition);
                            x++;
                            iteratePosition = new(x, 0, z);
                        }
                        //stare pozycje ju¿ s¹, albo powinny byæ
                        List<Vector3Int> newPositions = FreeSpaceToPut(oldPositions.Count);
                        moveTile(newPositions, oldPositions);
                    }
                    else if(board.ContainsKey(minusjeden) && board.ContainsKey(plusdwa))
                    {
                        List<Vector3Int> oldPositions = new();
                        oldPositions.Add(position);
                        int x = (position.x - 1);
                        int z = position.z;
                        Vector3Int iteratePosition = new(x, 0, z);
                        while (board.ContainsKey(iteratePosition))
                        {
                            oldPositions.Add(iteratePosition);
                            x--;
                            iteratePosition = new(x, 0, z);
                        }
                        oldPositions.Sort((a, b) => a.x.CompareTo(b.x));
                        List<Vector3Int> newPositions = FreeSpaceToPut(oldPositions.Count);
                        moveTile(newPositions, oldPositions);
                    }

                    if(position.x < minX)//jeœli jest poza map¹ z lewej
                    {
                        List<Vector3Int> oldPositions = new ();
                        oldPositions.Add(position);
                        int x = (position.x+1);
                        int z  = position.z;
                        Vector3Int iteratePosition = new(x,0,z);
                        while(board.ContainsKey(iteratePosition))
                        {
                            oldPositions.Add(iteratePosition);
                            x++;
                            iteratePosition = new(x, 0, z);
                        }
                        //stare pozycje ju¿ s¹, albo powinny byæ
                        List<Vector3Int> newPositions = FreeSpaceToPut(oldPositions.Count);
                        moveTile(newPositions, oldPositions);
                        // FreeSpaceToPut
                        //przesun¹æ na prawo
                    }
                    else if (position.x > maxX)//jeœli jest poza map¹ z prawej
                    {
                        List<Vector3Int> oldPositions = new();
                        oldPositions.Add(position);
                        int x = (position.x - 1);
                        int z = position.z;
                        Vector3Int iteratePosition = new(x, 0, z);
                        while (board.ContainsKey(iteratePosition)) 
                        {
                            oldPositions.Add(iteratePosition);
                            x--;
                            iteratePosition = new(x, 0, z);
                        }
                        oldPositions.Sort((a, b) => a.x.CompareTo(b.x));
                        List<Vector3Int> newPositions = FreeSpaceToPut(oldPositions.Count);
                        moveTile(newPositions, oldPositions);
                        //przesun¹æ w lewo
                    }
                    
                    ifBreak = true;
                    break;
                }
                
            }
            if (ifBreak) break;
        }
    }
    List<Vector3Int> FindSequencesNeighbours(Dictionary<Vector3Int, Tile> board)
    {
        List <Vector3Int> freeSpace = new List<Vector3Int>();
        //int maxX = 8;
        //int minX = -9;
        //int maxZ = 2;
        //int minZ = -4;
        Vector3Int tempPosition = new Vector3Int();
        Vector3Int temp = new Vector3Int();
        for(int z = minZ; z <= maxZ; z++)
        {
            for(int x = minX;x<=maxX;x++)
            {
                temp = new((x-1), 0, z);
                tempPosition = new(x, 0, z);
                if (board.ContainsKey(tempPosition))
                {
                    
                    freeSpace.Add(temp);
                    while (board.ContainsKey(tempPosition))
                    {
                        x++;
                        tempPosition = new(x, 0, z);

                    }
                    freeSpace.Add(tempPosition);
                }
            }
        }

        return freeSpace;
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

    /// <summary>
    /// funkcja do przeniesienia ca³ej sekwencji p³ytek na mapie
    /// </summary>
    /// <param name="newGridPositions">lista nowych pozycji dla p³ytek</param>
    /// <param name="oldGridPositions">lista starych pozycji p³ytek</param>
    void moveTile(List <Vector3Int> newGridPositions, List<Vector3Int> oldGridPositions)
    {
        Dictionary<Vector3Int, Tile> board = GameController.Instance.GetBoardDictionary().board;
        for (int i =1,j=0;i< newGridPositions.Count;i++,j++)
        {
            if (oldGridPositions.Count > j && board.ContainsKey(oldGridPositions[j]))
            {
                int selectedObjectIndex = placementSystem.GetGridData().getRepresentationIndex(oldGridPositions[j]);
                placementSystem.GetGridData().MoveObjectAt(newGridPositions[i], oldGridPositions[j], database.objectsData[0].Size);
                objectPlacer.MoveObjectTo(selectedObjectIndex, grid.CellToWorld(newGridPositions[i]));
                GameController.Instance.GetBoardDictionary().MoveObjectAt(newGridPositions[i], oldGridPositions[j]);
            }
            else break;

        }
        //void MoveObjectTo(int gameObjectIndex, Vector3 newPosition)//indeks z listy i pozycja ostateczna
    }
    /// <summary>
    /// Przesuniêcie losowej sekwencji jeœli znajduj¹ siê bezpoœrednio obok siebie
    /// </summary>
    /// <param name="board">referencja na g³ówn¹ mapê</param>
    /// <param name="freePositions">lista pozycji obok sekwencji</param>
    /// <returns>listê wolnych pozycji</returns>
    List<Vector3Int> MoveBugableSequence(ref Dictionary<Vector3Int, Tile> board, List<Vector3Int> freePositions)
    {
        HashSet<Vector3Int> uniquePositions = new HashSet<Vector3Int>();
        List<Vector3Int> duplicatePositions = new List<Vector3Int>();

        foreach (var position in freePositions)
        {
            if (!uniquePositions.Add(position))
            {
                // Jeœli `Add` zwraca `false`, to znaczy, ¿e `position` ju¿ istnieje w `uniquePositions`
                duplicatePositions.Add(position);
            }
        }
        if (duplicatePositions.Count > 0)
        {
            foreach (var position in duplicatePositions)
            {
                // Coœ do zrobienia z ka¿d¹ zduplikowan¹ pozycj¹
                // Debug.Log("Zduplikowana pozycja: " + duplicate);
                bool prawoCzyLewo = Random.value > 0.5f;
                if (prawoCzyLewo)
                {

                    List<Vector3Int> oldPositions = new();
                    //oldPositions.Add(position);
                    int x = (position.x + 1);
                    int z = position.z;
                    Vector3Int iteratePosition = new(x, 0, z);
                    while (board.ContainsKey(iteratePosition))
                    {
                        oldPositions.Add(iteratePosition);
                        x++;
                        iteratePosition = new(x, 0, z);
                    }
                    //stare pozycje ju¿ s¹, albo powinny byæ
                    List<Vector3Int> newPositions = FreeSpaceToPut(oldPositions.Count);
                    moveTile(newPositions, oldPositions);
                }
                else
                {
                    List<Vector3Int> oldPositions = new();
                    //oldPositions.Add(position);
                    int x = (position.x - 1);
                    int z = position.z;
                    Vector3Int iteratePosition = new(x, 0, z);
                    while (board.ContainsKey(iteratePosition))
                    {
                        oldPositions.Add(iteratePosition);
                        x--;
                        iteratePosition = new(x, 0, z);
                    }
                    oldPositions.Sort((a, b) => a.x.CompareTo(b.x));
                    List<Vector3Int> newPositions = FreeSpaceToPut(oldPositions.Count);
                    moveTile(newPositions, oldPositions);
                }
            }
            return FindSequencesNeighbours(board);
        }
        return freePositions;


    }
    /// <summary>
    /// funkcja do uzyskania ostatecznego wyniku gry
    /// </summary>
    /// <returns></returns>
    public int FinalScore()
    {
        int score = 0;
        foreach (Tile tile in computerPlayerHand)
        {
            score += tile.GetNumber();
        }
        return score;
        
    }
}
