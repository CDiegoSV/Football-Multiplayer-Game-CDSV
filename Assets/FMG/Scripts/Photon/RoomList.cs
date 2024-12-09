using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;


public class RoomList : MonoBehaviourPunCallbacks
{

    #region References

    [SerializeField] private Transform _roomListGameObject;
    [SerializeField] private GameObject _roomPrefab;


    private List<RoomInfo> _roomList = new List<RoomInfo>();

    #endregion

    #region Photon Methods

    public override void OnRoomListUpdate(List<RoomInfo> p_roomList)
    {
        if (_roomList.Count <= 0)
        {
            _roomList = new List<RoomInfo>(p_roomList);
        }
        else
        {
            List<RoomInfo> updatedRoomList = new List<RoomInfo>(_roomList);

            foreach (RoomInfo roomInfo in p_roomList)
            {
                int index = updatedRoomList.FindIndex(r => r.Name == roomInfo.Name);

                if (roomInfo.RemovedFromList)
                {
                    if (index != -1)
                    {
                        updatedRoomList.RemoveAt(index);
                    }
                }
                else
                {
                    if (index != -1)
                    {
                        updatedRoomList[index] = roomInfo;
                    }
                    else
                    {
                        updatedRoomList.Add(roomInfo);
                    }
                }
            }

            _roomList = updatedRoomList;
        }
        UpdateRoomListUI();
    }



    #endregion

    #region RuntimeMethods

    private void UpdateRoomListUI()
    {
        foreach (Transform roomItem in _roomListGameObject)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (RoomInfo roomInfo in _roomList)
        {
            GameObject roomInstance = Instantiate(_roomPrefab, _roomListGameObject);

            roomInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = roomInfo.Name;
            roomInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
            switch(roomInfo.MaxPlayers)
            {
                case 2:
                    roomInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "1 vs 1";
                    break;
                case 4:
                    roomInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "2 vs 2";
                    break;
                case 6:
                    roomInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "3 vs 3";
                    break;
            }
        }
        _roomListGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_roomListGameObject.GetComponent<RectTransform>().sizeDelta.x, 120 * _roomList.Count);
    }

    #endregion

    #region Public Methods

    public void OnClickJoinRooomButton()
    {
        PhotonNetwork.JoinRoom(transform.GetChild(0).GetComponent<TextMeshProUGUI>()?.text);
    }

    #endregion
}
