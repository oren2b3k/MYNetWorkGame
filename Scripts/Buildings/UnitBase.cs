using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] Health health;

    #region Server

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDeSpawned;

    public static event Action<int> ServerOnPlayerDie;
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerOnHandleDeath;
        ServerOnBaseSpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerOnHandleDeath;
        ServerOnBaseDeSpawned?.Invoke(this);
    }
    [Server]
    private void ServerOnHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }
    #endregion


}
