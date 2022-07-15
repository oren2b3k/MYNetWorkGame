using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using System;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]Unit unitprefab;
    [SerializeField] Transform unittransform;

    [SerializeField] Health health;

    [SerializeField] TMP_Text queuespawnunittext;
    [SerializeField] Image unitProssimage;

    [SerializeField] int maxunitsqueue = 5;
    [SerializeField] float spawnmoverange;
    [SerializeField] float unitspawnduretion;

    [SyncVar(hook =nameof(ClientOnHandleQueueunits))]
    [SerializeField] int queuedunits;
    [SyncVar]
    float unittimer;


    RTSPlayer player;
    private float prossimagevielsity;

    private void Update()
    {
        if(isServer)
        {
            Prodesunits();
        }
        if(isClient)
        {
            UnitUpdatetimerDisplay();
        }
    }

    private void UnitUpdatetimerDisplay()
    {
        float Proresses = unittimer / unitspawnduretion;
        if (Proresses < unitProssimage.fillAmount)
        {
            unitProssimage.fillAmount = Proresses;
        }
        else
        {
            unitProssimage.fillAmount =Mathf.SmoothDamp(unitProssimage.fillAmount, Proresses,ref prossimagevielsity,.1f);
        }

    }

    [Server]
    private void Prodesunits()
    {
        if (queuedunits == 0) return;
        unittimer+=Time.deltaTime;
        if (unittimer < unitspawnduretion) return;
        GameObject unitinstance = Instantiate(unitprefab.gameObject, unittransform.position, unittransform.rotation);
        NetworkServer.Spawn(unitinstance, connectionToClient);
        NetworkServer.AddPlayerForConnection(connectionToClient, unitinstance);
         Vector3 Spawnoffect= UnityEngine.Random.insideUnitSphere* spawnmoverange;
        Spawnoffect.y=unittransform.position.y;
        UnitMovement unitmovement = unitinstance.GetComponent<UnitMovement>();
        unitmovement.ServerMoveunit(Spawnoffect+unittransform.position);
        queuedunits--;
        unittimer = 0;
    }
    #region client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!hasAuthority) return;
        CmdSpawnUnit();
    }

    [Client]
    public void ClientOnHandleQueueunits(int oldunitreminig,int newunitsremining)
    {
        queuespawnunittext.text = newunitsremining.ToString();
    }
    #endregion

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDeath;
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDeath;
    }
    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedunits == maxunitsqueue) return;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();
        if (player.GetResurces() < unitprefab.GetCost()) return;
        connectionToClient.identity.GetComponent<RTSPlayer>().SetResurces(player.GetResurces() - unitprefab.GetCost());
        queuedunits++;
    }
    #endregion
}
