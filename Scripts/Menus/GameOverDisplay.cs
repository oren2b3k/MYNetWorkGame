using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text winnertmp;

    [SerializeField] GameObject GameOverDislpayobj;
    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }
    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        GameOverDislpayobj.SetActive(true);
        winnertmp.text = $"{winner} Has Won";
    }

    public void LeaveGame()
    {
        if(NetworkServer.active&&NetworkClient.active)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
