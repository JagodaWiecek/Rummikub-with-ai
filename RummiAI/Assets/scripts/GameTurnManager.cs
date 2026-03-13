using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// klasa do aktualizowania wygl¹du interfejsu gry
/// </summary>
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
    GameObject AITilesText;

    [SerializeField]
    TakeTile takeTile;
    [SerializeField]
    public TurnController turnController;

    // Start is called before the first frame update
    void Start()
    {
        //if else (GameController.Instance.gameIndex == 0) currentPlayerId = 0; //TO REMOVE !!
        if(GameController.Instance.gameIndex != 3) currentPlayerId = Random.Range(0,4);
        else currentPlayerId = Random.Range(0, 2);
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
        else if(GameController.Instance.gameIndex == 1 || GameController.Instance.gameIndex == 4)
        {

            if(currentTurnTime <= 0)
            {
                if (currentPlayerId == 0)
                {
                    GameController.Instance.GetPlayerAI().EndOfTime();
                    ChangeTurn();
                }
                else 
                    ChangeTurn();
            }
        }
        else if (GameController.Instance.gameIndex == 2)
        {
            if (currentTurnTime <= 0)
            {
                if (currentPlayerId == 3)
                {
                    GameController.Instance.GetPlayerAI().EndOfTime();
                    ChangeTurn();
                }
                else if (currentPlayerId == 0)
                {
                    turnController.TakeTile();//ju¿ ma ChangeTurn();
                }
                else
                    ChangeTurn();
            }
        }
        else if (GameController.Instance.gameIndex == 3)
        {
            if (currentTurnTime <= 0)
            {
                if (currentPlayerId == 1)
                {
                    GameController.Instance.GetPlayerAI().EndOfTime();
                    ChangeTurn();
                }
                else if (currentPlayerId == 0)
                {
                    turnController.TakeTile();//ju¿ ma ChangeTurn();
                }
                else
                    ChangeTurn();
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
        else if (GameController.Instance.gameIndex == 1)
        {
            if (currentPlayerId == 0)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: Agent";
                //takeTile.EnableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 1)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: MrComputer";
                //takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 2)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: MissComputer";
                // takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 3)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: Computer";
                //takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            SetPlayersTileCountUI();
            ResetTime();
            SetTimeUI();
            //gra boty vs ai
        }
        else if (GameController.Instance.gameIndex == 2)
        {
            //gra player vs ai vs boty
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
                currentPlayerText.GetComponent<Text>().text = "Player: Agent";
                takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            SetPlayersTileCountUI();
            ResetTime();
            SetTimeUI();
        }
        else if (GameController.Instance.gameIndex == 3)
        {
            if (currentPlayerId == 0)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: You";
                takeTile.EnableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 1)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: Agent";
                takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            SetPlayersTileCountUI();
            ResetTime();
            SetTimeUI();
            // gra player vs ai
        }
        else if (GameController.Instance.gameIndex == 4)
        {
            if (currentPlayerId == 0)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: Agent";
                //takeTile.EnableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 1)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: MrComputer";
                //takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 2)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: MissComputer";
                // takeTile.DisableAllButtons();
                //SetPlayersTileCountUI();
            }
            else if (currentPlayerId == 3)
            {
                currentPlayerText.GetComponent<Text>().text = "Player: Computer";
                //takeTile.DisableAllButtons();
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
        else if (GameController.Instance.gameIndex == 1 || GameController.Instance.gameIndex == 4)
        {
            MissComputerTilesText.GetComponent<Text>().text = "MissComputer: " + GameController.Instance.GetMissComputerPlayer().GetList().Count.ToString();
            MrComputerTilesText.GetComponent<Text>().text = "MrComputer: " + GameController.Instance.GetMrComputerPlayer().GetList().Count.ToString();
            ComputerTilesText.GetComponent<Text>().text = "Computer: " + GameController.Instance.GetComputerPlayer().GetList().Count.ToString();
            AITilesText.GetComponent<Text>().text = "Agent: " + GameController.Instance.GetPlayerAI().GetList().Count.ToString();// 
        }
        else if(GameController.Instance.gameIndex == 2)
        {
            MissComputerTilesText.GetComponent<Text>().text = "MissComputer: " + GameController.Instance.GetMissComputerPlayer().GetList().Count.ToString();
            MrComputerTilesText.GetComponent<Text>().text = "MrComputer: " + GameController.Instance.GetMrComputerPlayer().GetList().Count.ToString();
            AITilesText.GetComponent<Text>().text = "Agent: " + GameController.Instance.GetPlayerAI().GetList().Count.ToString();// 
        }
        else if (GameController.Instance.gameIndex == 3)
        {
            AITilesText.GetComponent<Text>().text = "Agent: " + GameController.Instance.GetPlayerAI().GetList().Count.ToString();// 
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
        if (GameController.Instance.gameIndex == 3)
            if (currentPlayerId == 2) currentPlayerId = 0;
        
        if (currentPlayerId == 4) currentPlayerId = 0;

        if (GameController.Instance.gameIndex != 1 && GameController.Instance.gameIndex != 4) { takeTile.SetButtonNumber(); }
        EndTurn();
        SetUIText();
    }
    public void EndTurn()
    {
        foreach (KeyValuePair<Vector3Int, Tile> tile in GameController.Instance.GetBoardDictionary().board)
        {
            tile.Value.SetPut(true);
        }
    }
    /// <summary>
    /// resetuje zmienn¹ float turnTime
    /// </summary>
    public void ResetTime()
    {
        currentTurnTime = turnTime;
    }

    public void Reset()
    {
        currentPlayerId = Random.Range(0, 4);
        turnTime = 60;
        currentTurnTime = turnTime;
        SetUIText();
    }

}
