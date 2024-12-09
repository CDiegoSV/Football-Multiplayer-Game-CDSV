using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviourPunCallbacks
{

    #region References

    [SerializeField] private GameObject[] playerCharacters;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    #endregion

    #region Runtime Variables

    private PhotonView _photonView;
    [SerializeField]private string[] _characterArray;
    [SerializeField] private int _currentSelectedCharacter;


    #endregion

    #region Public Methods
    public void CharacterSelectionIndex(int characterIndex)
    {
        _currentSelectedCharacter += characterIndex;
        if (_currentSelectedCharacter == 0)
        {
            leftArrow.SetActive(false);
        }
        else if(_currentSelectedCharacter == playerCharacters.Length -1)
        {
            rightArrow.SetActive(false);
        }
        else
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
        Debug.Log("El indice de personaje es: " + _currentSelectedCharacter.ToString());
        CharacterSelection[] characterSelectionScripts = FindObjectsByType<CharacterSelection>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        foreach (CharacterSelection gameObject in characterSelectionScripts)
        {
            if(gameObject != this)
            {
                gameObject.SetCurrentSelectedCharacter = _currentSelectedCharacter;
            }
        }
        CameraManager.instance.setCurrentCameraFollowAndLookAt = playerCharacters[_currentSelectedCharacter].transform;
    }

    public void SelectCharacter()
    {

        if (_characterArray == null || _characterArray.Length == 0)
        {
            Debug.LogError("El arreglo de personajes no está inicializado o está vacío.");
            return;
        }

        if (_currentSelectedCharacter < 0 || _currentSelectedCharacter >= _characterArray.Length)
        {
            Debug.LogError("Índice de personaje seleccionado fuera de los límites.");
            return;
        }

        Hashtable m_playerProperties = new Hashtable
        {
            ["Character"] = _characterArray[_currentSelectedCharacter]
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_playerProperties);
        Debug.Log("Propiedades personalizadas establecidas correctamente. El personaje seleccionado fue: " + _currentSelectedCharacter.ToString());
    }

    #endregion

    #region GettersSetters

    public int SetCurrentSelectedCharacter
    {
        set { _currentSelectedCharacter = value; }
    }

    #endregion
}
