using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class Building : NetworkBehaviour
{
    [SerializeField] Sprite icone;
    [SerializeField] int id = -1;

    [SerializeField] int price = 100;

    [SerializeField] GameObject previewbuilding;

    public GameObject GetPreviewbuilding() => previewbuilding;
    public int Getid() => id;
    public int Getprice() => price;
    public Sprite Geticone() => icone;

    public static event Action<Building> ServerOnBildingSpawnend;
    public static event Action<Building> ServerOnBildingDeSpawnend;

    public static event Action<Building> AuthorityOnBildingSpawnend;
    public static event Action<Building> AuthorityOnBildingDeSpawnend;
    #region Client 

    public override void OnStartAuthority()
    {
        AuthorityOnBildingSpawnend?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnBildingDeSpawnend?.Invoke(this);
    }
    #endregion

    #region Server
    public override void OnStartServer()
    {
        ServerOnBildingSpawnend?.Invoke(this);
    }
    public override void OnStopServer()
    {
        ServerOnBildingDeSpawnend?.Invoke(this);
    }
    #endregion
}
