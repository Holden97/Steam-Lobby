// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;
    /// <summary>
    /// 这里之后需要修改，应该将其改为在场景根节点下创建挂载该组件的新物体
    /// </summary>
    private void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [Header("UI Element")]
    [SerializeField] private Text LobbyNameText;
    [SerializeField] private GameObject ContentPanel;
    [SerializeField] private GameObject PlayerListItemPrefab;
    [SerializeField] private Button ReadyUpButton;
    [SerializeField] private Button StartGameButton;

    public bool havePlayerListItemBeenCreated = false;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public GameObject localGamePlayerObject;
    public GamePlayer localGamePlayerScript;

    public ulong currentLobbyId;
    private LocalNetworkManager _networkmanager;
    private LocalNetworkManager networkManager
    {
        get
        {
            if (_networkmanager != null)
            {
                return _networkmanager;
            }
            return _networkmanager = LocalNetworkManager.singleton as LocalNetworkManager;
        }
    }

    private void Awake()
    {
        CreateInstance();
        ReadyUpButton.gameObject.SetActive(true);
        ReadyUpButton.GetComponentInChildren<Text>().text = "Ready Up";
        StartGameButton.gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        Debug.Log("Executing Updating");
        if (!havePlayerListItemBeenCreated)
        {
            CreatePlayerListItems();
        }
        if (playerListItems.Count < networkManager.GamePlayers.Count)
        {

        }
    }

    private void CreateNewPlayerListItems()
    {
        Debug.Log("Excuting CreateNewPlayerListItems");
        foreach (var player in networkManager.GamePlayers)
        {
            if (!playerListItems.Any(b => b.connectionId == player.connetionId)

        }
    }

    public void FindLocalGamePlayer()
    {
        localGamePlayerObject = GameObject.Find("LocalGamePlayer"); ;
        localGamePlayerScript = localGamePlayerObject.GetComponent<GamePlayer>();

    }

    private void CreatePlayerListItems()
    {
        Debug.Log($"Executing CreatePlayerListItems. This many players to Create{Ga}")
    }
}
