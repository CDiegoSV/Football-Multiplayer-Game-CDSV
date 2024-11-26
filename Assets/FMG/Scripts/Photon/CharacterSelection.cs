using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterSelection : MonoBehaviourPunCallbacks
{
    #region Runtime Variables

    private int[] _characterArray;
    private int _currentSelectedCharacter = 0;
    

    #endregion

    #region Public Methods
    public void CharacterSelectionIndex(int characterIndex)
    {
        _currentSelectedCharacter = characterIndex;
    }

    public void SelectCharacter()
    {
        Hashtable m_playerProperties = new Hashtable();

        m_playerProperties["Character"] = _characterArray[_currentSelectedCharacter];
        photonView.Owner.SetCustomProperties(m_playerProperties);
    }

    #endregion


}
