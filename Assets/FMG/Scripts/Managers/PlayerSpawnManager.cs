using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{
    public static PlayerSpawnManager instance;

    PhotonView myPV;

    Transform spawnTransform;
    [SerializeField] Transform playersParentGameObject;

    [SerializeField] Transform[] spawnPositions;
    [SerializeField] private List<GameObject> prefabModels;

    #region RuntimeVariables

    private string prefabName;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        
    }

    private void Start()
    {
        myPV = GetComponent<PhotonView>();

        if (LevelNetworkManager.Instance?.getCurrentPlayerCount > 0)
        {
            spawnTransform = spawnPositions[(int)LevelNetworkManager.Instance?.getCurrentPlayerCount - 1];
        }
        if(LevelNetworkManager.Instance == null)
        {
            spawnTransform = spawnPositions[0];
        }

        Invoke("InstantiatePlayer", 3f);
    }

    #endregion

    #region Runtime Methods

    private void InstantiatePlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
        {
            var selectedCharacter = PhotonNetwork.LocalPlayer.CustomProperties["Character"];
            Debug.Log($"El personaje seleccionado es: {selectedCharacter}");
            prefabName = (string)selectedCharacter;
        }
        else
        {
            Debug.LogError("No se encontró la propiedad 'Character' en las propiedades del jugador.");
        }

        GameObject playerInstance = PhotonNetwork.Instantiate("SoccerPlayer", spawnTransform.position, Quaternion.identity);
        //photonView.RPC("SetGOParent", RpcTarget.AllBuffered, parameters: (playerInstance.transform, playersParentGameObject.transform));
        SetGOParent(playerInstance.transform, playersParentGameObject.transform);
        GameObject playerModelInstance = PhotonNetwork.Instantiate(prefabName, spawnTransform.position, Quaternion.identity);
        //photonView.RPC("SetGOParent", RpcTarget.AllBuffered, parameters: (playerModelInstance.transform, playerInstance.transform));
        SetGOParent(playerModelInstance.transform, playerInstance.transform);
        int viewID = playerModelInstance.GetPhotonView().ViewID;
        photonView.RPC("AddMemberToTargetGroupRPC", RpcTarget.AllBuffered, viewID);
    }

    #endregion

    #region Runtime Methods

    /// <summary>
    /// Sets the parent of "gameObject".
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="parent"></param>
    private void SetGOParent(Transform gameObject, Transform parent)
    {
        gameObject.SetParent(parent);
    }

    #endregion

    #region RPC Methods

    [PunRPC]
    private void AddMemberToTargetGroupRPC(int viewID)
    {
        GameObject targetObject = PhotonView.Find(viewID).gameObject;
        if (targetObject != null)
        {
            Transform targetTransform = targetObject.transform;
            CameraManager.instance.AddMembersToTargetGroup = targetTransform;
        }
    }

    #endregion
}
