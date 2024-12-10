using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{
    public static PlayerSpawnManager instance;

    PhotonView myPV;

    Transform spawnTransform;
    [SerializeField] GameObject playersParentGameObject;

    [SerializeField] Transform[] spawnPositions;
    [SerializeField] private List<GameObject> prefabModels;
    [SerializeField] private Color blueTeamColor;
    [SerializeField] private Color greenTeamColor;

    #region RuntimeVariables

    private int playerIndex;
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
            playerIndex = (int)LevelNetworkManager.Instance?.getCurrentPlayerCount;
        }
        if(LevelNetworkManager.Instance == null)
        {
            spawnTransform = spawnPositions[0];
        }

        Invoke("InstantiatePlayer", 1f);
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

        int playerParentViewID = playersParentGameObject.GetPhotonView().ViewID;

        GameObject playerInstance = PhotonNetwork.Instantiate("SoccerPlayer", spawnTransform.position, Quaternion.identity);
        GameManager.instance.AddPlayerToTheGameList = playerInstance.GetComponent<PlayerController>();
        int playerInstanceViewID = playerInstance.GetPhotonView().ViewID;
        photonView.RPC("SetGOParent", RpcTarget.AllBuffered, playerInstanceViewID, playerParentViewID);
        photonView.RPC("SetTeamColor", RpcTarget.AllBuffered, playerInstanceViewID, playerIndex);
        GameObject playerModelInstance = PhotonNetwork.Instantiate(prefabName, spawnTransform.position, Quaternion.identity);
        int playerModelViewID = playerModelInstance.GetPhotonView().ViewID;
        photonView.RPC("SetGOParent", RpcTarget.AllBuffered, playerModelViewID, playerInstanceViewID);
        photonView.RPC("AddMemberToTargetGroupRPC", RpcTarget.AllBuffered, playerModelViewID);
    }

    #endregion

    #region Runtime Methods

    

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

    [PunRPC]
    /// <summary>
    /// Sets the parent of "gameObject".
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="parent"></param>
    private void SetGOParent(int gameObjectViewID, int parentViewID)
    {
        GameObject targetObject = PhotonView.Find(gameObjectViewID).gameObject;
        GameObject parentObject = PhotonView.Find(parentViewID).gameObject;
        targetObject.transform.SetParent(parentObject.transform);
    }

    [PunRPC]
    private void SetTeamColor(int viewID, int playerIndex)
    {
        GameObject targetObject = PhotonView.Find(viewID).gameObject;
        if (playerIndex % 2 == 0)
        {
            targetObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = greenTeamColor;
            targetObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Image>().color = greenTeamColor;
        }
        else
        {
            targetObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = blueTeamColor;
            targetObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Image>().color = blueTeamColor;
        }
    }

    #endregion
}
