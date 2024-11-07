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
    GameObject MissComputerTilesText;
    [SerializeField]
    GameObject MrComputerTilesText;
    [SerializeField]
    GameObject ComputerTilesText;

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
        if (GameController.Instance.gameIndex == 0)
        {
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
    }
    /// <summary>
    /// funkcja do ustawienia ui, widocznego dla u¿ytkownika
    /// </summary>
    public void SetUIText()
    {//.GetComponent<Text>().text
        if (GameController.Instance.gameIndex == 0)
        {
            if (currentPlayerId == 0)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: You";
                takeTile.EnableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 1)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: MrComputer";
                takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 2)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: MissComputer";
                takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 3)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: Computer";
                takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            SetPlayersTileCountUI();
            ResetTime();
            SetTimeUI();

        }
    }

    public void SetPlayersTileCountUI()
    {
        if (GameController.Instance.gameIndex == 0)
        {
            MissComputerTilesText.GetComponent<Text>().text = "MissComputer: " + GameController.Instance.GetMissComputerPlayer().GetList().Count.ToString();
            MrComputerTilesText.GetComponent<Text>().text = "MrComputer: " + GameController.Instance.GetMrComputerPlayer().GetList().Count.ToString();
            ComputerTilesText.GetComponent<Text>().text = "Computer: " + GameController.Instance.GetComputerPlayer().GetList().Count.ToString();
        }
    }
    /// <summary>
    /// Ustawia liczbe czasu na przestrzeni ui
    /// </summary>
    public void SetTimeUI()
    {
        timeText.GetComponent<Text>().text = "Time: "+ Mathf.FloorToInt(currentTurnTime);
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
