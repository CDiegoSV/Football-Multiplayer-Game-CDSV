using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Enums

public enum GameStates { NONE, WAITINGFORPLAYERS, GAME, VICTORY}

#endregion

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Singleton

    public static GameManager instance;

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

    #endregion

    #region References

    [SerializeField] List<PlayerController> _allPlayersOfTheGame;
    [SerializeField] GameObject _ball;

    #endregion

    #region RuntimeVariables

    private GameStates _currentGameState;
    private int _currentBlueTeamPoints;
    private int _currentGreenTeamPoints;

    private bool _gameStarted;

    private float timerSecondsLeft = 0;

    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeGameManager();
    }

    private void FixedUpdate()
    {
        ExecutingState();
    }

    #endregion

    #region Public Methods
    public void GameStateMechanic(GameStates nextState)
    {
        switch (nextState)
        {
            case GameStates.WAITINGFORPLAYERS:
                if(_currentGameState == GameStates.NONE)
                {
                    photonView.RPC("StateMechanic", RpcTarget.All, nextState);
                }
                break;
            case GameStates.GAME:
                if (_currentGameState == GameStates.WAITINGFORPLAYERS)
                {
                    photonView.RPC("StateMechanic", RpcTarget.All, nextState);
                }
                break;
            case GameStates.VICTORY:
                if (_currentGameState == GameStates.GAME)
                {
                    photonView.RPC("StateMechanic", RpcTarget.All, nextState);
                }
                break;
        }
    }

    public void AddPointToBlueTeam()
    {
        photonView.RPC("UpdatePointOfTeamUIRPC", RpcTarget.AllBuffered, true);
    }

    public void AddPointToGreenTeam()
    {
        photonView.RPC("UpdatePointOfTeamUIRPC", RpcTarget.AllBuffered, false);
    }

    public void SetActiveBall(bool value)
    {
        _ball.SetActive(value);
    }

    #endregion

    #region Runtime Methods

    private void InitializeGameManager()
    {
        GameStateMechanic(GameStates.WAITINGFORPLAYERS);
    }

    private void InitializeState()
    {
        if (photonView.IsMine)
        {
            switch (_currentGameState)
            {
                case GameStates.WAITINGFORPLAYERS:
                    InitializeWaitingForPlayersState();
                    break;
                case GameStates.GAME:
                    InitializeGameState();
                    break;
                case GameStates.VICTORY:
                    InitializeVictoryState();
                    break;
            }
        }
    }

    private void ExecutingState()
    {
        Debug.Log("Current Game State: " +  _currentGameState.ToString());
        if (photonView.IsMine)
        {
            switch (_currentGameState)
            {
                case GameStates.WAITINGFORPLAYERS:
                    ExecutingWaitingForPlayersState();
                    break;
                case GameStates.GAME:
                    ExecutingGameState();
                    break;
                case GameStates.VICTORY:
                    ExecutingVictoryState();
                    break;
            }
        }
    }

    private void FinalizeState()
    {
        if (photonView.IsMine)
        {
            switch (_currentGameState)
            {
                case GameStates.WAITINGFORPLAYERS:
                    FinalizeWaitingForPlayersState();
                    break;
                case GameStates.GAME:
                    FinalizeGameState();
                    break;
                case GameStates.VICTORY:
                    FinalizeVictoryState();
                    break;
            }
        }
    }

    #endregion

    #region StateMethods

    #region WaitingForPlayers

    private void InitializeWaitingForPlayersState()
    {

    }
    private void ExecutingWaitingForPlayersState()
    {

    }

    private void FinalizeWaitingForPlayersState()
    {

    }

    #endregion

    #region Game

    private void InitializeGameState()
    {
        photonView.RPC("StartCoroutineRPC", RpcTarget.AllBuffered, "GameStartCoroutine");
    }
    private void ExecutingGameState()
    {
        if(_gameStarted)
        {
            if (timerSecondsLeft > 0)
            {
                timerSecondsLeft -= Time.fixedDeltaTime;
                //UIManager.Instance.TimerTextUpdate(timerSecondsLeft);
                photonView.RPC("UpdateUIRPC", RpcTarget.All, timerSecondsLeft);
            }
            else
            {
                timerSecondsLeft = 0;
                GameStateMechanic(GameStates.VICTORY);
                //UIManager.Instance.TimerTextUpdate(timerSecondsLeft);
                photonView.RPC("UpdateUIRPC", RpcTarget.All, timerSecondsLeft);
            }
        }
        else
        {
            if(timerSecondsLeft > 0)
            {
                timerSecondsLeft -= Time.fixedDeltaTime;
            }
            else
            {
                timerSecondsLeft = 0;
            }
            //UIManager.Instance.TimerTextUpdate(timerSecondsLeft);
            photonView.RPC("UpdateUIRPC", RpcTarget.All, timerSecondsLeft);
        }
    }

    private void FinalizeGameState()
    {

    }

    #endregion

    #region Victory

    private void InitializeVictoryState()
    {
        if (_currentBlueTeamPoints > _currentGreenTeamPoints)
        {
            photonView.RPC("VictoryPanelUIRPC", RpcTarget.All, false, true);
        }
        else if(_currentGreenTeamPoints > _currentBlueTeamPoints)
        {
            photonView.RPC("VictoryPanelUIRPC", RpcTarget.All, false, false);
        }
        else
        {
            photonView.RPC("VictoryPanelUIRPC", RpcTarget.All, true, false);
        }
        foreach (PlayerController player in _allPlayersOfTheGame)
        {
            player.CanMove = false;
        }
    }
    private void ExecutingVictoryState()
    {

    }

    private void FinalizeVictoryState()
    {

    }

    #endregion

    #endregion

    #region Photon Methods
    public void OnEvent(EventData photonEvent)
    {
        switch(photonEvent.Code)
        {
            case 1:
                GameStateMechanic(GameStates.GAME);
                break;
        }
    }

    [PunRPC]
    private void StateMechanic(GameStates nextState)
    {
        FinalizeState();
        _currentGameState = nextState;
        InitializeState();
    }

    [PunRPC]
    private void StartCoroutineRPC(string coroutineName)
    {
        StartCoroutine(coroutineName);
    }

    [PunRPC]
    private void UpdateUIRPC(float secondsLeft)
    {
        UIManager.Instance.TimerTextUpdate(secondsLeft);
    }

    [PunRPC]
    private void VictoryPanelUIRPC(bool draw, bool blueTeamWins)
    {
        _ball.SetActive(false);
        UIManager.Instance.VictoryPanel(draw, blueTeamWins);
    }

    [PunRPC]
    private void UpdatePointOfTeamUIRPC(bool blueTeam)
    {
        if(blueTeam)
        {
            _currentBlueTeamPoints++;
            UIManager.Instance.UpdateTeamPoints(blueTeam, _currentBlueTeamPoints);
        }
        else
        {
            _currentGreenTeamPoints++;
            UIManager.Instance.UpdateTeamPoints(blueTeam, _currentGreenTeamPoints);
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator GameStartCoroutine()
    {
        timerSecondsLeft = 3f;
        yield return new WaitForSeconds(3f);
        _ball.SetActive(true);
        timerSecondsLeft = 120f;
        _gameStarted = true;
    }

    #endregion

    #region GettersSetters

    public GameStates GetCurrentGameState
    {
        get { return _currentGameState; }
    }


    public PlayerController AddPlayerToTheGameList
    {
        set { _allPlayersOfTheGame.Add(value); }
    }

    #endregion
}
