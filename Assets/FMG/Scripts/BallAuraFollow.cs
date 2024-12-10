using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAuraFollow : MonoBehaviour
{
    #region References

    [SerializeField] GameObject _ball;

    #endregion

    #region Runtime Variables

    private bool _inPosition;

    #endregion

    #region Unity Methods

    private void FixedUpdate()
    {
        if (_ball.activeSelf)
        {
            transform.position = _ball.transform.position;

            if(_inPosition)
            {
                _inPosition = false;
            }
        }
        else
        {
            if(!_inPosition)
            {
                transform.position = new Vector3(0f, -2f, 0f);
                _inPosition = true;
            }
        }
    }

    #endregion
}
