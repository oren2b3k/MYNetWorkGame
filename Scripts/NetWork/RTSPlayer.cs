using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> myUnits = new List<Unit>();
    [SerializeField]private List<Building> mybuildings = new List<Building>();

    [SerializeField] private Transform cameraTransform = null;
    [SyncVar(hook =nameof(ClientUpdateResurces))]
     int resurces=500;
    public event Action<int> ClientHandleResucesUpdated;
    [SerializeField]Building[] allbuildings;

    [SyncVar(hook =nameof(AuthorityHandleOnPartyOwnerupdated))]
    [SerializeField]bool IsParyOwner=false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    string DisplayName;
    public string GetDisplayName() => DisplayName;

    public bool GetIsParyOwner() => IsParyOwner;
    [SerializeField] LayerMask buildblocklayer = new LayerMask();
    [SerializeField] float buildingrangelimit=5;
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildblocklayer)) { return false; }
        foreach (Building building in mybuildings)
        {
               if((point - building.transform.position).sqrMagnitude <= buildingrangelimit * buildingrangelimit)
                return true;
        }

            return false;
    }

    public Transform GetCameraTransform() => cameraTransform;

    public static event Action ClientOnInfoUpdated;
    public int GetResurces() => resurces;

    Color Teamcolor;

    public static event Action<bool> AuthorityOnpartyOwnerStateUpdated;

    public Color GetTeamcolor() => Teamcolor;
    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDeSpawned += ServerHandleUnitDespawned;
        Building.ServerOnBildingSpawnend += ServerHandleBuildingSpawaend;
        Building.ServerOnBildingDeSpawnend += ServerHandleBuildingDeSpawaend;
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void SetTeamColor(Color newteamcolor) => Teamcolor=newteamcolor;
    [Server]
    public void SetResurces(int newresurses) => resurces=newresurses;
    [Server]
    public void SetDisplayName(string newdisplayname)=>DisplayName=newdisplayname;
    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDeSpawned -= ServerHandleUnitDespawned;
        Building.ServerOnBildingSpawnend -= ServerHandleBuildingSpawaend;
        Building.ServerOnBildingDeSpawnend -= ServerHandleBuildingDeSpawaend;
    }
    [Server]
    public void SetIsParyOwner(bool partyowner) { IsParyOwner = partyowner; }
    [Command]
    public void CmdStartGame()
    {
        if (!GetIsParyOwner()) return;
        ((CustomNetWorkManger)NetworkManager.singleton).StartGame();
    }
    [Command]
    public void CmdTryPlaceBuilding(int buildingid,Vector3 point)
    {
        Building buildingtoplace=null;
        for (int i = 0; i < allbuildings.Length; i++)
        {
            if (buildingid == allbuildings[i].Getid())
            {
                buildingtoplace = allbuildings[i];break;
            }
        }
        if (buildingtoplace == null) return;
        if (resurces < buildingtoplace.Getprice()) return;
        if (!CanPlaceBuilding(buildingtoplace.GetComponent<BoxCollider>(),point)) return;
        GameObject buildingtoplatinstance=  Instantiate(buildingtoplace.gameObject,point,Quaternion.identity);
        NetworkServer.Spawn(buildingtoplatinstance, connectionToClient);
        SetResurces(resurces-buildingtoplace.Getprice());
    }
    private void ServerHandleBuildingDeSpawaend(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        mybuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawaend(Building building)
    {
        if (building.connectionToClient != connectionToClient) return;
        mybuildings.Add(building);
    }
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }


    #endregion

    #region Client

    public void ClientUpdateResurces(int oldresurces, int currentresurces)
    {
        ClientHandleResucesUpdated?.Invoke(currentresurces);
    }
    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }
        Unit.AhthorityOnUnitSpawnd += AuthorityHandleUnitSpawned;
        Unit.AhthorityOnUnitDeSpawnd += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBildingSpawnend += AuthorityHandleOnBuildingSpawned;
        Building.AuthorityOnBildingDeSpawnend += AuthorityHandleOnBuildingDeSpawned;
    }
    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }
        DontDestroyOnLoad(this.gameObject);
       ((CustomNetWorkManger)NetworkManager.singleton).Players.Add(this);Debug.Log("add");
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        if (!isClientOnly) return;
        ((CustomNetWorkManger)NetworkManager.singleton).Players.Remove(this); Debug.Log("remove");
        if (!hasAuthority) { return; }
        Unit.AhthorityOnUnitSpawnd -= AuthorityHandleUnitSpawned;
        Unit.AhthorityOnUnitDeSpawnd -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBildingSpawnend -= AuthorityHandleOnBuildingSpawned;
        Building.AuthorityOnBildingDeSpawnend -= AuthorityHandleOnBuildingDeSpawned;
    }
    private void AuthorityHandleOnPartyOwnerupdated(bool oldbool,bool newbool)
    {
        if (!hasAuthority) return;
        Debug.Log("Invoke Owner Function");
        AuthorityOnpartyOwnerStateUpdated?.Invoke(newbool);
    }
    private void AuthorityHandleOnBuildingSpawned(Building building)
    {
        mybuildings.Add(building);
    }
    public void ClientHandleDisplayNameUpdated(string oldname, string newname)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    private void AuthorityHandleOnBuildingDeSpawned(Building building)
    {
        mybuildings.Remove(building);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    #endregion

}
