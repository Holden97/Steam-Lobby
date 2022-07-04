// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
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
    /// <summary>
    /// 玩家显示列表
    /// </summary>
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
            CreateNewPlayerListItems();
        }
    }

    private void CreateNewPlayerListItems()
    {
        Debug.Log("Excuting CreateNewPlayerListItems");
        foreach (var player in networkManager.GamePlayers)
        {
            if (!playerListItems.Any(b => b.connectionId == player.connetionId))
            {
                Debug.Log($"CreateNewPlayerListItems:Pllayer not fount in playerListItems:{player.playerName}");
                GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem newPlayerListItemScript = newPlayerListItem.GetComponent<PlayerListItem>();

                newPlayerListItemScript.BindData(player);

                newPlayerListItem.transform.SetParent(ContentPanel.transform);
                newPlayerListItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerListItemScript);
            }

        }
    }

    private void RemovePlayerListItems()
    {
        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();
        foreach (var playerListItem in playerListItems)
        {
            if (!networkManager.GamePlayers.Any(b => b.connetionId == playerListItem.connectionId))
            {
                Debug.Log($"RemovePlayerListItem:player list item from connection id:{playerListItem.connectionId}");
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem item in playerListItemsToRemove)
            {
                GameObject playerListItemToRemoveObject = item.gameObject;
                playerListItems.Remove(item);
                Destroy(playerListItemToRemoveObject);
                //指向的地址置空
                playerListItemToRemoveObject = null;
            }
        }
    }

    public void UpdatePlayerListItems()
    {
        Debug.Log("Executing UpdatePlayerListItems");
        foreach (GamePlayer player in networkManager.GamePlayers)
        {
            foreach (PlayerListItem item in playerListItems)
            {
                if (item.connectionId == player.connetionId)
                {
                    item.BindData(player);
                    if (player == localGamePlayerScript)
                    {
                        ChangeReadyUpButtonText();
                    }
                }
            }
        }
    }

    void ChangeReadyUpButtonText()
    {
        if (localGamePlayerScript.isPlayerReady)
            ReadyUpButton.GetComponentInChildren<Text>().text = "Unready";
        else
            ReadyUpButton.GetComponentInChildren<Text>().text = "Ready Up";
    }

    private void CheckIfAllPlayersAreReady()
    {
        Debug.Log("Excuting CheckIfAllPlayersAreReady");
        bool ready = false;
        foreach (var item in networkManager.GamePlayers)
        {
            if (item.isPlayerReady)
            {
                ready = true;
            }
            else
            {
                Debug.Log($"CheckIfAllPlayersAreReady:Not all players are Ready.Waiting for{item.playerName}");
                ready = false;
                break;
            }
        }
        if (ready)
        {
            Debug.Log($"CheckIfAllPlayersAreReady:All players are ready!");
            if (localGamePlayerScript.IsGameLeader)
            {
                Debug.Log("CheckIfAllPlayersAreReady:Local player is the game leader.You can start the game now");
                StartGameButton.gameObject.SetActive(true);
            }
            else
            {
                StartGameButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (StartGameButton.gameObject.activeInHierarchy)
            {
                StartGameButton.gameObject.SetActive(false);
            }
        }
    }

    public void FindLocalGamePlayer()
    {
        localGamePlayerObject = GameObject.Find("LocalGamePlayer"); ;
        localGamePlayerScript = localGamePlayerObject.GetComponent<GamePlayer>();
    }

    private void CreatePlayerListItems()
    {
        Debug.Log($"Executing CreatePlayerListItems. This many players to Create{networkManager.GamePlayers.Count}");
        foreach (var player in networkManager.GamePlayers)
        {
            Debug.Log($"CreatePlayerListItems:Creating playerlistitem for player:{ player.playerName}");
            GameObject newPlayerListItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem listItem=newPlayerListItem.GetComponent<PlayerListItem>();

            listItem.playerName = player.playerName;
            listItem.playerName = player.playerName;
            listItem.playerName = player.playerName;
            listItem.playerName = player.playerName;
            listItem.playerName = player.playerName;


        }
    }
}
