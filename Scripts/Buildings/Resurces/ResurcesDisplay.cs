using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class ResurcesDisplay : MonoBehaviour
{
    RTSPlayer player;

    [SerializeField]TMP_Text resurcrestext;
    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        player.ClientHandleResucesUpdated += ClientHandleResucoresUpdate;
        ClientHandleResucoresUpdate(player.GetResurces());
    }
    private void OnDestroy()
    {
        player.ClientHandleResucesUpdated -= ClientHandleResucoresUpdate;
    }

    private void ClientHandleResucoresUpdate(int currentresuroses)
    {
        resurcrestext.text = "Resurces: "+currentresuroses.ToString();

    }
}
