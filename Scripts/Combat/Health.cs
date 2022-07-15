using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SerializeField] int maxhealthpoints = 100;

    [SyncVar(hook =nameof(HealthChegedRaiseEvent))]
    [SerializeField] int currenthealthpoints = 0;

    public event Action ServerOnDie;
    public event Action<int,int> ClientOnHealthChanged;

    [SyncVar(hook =nameof(GotHitevent))]
    bool IsHit = false;

    public event Action<bool> ClientOnGotHit;
    public bool GetIsHit() => IsHit;
    public void SetIsHit(bool hit)=>IsHit = hit;


    #region Client
    void HealthChegedRaiseEvent(int currenthealth,int newhealth)
    { 
        ClientOnHealthChanged?.Invoke(newhealth, maxhealthpoints);
    }
    void GotHitevent(bool old,bool newvb)
    {
        ClientOnGotHit?.Invoke(newvb);
    }
    #endregion

    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnPlayerDie += ServerOnHandlePlayerDiead;
        currenthealthpoints = maxhealthpoints;
    }
    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerOnHandlePlayerDiead;
    }
    [Server]
    private void ServerOnHandlePlayerDiead(int id)
    {
        if (connectionToClient.connectionId != id) return;
        DealDamge(currenthealthpoints);
    }

    [Server]
    public void DealDamge(int takedamge)
    {
        if (currenthealthpoints == 0) return;
        if(!IsHit)IsHit = true;
        currenthealthpoints = Mathf.Max(currenthealthpoints-takedamge, 0);
        if (currenthealthpoints != 0) return;
        ServerOnDie?.Invoke();
    }
    #endregion


}
