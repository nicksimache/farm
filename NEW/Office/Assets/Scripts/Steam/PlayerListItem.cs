using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarRecieved;

    public TMP_Text PlayerNameText;
    public RawImage PlayerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start(){
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }
    
    private void OnImageLoaded(AvatarImageLoaded_t callback){
        if(callback.m_steamID.m_SteamID == PlayerSteamID){
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else {
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage){
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if(isValid){
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if(isValid){
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }

        }
        AvatarRecieved = true;
        return texture;
     
    }

    void GetPlayerIcon(){
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if(ImageID == -1) return;
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    public void SetPLayerValues(){
        PlayerNameText.text = PlayerName;

        if(!AvatarRecieved) GetPlayerIcon();
    }
}
