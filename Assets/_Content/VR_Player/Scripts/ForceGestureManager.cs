using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ControlsLIB;

/*TODO
 * Check if a hand is holding the grip action
 * Take controller position to my pisition to figure out which quadrant I am in
 * If Pass threshold do action in quadrant
 * Get position on grip, and position when grip released
 */



[RequireComponent(typeof(VRPlayerController))]
public class ForceGestureManager : MonoBehaviour
{
    public float ForceThreshold = 0.25f;

    public UnityEvent onForceYankForward;
    public UnityEvent onForceYankBackward;
    public UnityEvent onForceYankUp;
    public UnityEvent onForceYankDown;
    public UnityEvent onForceYankLeft;
    public UnityEvent onForceYankRight;

    private VRPlayerController _playerController;
    private GameObject _rightHand;
    private ForceGestureHand _rightGestureHand;
    private ClimbHand _rightClimbHand;
    private GameObject _leftHand;
    private ForceGestureHand _leftGestureHand;
    private ClimbHand _leftClimbHand;

    
    // Start is called before the first frame update
    void Awake()
    {
        if(!_playerController)
            _playerController = GetComponent<VRPlayerController>();
        if (!_rightHand)
            _rightHand = _playerController.RightHand;
        if (!_leftHand)
            _leftHand = _playerController.LeftHand;
        _rightGestureHand = _rightHand.GetComponent<ForceGestureHand>();
        _leftGestureHand = _leftHand.GetComponent<ForceGestureHand>();
        _rightClimbHand = _rightHand.GetComponent<ClimbHand>();
        _leftClimbHand = _leftHand.GetComponent<ClimbHand>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_rightGestureHand.ActiveGesture && _rightGestureHand.HandClosedPosition != Vector3.zero && _rightGestureHand.HandOpenPosition != Vector3.zero)
        {
            if(Vector3.Magnitude(_rightClimbHand.ControllerDelta) >= ForceThreshold)
                RunForceEvent(CheckGesture(_rightHand, _rightGestureHand.HandClosedPosition, _rightGestureHand.HandOpenPosition));
            _rightGestureHand.ResetHand();
        }

        if (!_leftGestureHand.ActiveGesture && _leftGestureHand.HandClosedPosition != Vector3.zero && _leftGestureHand.HandOpenPosition != Vector3.zero)
        {
            if (Vector3.Magnitude(_leftClimbHand.ControllerDelta) >= ForceThreshold)
                RunForceEvent(CheckGesture(_leftHand, _leftGestureHand.HandClosedPosition, _leftGestureHand.HandOpenPosition));
            _leftGestureHand.ResetHand();
        }
    }

    private void RunForceEvent(GestureDirection _gesture)
    {
        if (_gesture == GestureDirection.Forward)
            RunEvent(onForceYankForward);
        if (_gesture == GestureDirection.Backward)
            RunEvent(onForceYankBackward);
        if (_gesture == GestureDirection.Up)
            RunEvent(onForceYankUp);
        if (_gesture == GestureDirection.Down)
            RunEvent(onForceYankDown);
        if (_gesture == GestureDirection.Right)
            RunEvent(onForceYankRight);
        if (_gesture == GestureDirection.Left)
            RunEvent(onForceYankLeft);
        if (_gesture == GestureDirection.None)
            Debug.LogError("Force Gesture did something wrong");
    }

    private GestureDirection CheckGesture(GameObject hand, Vector3 _gripPosition, Vector3 _unGripPosition)
    {
        //Vector3 _PositionChecker = new Vector3(transform.localPosition.x, _gripPosition.y, transform.localPosition.z); //may not need
        //float _gripDistance = Vector3.Distance(_PositionChecker, _gripPosition); //May not need
        //float _unGripDistance = Vector3.Distance(_PositionChecker, _unGripPosition); //May not need
        //Vector3 _gripDirection = (_gripPosition - _PositionChecker).normalized; //may not need
        //_gripDirection = transform.InverseTransformDirection(_gripDirection); //may not need
        Vector3 _unGripDirection = (_unGripPosition - _gripPosition).normalized;
        _unGripDirection = hand.transform.InverseTransformDirection(_unGripDirection);

        //Check which position the ungrip was in
        if (Mathf.Abs(_unGripDirection.z) >= 0.8f)
        {
            //forward (0, 0, 1)
            //backward (0, 0, -1)
            if (_unGripDirection.z > 0f) //go forward
            {
                return GestureDirection.Forward;
            }
            else //go backward
            {
                return GestureDirection.Backward;
            }
        }
        if (Mathf.Abs(_unGripDirection.y) >= 0.8f)
        {

            //up (0, 1, 0)
            //down (0, -1, 0)
            if (_unGripDirection.y > 0f) //go up
            {
                return GestureDirection.Up;
            }
            else //go down
            {
                return GestureDirection.Down;
            }
        }
        if (Mathf.Abs(_unGripDirection.x) >= 0.8f)
        {

            //right (1, 0, 0)
            //left (-1, 0, 0)
            if (_unGripDirection.x > 0f) //go right
            {
                return GestureDirection.Right;
            }
            else //go left
            {
                return GestureDirection.Left;
            }
        }
        return GestureDirection.None;
    }

    private void RunEvent(UnityEvent _event)
    {
        _event.Invoke();
    }
}
