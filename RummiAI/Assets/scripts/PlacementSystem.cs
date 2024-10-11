using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    private void Update()
    {

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;

        if (grid.CellToWorld(gridPosition).x > 7.875 && grid.CellToWorld(gridPosition).x < 28 && grid.CellToWorld(gridPosition).z >8 && grid.CellToWorld(gridPosition).z < 18.5)// cellIndicator.transform.position.z = 19.15;
        {
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        }
    }
}
