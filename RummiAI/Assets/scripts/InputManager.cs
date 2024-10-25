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

    public event Action onClicked, onExit, onMove;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) { 
            onClicked?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Q)) { 
            onExit?.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            onMove?.Invoke();
        }
    }

    public bool isPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

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

    public Vector3 GetMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        Vector3 position = ray.GetPoint(100);
        return position;
    }


}
