// Author：GuoYiBo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int connectionId;
    public bool isPlayerReady = false;
    public ulong playerSteamId;
    private bool avaterRetrieved = false;

    [SerializeField] private Text PlayerNameText;
    [SerializeField] private Text PlayerReadyStatus;
    [SerializeField] private RawImage PlayerAvater;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    private void Start()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == playerSteamId)
        {
            PlayerAvater.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }

    private Texture GetSteamImageAsTexture(int m_iImage)
    {
        Debug.Log($"Executing GetSteamImageAsTexture for player:{this.playerName}");
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(m_iImage, out uint width, out uint height);
        if (isValid)
        {
            Debug.Log("GetSteamImageAsTexture:Image size is valid?");
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(m_iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                Debug.Log("GetSteamImageAsTexture:Image size is valid for GetImageRGBA");
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avaterRetrieved = true;
        return texture;
    }

    private void GetPlayerAvatar()
    {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamId);

        if (imageId == -1)
        {
            return;
        }

        PlayerAvater.texture = GetSteamImageAsTexture(imageId);
    }

    public void SetPlayerListItemValues()
    {
        PlayerNameText.text = playerName;
        UpdatePlayerItemReadyStatus();
        if (!avaterRetrieved)
        {
            GetPlayerAvatar();
        }
    }

    public void UpdatePlayerItemReadyStatus()
    {
        if (isPlayerReady)
        {
            PlayerReadyStatus.color = Color.green;
            PlayerReadyStatus.text = "Ready";
        }
        else
        {
            PlayerReadyStatus.color = Color.red;
            PlayerReadyStatus.text = "NotReady";
        }
    }
}
