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
        NumberOfPlayersText.text = $"Players:{numberOfPlyaers}/{maxNumberOfPlayers}";
    }

    public void JoinLobby()
    {
        Debug.Log($"JoinLobby:Player selected tojoin lobby with steam id of:{lobbySteamId}");
        SteamLobby.instance.JoinLobby(lobbySteamId);
    }

    public void BindData(CSteamID lobbyID, string pchKey)
    {
        lobbySteamId = (CSteamID)lobbyID.m_SteamID;
        lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, pchKey);
        numberOfPlyaers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyID.m_SteamID);
        maxNumberOfPlayers = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyID.m_SteamID);
        SetLobbyItemValues();
    }
}
