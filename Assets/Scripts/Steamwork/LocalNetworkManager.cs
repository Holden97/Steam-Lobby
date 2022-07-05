// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class LocalNetworkManager : NetworkManager
{
    [HideInInspector] public static string SteamworksLobbySceneName = "Scene_SteamworksLobby";
    [HideInInspector] public static string SteamworksSceneName = "Scene_Steamworks";
    [SerializeField] private GamePlayer gamePlayerPrefab;
    [SerializeField] public int minPlayers = 1;
    /// <summary>
    /// 房间内玩家列表
    /// </summary>
    public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

    public bool CanStartGame
    {
        get
        {
            if (numPlayers < minPlayers) return false;
            foreach (var item in GamePlayers)
            {
                if (!item.isPlayerReady)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public override void OnStartServer()
    {
        Debug.Log("Starting Server");
        ServerChangeScene(SteamworksLobbySceneName);
    }

    public override void OnClientConnect()
    {
        Debug.Log("Client connet");
        base.OnClientConnect();
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="conn">客户端连接</param>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"Checking if player is in correct scene.Player's scene name is:{SceneManager.GetActiveScene().name}.Correct scene name is:{SteamworksLobbySceneName}");
        if (SceneManager.GetActiveScene().name == SteamworksLobbySceneName)
        {
            bool isGameLeader = GamePlayers.Count == 0;//第一个加入的人成为leader

            GamePlayer GamePlayerInstance = Instantiate(gamePlayerPrefab);

            GamePlayerInstance.IsGameLeader = isGameLeader;
            GamePlayerInstance.connetionId = conn.connectionId;
            GamePlayerInstance.playerNumber = GamePlayers.Count + 1;

            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.current_lobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
            Debug.Log($"Player added.{GamePlayerInstance.playerName}.Player connection id:{GamePlayerInstance.connetionId}");
        }
    }

    public void StartGame()
    {
        if (CanStartGame && SceneManager.GetActiveScene().name == SteamworksLobbySceneName)
        {
            ServerChangeScene(SteamworksLobbySceneName);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            GamePlayer player =conn.identity.GetComponent<GamePlayer>();
            GamePlayers.Remove(player);
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GamePlayers.Clear();
    }

    public void HostShutDownServer()
    {
        GameObject networkManagerObject = GameObject.Find("NetWorkManager");
        Destroy(GetComponent<SteamManager>());
        Destroy(networkManagerObject);
        ResetStatics();
        SceneManager.LoadScene(SteamworksSceneName);

        Start();
    }
}
