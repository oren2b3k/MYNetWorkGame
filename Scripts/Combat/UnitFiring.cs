using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] Targeter targeter;
    //bullet
    [SerializeField] GameObject projectile;
    [SerializeField] Transform pointToShot;

    [SerializeField] float speedrotation;
    [SerializeField] float shootrange;
    [SerializeField] float firerate = 2;
    //bullet setting

    float lasttimefired = 0;

    [ServerCallback]
    private void Update()
    {
        if (!CanShotAtTarget()) return;
        Shoot();
    }
    [ServerCallback]
    private void Shoot()
    {
        Quaternion targedirection = Quaternion.LookRotation(targeter.GetTarget().transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targedirection, speedrotation * Time.deltaTime);
        if (Time.time > (1 / firerate) + lasttimefired)
        {
            Quaternion projectilerotation = Quaternion.LookRotation(targeter.GetTarget().GettargetTransform().position - pointToShot.position);
            GameObject bulletprojectile = Instantiate(projectile, pointToShot.position, projectilerotation);
            NetworkServer.Spawn(bulletprojectile, connectionToClient);
            lasttimefired = Time.time;
        }      
    }

    [Server]
    private bool CanShotAtTarget()
    {
        if (targeter.GetTarget()==null) return false;
        if(!((targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= shootrange * shootrange))return false;
        return true;
    }
}
