using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Targeter : NetworkBehaviour
{
    [SerializeField]Targetable target;

    [SerializeField] Targetable lasttaget;


    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
        enabled = false;
    }

    [Command]
    public void CmdSetTarget(GameObject targetgameobj)
    {
        if (!targetgameobj.TryGetComponent<Targetable>(out Targetable targeted)) return;
        Settarget(targeted);
    }

    public void Settarget(Targetable newtarget) { lasttaget = target; target = newtarget; }

    public Targetable GetTarget() => target;
    public Targetable GetLastTarget() => lasttaget;

    [Server]
    public void ClearTarget()
    {
        target=null;
    }
}
