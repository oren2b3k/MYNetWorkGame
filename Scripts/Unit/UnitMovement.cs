using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class UnitMovement : NetworkBehaviour
{

   [SerializeField] NavMeshAgent m_NavMeshAgent;
    [SerializeField] Targeter m_targeter;
    [SerializeField] float chaserange;

    #region ServerDo

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver +=ServerOnHandleGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerOnHandleGameOver;
    }
    [Server]
    private void ServerOnHandleGameOver()
    {
        m_NavMeshAgent.ResetPath();
        enabled = false;
    }

    [ServerCallback]
    private void Update()
    {
        //if we got targat then chase it and only if we are not in range
        Targetable target = (m_targeter.GetTarget() != null ? m_targeter.GetTarget() : null);
        if(target != null)
            if((target.transform.position-transform.position).sqrMagnitude> chaserange*chaserange)
            {
                m_NavMeshAgent.SetDestination(target.transform.position);
            }
        else if(m_NavMeshAgent.hasPath)
        {
            m_NavMeshAgent.ResetPath();
        }

        //if we have path then move until you reaching to the stopdistance
        if (!m_NavMeshAgent.hasPath) return;
        if (m_NavMeshAgent.remainingDistance > m_NavMeshAgent.stoppingDistance) return;
        m_NavMeshAgent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 pos)
    {
        ServerMoveunit(pos);
    }

    internal void ServerMoveunit(Vector3 pos)
    {
        m_targeter.ClearTarget();
        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1, NavMesh.AllAreas)) return;
        m_NavMeshAgent.SetDestination(hit.position);
    }

    #endregion
}
