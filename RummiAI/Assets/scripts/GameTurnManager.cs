using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTurnManager : MonoBehaviour
{
    public int currentPlayerId;//bêdzie losowe
    public int turnTime;
    public float currentTurnTime;
    [SerializeField]
    GameObject timeText;
    [SerializeField]
    GameObject currentPlayerText;

    [SerializeField]
    TakeTile takeTile;
    [SerializeField]
    TurnController turnController;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayerId = 0;
        turnTime = 60;
        currentTurnTime = turnTime;
        SetUIText();
    }

    // Update is called once per frame
    void Update()
    {
        currentTurnTime -= Time.deltaTime;
        SetTimeUI();
        if (currentTurnTime <= 0)
        {
            if (currentPlayerId == 0)
            {
                turnController.TakeTile();//ju¿ ma ChangeTurn();
            }
            else if (currentPlayerId != 0)
            {
                ///nowa tura i cofniêcie ruchów z mapy jeœli coœ siê wydarzy³o i danie nowej karty graczowi pod indeksem
                ///SetUIText();
                ChangeTurn();
            }
        }
    }

    public void SetUIText()
    {
        if (currentPlayerId == 0)
        {
            currentPlayerText.GetComponent<Text>().text = "Player: You";
            takeTile.EnableAllButtons();
        }
        else
        {
            currentPlayerText.GetComponent<Text>().text = "Player: Computer" + currentPlayerId.ToString();
            takeTile.DisableAllButtons();
        }
        ResetTime();
        SetTimeUI();

    }
    /// <summary>
    /// Ustawia liczbe czasu na przestrzeni ui
    /// </summary>
    public void SetTimeUI()
    {
        timeText.GetComponent<Text>().text = "Time: "+ Mathf.FloorToInt(currentTurnTime);//Mathf.FloorToInt(currentTime)
    }
    public void ChangeTurn()
    {
        currentPlayerId++;
        if (currentPlayerId == 4) currentPlayerId = 0;
        SetUIText();
    }
    /// <summary>
    /// resetuje zmienn¹ float turnTime
    /// </summary>
    public void ResetTime()
    {
        currentTurnTime = turnTime;
    }

}
