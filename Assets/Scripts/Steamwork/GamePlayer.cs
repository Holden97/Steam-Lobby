// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Steamworks;

public class GamePlayer : NetworkBehaviour
{
    [Header("GamePlayer Info")]
    [SyncVar(hook = nameof(HandlePlayerNameUpdate))] public string playerName;
    [SyncVar] public int connetionId;
    [SyncVar] public int playerNumber;
    [Header("Game Info")]
    [SyncVar] public bool IsGameLeader = false;
    [SyncVar(hook = nameof(HandlePlayerReadyStatusChange))] public bool isPlayerReady;
    [SyncVar] public ulong playerSteamId;

    private LocalNetworkManager _networkManager;
    private LocalNetworkManager networkManager
    {
        get
        {
            if (_networkManager == null)
            {
                _networkManager = LocalNetworkManager.singleton as LocalNetworkManager;
            }
            return _networkManager;
        }
    }
    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyManager.instance.FindLocalGamePlayer();
        LobbyManager.instance.UpdateLobbyName();
    }
    public override void OnStartClient()
    {
        networkManager.GamePlayers.Add(this);
        LobbyManager.instance.UpdateLobbyName();
        LobbyManager.instance.UpdateUI();
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void HandlePlayerNameUpdate(string OldValue, string newValue)
    {
        Debug.Log($"Player name has been update for:{OldValue}/{newValue}");
        if (isServer)
        {
            this.playerName = newValue;
        }
        if (isClient)
        {
            LobbyManager.instance.UpdateUI();
        }
    }

    public void ChangeReadyStatus()
    {
        Debug.Log($"Executing ChangeReadyStatus for player:{playerName}");
        if (hasAuthority)
        {
            CmdChangePlayerReadyStatus();
        }
    }

    [Command]
    private void CmdChangePlayerReadyStatus()
    {
        Debug.Log($"Excuting CmdChangePlayerReadyStatus on the server for player:{playerName}");
        HandlePlayerReadyStatusChange(isPlayerReady, !isPlayerReady);
    }

    private void HandlePlayerReadyStatusChange(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            isPlayerReady = newValue;
        }
        if (isClient)
        {
            LobbyManager.instance.UpdateUI();
        }
    }

    public void CanLobbyStartGame()
    {
        if (hasAuthority)
        {
            CmdCanLobbyStartGame();
        }
    }

    [Command]
    private void CmdCanLobbyStartGame()
    {
        _networkManager.StartGame();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        Debug.Log("CmdSetPlayerName:Setting player name to" + playerName);
        HandlePlayerNameUpdate(this.playerName, playerName);
    }

    public void QuitLobby()
    {
        if (hasAuthority)
        {
            if (IsGameLeader)
            {
                _networkManager.StopHost();
            }
            else
            {
                _networkManager.StopClient();
            }
        }
    }

    private void OnDestroy()
    {
        //清除所有玩家？
        if (hasAuthority)
        {
            LobbyManager.instance.DestroyPlayerListItems();
            SteamMatchmaking.LeaveLobby((CSteamID)LobbyManager.instance.currentLobbyId);
        }
    }
}
