using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    List<UnitBase> bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;

    public static event Action ServerOnGameOver;
    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += HandleUnitSpawned;
        UnitBase.ServerOnBaseDeSpawned += HandleUnitDeSpawned;
    }
    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= HandleUnitSpawned;
        UnitBase.ServerOnBaseDeSpawned -= HandleUnitDeSpawned;
    }
    [Server]
    public void HandleUnitSpawned(UnitBase ubase)
    {
        bases.Add(ubase);
    }
    [Server]
    public void HandleUnitDeSpawned(UnitBase ubase)
    {
        bases.Remove(ubase);
        if (bases.Count != 1) return;
        int winnerid = bases[0].connectionToClient.connectionId;
        RpcGameOver($"player {winnerid} ");
        ServerOnGameOver?.Invoke();
    }
    #endregion

    #region Client
    [ClientRpc]
    public void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }
    #endregion
}
