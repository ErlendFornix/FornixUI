using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class represengting the physical button that will interact with coliders tagged as user, getting pressed down when colliding to a limit
/// </summary>
public class PhysicKinematicButton : MonoBehaviour
{
    #region Definition
    [Tooltip("The maximum position buttony should end up when unpressed")]
    [SerializeField] Vector3 _MaxPosition;
    [Tooltip("The maximum position buttony should end up when pressed")]
    [SerializeField] Vector3 _MinPosition;
    [Tooltip("ammount of steps needed to reach min position from max position, defines precision of movement on touch")]
    [SerializeField] int _Steps;
    [SerializeField] UnityEvent _OnButtonPressed;
    [SerializeField] UnityEvent _OnButtonReleased;

    [SerializeField] private string _Tag;

    //bool _userColiding=> _userColliderCount>0;
    bool _userColiding;
    bool _userColiderDelay;
    int _userColliderCount;
    bool _clickLatch;

    #endregion
    #region Methods
    private void FixedUpdate()
    {
        if (_userColliderCount == 0)
        {
            if (_userColiderDelay)
            {
                _userColiding = false;
            }
            else
            {
                _userColiderDelay = true;
            }
        }
        else
        {
            _userColiding = true;
            _userColiderDelay = false;
        }

        if (_userColiding) //check if move towards minimum pos else move to max pos
        {
            if (transform.localPosition != _MinPosition)
            {
                transform.localPosition = transform.localPosition + GetPressStep();
            }
            else
            {
                if (!_clickLatch) //check if we didn't triggered press already in this cycle
                {
                    _OnButtonPressed?.Invoke();
                    Debug.Log("ButtonISPRessed");
                    _clickLatch = true;
                }
            }
        }
        else
        {
            if (transform.localPosition != _MaxPosition)
            {
                transform.localPosition = transform.localPosition + GetReleaseStep();
            }
            else
            {
                if (_clickLatch) //check if we didn't triggered press already in this cycle
                {
                    _OnButtonReleased?.Invoke();
                    Debug.Log("ButtonISReleased");

                    _clickLatch = false;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _Tag)
        {
            _userColliderCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == _Tag)
        {
            _userColliderCount--;
        }
    }

    private Vector3 GetPressStep()
    {
        return (_MinPosition - _MaxPosition) / _Steps;
    }
    private Vector3 GetReleaseStep()
    {
        return (_MaxPosition - _MinPosition) / _Steps;
    }

    #endregion
}
