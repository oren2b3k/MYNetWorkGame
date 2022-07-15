using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;
public class Unit : NetworkBehaviour
{

    [SerializeField] UnitMovement unitmovement;
    public UnitMovement GetUnitMovement() => unitmovement;
    [SerializeField] Targeter targeter;
     public Targeter GetTargeter() => targeter;

    [SerializeField] int cost = 10;
    public int GetCost() => cost;
    [SerializeField] Health health;

    #region Client
    //client spawn unit (not relrtiv to the Server)
    [SerializeField] UnityEvent OnSelected;
    [SerializeField] UnityEvent OnDeselected;
    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        OnSelected?.Invoke();
    }
    [Client]
    public void DeSelect()
    {
        if (!hasAuthority) return;
        OnDeselected?.Invoke();
    }

    public static event Action<Unit> AhthorityOnUnitSpawnd;
    public static event Action<Unit> AhthorityOnUnitDeSpawnd;
    public override void OnStartAuthority()
    {
        AhthorityOnUnitSpawnd?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AhthorityOnUnitDeSpawnd?.Invoke(this);
    }
    #endregion

    #region Server
    //Server spawn unit (not relrtiv to the client)
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDeSpawned;
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie+= HandleDeathOnServer;
    }
    public override void OnStopServer()
    {
        ServerOnUnitDeSpawned?.Invoke(this);
        health.ServerOnDie -= HandleDeathOnServer;
    }
    [Server]
    public void HandleDeathOnServer()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion
}
