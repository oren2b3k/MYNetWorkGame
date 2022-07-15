using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject landingpage;
    [SerializeField] bool usingSteam=false;
    protected Callback<LobbyCreated_t> lobbycreated;
    protected Callback<GameLobbyJoinRequested_t> gamelobbyrequsetd;
    protected Callback<LobbyEnter_t> lobbyented;

    private void Start()
    {
        if (!usingSteam) return;
        lobbycreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gamelobbyrequsetd = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyented = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    private void OnLobbyEnter(LobbyEnter_t callback) 
    {
        if (NetworkServer.active) return;
        string hostaddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAdress");
        NetworkManager.singleton.networkAddress = hostaddress;
        NetworkManager.singleton.StartClient();
        landingpage.SetActive(false);
    }
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyCreated(LobbyCreated_t callback) 
    {
        if (callback.m_eResult != EResult.k_EResultOK) { landingpage.SetActive(true); return; }
        NetworkManager.singleton.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
    }

    public void StartHost()
    {
        landingpage.SetActive(false);
        if(usingSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly,4);
            return;
        }
        NetworkManager.singleton.StartHost();
    }
    public void EXit()
    {
        Application.Quit();
    }
    public void Resize()
    {
        if (Screen.fullScreen)
            Screen.SetResolution(1920, 1080, false);
        else
            Screen.fullScreen=true;
    }
}
