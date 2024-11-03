using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstTurnController : MonoBehaviour
{
    [SerializeField]
    int sum ;
    [SerializeField]
    int jokerAmount ;//max 2
    [SerializeField]
    Vector3Int joker1;
    [SerializeField]
    Vector3Int joker2;


    // Start is called before the first frame update
    void Start()
    {
        joker1 = new Vector3Int(-20, -20, -20);
        joker2 = new Vector3Int(-20, -20, -20);
        sum = 0;
        jokerAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FirstTurnController()
    {
        joker1 = new Vector3Int(-20, -20, -20);
        joker2 = new Vector3Int(-20, -20, -20);
        sum = 0;
        jokerAmount = 0;
    }
    public void Reset()
    {
        sum = 0;
        jokerAmount = 0;
        joker1 = new Vector3Int(-20, -20, -20);
        joker2 = new Vector3Int(-20, -20, -20);
    }
    /// <summary>
    /// funkcja s³u¿y do poinformowania ¿e w zmiennej joker1 znajduje siê informacja czy jest z niej obiekt czy nie
    /// </summary>
    /// <returns>true gdy obiekt jest "pusty", false gdy posiada pozycje</returns>
    public bool FirstJokerNull()
    {
        if(joker1 == new Vector3Int(-20, -20, -20))
            return true;    
        return false;
    }
    /// <summary>
    /// funkcja s³u¿y do poinformowania ¿e w zmiennej joker2 znajduje siê informacja czy jest z niej obiekt czy nie
    /// </summary>
    /// <returns>true gdy obiekt jest "pusty", false gdy posiada pozycje</returns>
    public bool SecondJokerNull()
    {
        if (joker2 == new Vector3Int(-20, -20, -20))
            return true;
        return false;
    }

    /// <summary>
    /// Wyczyszczenie wartoœci jokera przy usuwaniu
    /// </summary>
    /// <param name="gridPosition"></param>
    public void SetNullJoker(Vector3Int gridPosition)
    {
        if (joker1 == gridPosition) joker1 = new Vector3Int(-20, -20, -20);
        else if (joker2 == gridPosition) joker2 = new Vector3Int(-20, -20, -20);
        else Debug.LogError("Nie ma na pozycji jokera");
    }
    /// <summary>
    /// Funkcja do zwiêkszenia wartoœci sumy
    /// wywo³ana przy dodawaniu p³ytek na planszê
    /// </summary>
    /// <param name="number">wartoœæ liczbowa p³ytki</param>
    /// <param name="gridPosition">pozycja obiektu, istotna przy stawianiu jokera</param>
    public void Increment(int number, Vector3Int gridPosition)
    {
        if(number==30)
        {
            jokerAmount++;
            if (FirstJokerNull()) joker1 = gridPosition;
            else if (SecondJokerNull()) joker2 = gridPosition;
            else Debug.LogError("B³¹d iloœci jokerów");

        }
        else
        {
            sum += number;
        }
    }
    /// <summary>
    /// Funkcja do zmniejszenia wartoœci sumy
    /// wywo³ana przy usuwaniu p³ytki z planszy
    /// </summary>
    /// <param name="number">wartoœæ liczbowa p³ytki</param>
    /// <param name="gridPosition">pozycja obiektu, istotna przy stawianiu jokera</param>
    public void Decrease(int number, Vector3Int gridPosition)
    {
        if (number == 30)
        {
            jokerAmount--;
            SetNullJoker(gridPosition);
        }
        else sum-=number;
    }
    /// <summary>
    /// Funkcja do zmiany pozycji jokera na mapie
    /// </summary>
    /// <param name="gridPosition">pozycja docelowa</param>
    /// <param name="previousPosition">poprzednia pozycja jokera</param>
    public void ChangeJokerPosition(Vector3Int gridPosition, Vector3Int previousPosition)
    {
        if (joker1 == previousPosition) joker1 = gridPosition;
        else if (joker2 == previousPosition) joker2= gridPosition;  
    }

    public void CheckFirstTurnValidity(Dictionary<Vector3Int, Tile> board)
    {
        if (!FirstJokerNull())
        {
            ///wstaw pozycje jokera do funkcji, która sprawdzi jak¹ wartoœæ zastêpuje
            ///zwrócona wartoœæ ma zostaæ dodana do sumy
            sum += GetJokerAmount(joker1, board);
        }
        if (!SecondJokerNull()) 
        {
            ///wstaw pozycje jokera do funkcji, która sprawdzi jak¹ wartoœæ zastêpuje
            ///zwrócona wartoœæ ma zostaæ dodana do sumy
            sum += GetJokerAmount(joker2, board);
        }

        Debug.Log("Suma wynosi:" + sum);
    }
    /// <summary>
    /// funkcja do sprawdzenia wartoœci, któr¹ zastêpuje joker
    /// </summary>
    /// <param name="jokerPosition">pozycja sprawdzanego jokera</param>
    /// <param name="board">s³ownik, w którym znajduj¹ siê wszystkie p³ytki</param>
    /// <returns></returns>
    public int GetJokerAmount(Vector3Int jokerPosition, Dictionary<Vector3Int, Tile> board) 
    {
        Vector3Int plusjeden = new Vector3Int(jokerPosition.x + 1, jokerPosition.y, jokerPosition.z);
        Vector3Int plusdwa = new Vector3Int(jokerPosition.x + 2, jokerPosition.y, jokerPosition.z);
        Vector3Int minusjeden = new Vector3Int(jokerPosition.x - 1, jokerPosition.y, jokerPosition.z);
        Vector3Int minusdwa = new Vector3Int(jokerPosition.x - 2, jokerPosition.y, jokerPosition.z);

        if(board.ContainsKey(plusjeden) && board.ContainsKey(minusjeden))
        {
            if (board[plusjeden].GetNumber()==30 || board[minusjeden].GetNumber() == 30) Debug.Log("Jest drugi joker"); //do sprawdzenia kolejna pozycja
            Debug.Log("Joker po œrodku");
        }
        else if(board.ContainsKey(plusjeden) && board.ContainsKey(plusdwa))
        {
            if (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30) Debug.Log("Jest drugi joker");//do sprawdzenia kolejna pozycja
            Debug.Log("Joker po lewej");
        }
        else if (board.ContainsKey(minusjeden) && board.ContainsKey(minusdwa))
        {
            if (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30) Debug.Log("Jest drugi joker");//do sprawdzenia kolejna pozycja
            Debug.Log("Joker po prawej");
        }
        else Debug.Log("B³¹d");
        return 0; 
    }


}
