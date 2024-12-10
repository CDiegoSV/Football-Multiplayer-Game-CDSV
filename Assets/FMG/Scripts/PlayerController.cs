using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    #region References
    [Header("Player References")]

    [SerializeField] private Transform playerBall;

    #endregion

    #region Runtime Variables

    [SerializeField] private Rigidbody _rigidBody;
    private Animator _animator;
    private Vector3 _inputDirection;
    private Transform _mainCameraTransform;

    private float movementSpeed = 5f;
    private float rotationSpeed = 10f;

    private float horizontalInputAxis;
    private float verticalInputAxis;

    private bool canKick;
    private bool canMove;

    #endregion


    #region UnityMethods

    private void Start()
    {
        if(photonView.IsMine)
        {
            InitializeTheAvatar();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if(canMove)
            {
                horizontalInputAxis = Input.GetAxisRaw("Horizontal");
                verticalInputAxis = Input.GetAxisRaw("Vertical");
                _animator.SetFloat("InputMagnitude", _inputDirection.magnitude);
                PlayerKick();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseToggle();
            }
        }
    }

    private void FixedUpdate()
    {
        if(photonView.IsMine && canMove)
        {
            RbMove();
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Ball") && !canKick)
        {
            other.transform.position = new Vector3(playerBall.position.x, playerBall.position.y + other.gameObject.transform.position.y, playerBall.position.z);
            int otherViewID = other.gameObject.GetPhotonView().ViewID;
            int playerBallViewID = playerBall.gameObject.GetPhotonView().ViewID;
            photonView.RPC("SetGOParent", RpcTarget.All, otherViewID, playerBallViewID);
            canKick = true;
        }
    }

    #endregion

    #region Runtime Methods 

    private void InitializeTheAvatar()
    {
        canMove = true;
        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody>();
        }
        if (_mainCameraTransform == null)
        {
            _mainCameraTransform = Camera.main.gameObject.transform;
        }
        if (_animator == null)
        {
            _animator = transform.GetChild(2).GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Moves and rotates the avatar.
    /// </summary>
    private void RbMove()
    {
        

        _inputDirection = new Vector3(horizontalInputAxis, 0f, verticalInputAxis).normalized;


        if (_inputDirection.magnitude >= 0.1f)
        {
            Vector3 cameraForward = _mainCameraTransform.forward;
            Vector3 cameraRight = _mainCameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * _inputDirection.z + cameraRight * _inputDirection.x;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            _rigidBody.MovePosition(_rigidBody.position + moveDirection * movementSpeed * Time.fixedDeltaTime);
        }
    }

    private void Kick()
    {
        _animator.SetBool("Kicked", false);
        GameObject ball = playerBall.GetChild(0).gameObject;
        int ballViewID = ball.GetPhotonView().ViewID;
        photonView.RPC("SetNullGOParent", RpcTarget.All, ballViewID);
        StartCoroutine(CoolDownTimer());
    }

    /// <summary>
    /// Kicks the ball if it's posible.
    /// </summary>
    private void PlayerKick()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canKick && playerBall.GetChild(0) != null)
        {
            Invoke("Kick", 0.3f);
            _animator.SetBool("Kicked", true);

        }
    }

    #endregion

    #region PublicMethods

    public void PauseToggle()
    {
        canMove = !canMove;
        UIManager.Instance.PausePanelToggle();
    }

    #endregion

    #region RPC Methods

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
        targetObject.transform.SetParent(parentObject.transform, true);
        targetObject.GetComponent<Rigidbody>().isKinematic = true;
        targetObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    [PunRPC]
    /// <summary>
    /// Sets null the parent of "gameObject".
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="parent"></param>
    private void SetNullGOParent(int gameObjectViewID)
    {
        GameObject targetObject = PhotonView.Find(gameObjectViewID).gameObject;
        targetObject.GetComponent<Rigidbody>().isKinematic = false;
        targetObject.transform.SetParent(null, true);
        targetObject.GetComponent<Rigidbody>().AddForce(transform.forward * 15f, ForceMode.Impulse);
    }

    #endregion

    #region Coroutines

    private IEnumerator CoolDownTimer()
    {
        yield return new WaitForSeconds(1);
        canKick = false;
    }

    #endregion

    #region GettersSetters

    public bool CanMove
    {
        get { return canMove; }
        set {  canMove = value; }
    }

    #endregion
}
