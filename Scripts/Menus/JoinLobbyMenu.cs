using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject landingpage;
    [SerializeField] TMP_Text addressinput;
    [SerializeField] Button joinbutton;
    private void OnEnable()
    {
        CustomNetWorkManger.ClientOnConnected += HandleClientConnected;
        CustomNetWorkManger.ClientOnDisConnected += HandleClientDisConnected;
    }


    private void OnDisable()
    {
        CustomNetWorkManger.ClientOnConnected -= HandleClientConnected;
        CustomNetWorkManger.ClientOnDisConnected -= HandleClientDisConnected;
        
    }

    public void JoinLobby()
   {
        string lobbytojoin = addressinput.text;
        Debug.Log(lobbytojoin);
        NetworkManager.singleton.networkAddress=lobbytojoin;
        NetworkManager.singleton.StartClient();
        joinbutton.interactable = false;
   }

    public void HandleClientConnected()
    {
        joinbutton.interactable = true;
        gameObject.SetActive(false);
        landingpage.SetActive(false);
    }
    public void HandleClientDisConnected()
    {
        joinbutton.interactable = true;

    }
}
