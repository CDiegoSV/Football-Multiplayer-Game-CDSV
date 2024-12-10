using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelNetworkManager : MonoBehaviourPunCallbacks
{
    public static LevelNetworkManager Instance { get; private set; }

    #region Unity Methods

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    #region Public Methods

    public void DisconnectCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }




    #endregion

    #region Photon Methods


    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("Entró nuevo usuario: " + newPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && GameManager.instance.GetCurrentGameState == GameStates.WAITINGFORPLAYERS)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("Room lleno. Cambiando visibilidad a false.");
            SetGameStartEvent();
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("Room lleno. Cambiando visibilidad a false.");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Salió el usuario: " + otherPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers && !PhotonNetwork.CurrentRoom.IsVisible)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            Debug.Log("Un jugador salió. Cambiando visibilidad a true.");
        }
    }


    #endregion

    #region Events

    private void SetGameStartEvent()
    {
        byte m_ID = 1;
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(m_ID, null, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion

    #region Getters And Setters

    public int getCurrentPlayerCount
    {
        get
        {
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    #endregion
}
