using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理大厅界面切换
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    /// <summary>
    /// 创建/加入房间按钮组
    /// </summary>
    [Header("UI Stuff")]
    [SerializeField]
    private GameObject createOrJoinButtons = null;
    [Header("Lobby List UI")]
    [SerializeField] private GameObject LobbyListCanvas;
    [SerializeField] private GameObject LobbyListItemPrefab;
    [SerializeField] private GameObject ContentPanel;
    [SerializeField] private GameObject LobbyListScrollRect;
    [SerializeField] private TMP_InputField searchBox;
    public bool didPlayerSearchForLobbies = false;

    [Header("Create Lobby UI")]
    [SerializeField] private GameObject CreateLobbyCanvas;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Toggle friendsOnlyToggle;
    public bool didPlayerNameTheLobby = false;
    public string lobbyName;

    public List<GameObject> listOfLobbyListItems = new List<GameObject>();

    public void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Awake()
    {
        CreateInstance();
        createOrJoinButtons.SetActive(true);
        LobbyListCanvas.SetActive(false);
        CreateLobbyCanvas.SetActive(false);
    }

    public void CreateLobby()
    {
        createOrJoinButtons.SetActive(false);
        CreateLobbyCanvas.SetActive(true);
    }

    public void CreateNewLobby()
    {
        ELobbyType newLobbyType;
        if (friendsOnlyToggle.isOn)
        {
            Debug.Log("CreateNewLobby:friendsOnlyToggle is ON.Making lobby friends only.");
            newLobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
        }
        else
        {
            Debug.Log("CreateNewLobby:friendsOnlyToggle is OFF.Making lobby friends only.");
            newLobbyType = ELobbyType.k_ELobbyTypePublic;
        }

        if (!string.IsNullOrEmpty(lobbyNameInputField.text))
        {
            Debug.Log($"CreateNewLobby:player create a lobby name of{lobbyNameInputField.text}");
            didPlayerNameTheLobby = true;
            lobbyName = lobbyNameInputField.text;
        }

        SteamLobby.instance.CreateNewLobby(newLobbyType);
    }

    public void GetListOfLobbies()
    {
        Debug.Log("Trying to get list of available lobbies ...");
        createOrJoinButtons.SetActive(false);
        LobbyListCanvas.SetActive(true);

        SteamLobby.instance.GetListOfLobbies();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log($"Lobby{i}:{SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name")} number of players:" +
                    $"{SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDs[i].m_SteamID)}." +
                    $"max players:" +
                    $"{SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDs[i].m_SteamID)}");

                if (didPlayerSearchForLobbies)
                {
                    Debug.Log("OnGetLobbyInfo:Player searched for lobbies");
                    if (SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name").ToLower().Contains(searchBox.text.ToLower()))
                    {
                        listOfLobbyListItems.Add(CreateItem(lobbyIDs[i]));
                    }
                }
                else
                {
                    Debug.Log("OnGetLobbyInfo:Player DID NOT search for lobbies");
                    listOfLobbyListItems.Add(CreateItem(lobbyIDs[i]));
                }

                return;
            }
        }
        //??
        if (didPlayerSearchForLobbies)
            didPlayerSearchForLobbies = false;
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

    public void SearchForLobby()
    {
        didPlayerSearchForLobbies = !string.IsNullOrEmpty(searchBox.name);
        SteamLobby.instance.GetListOfLobbies();
    }

    private GameObject CreateItem(CSteamID lobbyID)
    {
        GameObject newLobbyListItem = Instantiate(LobbyListItemPrefab);
        LobbyListItem item = newLobbyListItem.GetComponent<LobbyListItem>();

        item.BindData(lobbyID, "name");

        newLobbyListItem.transform.SetParent(ContentPanel.transform);
        newLobbyListItem.transform.localScale = Vector3.one;

        return newLobbyListItem;
    }

    public void BackToMainMenu()
    {
        createOrJoinButtons.SetActive(true);
        CreateLobbyCanvas.SetActive(false);
        LobbyListCanvas.SetActive(false);

        if (listOfLobbyListItems.Count > 0)
        {
            DestroyOldLobbyListItems();
        }
        lobbyName = null;
        searchBox.text = "";
        lobbyNameInputField.text = "";
        didPlayerSearchForLobbies = false;
        didPlayerNameTheLobby = false;
        friendsOnlyToggle.isOn = false;
    }
}
