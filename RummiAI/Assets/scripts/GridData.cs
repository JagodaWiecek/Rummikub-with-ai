using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
    
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID, 
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new(positionToOccupy,ID,placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"place is occupied by other tile {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        //Debug.Log(returnVal[0].x +" "+ returnVal[0].y + " " + returnVal[0].z);
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }

        return true;
    }

    internal int getRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiecPositons) { 
            placedObjects.Remove(pos);
        }
    }
}
public class PlacementData
{
    public List<Vector3Int> occupiecPositons = new();

    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

   // public Tile tile { get; private set; } //mo¿e do u¿ycia
    
    public PlacementData(List<Vector3Int> occupiecPositons, int iD, int placedObjectIndex)
    {
        this.occupiecPositons = occupiecPositons;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }

}

