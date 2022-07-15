using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class BuildingButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Building building;

    [Header("Getting form the building")]
    [SerializeField]GameObject previewbuilding=null;
    [SerializeField] TMP_Text buildingprice;
    [SerializeField] Sprite buildingicone;
    Renderer buildingprevrenderer;


    [Header("layar for floor check")]
    [SerializeField] LayerMask floor = new LayerMask();

    [Header("")]
    RTSPlayer player;
    Camera cam;
    [SerializeField]BoxCollider boxbuilding;

    [SerializeField] GameObject UNitTakeCare;

    private void Start()
    {
        previewbuilding = null;
        cam =Camera.main;
        buildingicone = building.Geticone();
        buildingprice.text=building.Getprice().ToString();
        boxbuilding=building.GetComponent<BoxCollider>();
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }


    private void Update()
    {
        if (previewbuilding == null) return;
        UpdateBuildingpreview();
    }

    private void UpdateBuildingpreview()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floor)) return;
        previewbuilding.transform.position = hit.point;
        if (!previewbuilding.activeSelf) 
            previewbuilding.SetActive(true);
        Color color = player.CanPlaceBuilding(boxbuilding, hit.point) ? Color.green : Color.red;
        buildingprevrenderer.material.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (player.GetResurces() < building.Getprice()) return;
        previewbuilding= Instantiate(building.GetPreviewbuilding());
        previewbuilding.SetActive(false);
        buildingprevrenderer = previewbuilding.GetComponentInChildren<Renderer>();
        UNitTakeCare.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (previewbuilding == null) return;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity,floor))
        {
            player.CmdTryPlaceBuilding(building.Getid(),hit.point);
        }
        UNitTakeCare.SetActive(true);
        Destroy(previewbuilding);
    }
}
