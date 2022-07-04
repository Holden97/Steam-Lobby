using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class MessageManager : NetworkBehaviour
{
    public static MessageManager instance;
    [SerializeField] private TMP_InputField messageBox;
    [SerializeField] private Text messageText;
    [SyncVar(hook = nameof(HandleNewMessageText))] public string messageTextSynced = "New Text";

    public void HandleNewMessageText(string oldValue,string newValue)
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

    [Command]
    private void CmdSendMessageToPlayers(string newMessage)
    {
        HandleNewMessageText(messageTextSynced,newMessage);
    }

    public void SendMessageToPlayers()
    {
        if(!string.IsNullOrEmpty)
    }
}
