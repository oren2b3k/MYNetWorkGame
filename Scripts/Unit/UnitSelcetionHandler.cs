using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSelcetionHandler : MonoBehaviour
{
    [SerializeField] LayerMask layerMask=new LayerMask();
    Camera m_cam;
    List<Unit> selected_units = new List<Unit>();
    public List<Unit> GetSelectedUnits() => selected_units;

    [SerializeField] RectTransform m_rectTransform;
    RTSPlayer m_player;
    Vector2 m_start_Mouse_position;
    private void Start()
    {

        m_cam = Camera.main;
        m_player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        Unit.AhthorityOnUnitDeSpawnd += AhthorityOnHandleUnitDeSpawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        enabled = false;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        Unit.AhthorityOnUnitDeSpawnd -= AhthorityOnHandleUnitDeSpawned;
    }
    private void AhthorityOnHandleUnitDeSpawned(Unit obj)
    {      
           selected_units.Remove(obj);        
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Start Selection and clearing the last one
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            //Stop Selection and hovering it
            EndSelectingArea();
        }
    }


    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selcted_unit in selected_units)
            {
                selcted_unit.DeSelect();
            }
            selected_units.Clear();
        }
        m_rectTransform.gameObject.SetActive(true);
        m_start_Mouse_position=Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }
    private void UpdateSelectionArea()
    {
        Vector2 mousepotion=Mouse.current.position.ReadValue();
        float width=mousepotion.x-m_start_Mouse_position.x;
        float high = mousepotion.y - m_start_Mouse_position.y;
         m_rectTransform.sizeDelta=new Vector2(Mathf.Abs(width),Mathf.Abs(high));
        m_rectTransform.anchoredPosition = m_start_Mouse_position+new Vector2(width/2,high/2);
    }
    private void EndSelectingArea()
    {
        m_rectTransform.gameObject.SetActive(false);
        if (m_rectTransform.sizeDelta.magnitude == 0)//Only Clicked
        {
            Ray ray = m_cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
            if (!unit.hasAuthority) return;
            selected_units.Add(unit);
            foreach (Unit selcted_unit in selected_units)
            {
                selcted_unit.Select();
            }
            return;
        }
        Vector2 min = m_rectTransform.anchoredPosition-m_rectTransform.sizeDelta/2;
        Vector2 max = m_rectTransform.anchoredPosition + m_rectTransform.sizeDelta / 2;
        foreach (Unit unit in m_player.GetMyUnits())
        {
            if (selected_units.Contains(unit)) continue;
            Vector2 screenpoint = m_cam.WorldToScreenPoint(unit.transform.position);
            if(screenpoint.x > min.x&&screenpoint.x<max.x&&screenpoint.y>min.y&&screenpoint.y<max.y)
            {
                selected_units.Add(unit);
                unit.Select();
            }
        }
    }
}
