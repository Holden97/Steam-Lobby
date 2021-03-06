using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageManager : NetworkBehaviour
{
    public static MessageManager instance;
    [SerializeField] private TMP_InputField messageBox;
    [SerializeField] private Text messageText;
    [SyncVar(hook = nameof(HandleNewMessageText))] public string messageTextSynced = "New Text";

    public void HandleNewMessageText(string oldValue, string newValue)
    {
        Debug.Log($"HandleNewMessageText with new value:{newValue}");
        if (isServer)
        {
            messageTextSynced = newValue;
        }
        if (isClient && oldValue != newValue)
        {
            UpdateMessageText(newValue);
        }
    }

    private void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Awake()
    {
        CreateInstance();
    }

    private void UpdateMessageText(string newMessage)
    {
        messageText.text = newMessage;
    }

    private NetworkManager networkManager
    {
        get
        {
            return LocalNetworkManager.singleton;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSendMessageToPlayers(string newMessage)
    {
        HandleNewMessageText(messageTextSynced, newMessage);
    }

    public void SendMessageToPlayers()
    {
        if (!string.IsNullOrEmpty(messageBox.text))
        {
            string newMessage = messageBox.text;
            CmdSendMessageToPlayers(newMessage);
        }
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name == LocalNetworkManager.SteamworksGameSceneName)
        {
            networkManager?.ServerChangeScene(LocalNetworkManager.SteamworksLobbySceneName);
        }
    }
}
