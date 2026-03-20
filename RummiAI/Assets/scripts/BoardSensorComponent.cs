using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Collections.Generic;

public class BoardSensorComponent : SensorComponent
{
    public int minX = -12, maxX = 11, minZ = -4, maxZ = 5;

    public override ISensor[] CreateSensors()
    {
        int width = maxX - minX + 1;
        int height = maxZ - minZ + 1;

        // Przekazujemy funkcjê GetBoardSafe, która nie wywali b³êdu
        return new ISensor[] {
            new BoardSensor("RummikubBoard", width, height, minX, minZ, GetBoardSafe)
        };
    }

    private Dictionary<Vector3Int, Tile> GetBoardSafe()
    {
        // Sprawdzamy czy Instance istnieje
        if (GameController.Instance == null) return null;

        var boardDict = GameController.Instance.GetBoardDictionary();
        // Sprawdzamy czy s³ownik istnieje
        if (boardDict == null) return null;

        return boardDict.board;
    }
}