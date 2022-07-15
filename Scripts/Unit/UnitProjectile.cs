using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float destroyaftertime;
    [SerializeField] float speed;

    [SerializeField] int damge;
    private void Start()
    {
        rb.velocity=transform.forward*speed;
    }
    public override void OnStartServer()
    {
       Invoke(nameof(SelfDestroy),destroyaftertime);
    }
    [Server] 
    public void SelfDestroy()
    { 
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))        
            if (connectionToClient == networkIdentity.connectionToClient) return;
        if (networkIdentity.TryGetComponent<Health>(out Health health))
        { health.DealDamge(damge); }
        SelfDestroy();
    }
}
