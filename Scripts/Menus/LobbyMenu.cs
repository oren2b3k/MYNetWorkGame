using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject lobbyUi;
    [SerializeField] Button StartButton;
    [SerializeField] TMP_Text[] playersnames;
    private void OnEnable()
    {
        CustomNetWorkManger.ClientOnConnected += HandleClinetConnected;
        RTSPlayer.AuthorityOnpartyOwnerStateUpdated += HandleAuthoritySetPartyOwner;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    public void HandleAuthoritySetPartyOwner(bool state)
    {
        StartButton.gameObject.SetActive(state);
    }
    public void ClientHandleInfoUpdated()
    {
        List<RTSPlayer> players = ((CustomNetWorkManger)NetworkManager.singleton).Players;
        for (int i = 0; i < players.Count; i++)
        {
            playersnames[i].text = players[i].GetDisplayName();
        }
        for (int i = players.Count; i < playersnames.Length; i++)
        {
            playersnames[i].text = "Wating for player...";
        }
        StartButton.interactable = players.Count > 1;
    }
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }

    private void OnDisable()
    {
        CustomNetWorkManger.ClientOnConnected -= HandleClinetConnected;
        RTSPlayer.AuthorityOnpartyOwnerStateUpdated -= HandleAuthoritySetPartyOwner;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    public void HandleClinetConnected()
    {
        lobbyUi.SetActive(true);
    }
    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected) { NetworkManager.singleton.StopHost(); }
        else { NetworkManager.singleton.StopClient(); SceneManager.LoadScene(0); }
    }
}
