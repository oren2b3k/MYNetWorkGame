using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ResuorsesGanertor : NetworkBehaviour
{
    [SerializeField] Health health;

    [SerializeField]int resuorsesperinterval=5;
    [SerializeField] float interval=1;

    RTSPlayer player;
    float timer;

    public override void OnStartServer()
    {
        timer = interval;
        player =connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServerOnDie;
        GameOverHandler.ServerOnGameOver += ServerOnGameOver;
    }
    public override void OnStopServer()
    {
        health.ServerOnDie += ServerOnDie;
        GameOverHandler.ServerOnGameOver += ServerOnGameOver;
    }
    [ServerCallback]
    private void Update()
    {
        timer-=Time.deltaTime;
        if(timer <= 0)
        {
            player.SetResurces(player.GetResurces()+resuorsesperinterval);
            timer += interval;
        }
    }
    [Server]
    private void ServerOnGameOver()
    {
       enabled = false;
    }
    [Server]
    private void ServerOnDie()
    {
       NetworkServer.Destroy(gameObject);
    }
}
