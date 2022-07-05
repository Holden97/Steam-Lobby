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

        SteamLobby.instance
    }
}
