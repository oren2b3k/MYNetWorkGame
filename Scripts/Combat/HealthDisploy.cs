using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class HealthDisploy : NetworkBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image healthimage;
   
    [SerializeField] GameObject healthcanvas;


    private void Awake()
    {
        health.ClientOnHealthChanged += UpdateCurrentHealthImage;
        health.ClientOnGotHit += ShowHealth;
    }
    private void OnDestroy()
    {
        health.ClientOnHealthChanged -= UpdateCurrentHealthImage;
        health.ClientOnGotHit -= ShowHealth;
    }

    #region Client
    public override void OnStartAuthority()
    {
        healthcanvas.SetActive(true);
    }
    public override void OnStopAuthority()
    {
        healthcanvas.SetActive(false);
    }

    [Client]
    public void ShowHealth(bool hit)
    {
            if (!hit) return;
            if (healthcanvas.activeSelf) return;
            healthcanvas.SetActive(true);      
    }


    void UpdateCurrentHealthImage(int currenthealth,int maxhealth)
    {
        healthimage.fillAmount = (float)currenthealth / maxhealth;
    }
    #endregion


}
