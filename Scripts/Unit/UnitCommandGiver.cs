using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] UnitSelcetionHandler unitSelcetionHandler;
    [SerializeField] LayerMask layerMask=new LayerMask();
    Camera m_cam;
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        m_cam = Camera.main;
    }
    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
    private void ClientHandleGameOver(string obj)
    {
      enabled = false;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;
        Ray ray=m_cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, layerMask)) return;
        if (hit.collider.TryGetComponent<Targetable>(out Targetable targetable))
        {
            if (targetable.hasAuthority)
            {
                TryMoveTo(hit.point);return;
            }
            TrySetTarget(targetable);
            return;
        }
        TryMoveTo(hit.point);
    }

    private void TrySetTarget(Targetable t)
    {
        foreach (Unit unit in unitSelcetionHandler.GetSelectedUnits())
        {
            unit.GetTargeter().CmdSetTarget(t.gameObject);
        }
    }

    private void TryMoveTo(Vector3 point)
    {
        foreach (Unit unit in unitSelcetionHandler.GetSelectedUnits())
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
