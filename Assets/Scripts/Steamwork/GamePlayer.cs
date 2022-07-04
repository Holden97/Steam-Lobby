// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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

    private void HandlePlayerReadyStatusChange(bool oldValue, bool newValue)
    {
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        Debug.Log("CmdSetPlayerName:Setting player name to"+playerName);
        HandlePlayerNameUpdate(this.playerName, playerName);
    }
}
