using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonConnection : MonoBehaviourPunCallbacks
{

    #region References
    [Header("Menu References")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] GameObject createButton;
    [SerializeField] CharacterSelection characterSelection;

    [Header("Loading Panel References")]
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Animator panelAnimator;
    [SerializeField] AnimationClip outClip;

    
    #endregion

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnConnectedToMaster()
    {
        print("Se ha conectado al Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = "";
        print("Ha entrado al lobby Ab");
        panelAnimator.SetBool("LoadingPanelOut", true);
        StartCoroutine(LoadingSceneOut());
        //PhotonNetwork.JoinOrCreateRoom("TestRoom", newRoomInfo(), null);
    }

    public override void OnJoinedRoom()
    {
        print("Entro a Room: " + PhotonNetwork.CurrentRoom.Name);
        characterSelection.SelectCharacter();
        PhotonNetwork.LoadLevel(1);
        //PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("Error al crear Room: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("Error al intentar unirse al Room: " + message);
    }
    RoomOptions newRoomInfo()
    {
        RoomOptions roomOptions = new RoomOptions();
        switch (dropdown.value)
        {
            case 0:
                roomOptions.MaxPlayers = 2;
                break;
            case 1:
                roomOptions.MaxPlayers = 4;
                break;
            case 2:
                roomOptions.MaxPlayers = 6;
                break;
        }
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    private IEnumerator LoadingSceneOut()
    {
        panelAnimator.SetBool("LoadingPanelOut", true);
        yield return new WaitForSeconds(outClip.length);
        loadingPanel.SetActive(false);
        CameraManager.instance.ChangeCurrentCameraTo(1);
        yield return new WaitForSeconds(2f);
        menuPanel.SetActive(true);
    }



    public void OnClickCreateRoomButton()
    {
        if (inputField.text != "")
        {
            PhotonNetwork.CreateRoom(inputField.text, newRoomInfo(), null);
        }
    }

    public void OnClickQuitButton()
    {
        Application.Quit();
    }
}
