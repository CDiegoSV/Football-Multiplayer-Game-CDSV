using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterSelection : MonoBehaviourPunCallbacks
{

    #region References

    [SerializeField] private GameObject[] playerCharacters;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    #endregion

    #region Runtime Variables

    private int[] _characterArray;
    private int _currentSelectedCharacter = 0;
    

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

        CameraManager.instance.setCurrentCameraFollowAndLookAt = playerCharacters[_currentSelectedCharacter].transform;
    }

    public void SelectCharacter()
    {
        Hashtable m_playerProperties = new Hashtable();

        m_playerProperties["Character"] = _characterArray[_currentSelectedCharacter];
        photonView.Owner.SetCustomProperties(m_playerProperties);
    }

    #endregion


}
