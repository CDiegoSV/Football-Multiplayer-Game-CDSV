using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : MonoBehaviourPunCallbacks
{
    #region References

    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] CinemachineImpulseSource _impulseSource;

    #endregion

    #region Knobs

    [SerializeField] private bool _isTeamBlue;

    #endregion

    #region Unity Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && photonView.IsMine)
        {
            other.gameObject.transform.position = new Vector3(0, 2, 0);
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            photonView.RPC("StartGoalCoroutine", RpcTarget.All);
            if(_isTeamBlue)
            {
                GameManager.instance.AddPointToGreenTeam();
            }
            else
            {
                GameManager.instance.AddPointToBlueTeam();
            }
        }
    }

    #endregion

    #region RPC Methods

    [PunRPC]
    private void GoalShakeAndParticles()
    {
        _particleSystem.gameObject.SetActive(true);
        _particleSystem.Play();
        _impulseSource.GenerateImpulse(10);
    }

    [PunRPC]
    private void StartGoalCoroutine()
    {
        StartCoroutine(GoalCoroutine());
    }

    #endregion

    #region Coroutines

    private IEnumerator GoalCoroutine()
    {
        photonView.RPC("GoalShakeAndParticles", RpcTarget.All);
        GameManager.instance.SetActiveBall(false);
        yield return new WaitForSeconds(3f);
        GameManager.instance.SetActiveBall(true);
    }

    #endregion
}
