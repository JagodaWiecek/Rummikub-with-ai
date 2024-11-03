using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemovingState : IPlacementState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData tileData;
    ObjectPlacer objectPlacer;
    Tile tile;
    /// <summary>
    /// kontrolelr klasy
    /// klasa odpowiada za usuwanie p³ytek 3d z mapy
    /// </summary>
    /// <param name="grid">obiekt siatki, wykorzystywany do ustalenia pozycji</param>
    /// <param name="previewSystem">klasa tworz¹ca obiekt wizualny dla u¿ytkownika</param>
    /// <param name="tileData">zmienna klasy odpowiedzialnej za przechowywanie danych mapy</param>
    /// <param name="objectPlacer">klasa odpowiedzialna za fizyczne tworzenie i usuwanie obiektów z mapy</param>
    /// <param name="tile">klasa p³ytek rummikib, przechowuje informacje na temat liczby i koloru</param>
    public RemovingState( Grid grid,
                         PreviewSystem previewSystem,
                         GridData tileData,
                         ObjectPlacer objectPlacer,
                         Tile tile)
    {
        
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.tileData = tileData;
        this.objectPlacer = objectPlacer;
        this.tile = tile;

        previewSystem.StartShowingRemovePreview();///tworzy obiekt na mapie jako interaktywny obiekt wizualny

    }
    /// <summary>
    /// funkcja koñcz¹ca usuwanie obiektu z mapy
    /// </summary>
    public void EndState()
    {
        previewSystem.StopShowingPreview();///usuniêcie obiektu wizualnego z mapy
    }
    /// <summary>
    /// funkcja uaktywniaj¹ca swoje g³ówne dzia³anie
    /// usuniêcia obiektu z mapy
    /// </summary>
    /// <param name="gridPosition">pozycja, z której zostanie usuniêty obiekt</param>
    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (tileData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = tileData;
        }
        if (selectedData != null) 
        {
            gameObjectIndex = selectedData.getRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
                return;
            if (GameController.Instance != null && GameController.Instance.GetBoardDictionary().board.ContainsKey(gridPosition) == true)
            {
                if (GameController.Instance.GetBoardDictionary().board[gridPosition].GetPut())
                    Debug.Log("nie mo¿na usun¹æ ju¿ po³o¿onego obiektu");
                else
                {

                    selectedData.RemoveObjectAt(gridPosition);
                    objectPlacer.RemoveObjectAt(gameObjectIndex);
                    if (GameController.Instance.GetPlayer().GetFirstTour())
                    {
                        GameController.Instance.firstTurnController.Decrease(GameController.Instance.GetBoardDictionary().board[gridPosition].getTile().GetNumber(), gridPosition);

                    }
                    objectPlacer.TakeBackTile(GameController.Instance.GetBoardDictionary().board[gridPosition].getTile());//tile
                    GameController.Instance.GetBoardDictionary().board.Remove(gridPosition);//Remove(keyToRemove)
                }
            }
            else return;
        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        //Debug.Log("gridPosition on remove" + gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
        

    }

    /// <summary>
    /// zmienna do sprawdzenia poprawnoœci
    /// przy usuwaniu pokazuje miejsce wskazane przez u¿ytkownika
    /// 
    /// </summary>
    /// <param name="gridPosition">pozycja wskazana przez u¿ytkownika</param>
    /// <returns>zwraca false gdy podana pozycja jest pusta, gdy miejsce jest zajête, to zwróci true</returns>
    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(tileData.CanPlaceObjectAt(gridPosition,Vector2Int.one));
    }

    /// <summary>
    /// funkcja do aktualizacji pozycji obiektu
    /// </summary>
    /// <param name="gridPosition"></param>
    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        if (gridPosition.x > -10 && gridPosition.x < 9 && gridPosition.z > -5 && gridPosition.z < 3) 
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }

    /// <summary>
    /// getter 
    /// </summary>
    /// <returns>zwraca pozycje</returns>
    public Vector3Int GetGridPosition()
    {

        return Vector3Int.zero;
    }
}
