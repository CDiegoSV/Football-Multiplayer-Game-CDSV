using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinRoomButton : MonoBehaviour
{
    public void OnClickJoinRooomButton()
    {
        PhotonNetwork.JoinRoom(transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>()?.text);
    }
    
}
