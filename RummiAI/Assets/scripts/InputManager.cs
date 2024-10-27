using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action onClicked, onExit;

    /// <summary>
    /// funkcja wykonuj¹ca siê co ka¿d¹ klatkê
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { 
            onClicked?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Q)) { 
            onExit?.Invoke();
        }
    }
    /// <summary>
    /// funkcja s³u¿¹ca sprawdzeniu czy kursor wykonuj¹cy akcje znajduje siê nad ui czy te¿ nie
    /// </summary>
    /// <returns>zwraca zmienn¹ bool, jeœli kursor bêdzie zwrócony na ui to true, w innym przypadku false</returns>
    public bool isPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// Przekazanie pozycji myszki w zale¿noœci od tego, czy wystzrelony promieñ ray uderzy³ w obiekt czy nie
    /// </summary>
    /// <returns>jeœli obiekt zosta³ wykryty to zwróci pozycjê obiektu, w innym przypadku zwróci poprzedni¹ pozycje</returns>
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }



}
