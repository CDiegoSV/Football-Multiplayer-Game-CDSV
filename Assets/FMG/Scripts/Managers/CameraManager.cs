using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    #region References
    [Header("Camera References")]

    [SerializeField] private List<CinemachineVirtualCameraBase> cameraList;

    #endregion

    #region Runtime Variables

    private CinemachineVirtualCameraBase _currentCamera;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
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
        foreach (CinemachineVirtualCameraBase camera in cameraList)
        {
            camera.Priority = 0;
        }
        cameraList[0].Priority = 10;
        _currentCamera = cameraList[0];
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Changes the priority of the previous camera to 0 and gives 10 to the new one from the camera list.
    /// </summary>
    /// <param name="cameraIndex"></param>
    public void ChangeCurrentCameraTo(int cameraIndex)
    {
        _currentCamera.Priority = 0;
        cameraList[cameraIndex].Priority = 10;
    }

    #endregion
}
