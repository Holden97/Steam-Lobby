// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;

public class LobbyListItem : MonoBehaviour
{
    public CSteamID lobbySteamId;
    public string lobbyName;
    public int numberOfPlyaers;
    public int maxNumberOfPlayers;

    [SerializeField] private Text LobbyNameText;
    [SerializeField] private Text NumberOfPlayersText;

    public void SetLobbyItemValues()
    {
        LobbyNameText.text = lobbyName;
        NumberOfPlayersText.text = $"Number of Players:{NumberOfPlayersText}/{maxNumberOfPlayers}";
    }

    public void JoinLobby()
    {
        Debug.Log($"JoinLobby:Player selected tojoin lobby with steam id of:{lobbySteamId}");
        SteamLobby.Instance.JoinLobby(lobbySteamId);
    }
}
