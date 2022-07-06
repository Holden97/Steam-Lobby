using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance { get; private set; }
    private List<GameObject> listOfLobbyListItems = new List<GameObject>();

    private NetworkManager networkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;


    public ulong current_lobbyID;
    public List<CSteamID> lobbyIDS = new List<CSteamID>();

    private const string HostAddressKey = "HostAddress";

    private void Awake()
    {
        Debug.LogError($"steamlobby awake!{GetHashCode()}");
    }
    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager == null)
        {
            Debug.LogError("networkManager is null!");
            return;
        }

        if (!SteamManager.Initialized) { return; }
        CreateInstance();

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        current_lobbyID = callback.m_ulSteamIDLobby;
        Debug.Log($"OnLobbyEntered for lobby with id{current_lobbyID}");
        if (NetworkServer.active)
        {
            return;
        }
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
        lobbyIDS.Clear();
        if (GameObject.Find("MainMenuManager"))
        {
            if (MainMenuManager.instance.listOfLobbyListItems.Count > 0)
            {
                MainMenuManager.instance.DestroyOldLobbyListItems();
            }
        }
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("OnGameLobbyJoinRequested");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        Debug.Log($"JoinLobby:Will try to join lobby with steam id:{lobbyId}");
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        Debug.Log("OnLobbyCreated");

        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());
        if (MainMenuManager.instance.didPlayerNameTheLobby)
        {
            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "name",
                MainMenuManager.instance.lobbyName);
            MainMenuManager.instance.didPlayerNameTheLobby = false;
        }
        else
        {
            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "name",
                $"{SteamFriends.GetPersonaName()}'s lobby");
        }
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    public void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void DestroyOldLobbyListItems()
    {
        Debug.Log("DestroyOldLobbyListItems");
        foreach (GameObject lobbyListItem in listOfLobbyListItems)
        {
            GameObject itemToDestroy = lobbyListItem;
            Destroy(itemToDestroy);
            itemToDestroy = null;
        }
        listOfLobbyListItems.Clear();
    }
    public void GetListOfLobbies()
    {
        if (lobbyIDS.Count > 0)
        {
            lobbyIDS.Clear();
        }
        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);

        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
    }

    public void CreateNewLobby(ELobbyType lobbyType)
    {
        SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
    }

    private void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        MainMenuManager.instance.DisplayLobbies(lobbyIDS, result);
    }

    private void OnGetLobbiesList(LobbyMatchList_t result)
    {
        Debug.Log($"Found {result.m_nLobbiesMatching} lobbies");
        if (MainMenuManager.instance.listOfLobbyListItems.Count > 0)
        {
            MainMenuManager.instance.DestroyOldLobbyListItems();
        }
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnDestroy()
    {
        Debug.LogError($"steamlobby ondestroy!:{GetHashCode()}");
    }
}
