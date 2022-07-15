using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TeamColors : NetworkBehaviour
{
    [SerializeField] Renderer[] renderers;

    [SyncVar(hook =nameof(ClientHandleTeamColor))]
    [SerializeField]Color color=new Color();
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        color =player.GetTeamcolor();
    }

    public void ClientHandleTeamColor(Color oldcolor,Color newcolor)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].materials.Length != 1)
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    renderers[i].materials[j].color = newcolor;
                }
            else { renderers[i].material.color = newcolor; }
        }
    }
}
