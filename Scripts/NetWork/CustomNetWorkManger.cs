using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
public class CustomNetWorkManger : NetworkManager
{
    [SerializeField] GameObject unitBase_Prefab;

    [SerializeField] GameOverHandler gameoverhandlerprefab;
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisConnected;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();
    bool IsGameInPross = false;

    #region Clietn
    public override void OnClientConnect()
    {
        ClientOnConnected?.Invoke();    
        base.OnClientConnect();
    }
    public override void OnClientDisconnect()
    {
        ClientOnDisConnected?.Invoke();
        base.OnClientDisconnect();
    }
    public override void OnStopClient()
    {
       Players.Clear(); 
    }
    #endregion
    #region Server
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        Players.Add(player);
        player.SetDisplayName("Player :"+Players.Count );
        Color randcolor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        player.SetTeamColor(randcolor);
        player.SetIsParyOwner(Players.Count == 1);
        Debug.Log(Players.Count);
    }

    public override void OnServerSceneChanged(string sceneName)
    {     
        if(SceneManager.GetActiveScene().name.StartsWith("Scene_Menu"))
        {
            GameOverHandler gameOverHandler = Instantiate(gameoverhandlerprefab);
            NetworkServer.Spawn(gameOverHandler.gameObject);
            foreach(RTSPlayer player in Players)
            {
                GameObject spw=  Instantiate(unitBase_Prefab,GetStartPosition().position,Quaternion.identity);
                NetworkServer.Spawn(spw,player.connectionToClient);
            }
        }
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!IsGameInPross) return;
                conn.Disconnect();
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
       RTSPlayer player=conn.identity.GetComponent<RTSPlayer>();    
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }
    public override void OnStopServer()
    {
        Players.Clear();
        IsGameInPross = false;
    }
    public void StartGame()
    {
        if (Players.Count < 2) return;
        IsGameInPross=true;
        ServerChangeScene("Scene_Menu");
    }
    #endregion
}
