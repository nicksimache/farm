using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIDNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;

    private SteamNetworkManager manager;

    private SteamNetworkManager Manager{
        get{
            if(manager != null){
                return manager;
            }
            return manager = SteamNetworkManager.singleton as SteamNetworkManager;
        }
    }

    public override void OnStartAuthority(){
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient(){
        Manager.connectedPlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();

    }

    public override void OnStopClient(){
        Manager.connectedPlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName){
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue){
        if(isServer){
            this.PlayerName = NewValue;
        }
        if(isClient){
            LobbyController.Instance.UpdatePlayerList();
        }
    }
}
