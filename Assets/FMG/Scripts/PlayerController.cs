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

    #endregion


    #region UnityMethods

    private void Start()
    {
        InitializeTheAvatar();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            horizontalInputAxis = Input.GetAxisRaw("Horizontal");
            verticalInputAxis = Input.GetAxisRaw("Vertical");
            _animator.SetFloat("InputMagnitude", _inputDirection.magnitude);
            PlayerKick();
        }
    }

    private void FixedUpdate()
    {
        if(photonView.IsMine)
        {
            RbMove();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball") && !canKick)
        {
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.position = new Vector3(playerBall.position.x, playerBall.position.y + other.gameObject.transform.position.y, playerBall.position.z);
            other.transform.SetParent(playerBall, true);
            canKick = true;
        }
    }

    #endregion

    #region Runtime Methods 

    private void InitializeTheAvatar()
    {
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
            _animator = transform.GetChild(1).GetComponent<Animator>();
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
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.transform.SetParent(null, true);
        ball.GetComponent<Rigidbody>().AddForce(transform.forward * 15f, ForceMode.Impulse);
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

    #region Coroutines

    private IEnumerator CoolDownTimer()
    {
        yield return new WaitForSeconds(1);
        canKick = false;
    }

    #endregion

}
