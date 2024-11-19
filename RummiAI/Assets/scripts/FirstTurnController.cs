using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    List<Vector3Int> positionsList;


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
        positionsList.Clear();
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
        AddPosition(gridPosition);
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
        else
        {
            sum -= number;

        }
          RemovePosition(gridPosition );
    }
    /// <summary>
    /// Funkcja do zmiany pozycji jokera na mapie
    /// </summary>
    /// <param name="gridPosition">pozycja docelowa</param>
    /// <param name="previousPosition">poprzednia pozycja jokera</param>
    public void ChangePosition(Vector3Int gridPosition, Vector3Int previousPosition)
    {
        if (joker1 == previousPosition) joker1 = gridPosition;
        else if (joker2 == previousPosition) joker2= gridPosition;
        MovePosition(gridPosition, previousPosition);

    }

    public bool CheckFirstTurnValidity(Dictionary<Vector3Int, Tile> board)
    {
        if (!FirstJokerNull())
        {
            ///wstaw pozycje jokera do funkcji, która sprawdzi jak¹ wartoœæ zastêpuje
            ///zwrócona wartoœæ ma zostaæ dodana do sumy
            int numer = GetJokerAmount(joker1, board);
            //Debug.Log("joker1 imituje numer: "+ numer);
            sum += numer;
        }
        if (!SecondJokerNull()) 
        {
            ///wstaw pozycje jokera do funkcji, która sprawdzi jak¹ wartoœæ zastêpuje
            ///zwrócona wartoœæ ma zostaæ dodana do sumy
            int numer = GetJokerAmount(joker2, board);
            //Debug.Log("joker2 imituje numer: " + numer);
            sum += numer;
        }

         Debug.Log("Suma wynosi:" + sum);
        return sum>=30 & CheckSequences();
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
            if (board[plusjeden].GetNumber()==30 || board[minusjeden].GetNumber() == 30)
            {
                
                if(board.ContainsKey(plusdwa))
                {
                    //Vector3Int kolejnyJoker;
                    if (board[plusjeden].GetNumber() == 30)
                    {
                        //plusdwa i minusjeden s¹ git
                        if ((board[plusdwa].GetNumber() - 3) == board[minusjeden].GetNumber())
                            return (board[minusjeden].GetNumber() + 1);
                        else if (board[plusdwa].GetNumber() == board[minusjeden].GetNumber())
                            return board[minusjeden].GetNumber();
                        else return 0;
                    }
                    else if (board[minusjeden].GetNumber() == 30)
                    {
                        //plusjeden i plusdwa s¹ git 
                        if ((board[plusjeden].GetNumber() + 1) == board[minusjeden].GetNumber())
                            return (board[plusjeden].GetNumber() - 1);
                        else if (board[plusdwa].GetNumber() == board[plusjeden].GetNumber())
                            return board[plusjeden].GetNumber();
                        else return 0;
                    }
                }
                else if(board.ContainsKey(minusdwa))
                {
                    if (board[plusjeden].GetNumber() == 30)
                    {
                        //minusjeden i minusdwa s¹ git
                        if ((board[minusdwa].GetNumber() + 1 ) == board[minusjeden].GetNumber())
                            return (board[minusjeden].GetNumber() + 1);
                        else if (board[minusdwa].GetNumber() == board[minusjeden].GetNumber())
                            return board[minusjeden].GetNumber();
                        else return 0;
                    }
                    else if (board[minusjeden].GetNumber() == 30)
                    {
                        //plusjeden i minusdwa s¹ git
                        if ((board[minusdwa].GetNumber() +3) == board[plusjeden].GetNumber())
                            return (board[plusjeden].GetNumber() - 1);
                        else if (board[minusdwa].GetNumber() == board[plusjeden].GetNumber())
                            return board[plusjeden].GetNumber();
                        else return 0;
                    }
                }
                else
                {
                    //tu zwracamy wartoœæ jokera jako ¿e s¹ to ró¿ne kolory, ta sama liczba
                    if (board[plusjeden].GetNumber() == 30)
                    {
                        return board[minusjeden].GetNumber();
                    }
                    else if (board[minusjeden].GetNumber() == 30)
                    {
                        return board[plusjeden].GetNumber();
                    }
                    else return 0;
                }
                //Debug.Log("Jest drugi joker"); //do sprawdzenia kolejna pozycja

            }
            else//nie ma drugiego jokera 
            {
                //œrodek
                if (board[plusjeden].GetNumber() == board[minusjeden].GetNumber())
                    return board[plusjeden].GetNumber();
                else if ((board[plusjeden].GetNumber() - 2) == board[minusjeden].GetNumber())
                    return (board[minusjeden].GetNumber()+1);
            }
            //Debug.Log("Joker po œrodku");
        }
        else if(board.ContainsKey(plusjeden) && board.ContainsKey(plusdwa))
        {
            if (board[plusjeden].GetNumber() == 30 || board[plusdwa].GetNumber() == 30)
            {
                Vector3Int plustrzy = new Vector3Int(jokerPosition.x + 3, jokerPosition.y, jokerPosition.z);
                if (board.ContainsKey(plustrzy))
                {
                    //sprawdzenie trzeciego i niejokera
                    if (board[plusjeden].GetNumber() == 30)
                    {
                        //plusdwa i plustrzy git
                        if ((board[plusdwa].GetNumber() + 1) == board[plustrzy].GetNumber())
                            return (board[plusdwa].GetNumber() - 2);
                        else if (board[plusdwa].GetNumber() == board[plustrzy].GetNumber())
                            return board[plusdwa].GetNumber();
                        else return 0;
                    }
                    else if (board[plusdwa].GetNumber() == 30)
                    {
                        //plusjeden i plustrzy git
                        if ((board[plusjeden].GetNumber() + 2) == board[plustrzy].GetNumber())
                            return (board[plusjeden].GetNumber() - 1);
                        else if (board[plusjeden].GetNumber() == board[plustrzy].GetNumber())
                            return board[plusjeden].GetNumber();
                        else return 0;
                    }
                    else return 0;
                }
                else
                {
                    //tu zwracamy wartoœæ jokera jako ¿e s¹ to ró¿ne kolory, ta sama liczba
                    if (board[plusjeden].GetNumber() == 30)
                        return board[plusdwa].GetNumber();
                    else if (board[plusdwa].GetNumber() == 30)
                        return board[plusjeden].GetNumber();
                }
                Debug.Log("Jest drugi joker");//do sprawdzenia kolejna pozycja
            }

            else//nie ma drugiego jokera
            {
                if (board[plusjeden].GetNumber() == board[plusdwa].GetNumber())
                    return board[plusjeden].GetNumber();
                else if ((board[plusjeden].GetNumber() +1 ) == board[plusdwa].GetNumber())
                    return (board[plusjeden].GetNumber() - 1);
            }
            Debug.Log("Joker po lewej");
        }
        else if (board.ContainsKey(minusjeden) && board.ContainsKey(minusdwa))
        {
            if (board[minusjeden].GetNumber() == 30 || board[minusdwa].GetNumber() == 30) 
            {
                Vector3Int minustrzy = new Vector3Int(jokerPosition.x - 3, jokerPosition.y, jokerPosition.z);
                if (board.ContainsKey(minustrzy))
                {
                    //sprawdzenie trzeciego i niejokera
                    if(board[minusjeden].GetNumber() == 30)
                    {
                        //minusdwa i minustrzy git
                        if ((board[minusdwa].GetNumber() - 1) == board[minustrzy].GetNumber())
                            return (board[minusdwa].GetNumber() + 2);
                        else if (board[minusdwa].GetNumber() == board[minustrzy].GetNumber())
                            return board[minusdwa].GetNumber();
                        else return 0;
                    }
                    else if(board[minusdwa].GetNumber() == 30)
                    {
                        //minusjeden i minustrzy git
                        if ((board[minusjeden].GetNumber() - 2) == board[minustrzy].GetNumber())
                            return (board[minusjeden].GetNumber() + 1);
                        else if (board[minusjeden].GetNumber() == board[minustrzy].GetNumber())
                            return board[minusjeden].GetNumber();
                        else return 0;
                    }
                }
                else
                {
                    //tu zwracamy wartoœæ jokera jako ¿e s¹ to ró¿ne kolory, ta sama liczba
                    if (board[minusjeden].GetNumber() == 30)
                        return board[minusdwa].GetNumber();
                    else if (board[minusdwa].GetNumber() == 30)
                        return board[minusjeden].GetNumber(); 
                    else return 0;
                }
                Debug.Log("Jest drugi joker");
            }//do sprawdzenia kolejna pozycja'
            else //nie ma drugiego jokera
            {
                if (board[minusjeden].GetNumber() == board[minusdwa].GetNumber())
                    return board[minusjeden].GetNumber();
                else if ((board[minusjeden].GetNumber() - 1) == board[minusdwa].GetNumber())
                    return (board[minusjeden].GetNumber() + 1);
                else return 0;
            }
            Debug.Log("Joker po prawej");
        }
        //else Debug.Log("B³¹d");
        return 0; 
    }

    public void AddPosition(Vector3Int position)
    {
        positionsList.Add(position);
    }
    public void RemovePosition(Vector3Int position) 
    {
        for (int i = 0; i < positionsList.Count; i++) 
        {
            if( positionsList[i]== position)
            {
                positionsList.RemoveAt(i);
                break;
            }


        }
    }
    public void MovePosition(Vector3Int newPosition, Vector3Int previousPosition) 
    {
        for (int i = 0; i < positionsList.Count; i++)
        {
            if (positionsList[i] == previousPosition)
            {
                positionsList[i] = newPosition;
                break;
            }
        }
    }

    public bool CheckSequences()
    {

        // Grupowanie pozycji wed³ug wspólnego 'z'
        var groups = positionsList
            .GroupBy(position => position.z)
            .ToList();

        foreach (var group in groups)
        {
            // Sortowanie pozycji wed³ug 'x' w ka¿dej grupie
            var sortedGroup = group
                .OrderBy(position => position.x)
                .ToList();

            int consecutiveCount = 1;
            bool hasValidSequence = false;

            for (int i = 1; i < sortedGroup.Count; i++)
            {
                if (sortedGroup[i].x == sortedGroup[i - 1].x + 1)
                {
                    consecutiveCount++;
                }
                else
                {
                    // Sprawdzanie, czy zakoñczona sekwencja jest poprawna
                    if (consecutiveCount >= 3)
                    {
                        hasValidSequence = true;
                    }
                    consecutiveCount = 1; // Rozpoczynanie nowej sekwencji
                }
            }

            // Sprawdzanie ostatniej sekwencji w grupie
            if (consecutiveCount >= 3)
            {
                hasValidSequence = true;
            }
            else hasValidSequence = false;

            if (!hasValidSequence)
            {
                return false; // Jeœli jakikolwiek poziom z nie ma poprawnych sekwencji
            }
        }

        return true; // Wszystkie poziomy z maj¹ poprawne sekwencje
    }

}
